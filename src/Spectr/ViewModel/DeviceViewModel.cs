using Spectr.Commands;
using Spectr.Model;
using Spectr.Model.DataContext;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Spectr.ViewModel
{
    internal class DeviceViewModel : ViewModelBase
    {
        private ObservableCollection<Device> _device;
        private Device _selectedDevice;
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

        public Device SelectedDevice
        {
            get => _selectedDevice;
            set
            {
                if (Equals(value, _selectedDevice)) return;
                _selectedDevice = value;
                OnPropertyChanged(nameof(SelectedDevice));
            }
        }

        private string _insertSerialNumber;
        private string _insertType;
        private string _insertCompany;
        private string _insertModel;
        private int _insertManufactureYear;
        public string InsertSerialNumber
        {
            get => _insertSerialNumber;
            set
            {
                if (value == _insertSerialNumber) return;
                _insertSerialNumber = value;
                OnPropertyChanged(nameof(InsertSerialNumber));
            }
        }
        public string InsertType
        {
            get => _insertType;
            set
            {
                if (value == _insertType) return;
                _insertType = value;
                OnPropertyChanged(nameof(InsertType));
            }
        }
        public string InsertCompany
        {
            get => _insertCompany;
            set
            {
                if (value == _insertCompany) return;
                _insertCompany = value;
                OnPropertyChanged(nameof(InsertCompany));
            }
        }
        public string InsertModel
        {
            get => _insertModel;
            set
            {
                if (value == _insertModel) return;
                _insertModel = value;
                OnPropertyChanged(nameof(InsertModel));
            }
        }
        public int InsertManufactureYear
        {
            get => _insertManufactureYear;
            set
            {
                if (value == _insertManufactureYear) return;
                _insertManufactureYear = value;
                OnPropertyChanged(nameof(InsertManufactureYear));
            }
        }


        #region Отображение Устройств

        private async void InitializeDataAsync()
        {
            await LoadDeviceData();
        }
        private async Task LoadDeviceData()
        {
            using (var context = new ApplicationContext())
            {
                var devices = await context.Device.ToListAsync();
                Device = new ObservableCollection<Device>(devices);
            }
        }

        #endregion


        #region Добавление устройства

        public ICommand AddDeviceCommand { get; }
        private bool CanAddDeviceCommandExecute(object parameter)
        {
            if (string.IsNullOrEmpty(InsertSerialNumber) ||
                string.IsNullOrEmpty(InsertType) ||
                string.IsNullOrEmpty(InsertCompany) ||
                string.IsNullOrEmpty(InsertModel))
            {
                return false;
            }
            return !HasErrors ? true : false;
        }

        private async void OnAddDeviceCommandExecuted(object parameter)
        {
            Device newDevice = new Device
            {
                SerialNumber = InsertSerialNumber,
                Type = InsertType,
                Company = InsertCompany,
                Model = InsertModel,
                ManufactureYear = InsertManufactureYear
            };

            using (var context = new ApplicationContext())
            {
                context.Device.Add(newDevice);
                await context.SaveChangesAsync();
            }

            Device.Add(newDevice);
            InsertSerialNumber = "";
            InsertType = "";
            InsertCompany = "";
            InsertModel = "";
            InsertManufactureYear = 0;

            InitializeDataAsync();
        }
        #endregion


        #region Удаление устройства
        public ICommand DeleteDeviceCommand { get; }
        private bool CanDeleteDeviceCommandExecute(object parameter)
        {
            return !HasErrors ? true : false;
        }
        private async void OnDeleteDeviceCommandExecuted(object parameter)
        {
            using (var context = new ApplicationContext())
            {
                if (context.Entry(SelectedDevice).State == EntityState.Detached)
                {
                    context.Device.Attach(SelectedDevice);
                }
                context.Device.Remove(SelectedDevice);
                await context.SaveChangesAsync();

                InitializeDataAsync();
            }
        }
        #endregion


        #region Редактирование устройства

        public ICommand UpdateDeviceCommand { get; }
        private bool CanUpdateDeviceCommandExecute(object parameter)
        {
            return !HasErrors ? true : false;
        }

        private async void OnUpdateDeviceCommandExecuted(object parameter)
        {
            if (SelectedDevice != null)
            {
                using (var context = new ApplicationContext())
                {
                    var existingDevice = await context.Device.FindAsync(SelectedDevice.DeviceID);

                    if (existingDevice != null)
                    {
                        existingDevice.SerialNumber = SelectedDevice.SerialNumber;
                        existingDevice.Type = SelectedDevice.Type;
                        existingDevice.Company = SelectedDevice.Company;
                        existingDevice.Model = SelectedDevice.Model;
                        existingDevice.ManufactureYear = SelectedDevice.ManufactureYear;

                        await context.SaveChangesAsync();
                    }
                }
            }

            InitializeDataAsync();
        }

        #endregion


        #region Валидация устройств

        private bool _hasErrors = false;
        public bool HasErrors
        {
            get => _hasErrors;
            private set
            {
                if (value != _hasErrors)
                {
                    _hasErrors = value;
                    OnPropertyChanged(nameof(HasErrors));
                }
            }
        }

        #endregion

        public DeviceViewModel()
        {
            Device = new ObservableCollection<Device>();
            InitializeDataAsync();

            AddDeviceCommand = new LambdaCommand(OnAddDeviceCommandExecuted, CanAddDeviceCommandExecute);

            UpdateDeviceCommand = new LambdaCommand(OnUpdateDeviceCommandExecuted, CanUpdateDeviceCommandExecute);

            DeleteDeviceCommand = new LambdaCommand(OnDeleteDeviceCommandExecuted, CanDeleteDeviceCommandExecute);
        }
    }
}
