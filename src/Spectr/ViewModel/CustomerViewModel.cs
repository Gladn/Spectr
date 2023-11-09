using Spectr.Model;
using Spectr.Commands;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Diagnostics;

namespace Spectr.ViewModel
{
    /// <summary>
    /// Модель представления клиентов с бд
    /// </summary>
    internal class CustomerViewModel : ViewModelBase
    {

        private ObservableCollection<Customer> _customers;
        private Customer _selectedCustomer;
        public ObservableCollection<Customer> Customers
        {
            get { return _customers; }
            set
            {
                if (_customers != value)
                {
                    _customers = value;
                    OnPropertyChanged(nameof(Customers));
                }
            }
        }
        public Customer SelectedCustomer
        {
            get => _selectedCustomer;
            set
            {
                if (Equals(value, _selectedCustomer)) return;
                _selectedCustomer = value;
                OnPropertyChanged();
            }
        }

        private async Task LoadDataAsync()
        {
            Customers = await LoadDataFromDatabaseAsync();
        }

        private async Task<ObservableCollection<Customer>> LoadDataFromDatabaseAsync()
        {
            ObservableCollection<Customer> customers = new ObservableCollection<Customer>();

            using (SqlConnection con = new SqlConnection(Properties.Settings.Default.con))
            {
                await con.OpenAsync();

                using (SqlCommand command = new SqlCommand("SELECT * FROM Customer", con))
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Customer customer = new Customer
                        {
                            CustomerID = reader.GetInt32(reader.GetOrdinal("CustomerID")),
                            DocNumber = reader.GetString(reader.GetOrdinal("DocNumber")),
                            CustomerFirstName = reader.GetString(reader.GetOrdinal("CustomerFirstName")),
                            CustomerSecontName = reader.GetString(reader.GetOrdinal("CustomerSecontName")),
                            CustomerPatronymic = reader.GetString(reader.GetOrdinal("CustomerPatronymic")),
                            PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                            EmailAdress = reader.GetString(reader.GetOrdinal("EmailAdress"))
                        };
                        customers.Add(customer);
                    }
                }
            }
            return customers;
        }

        #region Добавление клиента

        public ICommand AddCustomerCommand { get; }

        private bool CanAddCustomerCommandExecute(object parameter)
        {
            // Логика, чтобы определить, можно ли выполнить добавление
            return true;
        }

        private async void OnAddCustomerExecuted(object parameter)
        {
            if (SelectedCustomer != null)
            {
                await AddCustomerAsync(SelectedCustomer);
                Customers.Add(SelectedCustomer);
                SelectedCustomer = new Customer();
                await LoadDataAsync(); // Обновите данные из базы после добавления клиента
            }
        }

        public async Task AddCustomerAsync(Customer customer)
        {
            using (SqlConnection con = new SqlConnection(Properties.Settings.Default.con))
            {
                await con.OpenAsync();

                using (SqlCommand command = new SqlCommand("INSERT INTO Customer " +
                                                           "VALUES (@DocNumber, @CustomerFirstName, @CustomerSecontName, @CustomerPatronymic, @PhoneNumber, @EmailAdress)", con))
                {
                    command.Parameters.AddWithValue("@DocNumber", customer.DocNumber);
                    command.Parameters.AddWithValue("@CustomerFirstName", customer.CustomerFirstName);
                    command.Parameters.AddWithValue("@CustomerSecontName", customer.CustomerSecontName);
                    command.Parameters.AddWithValue("@CustomerPatronymic", customer.CustomerPatronymic);
                    command.Parameters.AddWithValue("@PhoneNumber", customer.PhoneNumber);
                    command.Parameters.AddWithValue("@EmailAdress", customer.EmailAdress);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        #endregion


        #region Удаление клиента

        public ICommand DeleteCustomerCommand { get;  }

        private bool CanDeleteCustomerCommandExecute(object parameter)
        {
            // Логика, чтобы определить, можно ли выполнить удаление
            return true;
        }

        private void OnDeleteCustomerExecuted(object parameter)
        {

            if (parameter is Customer customer)
            {
                DeleteCustomerAsync(customer);
            }
        }

        public async Task DeleteCustomerAsync(Customer customer)
        {
            using (SqlConnection con = new SqlConnection(Properties.Settings.Default.con))
            {
                await con.OpenAsync();

                using (SqlCommand command = new SqlCommand("DELETE FROM Customer WHERE CustomerID=@CustomerID", con))
                {
                    command.Parameters.AddWithValue("@CustomerID", customer.CustomerID);
                    command.ExecuteNonQuery(); 
                }
            }

            Customers.Remove(customer);
            OnPropertyChanged(nameof(Customers));          
        }

        #endregion

        public CustomerViewModel()
        {
            Customers = new ObservableCollection<Customer>();
            SelectedCustomer = new Customer();

            LoadDataAsync();


            AddCustomerCommand = new LambdaCommand(OnAddCustomerExecuted, CanAddCustomerCommandExecute);

            DeleteCustomerCommand = new LambdaCommand(OnDeleteCustomerExecuted, CanDeleteCustomerCommandExecute);      
        }
    }
}
