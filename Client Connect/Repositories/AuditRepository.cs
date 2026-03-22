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
    public class AuditRepository : IAuditRepository
    {
        private readonly string _connectionString;

        public AuditRepository()
        {
            _connectionString = ConfigurationManager
                .ConnectionStrings["ClientManagementDB"].ConnectionString;
        }

        private IDbConnection Connection => new SqlConnection(_connectionString);

        public void Log(string tableName, int recordId, string action, string detail = null)
        {
            const string sql = @"
                INSERT INTO dbo.AuditLog (TableName, RecordId, Action, Detail, ActionDate)
                VALUES (@TableName, @RecordId, @Action, @Detail, GETDATE());";

            using (var db = Connection)
                db.Execute(sql, new { TableName = tableName, RecordId = recordId, Action = action, Detail = detail });
        }

        public IEnumerable<AuditLog> GetByRecord(string tableName, int recordId)
        {
            const string sql = @"
                SELECT AuditId, TableName, RecordId, Action, Detail, ActionDate
                FROM   dbo.AuditLog
                WHERE  TableName = @TableName AND RecordId = @RecordId
                ORDER BY ActionDate DESC;";

            using (var db = Connection)
                return db.Query<AuditLog>(sql, new { TableName = tableName, RecordId = recordId });
        }

        public IEnumerable<AuditLog> GetAll(int take = 100)
        {
            const string sql = @"
                SELECT TOP (@Take) AuditId, TableName, RecordId, Action, Detail, ActionDate
                FROM   dbo.AuditLog
                ORDER BY ActionDate DESC;";

            using (var db = Connection)
                return db.Query<AuditLog>(sql, new { Take = take });
        }
    }
}