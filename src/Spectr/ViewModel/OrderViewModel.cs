using Spectr.Commands;
using Spectr.Model;
using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows;

namespace Spectr.ViewModel
{
    public class OrderViewModel : ViewModelBase
    {
        private ObservableCollection<RepairOrder> _orders;
        private RepairOrder _insertSelectedOrder;

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



        #region Отборажение данных
        private async Task LoadDataAsync()
        {
            Orders = await LoadDataFromDatabaseAsync();
        }

        private async Task<ObservableCollection<RepairOrder>> LoadDataFromDatabaseAsync()
        {
            ObservableCollection<RepairOrder> orders = new ObservableCollection<RepairOrder>();

            try
            {
                using (SqlConnection con = new SqlConnection(Properties.Settings.Default.con))
                {
                    await con.OpenAsync();

                    using (SqlCommand command = new SqlCommand("select OrderID, DateStart, Customer.CustomerID, " +
                                                               "DocNumber, CustomerFirstName, CustomerSecontName, CustomerPatronymic,\r\n" +
                                                               "Device.DeviceID, SerialNumber, Device.Model, Employer.EmployerID, " +
                                                               "EmFirstName, EmSecondName,\r\nPlainDateEnd, Status, Discount, " +
                                                               "TotalCost, Comment from Repair\r\n" +
                                                               "join Employer on Employer.EmployerID = Repair.EmployerID\r\n" +
                                                               "join Customer on Customer.CustomerID = Repair.CustomerID\r\n" +
                                                               "join Device on Device.DeviceID = Repair.DeviceID", con))

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            RepairOrder order = new RepairOrder
                            {
                                OrderID = reader.GetInt32(reader.GetOrdinal("OrderID")),
                                DateStart = reader.GetDateTime(reader.GetOrdinal("DateStart")),
                                CustomerID = reader.GetInt32(reader.GetOrdinal("CustomerID")),
                                DeviceID = reader.GetInt32(reader.GetOrdinal("DeviceID")),
                                EmployerID = reader.GetInt32(reader.GetOrdinal("EmployerID")),
                                PlainDateEnd = reader.GetDateTime(reader.GetOrdinal("PlainDateEnd")),
                                Status = reader.GetBoolean(reader.GetOrdinal("Status")),
                                Discount = reader.IsDBNull(reader.GetOrdinal("Discount")) ? null : (decimal?)reader.GetDecimal(reader.GetOrdinal("Discount")),
                                TotalCost = reader.GetDecimal(reader.GetOrdinal("TotalCost")),
                                Comment = reader.IsDBNull(reader.GetOrdinal("Comment")) ? null : reader.GetString(reader.GetOrdinal("Comment")),
                            };
                            order.Customer = new Customer
                            {
                                CustomerID = reader.GetInt32(reader.GetOrdinal("CustomerID")),
                                DocNumber = reader.GetString(reader.GetOrdinal("DocNumber")),
                                CustomerFirstName = reader.GetString(reader.GetOrdinal("CustomerFirstName")),
                                CustomerSecontName = reader.GetString(reader.GetOrdinal("CustomerSecontName")),
                                CustomerPatronymic = reader.IsDBNull(reader.GetOrdinal("CustomerPatronymic")) ? null : reader.GetString(reader.GetOrdinal("CustomerPatronymic")),
                            };

                            order.Device = new Device
                            {
                                DeviceID = reader.GetInt32(reader.GetOrdinal("DeviceID")),
                                SerialNumber = reader.GetString(reader.GetOrdinal("SerialNumber")),
                                Model = reader.GetString(reader.GetOrdinal("Model")),
                            };

                            order.Employer = new Employer
                            {
                                EmployerID = reader.GetInt32(reader.GetOrdinal("EmployerID")),
                                EmFirstName = reader.GetString(reader.GetOrdinal("EmFirstName")),
                                EmSecondName = reader.GetString(reader.GetOrdinal("EmSecondName")),
                            };
                            orders.Add(order);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка отображения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return orders;
        }

        #endregion



        public OrderViewModel()
        {
            Orders = new ObservableCollection<RepairOrder>();

            Task task = LoadDataAsync();


        }
    }
}
