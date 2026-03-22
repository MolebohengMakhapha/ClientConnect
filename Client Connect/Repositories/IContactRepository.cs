using Client_Connect.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_Connect.Repositories
{
    public interface IContactRepository
    {
        IEnumerable<Contact> GetAll();
        Contact GetById(int contactId);
        int Create(string name, string surname, string email);
        void Update(int contactId, string name, string surname, string email);
        void SoftDelete(int contactId);
        void Restore(int contactId);
        bool EmailExists(string email, int excludeId = 0);
        IEnumerable<LinkedClient> GetLinkedClients(int contactId);
        IEnumerable<Client> GetAvailableClients(int contactId);
        void LinkClient(int contactId, int clientId);
        void UnlinkClient(int contactId, int clientId);
    }
}
