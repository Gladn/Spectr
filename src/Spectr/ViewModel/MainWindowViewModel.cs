using Spectr.Commands;
using Spectr.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Spectr.ViewModel
{
    internal class MainWindowViewModel : ViewModelBase
    {
        private readonly CustomerViewModel _customerViewModel;

        private ObservableCollection<Customer> _customers;
        public ObservableCollection<Customer> Customers
        {
            get { return _customers; }
            set { Set(ref _customers, value); }
        }


        #region Визуалка
        #region Заголовок
        private string _Title = "Мастерская ремонта Spectr";

        public string Title { 
            get { return _Title; } 
            set {
                if (Equals(_Title, value)) return;
                _Title = value;
                OnPropertyChanged();
            } 
        }
        #endregion

        #region Статус программы
        private string _Status = "Done";

        public string Status
        {
            get => _Status;
            set => Set(ref _Status, value);
        }
        #endregion
        #endregion


        #region Комманды

        #region Комманда закрытия приложения
        public ICommand CloseAppCommand { get; }

        private bool CanCloseAppCommandExecute(object p) => true;

        private void OnCloseAppCommandExecuted(object p)
        {
            Application.Current.Shutdown();
        }
        #endregion
        
        #endregion
        public MainWindowViewModel()
        {
            #region Команды

            CloseAppCommand = new LambdaCommand(OnCloseAppCommandExecuted, CanCloseAppCommandExecute);

            #endregion

            _customerViewModel = new CustomerViewModel();
            Task.Run(async () => await InitializeDataAsync());
        }

        private async Task InitializeDataAsync()
        {
            await _customerViewModel.LoadDataAsync();
            Customers = _customerViewModel.Customers;
        }
    }
}
