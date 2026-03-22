using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using Client_Connect.Models;

namespace Client_Connect.Repositories
{
    public class ContactRepository : IContactRepository
    {
        private readonly string _connectionString;
        private readonly IAuditRepository _audit;

        public ContactRepository()
        {
            _connectionString = ConfigurationManager
                .ConnectionStrings["ClientManagementDB"].ConnectionString;
            _audit = new AuditRepository();
        }

        private IDbConnection Connection => new SqlConnection(_connectionString);

        public IEnumerable<Contact> GetAll()
        {
            const string sql = @"
                SELECT  ct.ContactId,
                        ct.Name,
                        ct.Surname,
                        ct.Email,
                        ct.StateId,
                        s.Description AS StateDescription,
                        COUNT(cc.ClientId) AS ClientCount
                FROM    dbo.Contacts ct
                LEFT JOIN dbo.ClientContacts cc ON cc.ContactId = ct.ContactId
                INNER JOIN dbo.States s         ON s.StateId   = ct.StateId
                GROUP BY ct.ContactId, ct.Name, ct.Surname, ct.Email, ct.StateId, s.Description
                ORDER BY ct.Surname ASC, ct.Name ASC;";

            using (var db = Connection)
                return db.Query<Contact>(sql);
        }

        public Contact GetById(int contactId)
        {
            const string sql = @"
                SELECT  ct.ContactId, ct.Name, ct.Surname, ct.Email, ct.StateId,
                        s.Description AS StateDescription
                FROM    dbo.Contacts ct
                INNER JOIN dbo.States s ON s.StateId = ct.StateId
                WHERE   ct.ContactId = @ContactId;";

            using (var db = Connection)
                return db.QuerySingleOrDefault<Contact>(sql, new { ContactId = contactId });
        }

        public int Create(string name, string surname, string email)
        {
            const string sql = @"
                INSERT INTO dbo.Contacts (Name, Surname, Email, StateId, CreatedDate, ModifiedDate)
                VALUES (@Name, @Surname, @Email, 1, GETDATE(), GETDATE());
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            int newId;
            using (var db = Connection)
                newId = db.ExecuteScalar<int>(sql, new { Name = name, Surname = surname, Email = email });

            _audit.Log("Contacts", newId, "Created", $"Contact '{surname} {name}' created with email {email}.");
            return newId;
        }

        public void Update(int contactId, string name, string surname, string email)
        {
            const string sql = @"
                UPDATE dbo.Contacts
                SET    Name         = @Name,
                       Surname      = @Surname,
                       Email        = @Email,
                       ModifiedDate = GETDATE()
                WHERE  ContactId    = @ContactId;";

            using (var db = Connection)
                db.Execute(sql, new { Name = name, Surname = surname, Email = email, ContactId = contactId });

            _audit.Log("Contacts", contactId, "Updated", $"Contact updated to '{surname} {name}', email: {email}.");
        }

        public void SoftDelete(int contactId)
        {
            const string sql = @"
                UPDATE dbo.Contacts
                SET    StateId      = 0,
                       ModifiedDate = GETDATE()
                WHERE  ContactId    = @ContactId;";

            using (var db = Connection)
                db.Execute(sql, new { ContactId = contactId });

            _audit.Log("Contacts", contactId, "Deleted", "Contact marked as deleted.");
        }

        public void Restore(int contactId)
        {
            const string sql = @"
                UPDATE dbo.Contacts
                SET    StateId      = 1,
                       ModifiedDate = GETDATE()
                WHERE  ContactId    = @ContactId;";

            using (var db = Connection)
                db.Execute(sql, new { ContactId = contactId });

            _audit.Log("Contacts", contactId, "Restored", "Contact restored to active.");
        }

        public void Delete(int contactId)
        {
            const string sql = @"
                DELETE FROM dbo.ClientContacts WHERE ContactId = @ContactId;
                DELETE FROM dbo.Contacts        WHERE ContactId = @ContactId;";

            using (var db = Connection)
                db.Execute(sql, new { ContactId = contactId });

            _audit.Log("Contacts", contactId, "Permanently Deleted", "Contact and all linked records permanently removed.");
        }

        public bool EmailExists(string email, int excludeId = 0)
        {
            const string sql = @"
                SELECT COUNT(1) FROM dbo.Contacts
                WHERE Email = @Email AND ContactId <> @ExcludeId;";

            using (var db = Connection)
                return db.ExecuteScalar<int>(sql, new { Email = email, ExcludeId = excludeId }) > 0;
        }

        public IEnumerable<LinkedClient> GetLinkedClients(int contactId)
        {
            const string sql = @"
                SELECT  c.ClientId, c.Name, c.ClientCode
                FROM    dbo.Clients c
                INNER JOIN dbo.ClientContacts cc ON cc.ClientId = c.ClientId
                WHERE   cc.ContactId = @ContactId
                ORDER BY c.Name ASC;";

            using (var db = Connection)
                return db.Query<LinkedClient>(sql, new { ContactId = contactId });
        }

        public IEnumerable<Client> GetAvailableClients(int contactId)
        {
            const string sql = @"
                SELECT  ClientId, Name, ClientCode
                FROM    dbo.Clients
                WHERE   StateId = 1
                AND     ClientId NOT IN (
                            SELECT ClientId FROM dbo.ClientContacts WHERE ContactId = @ContactId
                        )
                ORDER BY Name ASC;";

            using (var db = Connection)
                return db.Query<Client>(sql, new { ContactId = contactId });
        }

        public void LinkClient(int contactId, int clientId)
        {
            const string sql = @"
                IF NOT EXISTS (SELECT 1 FROM dbo.ClientContacts
                               WHERE ClientId = @ClientId AND ContactId = @ContactId)
                    INSERT INTO dbo.ClientContacts (ClientId, ContactId, CreatedDate)
                    VALUES (@ClientId, @ContactId, GETDATE());";

            using (var db = Connection)
                db.Execute(sql, new { ClientId = clientId, ContactId = contactId });

            _audit.Log("Contacts", contactId, "Linked", $"Client ID {clientId} linked to contact.");
        }

        public void UnlinkClient(int contactId, int clientId)
        {
            const string sql = @"
                DELETE FROM dbo.ClientContacts
                WHERE ClientId = @ClientId AND ContactId = @ContactId;";

            using (var db = Connection)
                db.Execute(sql, new { ClientId = clientId, ContactId = contactId });

            _audit.Log("Contacts", contactId, "Unlinked", $"Client ID {clientId} unlinked from contact.");
        }
    }
}