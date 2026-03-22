using Client_Connect.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_Connect.Repositories
{
    public interface IAuditRepository
    {
        void Log(string tableName, int recordId, string action, string detail = null);
        IEnumerable<AuditLog> GetByRecord(string tableName, int recordId);
        IEnumerable<AuditLog> GetAll(int take = 100);
    }
}