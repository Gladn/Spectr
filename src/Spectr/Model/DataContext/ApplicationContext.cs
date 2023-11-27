using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace Spectr.Model.DataContext
{
    [Table("Device")]
    public class ApplicationContext : DbContext
    {
        
        public DbSet<Device> Device { get; set; }

        public ApplicationContext() : base(Properties.Settings.Default.con)
        {
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new DeviceConfiguration());
        }
    }
}
