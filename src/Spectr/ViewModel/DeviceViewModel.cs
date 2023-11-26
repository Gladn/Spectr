using Spectr.Model;
using Spectr.Model.DataContext;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Linq;

namespace Spectr.ViewModel
{
    internal class DeviceViewModel : ViewModelBase
    {
        private ObservableCollection<Device> _device;

        public ObservableCollection<Device> Device
        {
            get { return _device; }
            set
            {
                if (_device != value)
                {
                    _device = value;
                    OnPropertyChanged(nameof(Device));
                }
            }
        }

        private void LoadData()
        {
            using (var context = new AppContext())
            {
                Device = new ObservableCollection<Device>(context.Device.ToList());
            }
        }




        public DeviceViewModel()
        {

           LoadData();
        }
    }
}
