﻿using Spectr.Commands;
using Spectr.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
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
            get
            {

                return _insertSelectedCustomer;
            }
            set
            {
                if (value != _insertSelectedCustomer)
                {
                    _insertSelectedCustomer = value;
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

        private string _insertDocNumber;
        private string _insertCustomerFirstName;
        private string _insertCustomerSecondName;
        private string _insertCustomerPatronymic;
        private string _insertPhoneNumber;
        private string _insertEmailAdress;

        public string InsertDocNumber
        {
            get => _insertDocNumber;
            set
            {
                if (value == _insertDocNumber) return;
                _insertDocNumber = value;
                ValidateProperty(value, "InsertDocNumber");
                OnPropertyChanged(nameof(InsertDocNumber));
            }
        }

        public string InsertCustomerFirstName
        {
            get => _insertCustomerFirstName;
            set
            {
                if (value == _insertCustomerFirstName) return;
                _insertCustomerFirstName = value;
                ValidateProperty(value, "InsertCustomerFirstName");
                OnPropertyChanged(nameof(InsertCustomerFirstName));
            }
        }

        public string InsertCustomerSecondName
        {
            get => _insertCustomerSecondName;
            set
            {
                if (value == _insertCustomerSecondName) return;
                _insertCustomerSecondName = value;
               // ValidateProperty(value, "InsertCustomerSecondName");
                OnPropertyChanged(nameof(InsertCustomerSecondName));
                ValidateProperty(value, "InsertCustomerSecondName");
            }
        }

        public string InsertCustomerPatronymic
        {
            get => _insertCustomerPatronymic;
            set
            {
                if (value == _insertCustomerPatronymic) return;
                _insertCustomerPatronymic = value;
                ValidateProperty(value, "InsertCustomerPatronymic");
                OnPropertyChanged(nameof(InsertCustomerPatronymic));
            }
        }

        public string InsertPhoneNumber
        {
            get => _insertPhoneNumber;
            set
            {
                if (value == _insertPhoneNumber) return;
                _insertPhoneNumber = value;
                ValidateProperty(value, "InsertPhoneNumber");
                OnPropertyChanged(nameof(InsertPhoneNumber));
            }
        }

        public string InsertEmailAdress
        {
            get => _insertEmailAdress;
            set
            {
                if (value == _insertEmailAdress) return;
                _insertEmailAdress = value;
                ValidateProperty(value, "InsertEmailAdress");
                OnPropertyChanged(nameof(InsertEmailAdress));
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

            try
            {
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
                                CustomerSecondName = reader.GetString(reader.GetOrdinal("CustomerSecondName")),
                                CustomerPatronymic = reader.IsDBNull(reader.GetOrdinal("CustomerPatronymic")) ? null : reader.GetString(reader.GetOrdinal("CustomerPatronymic")),
                                PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                                EmailAdress = reader.IsDBNull(reader.GetOrdinal("EmailAdress")) ? null : reader.GetString(reader.GetOrdinal("EmailAdress")),
                            };
                            customers.Add(customer);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка отображения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return customers;
        }

        #endregion


        #region Добавление клиента
        public ICommand AddCustomerCommand { get; }
        private bool CanAddCustomerCommandExecute(object parameter)
        {
            if (string.IsNullOrEmpty(InsertDocNumber) ||
                string.IsNullOrEmpty(InsertCustomerFirstName) ||
                string.IsNullOrEmpty(InsertCustomerSecondName) ||
                string.IsNullOrEmpty(InsertPhoneNumber))
            {
                return false;
            }
            else if (HasErrors)
            {
                return false;
            }

            return true;
        }

        private async void OnAddCustomerExecuted(object parameter)
        {
            Customer newCustomer = new Customer
            {
                DocNumber = InsertDocNumber,
                CustomerFirstName = InsertCustomerFirstName,
                CustomerSecondName = InsertCustomerSecondName,
                CustomerPatronymic = InsertCustomerPatronymic,
                PhoneNumber = InsertPhoneNumber,
                EmailAdress = InsertEmailAdress
            };

            await AddCustomerAsync(newCustomer);
            Customers.Add(newCustomer);

            InsertDocNumber = "";
            InsertCustomerFirstName = "";
            InsertCustomerSecondName = "";
            InsertCustomerPatronymic = "";
            InsertPhoneNumber = "";
            InsertEmailAdress = "";

            await LoadDataAsync();
        }

        public async Task AddCustomerAsync(Customer customer)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(Properties.Settings.Default.con))
                {
                    await con.OpenAsync();

                    using (SqlCommand command = new SqlCommand("INSERT INTO Customer " +
                                                               "VALUES (@DocNumber, @CustomerFirstName, @CustomerSecondName, @CustomerPatronymic, @PhoneNumber, @EmailAdress)", con))
                    {
                        command.Parameters.AddWithValue("@DocNumber", customer.DocNumber);
                        command.Parameters.AddWithValue("@CustomerFirstName", customer.CustomerFirstName);
                        command.Parameters.AddWithValue("@CustomerSecondName", customer.CustomerSecondName);
                        command.Parameters.AddWithValue("@CustomerPatronymic", string.IsNullOrEmpty(customer.CustomerPatronymic) ? (object)DBNull.Value : customer.CustomerPatronymic);
                        command.Parameters.AddWithValue("@PhoneNumber", customer.PhoneNumber);
                        command.Parameters.AddWithValue("@EmailAdress", string.IsNullOrEmpty(customer.EmailAdress) ? (object)DBNull.Value : customer.EmailAdress);

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


        #region Удаление клиента
        public ICommand DeleteCustomerCommand { get; }
        private bool CanDeleteCustomerCommandExecute(object parameter)
        {
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
            try
            {
                using (SqlConnection con = new SqlConnection(Properties.Settings.Default.con))
                {
                    await con.OpenAsync();
                    using (SqlCommand command = new SqlCommand("DELETE FROM Customer WHERE CustomerID=@CustomerID", con))
                    {
                        command.Parameters.AddWithValue("@CustomerID", customer.CustomerID);
                        await command.ExecuteNonQueryAsync();
                    }
                }

                Customers.Remove(customer);
                OnPropertyChanged(nameof(Customers));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion


        #region Изменение клиента
        public ICommand UpdateCustomerCommand { get; }

        private bool CanUpdateCustomerCommandExecute(object parameter)
        {
            return true;
        }

        private async void OnUpdateCustomerExecuted(object parameter)
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
            try
            {
                using (SqlConnection con = new SqlConnection(Properties.Settings.Default.con))
                {
                    await con.OpenAsync();

                    using (SqlCommand command = new SqlCommand("UPDATE Customer " +
                                                               "SET DocNumber=@DocNumber, CustomerFirstName=@CustomerFirstName, " +
                                                               "CustomerSecondName=@CustomerSecondName, CustomerPatronymic=@CustomerPatronymic, " +
                                                               "PhoneNumber=@PhoneNumber, EmailAdress=@EmailAdress " +
                                                               "WHERE CustomerID=@CustomerID", con))
                    {
                        command.Parameters.AddWithValue("@CustomerID", customer.CustomerID);
                        command.Parameters.AddWithValue("@DocNumber", customer.DocNumber);
                        command.Parameters.AddWithValue("@CustomerFirstName", customer.CustomerFirstName);
                        command.Parameters.AddWithValue("@CustomerSecondName", customer.CustomerSecondName);
                        command.Parameters.AddWithValue("@CustomerPatronymic", customer.CustomerPatronymic);
                        command.Parameters.AddWithValue("@PhoneNumber", customer.PhoneNumber);
                        command.Parameters.AddWithValue("@EmailAdress", customer.EmailAdress);

                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion


        #region Валидация 

        private readonly Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        private bool _hasErrors;

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

        private void SetHasErrors()
        {
            HasErrors = _errors.Values.Any(list => list != null && list.Count > 0);
        }

        public IEnumerable GetErrors(string propertyName)
        {
            if (_errors.ContainsKey(propertyName))
            {
                return _errors[propertyName];
            }
            else
            {
                return null;
            }
        }

        private void AddError(string propertyName, string error)
        {
            if (!_errors.ContainsKey(propertyName))
            {
                _errors[propertyName] = new List<string>();
            }

            if (!_errors[propertyName].Contains(error))
            {
                _errors[propertyName].Add(error);
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }

        private void RemoveError(string propertyName)
        {
            if (_errors.ContainsKey(propertyName))
            {
                _errors.Remove(propertyName);
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }

        private void ValidateProperty(object value, string propertyName)
        {
            if (propertyName == "InsertPhoneNumber")
            {
                ValidatePhoneNumber(value, propertyName);
            }
            else
            {
                // Default validation for other properties
                if (value != null && value.ToString().Length > 15)
                {
                    AddError(propertyName, "Длинна не более 15 символов.");
                }
                else if (value == null || string.IsNullOrEmpty((string)value)) AddError(propertyName, "Длинна 0.");
                else
                {
                    RemoveError(propertyName);
                }

                SetHasErrors();
            }
        }

        private void ValidatePhoneNumber(object value, string propertyName)
        {
            if (value != null)
            {
                string phoneNumber = value.ToString();

                if (!phoneNumber.All(char.IsDigit) && !phoneNumber.All(c => char.IsWhiteSpace(c) || c == '(' || c == ')'))
                {
                    AddError(propertyName, "Номер телефона только числа.");
                }
                else
                {
                    RemoveError(propertyName);
                }
            }
            else
            {
                AddError(propertyName, "Нельзя пусто.");
            }

            SetHasErrors();
        }

        #endregion

        #region Визуалка

        public ICommand ClearCommand { get; }
        
        private bool CanClearCommandExecute(object parameter)
        {
            return true;
        }

        private void OnClearCommandExecuted(object parameter)
        {
            InsertDocNumber = "";
            InsertCustomerFirstName = "";
            InsertCustomerSecondName = "";
            InsertCustomerPatronymic = "";
            InsertPhoneNumber = "";
            InsertEmailAdress = "";
        }

        #endregion

        public CustomerViewModel()
        {
            Customers = new ObservableCollection<Customer>();

            Task task = LoadDataAsync();

            AddCustomerCommand = new LambdaCommand(OnAddCustomerExecuted, CanAddCustomerCommandExecute);

            DeleteCustomerCommand = new LambdaCommand(OnDeleteCustomerExecuted, CanDeleteCustomerCommandExecute);

            UpdateCustomerCommand = new LambdaCommand(OnUpdateCustomerExecuted, CanUpdateCustomerCommandExecute);

            ClearCommand = new LambdaCommand(OnClearCommandExecuted, CanClearCommandExecute);
        }
    }
}
