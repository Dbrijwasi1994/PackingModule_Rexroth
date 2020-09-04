using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeburringProcess_Peekay.Models
{
    class CustomerInfoEntity : INotifyPropertyChanged
    {
        public CustomerInfoEntity()
        {
            Country = "India";
        }

        internal bool IsRowChanged = false;

        private string customerID;
        public string CustomerID
        {
            get { return customerID; }
            set
            {
                customerID = value;
                NotifyPropertyChanged("CustomerID");
            }
        }

        private string customerName;
        public string CustomerName
        {
            get { return customerName; }
            set
            {
                customerName = value;
                NotifyPropertyChanged("CustomerName");
            }
        }

        private string address;
        public string Address
        {
            get { return address; }
            set
            {
                address = value;
                NotifyPropertyChanged("Address");
            }
        }

        private string place;
        public string Place
        {
            get { return place; }
            set
            {
                place = value;
                NotifyPropertyChanged("Place");
            }
        }

        private string state;
        public string State
        {
            get { return state; }
            set
            {
                state = value;
                NotifyPropertyChanged("State");
            }
        }

        private string country;
        public string Country
        {
            get { return country; }
            set
            {
                country = value;
                NotifyPropertyChanged("Country");
            }
        }

        private string pin;
        public string Pin
        {
            get { return pin; }
            set
            {
                pin = value;
                NotifyPropertyChanged("Pin");
            }
        }

        private string phone;
        public string Phone
        {
            get { return phone; }
            set
            {
                phone = value;
                NotifyPropertyChanged("Phone");
            }
        }

        private string email;
        public string Email
        {
            get { return email; }
            set
            {
                email = value;
                NotifyPropertyChanged("Email");
            }
        }

        private string contactPerson;
        public string ContactPerson
        {
            get { return contactPerson; }
            set
            {
                contactPerson = value;
                NotifyPropertyChanged("ContactPerson");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                IsRowChanged = true;
            }
        }
    }
}
