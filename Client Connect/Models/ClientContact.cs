namespace Client_Connect.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ClientContact
    {
        public int ClientContactId { get; set; }

        public int ClientId { get; set; }

        public int ContactId { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime CreatedDate { get; set; }

        public virtual Client Client { get; set; }

        public virtual Contact Contact { get; set; }
    }
}
