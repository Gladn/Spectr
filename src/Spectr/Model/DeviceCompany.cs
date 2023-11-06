using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Spectr.Model
{
    public class DeviceCompany
    {
        private int companyId;
        private string companyName;
        

        public int CompanyId { 
            get => companyId;  
            set 
            {
                if (companyId == value) return;
                companyId = value;
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
