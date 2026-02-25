using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Client_Connect.Models
{
    public class LinkedContact
    {
        public int ContactId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string FullName => $"{Surname} {Name}";
    }
}