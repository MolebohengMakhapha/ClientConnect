using Client_Connect.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Dapper;

namespace Client_Connect.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly string _connectionString;

        public ClientRepository()
        {
            _connectionString = ConfigurationManager
                .ConnectionStrings["ClientManagementDB"].ConnectionString;
        }

        private IDbConnection Connection => new SqlConnection(_connectionString);

        public IEnumerable<Client> GetAll()
        {
            const string sql = @"
        SELECT  c.ClientId,
                c.Name,
                c.ClientCode,
                c.StateId,
                s.Description AS StateDescription,
                COUNT(cc.ContactId) AS ContactCount
        FROM    dbo.Clients c
        LEFT JOIN dbo.ClientContacts cc ON cc.ClientId = c.ClientId
        INNER JOIN dbo.States s         ON s.StateId  = c.StateId
        GROUP BY c.ClientId, c.Name, c.ClientCode, c.StateId, s.Description
        ORDER BY c.Name ASC;";

            using (var db = Connection)
                return db.Query<Client>(sql);
        }

        public Client GetById(int clientId)
        {
            const string sql = @"
        SELECT  c.ClientId, c.Name, c.ClientCode, c.StateId,
                s.Description AS StateDescription
        FROM    dbo.Clients c
        INNER JOIN dbo.States s ON s.StateId = c.StateId
        WHERE   c.ClientId = @ClientId;";

            using (var db = Connection)
                return db.QuerySingleOrDefault<Client>(sql, new { ClientId = clientId });
        }

        public int Create(string name, string clientCode)
        {
            const string sql = @"
        INSERT INTO dbo.Clients (Name, ClientCode, StateId, CreatedDate, ModifiedDate)
        VALUES (@Name, @ClientCode, 1, GETDATE(), GETDATE());
        SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using (var db = Connection)
                return db.ExecuteScalar<int>(sql, new { Name = name, ClientCode = clientCode });
        }

        public void Update(int clientId, string name)
        {
            const string sql = @"
        UPDATE dbo.Clients
        SET    Name         = @Name,
               ModifiedDate = GETDATE()
        WHERE  ClientId     = @ClientId;";

            using (var db = Connection)
                db.Execute(sql, new { Name = name, ClientId = clientId });
        }

        public bool CodeExists(string code)
        {
            const string sql = "SELECT COUNT(1) FROM dbo.Clients WHERE ClientCode = @Code;";
            using (var db = Connection)
                return db.ExecuteScalar<int>(sql, new { Code = code }) > 0;
        }

        public bool NameExists(string name, int excludeId = 0)
        {
            const string sql = @"
                SELECT COUNT(1) FROM dbo.Clients
                WHERE Name = @Name AND ClientId <> @ExcludeId;";
            using (var db = Connection)
                return db.ExecuteScalar<int>(sql, new { Name = name, ExcludeId = excludeId }) > 0;
        }

        public IEnumerable<LinkedContact> GetLinkedContacts(int clientId)
        {
            const string sql = @"
                SELECT  ct.ContactId, ct.Name, ct.Surname, ct.Email
                FROM    dbo.Contacts ct
                INNER JOIN dbo.ClientContacts cc ON cc.ContactId = ct.ContactId
                WHERE   cc.ClientId = @ClientId
                ORDER BY ct.Surname ASC, ct.Name ASC;";

            using (var db = Connection)
                return db.Query<LinkedContact>(sql, new { ClientId = clientId });
        }

        public IEnumerable<Contact> GetAvailableContacts(int clientId)
        {
            const string sql = @"
        SELECT  ContactId, Name, Surname, Email
        FROM    dbo.Contacts
        WHERE   StateId   = 1
        AND     ContactId NOT IN (
                    SELECT ContactId FROM dbo.ClientContacts WHERE ClientId = @ClientId
                )
        ORDER BY Surname ASC, Name ASC;";

            using (var db = Connection)
                return db.Query<Contact>(sql, new { ClientId = clientId });
        }

        public void LinkContact(int clientId, int contactId)
        {
            const string sql = @"
                IF NOT EXISTS (SELECT 1 FROM dbo.ClientContacts
                               WHERE ClientId = @ClientId AND ContactId = @ContactId)
                    INSERT INTO dbo.ClientContacts (ClientId, ContactId)
                    VALUES (@ClientId, @ContactId);";

            using (var db = Connection)
                db.Execute(sql, new { ClientId = clientId, ContactId = contactId });
        }

        public void UnlinkContact(int clientId, int contactId)
        {
            const string sql = @"
                DELETE FROM dbo.ClientContacts
                WHERE ClientId = @ClientId AND ContactId = @ContactId;";

            using (var db = Connection)
                db.Execute(sql, new { ClientId = clientId, ContactId = contactId });
        }

        public void SoftDelete(int clientId)
        {
            const string sql = @"
        UPDATE dbo.Clients
        SET    StateId      = 0,
               ModifiedDate = GETDATE()
        WHERE  ClientId     = @ClientId;";

            using (var db = Connection)
                db.Execute(sql, new { ClientId = clientId });
        }

        public void Restore(int clientId)
        {
            const string sql = @"
        UPDATE dbo.Clients
        SET    StateId      = 1,
               ModifiedDate = GETDATE()
        WHERE  ClientId     = @ClientId;";

            using (var db = Connection)
                db.Execute(sql, new { ClientId = clientId });
        }
    }
}