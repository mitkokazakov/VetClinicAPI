using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VetClinic.Data.Models;

namespace VetClinic.Data
{
    public class VetClinicDbContext : IdentityDbContext<ApplicationUser>
    {
        public VetClinicDbContext()
        {

        }

        public VetClinicDbContext(DbContextOptions options) : base(options)
        {
            
        }

        public DbSet<Pet> Pet { get; set; }
        public DbSet<Image> Image { get; set; }
        public DbSet<Visitation> Visitation { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //if (!optionsBuilder.IsConfigured)
            //{
            //    optionsBuilder.UseSqlServer(@"Server=DESKTOP-3IBT8TO\SQLEXPRESS;Database=VetClinic;Integrated Security=True;");
            //}

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
