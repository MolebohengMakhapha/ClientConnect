using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace Client_Connect.Models
{
    public partial class ClientManagementDB : DbContext
    {
        public ClientManagementDB()
            : base("name=ClientManagementDB")
        {
        }

        public virtual DbSet<ClientContact> ClientContacts { get; set; }
        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<Contact> Contacts { get; set; }
        public virtual DbSet<State> States { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Client>()
                .Property(e => e.ClientCode)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Client>()
                .HasMany(e => e.ClientContacts)
                .WithRequired(e => e.Client)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Contact>()
                .HasMany(e => e.ClientContacts)
                .WithRequired(e => e.Contact)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<State>()
                .HasMany(e => e.Clients)
                .WithRequired(e => e.State)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<State>()
                .HasMany(e => e.Contacts)
                .WithRequired(e => e.State)
                .WillCascadeOnDelete(false);
        }
    }
}
