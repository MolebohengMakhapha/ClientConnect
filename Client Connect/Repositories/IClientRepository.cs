using Client_Connect.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_Connect.Repositories
{
    public interface IClientRepository
    {
        IEnumerable<Client> GetAll();
        Client GetById(int clientId);
        int Create(string name, string clientCode);
        void Update(int clientId, string name);
        bool CodeExists(string code);
        IEnumerable<LinkedContact> GetLinkedContacts(int clientId);
        IEnumerable<Contact> GetAvailableContacts(int clientId);
        void LinkContact(int clientId, int contactId);
        void UnlinkContact(int clientId, int contactId);
        bool NameExists(string name, int excludeId = 0);
        void Delete(int clientId);
    }
}

