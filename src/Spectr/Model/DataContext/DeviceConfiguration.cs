using System.Data.Entity.ModelConfiguration;

namespace Spectr.Model.DataContext
{
    public class DeviceConfiguration : EntityTypeConfiguration<Device>
    {
        public DeviceConfiguration()
        {
            ToTable("Device");
        }
    }
}