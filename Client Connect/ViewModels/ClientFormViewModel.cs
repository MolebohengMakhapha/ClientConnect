using Client_Connect.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Client_Connect.ViewModels
{
    public class ClientFormViewModel
    {
        public int ClientId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; }

        public string ClientCode { get; set; }
        public int StateId { get; set; }

        public bool IsNew => ClientId == 0;

        public List<LinkedContact> LinkedContacts { get; set; } = new List<LinkedContact>();
        public List<Contact> AvailableContacts { get; set; } = new List<Contact>();
    }
}