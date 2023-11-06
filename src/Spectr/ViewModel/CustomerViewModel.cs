using Spectr.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Threading.Tasks;


namespace Spectr.ViewModel
{
    /// <summary>
    /// Модель представления клиентов с бд
    /// </summary>
    public class CustomerViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Customer> _customers;

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

        public CustomerViewModel()
        {
            LoadDataAsync();
        }

        public async Task LoadDataAsync()
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
                            CustomerFirstName = reader.GetString(reader.GetOrdinal("CustomerFirstName")),
                            CustomerSecontName = reader.GetString(reader.GetOrdinal("CustomerSecontName")),
                            DocNumber = reader.GetString(reader.GetOrdinal("DocNumber"))
                        };
                        customers.Add(customer);
                    }
                }
            }

            return customers;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
