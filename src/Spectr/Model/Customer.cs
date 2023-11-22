using System;


namespace Spectr.Model
{  
    public class Customer
    {
        private int _customerID;
        private string _docNumber;
        private string _customerFirstName;
        private string _customerSecondName;
        private string _customerPatronymic;
        private string _phoneNumber;
        private string _emailAdress;

        public int CustomerID
        {
            get => _customerID;
            set
            {
                if (value == _customerID) return;
                _customerID = value;
            }
        }
        
        public string DocNumber
        {
            get => _docNumber;
            set
            {
                if (value == _docNumber) return;
                _docNumber = value;
            }
        }

        public string CustomerFirstName
        {
            get => _customerFirstName;
            set
            {
                if (value == _customerFirstName) return;
                _customerFirstName = value;
            }
        }
        public string CustomerSecondName
        {
            get => _customerSecondName;
            set
            {
                if (value == _customerSecondName) return;
                _customerSecondName = value;
            }
        }
        public string CustomerPatronymic
        {
            get => _customerPatronymic;
            set
            {
                if (value == _customerPatronymic) return;
                _customerPatronymic = value;
            }
        }
        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                if (value == _phoneNumber) return;
                _phoneNumber = value;
            }
        }
        public string EmailAdress
        {
            get => _emailAdress;
            set
            {
                if (value == _emailAdress) return;
                _emailAdress = value;
            }
        }       
    }
}
