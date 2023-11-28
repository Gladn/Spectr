using Spectr.Commands;
using Spectr.Model;
using Spectr.Model.DataContext;
using Spectr.View;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Xml.Linq;

namespace Spectr.ViewModel
{
    public class OrderViewModel : ViewModelBase
    {
        private ObservableCollection<RepairOrder> _orders;
        private RepairOrder _insertSelectedOrder;

        private ObservableCollection<Employer> _employers;

        public ObservableCollection<RepairOrder> Orders
        {
            get { return _orders; }
            set
            {
                if (_orders != value)
                {
                    _orders = value;
                    OnPropertyChanged(nameof(Orders));
                }
            }
        }

        public RepairOrder InsertSelectedOrder
        {
            get
            {

                return _insertSelectedOrder;
            }
            set
            {
                if (value != _insertSelectedOrder)
                {
                    _insertSelectedOrder = value;
                    OnPropertyChanged(nameof(InsertSelectedOrder));
                }
            }
        }

        public ObservableCollection<Employer> Employers
        {
            get { return _employers; }
            set
            {
                if (_employers != value)
                {
                    _employers = value;
                    OnPropertyChanged(nameof(Employers));
                }
            }
        }

        private Employer _selectedEmployer;
        public Employer SelectedEmployer
        {
            get { return _selectedEmployer; }
            set
            {
                if (_selectedEmployer != value)
                {
                    _selectedEmployer = value;
                    OnPropertyChanged(nameof(SelectedEmployer));
                }
            }
        }




        #region Отборажение данных        
        private DataTable _repairOrderDataTable;
        public DataTable RepairOrderDataTable
        {
            get { return _repairOrderDataTable; }
            set
            {
                _repairOrderDataTable = value;
                OnPropertyChanged(nameof(RepairOrderDataTable));
            }
        }

        public async void LoadDataRepairOrder()
        {
            RepairOrderDataTable = await LoadDataTableFromDatabaseAsync();
        }
        public async Task<DataTable> LoadDataTableFromDatabaseAsync()
        {
            DataTable dataTable = new DataTable();

            try
            {
                using (SqlConnection con = new SqlConnection(Properties.Settings.Default.con))
                {
                    await con.OpenAsync();

                    using (SqlCommand command = new SqlCommand("SELECT Repair.OrderID as 'ID',CONVERT(VARCHAR, DateStart, 103) AS 'Дата заказа',\r\n " +
                        "CONVERT(VARCHAR, PlainDateEnd, 103) AS 'Дата окончания', TotalCost as 'Цена',\r\n" +
                        "CONCAT(CustomerSecondName, ' ', LEFT(CustomerFirstName, 1) + '.',\r\n" +
                        "CASE WHEN LEN(CustomerPatronymic) > 0 THEN CONCAT(' ', LEFT(CustomerPatronymic, 1), '.') ELSE '' END) AS 'Заказчик',\r\n" +
                        "SerialNumber as 'Номер устройства',\r\nCONCAT(EmSecondName, ' ', LEFT(EmFirstName, 1) + '.') AS 'Работник',\r\n" +
                        "STRING_AGG(RepairCategory.Category, ', ') AS 'Категории', Comment as 'Комментарий'\r\nFROM Repair\r\n" +
                        "JOIN Employer ON Employer.EmployerID = Repair.EmployerID\r\nJOIN Customer ON Customer.CustomerID = Repair.CustomerID\r\n" +
                        "JOIN Device ON Device.DeviceID = Repair.DeviceID\r\nLEFT JOIN RepairCategoryJunction ON RepairCategoryJunction.OrderID = Repair.OrderID\r\n" +
                        "LEFT JOIN RepairCategory ON RepairCategoryJunction.CategoryID = RepairCategory.CategoryID\r\nGROUP BY Repair.OrderID, DateStart, PlainDateEnd, TotalCost,\r\n" +
                        "Customer.CustomerSecondName, Customer.CustomerFirstName, Customer.CustomerPatronymic,\r\nSerialNumber, EmFirstName, EmSecondName, Comment", con))

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        dataTable.Load(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка отображения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return dataTable;
        }



        private async Task EmLoadDataAsync()
        {
            Employers = await EmLoadDataFromDatabaseAsync();
            OnPropertyChanged(nameof(Employers));
        }



        private async Task<ObservableCollection<Employer>> EmLoadDataFromDatabaseAsync()
        {
            ObservableCollection<Employer> employers = new ObservableCollection<Employer>();

            try
            {
                using (SqlConnection con = new SqlConnection(Properties.Settings.Default.con))
                {
                    await con.OpenAsync();

                    using (SqlCommand command = new SqlCommand("SELECT * FROM Employer", con))
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Employer employer = new Employer
                            {
                                EmployerID = reader.GetInt32(reader.GetOrdinal("EmployerID")),
                                EmFirstName = reader.GetString(reader.GetOrdinal("EmFirstName")),
                                EmSecondName = reader.GetString(reader.GetOrdinal("EmSecondName")),
                                PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                                Salary = reader.GetDecimal(reader.GetOrdinal("Salary")),
                                PositionID = reader.GetInt32(reader.GetOrdinal("PositionID")),


                            };
                            employers.Add(employer);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка отображения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return employers;
        }
        #endregion

        #region Добавление данных
        
        // 1. Открыть окно с добалением
        // 2. Добавить информацию в базу
        public ICommand OpenAddOrderViewCommand { get; }

        private bool CanOpenAddOrderViewCommandExecute(object parameter)
        {           
            return true;
        }

        private void OnOpenAddOrderViewCommandExecuted(object parameter)
        {
            var window = new AddOrderView();
            window.ShowDialog();
        }
        #endregion















        public OrderViewModel()
        {
            Orders = new ObservableCollection<RepairOrder>();


            _ = EmLoadDataAsync();
 
            LoadDataRepairOrder();

            OpenAddOrderViewCommand = new LambdaCommand(OnOpenAddOrderViewCommandExecuted, CanOpenAddOrderViewCommandExecute);


        }
    }
}
