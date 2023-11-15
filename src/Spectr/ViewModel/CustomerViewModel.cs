using Spectr.Model;
using Spectr.Commands;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
<<<<<<< Updated upstream
=======
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
>>>>>>> Stashed changes
using System.Threading.Tasks;
using System.Windows.Input;


namespace Spectr.ViewModel
{
    /// <summary>
    /// Модель представления клиентов с бд
    /// </summary>
    public class CustomerViewModel : ViewModelBase, INotifyDataErrorInfo
    {
        private ObservableCollection<Customer> _customers;
        private Customer _insertSelectedCustomer;
        private Customer _updateSelectedCustomer;
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

        public Customer InsertSelectedCustomer
        {
<<<<<<< Updated upstream
            get => _insertSelectedCustomer;
=======
            get
            {
                if (_insertSelectedCustomer != null && _insertSelectedCustomer.DocNumber != null && _insertSelectedCustomer.DocNumber.Length > 4)
                {
                    AddError("The DocNumber must be at least 4 characters long.", nameof(InsertSelectedCustomer.DocNumber));
                }
                return _insertSelectedCustomer;
            }
>>>>>>> Stashed changes
            set
            {
                if (value != _insertSelectedCustomer)
                {
                    _insertSelectedCustomer = value;
                    ValidateDocNumber();
                    OnPropertyChanged(nameof(InsertSelectedCustomer));
                }
            }
        }


        public Customer UpdateSelectedCustomer
        {
            get => _updateSelectedCustomer;
            set
            {
                if (Equals(value, _updateSelectedCustomer)) return;
                _updateSelectedCustomer = value;
                OnPropertyChanged(nameof(UpdateSelectedCustomer));
            }
        }


        #region Отборажение данных
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

        #endregion


        #region Добавление клиента
        public ICommand AddCustomerCommand { get; }
        private bool CanAddCustomerCommandExecute(object parameter)
        {
            // Логика, чтобы определить, можно ли выполнить добавление
            return true;
        }

        private async void OnAddCustomerExecuted(object parameter)
        {
            if (InsertSelectedCustomer != null)
            {
                await AddCustomerAsync(InsertSelectedCustomer);
                Customers.Add(InsertSelectedCustomer);
                InsertSelectedCustomer = new Customer();
                await LoadDataAsync(); 
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

        private async void OnDeleteCustomerExecuted(object parameter)
        {
            if (parameter is Customer customer)
            {
                await DeleteCustomerAsync(customer);
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


        #region Изменение клиента
        public ICommand UpdateCustomerCommand { get; }
        private bool CanUpdateCustomerCommandExecute(object parameter)
        {
            return true;
        }

        private async void OnUpdateCustomerExecutedAsync(object parameter)
        {
            if (UpdateSelectedCustomer != null)
            {
                await UpdateCustomerAsync(UpdateSelectedCustomer);
                OnPropertyChanged(nameof(UpdateSelectedCustomer));
                await LoadDataAsync(); 
            }
        }

        public async Task UpdateCustomerAsync(Customer customer)
        {
            using (SqlConnection con = new SqlConnection(Properties.Settings.Default.con))
            {
                await con.OpenAsync();

                using (SqlCommand command = new SqlCommand("UPDATE Customer " +
                                                           "SET DocNumber=@DocNumber, CustomerFirstName=@CustomerFirstName, " +
                                                           "CustomerSecontName=@CustomerSecontName, CustomerPatronymic=@CustomerPatronymic, " +
                                                           "PhoneNumber=@PhoneNumber, EmailAdress=@EmailAdress " +
                                                           "WHERE CustomerID=@CustomerID", con))
                {
                    command.Parameters.AddWithValue("@CustomerID", customer.CustomerID);
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

<<<<<<< Updated upstream
=======

        private Dictionary<string, List<string>> _propertyNameToErrorsDictionary;

        public bool HasErrors => _propertyNameToErrorsDictionary.Any();

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        private void ValidateDocNumber()
        {
            if (InsertSelectedCustomer.DocNumber != null) ClearErrors(nameof(InsertSelectedCustomer.DocNumber));
            else  if (_insertSelectedCustomer != null && _insertSelectedCustomer.DocNumber != null && _insertSelectedCustomer.DocNumber.Length > 4)
            {
                AddError("The DocNumber must be at least 4 characters long.", nameof(InsertSelectedCustomer.DocNumber));
            }
        }

        public IEnumerable GetErrors(string propertyName)
        {
            if (_propertyNameToErrorsDictionary.ContainsKey(propertyName))
            {
                return _propertyNameToErrorsDictionary[propertyName];
            }
            else
            {
                return null;
            }
        }



        private void AddError(string errorMessage, string propertyName)
        {
            if (_propertyNameToErrorsDictionary == null)
            {
                _propertyNameToErrorsDictionary = new Dictionary<string, List<string>>();
            }

            if (!_propertyNameToErrorsDictionary.ContainsKey(propertyName))
            {
                _propertyNameToErrorsDictionary.Add(propertyName, new List<string>());
            }

            _propertyNameToErrorsDictionary[propertyName].Add(errorMessage);

            OnErrorsChanged(propertyName);
        }

        private void ClearErrors(string propertyName)
        {
            _propertyNameToErrorsDictionary.Remove(propertyName);

            OnErrorsChanged(propertyName);
        }

        private void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

>>>>>>> Stashed changes

        public CustomerViewModel()
        {
            Customers = new ObservableCollection<Customer>();
            InsertSelectedCustomer = new Customer();

<<<<<<< Updated upstream
            LoadDataAsync();
=======
            Task task = LoadDataAsync();
>>>>>>> Stashed changes

            AddCustomerCommand = new LambdaCommand(OnAddCustomerExecuted, CanAddCustomerCommandExecute);

            DeleteCustomerCommand = new LambdaCommand(OnDeleteCustomerExecuted, CanDeleteCustomerCommandExecute);

            UpdateCustomerCommand = new LambdaCommand(OnUpdateCustomerExecutedAsync, CanUpdateCustomerCommandExecute);
<<<<<<< Updated upstream
=======

           
            
            
>>>>>>> Stashed changes
        }
    }
}
