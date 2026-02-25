using Client_Connect.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Client_Connect.ViewModels
{
    public class ContactFormViewModel
    {
        public int ContactId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Surname is required.")]
        [StringLength(100, ErrorMessage = "Surname cannot exceed 100 characters.")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [StringLength(255)]
        public string Email { get; set; }

        public bool IsNew => ContactId == 0;

        public List<LinkedClient> LinkedClients { get; set; } = new List<LinkedClient>();
        public List<Client> AvailableClients { get; set; } = new List<Client>();
    }
}