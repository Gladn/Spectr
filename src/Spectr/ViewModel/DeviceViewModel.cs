using Spectr.Model;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Spectr.ViewModel
{
    internal class DeviceViewModel : ViewModelBase
    {
        private ObservableCollection<Device> _devices;

        public ObservableCollection<Device> Devices
        {
            get { return _devices; }
            set
            {
                if (_devices != value)
                {
                    _devices = value;
                    OnPropertyChanged(nameof(Devices));
                }
            }
        }


        #region Отборажение данных
        private async Task LoadDataAsync()
        {
            Devices = await LoadDataFromDatabaseAsync();
        }

        private async Task<ObservableCollection<Device>> LoadDataFromDatabaseAsync()
        {
            ObservableCollection<Device> devices = new ObservableCollection<Device>();

            using (SqlConnection con = new SqlConnection(Properties.Settings.Default.con))
            {
                await con.OpenAsync();

                using (SqlCommand command = new SqlCommand("SELECT * FROM Device", con))
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Device device = new Device
                        {
                            DeviceID = reader.GetInt32(reader.GetOrdinal("DeviceID")),
                            SerialNumber = reader.GetString(reader.GetOrdinal("SerialNumber")),
                            Type = reader.GetString(reader.GetOrdinal("Type")),
                            Company = reader.GetString(reader.GetOrdinal("Company")),
                            Model = reader.GetString(reader.GetOrdinal("Model")),
                            ManufactureYear = reader.GetInt32(reader.GetOrdinal("ManufactureYear"))
                        };
                        devices.Add(device);
                    }
                }
            }
            return devices;
        }

        #endregion

        public DeviceViewModel()
        {
            Devices = new ObservableCollection<Device>();
            
            LoadDataAsync();
        }
    }
}
