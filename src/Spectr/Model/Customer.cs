using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace Spectr.Model
{
    public class Customer : INotifyPropertyChanged
    {
        private int _customerID;
        private string _customerFirstName;
        private string _customerSecontName;
        private string _docNumber;

        public int CustomerID
        {
            get => _customerID;
            set
            {
                if (value == _customerID) return;
                _customerID = value;
                OnPropertyChanged();
            }
        }

        public string CustomerFirstName
        {
            get => _customerFirstName;
            set
            {
                if (value == _customerFirstName) return;
                _customerFirstName = value;
                OnPropertyChanged();
            }
        }

        public string CustomerSecontName
        {
            get => _customerSecontName;
            set
            {
                if (value == _customerSecontName) return;
                _customerSecontName = value;
                OnPropertyChanged();
            }
        }

        public string DocNumber
        {
            get => _docNumber;
            set
            {
                if (value == _docNumber) return;
                _docNumber = value;
                OnPropertyChanged();
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
