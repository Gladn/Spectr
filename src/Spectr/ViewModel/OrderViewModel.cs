using Spectr.Commands;
using Spectr.Model;
using Spectr.Model.DataContext;
using Spectr.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Xml.Linq;

namespace Spectr.ViewModel
{
    public class OrderViewModel : ViewModelBase
    {
        //private ObservableCollection<RepairOrder> _orders;
        //private RepairOrder _insertSelectedOrder;

        //public ObservableCollection<RepairOrder> Orders
        //{
        //    get { return _orders; }
        //    set
        //    {
        //        if (_orders != value)
        //        {
        //            _orders = value;
        //            OnPropertyChanged(nameof(Orders));
        //        }
        //    }
        //}

        //public RepairOrder InsertSelectedOrder
        //{
        //    get
        //    {

        //        return _insertSelectedOrder;
        //    }
        //    set
        //    {
        //        if (value != _insertSelectedOrder)
        //        {
        //            _insertSelectedOrder = value;
        //            OnPropertyChanged(nameof(InsertSelectedOrder));
        //        }
        //    }
        //}

        
        //private Employer _selectedEmployer;
        //public Employer SelectedEmployer
        //{
        //    get { return _selectedEmployer; }
        //    set
        //    {
        //        if (_selectedEmployer != value)
        //        {
        //            _selectedEmployer = value;
        //            OnPropertyChanged(nameof(SelectedEmployer));
        //        }
        //    }
        //}


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

        private DataTable _employerDataTable;
        public DataTable EmployerDataTable
        {
            get { return _employerDataTable; }
            set
            {
                _employerDataTable = value;
                OnPropertyChanged(nameof(EmployerDataTable));
            }
        }

        private DataTable _employerDataTableComboBox;
        public DataTable EmployerDataTableComboBox
        {
            get { return _employerDataTableComboBox; }
            set
            {
                _employerDataTableComboBox = value;
                OnPropertyChanged(nameof(EmployerDataTableComboBox));
            }
        }

        private DataTable _customerDataTable;
        public DataTable CustomerDataTable
        {
            get { return _customerDataTable; }
            set
            {
                _customerDataTable = value;
                OnPropertyChanged(nameof(CustomerDataTable));
            
            }
        }

        private DataTable _deviceDataTable;
        public DataTable DeviceDataTable
        {
            get { return _deviceDataTable; }
            set
            {
                _deviceDataTable = value;
                OnPropertyChanged(nameof(DeviceDataTable));
            
            }
        }

        private ICollectionView _customerDataView;
        public ICollectionView CustomerDataView
        {
            get { return _customerDataView; }
            set
            {
                if (_customerDataView != value)
                {
                    _customerDataView = value;
                    OnPropertyChanged(nameof(CustomerDataView));
                }
            }
        }

        
        private async Task LoadAllDataAsync()
        {

            List<DataTable> dataTablesLoaded = await LoadDataFromDatabaseAsync();

            
            EmployerDataTable = dataTablesLoaded[0];
   
            DataTable partialDataTable = dataTablesLoaded[0];
            foreach (DataColumn column in partialDataTable.Columns.Cast<DataColumn>().ToArray())
            {
                if (column.ColumnName != "ID" && column.ColumnName != "Работник")
                {
                    partialDataTable.Columns.Remove(column);
                }
            }
           
            EmployerDataTableComboBox = partialDataTable;

            CustomerDataView = CollectionViewSource.GetDefaultView(dataTablesLoaded[1]);

            CustomerDataTable = dataTablesLoaded[1];

            DeviceDataTable = dataTablesLoaded[2];
        }


