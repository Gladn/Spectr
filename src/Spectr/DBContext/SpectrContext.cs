using Spectr.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectr.DBContext
{

    internal class SpectrContext : DbContext
    {
        public SpectrContext(string connectionString = "SpectrDbConnectionString")
            : base(connectionString)
        {

        }
        public DbSet<Customer> Customer { get; set; }
        

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
           // modelBuilder.Configurations.Add(new CustomerConfig());
        }
    }
}
