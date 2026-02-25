using Client_Connect.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Client_Connect.Repositories
{
    public class ContactRepository : IContactRepository
    {
        private readonly string _connectionString;

        public ContactRepository()
        {
            _connectionString = ConfigurationManager
                .ConnectionStrings["ClientManagementDB"].ConnectionString;
        }

        private IDbConnection Connection => new SqlConnection(_connectionString);

        public IEnumerable<Contact> GetAll()
        {
            const string sql = @"
                SELECT  ct.ContactId,
                        ct.Name,
                        ct.Surname,
                        ct.Email,
                        COUNT(cc.ClientId) AS ClientCount
                FROM    dbo.Contacts ct
                LEFT JOIN dbo.ClientContacts cc ON cc.ContactId = ct.ContactId
                GROUP BY ct.ContactId, ct.Name, ct.Surname, ct.Email
                ORDER BY ct.Surname ASC, ct.Name ASC;";

            using (var db = Connection)
                return db.Query<Contact>(sql);
        }

        public Contact GetById(int contactId)
        {
            const string sql = @"
                SELECT ContactId, Name, Surname, Email
                FROM   dbo.Contacts
                WHERE  ContactId = @ContactId;";

            using (var db = Connection)
                return db.QuerySingleOrDefault<Contact>(sql, new { ContactId = contactId });
        }

        public int Create(string name, string surname, string email)
        {
            const string sql = @"
                INSERT INTO dbo.Contacts (Name, Surname, Email)
                VALUES (@Name, @Surname, @Email);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using (var db = Connection)
                return db.ExecuteScalar<int>(sql, new { Name = name, Surname = surname, Email = email });
        }

        public void Update(int contactId, string name, string surname, string email)
        {
            const string sql = @"
                UPDATE dbo.Contacts
                SET    Name = @Name, Surname = @Surname, Email = @Email
                WHERE  ContactId = @ContactId;";

            using (var db = Connection)
                db.Execute(sql, new { Name = name, Surname = surname, Email = email, ContactId = contactId });
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
                WHERE   ClientId NOT IN (
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
                    INSERT INTO dbo.ClientContacts (ClientId, ContactId)
                    VALUES (@ClientId, @ContactId);";

            using (var db = Connection)
                db.Execute(sql, new { ClientId = clientId, ContactId = contactId });
        }

        public void UnlinkClient(int contactId, int clientId)
        {
            const string sql = @"
                DELETE FROM dbo.ClientContacts
                WHERE ClientId = @ClientId AND ContactId = @ContactId;";

            using (var db = Connection)
                db.Execute(sql, new { ClientId = clientId, ContactId = contactId });
        }

        public void Delete(int contactId)
        {
            const string sql = @"
        DELETE FROM dbo.ClientContacts WHERE ContactId = @ContactId;
        DELETE FROM dbo.Contacts        WHERE ContactId = @ContactId;";

            using (var db = Connection)
                db.Execute(sql, new { ContactId = contactId });
        }
    }
}