        public async Task<List<DataTable>> LoadDataFromDatabaseAsync()
        {
            //DataTable dataTable = new DataTable();
            List<DataTable> dataTables = new List<DataTable>();

            try
            {
                using (SqlConnection con = new SqlConnection(Properties.Settings.Default.con))
                {
                    await con.OpenAsync();

                    using (SqlCommand command = new SqlCommand("SELECT EmployerID as 'ID', CONCAT(EmSecondName, ' ', LEFT(EmFirstName, 1) + '.') AS 'Работник', " +
                        "PhoneNumber as 'Номер телефона', Salary as 'Зарплата', " +
                        "Employer.PositionID as 'ID', PositionName as 'Должность'" +
                        "FROM Employer " +
                        "Join EmployerPosition ON EmployerPosition.PositionID = Employer.PositionID", con))
                    
                    using (SqlDataReader reader1 = await command.ExecuteReaderAsync())
                    {
                        DataTable dataTable1 = new DataTable();
                        dataTable1.Load(reader1);
                        dataTables.Add(dataTable1);
                    }
                    
                    using (SqlCommand command = new SqlCommand("SELECT CustomerID as 'ID', DocNumber as 'Документ', CONCAT(CustomerSecondName, ' ', LEFT(CustomerFirstName, 1) + '.', " +
                        "CASE WHEN LEN(CustomerPatronymic) > 0 THEN CONCAT(' ', LEFT(CustomerPatronymic, 1), '.') ELSE '' END) AS 'Клиент' FROM Customer; ", con))
                    
                    using (SqlDataReader reader2 = await command.ExecuteReaderAsync())
                    {
                        DataTable dataTable2 = new DataTable();
                        dataTable2.Load(reader2);
                        dataTables.Add(dataTable2);
                    }

                    using (SqlCommand command = new SqlCommand("SELECT DeviceID as 'ID', SerialNumber as 'Серийный_номер' FROM Device; ", con))
                    
                    using (SqlDataReader reader3 = await command.ExecuteReaderAsync())
                    {
                        DataTable dataTable3 = new DataTable();
                        dataTable3.Load(reader3);
                        dataTables.Add(dataTable3);
                    }
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка отображения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return dataTables;          
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

        #region Фильтры вкладки добавления

        private string _customerFilterText;
        public string CustomerFilterText
        {
            get { return _customerFilterText; }
            set
            {
                _customerFilterText = value;
                OnPropertyChanged(nameof(CustomerFilterText));
                ApplyFilterCustomer();
            }
        }

        private void ApplyFilterCustomer()
        {
            if (CustomerDataTable != null)
            {
                CustomerDataTable.DefaultView.RowFilter = $"Клиент LIKE '%{CustomerFilterText}%' OR Документ LIKE '%{CustomerFilterText}%'";
            }
        }

        private string _deviceFilterText;
        public string DeviceFilterText
        {
            get { return _deviceFilterText; }
            set
            {
                _deviceFilterText = value;
                OnPropertyChanged(nameof(DeviceFilterText));
                ApplyFilterDevice();
            }
        }

        private void ApplyFilterDevice()
        {
            if (DeviceDataTable != null)
            {
                DeviceDataTable.DefaultView.RowFilter = $"Серийный_номер LIKE '%{DeviceFilterText}%'";
            }
        }

        #endregion



        

        public ICommand AddOrderCommand { get; }

        private bool CanAddOrderCommandExecute(object parameter)
        {
            return true;
        }


        private async void OnAddOrderCommandExecuted(object parameter)
        {
            //Customer newCustomer = new Customer
            //{

            //};

            ////await AddCustomerAsync(newCustomer);
            ////Customers.Add(newCustomer);

            //InsertDocNumber = "";
            //InsertCustomerFirstName = "";
            //InsertCustomerSecondName = "";
            //InsertCustomerPatronymic = "";
            //InsertPhoneNumber = "";
            //InsertEmailAdress = "";
    


        }

        public async Task AddRepairOrderAsync(RepairOrder order)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(Properties.Settings.Default.con))
                {
                    await con.OpenAsync();

                    using (SqlCommand command = new SqlCommand("INSERT INTO Repair " +
                                                               "VALUES (@DateStart, @DateEnd, @CustomerID, @DeviceID, @EmployerID, @PlainDateEnd, @Status, @Discount, @TotalCost, @Comment)", con))
                    {
                        command.Parameters.AddWithValue("@DateStart", order.DateStart);
                        command.Parameters.AddWithValue("@PlainDateEnd", order.PlainDateEnd);
                        command.Parameters.AddWithValue("@CustomerID", order.CustomerID);
                        command.Parameters.AddWithValue("@DeviceID", order.DeviceID);
                        command.Parameters.AddWithValue("@EmployerID", order.EmployerID);
                        command.Parameters.AddWithValue("@Status", order.Status);
                        command.Parameters.AddWithValue("@Discount", order.Discount.HasValue ? (object)order.Discount.Value : DBNull.Value);
                        command.Parameters.AddWithValue("@TotalCost", order.TotalCost);
                        command.Parameters.AddWithValue("@Comment", string.IsNullOrEmpty(order.Comment) ? (object)order.Comment : DBNull.Value;

                        await command.ExecuteNonQueryAsync();
                   
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка добавления: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }





        #endregion

        /// <summary>
        /// ToDO:
        /// </summary>




        public OrderViewModel()
        {
            //Orders = new ObservableCollection<RepairOrder>();            

            LoadDataRepairOrder();

            OpenAddOrderViewCommand = new LambdaCommand(OnOpenAddOrderViewCommandExecuted, CanOpenAddOrderViewCommandExecute);

            _ = LoadAllDataAsync();

            AddOrderCommand = new LambdaCommand(OnAddOrderCommandExecuted, CanAddOrderCommandExecute);

            


        }

        
    }
}
