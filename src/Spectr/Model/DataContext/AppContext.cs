using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace Spectr.Model.DataContext
{
    [Table("Device")]
    public class AppContext : DbContext
    {
        
        public DbSet<Device> Device { get; set; }

        public AppContext() : base(Properties.Settings.Default.con)
        {
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new DeviceConfiguration());
        }
    }
}
