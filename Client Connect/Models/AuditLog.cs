namespace Client_Connect.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("AuditLog")]
    public partial class AuditLog
    {
        [Key]
        public int AuditId { get; set; }

        [Required]
        [StringLength(50)]
        public string TableName { get; set; }

        public int RecordId { get; set; }

        [Required]
        [StringLength(50)]
        public string Action { get; set; }

        [Required]
        [StringLength(255)]
        public string Detail { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime ActionDate { get; set; }
    }
}
