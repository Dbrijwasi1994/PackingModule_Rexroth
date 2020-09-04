using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DeburringProcess_Peekay.Models
{
    public class EmpInfoEntity : INotifyPropertyChanged, IDataErrorInfo
    {
        internal bool IsRowchnaged;

        public EmpInfoEntity()
        {
            _EmpId = string.Empty;
            _Name = string.Empty;
            _Phone = string.Empty;
            _IsAdmin = false;
            _Email = string.Empty;
            _Password = string.Empty;
            _EmployeeRole = string.Empty;
        }

        private string _EmpId;
        public string EmpId
        {
            get { return _EmpId; }
            set
            {
                _EmpId = value;
                NotifyPropertyChanged("EmpId");
            }
        }

        private string _Name;
        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                NotifyPropertyChanged("Name");
            }
        }

        private string _Phone;
        public string Phone
        {
            get { return _Phone; }
            set
            {
                _Phone = value;
                NotifyPropertyChanged("Phone");
            }
        }

        private bool _IsAdmin;
        public bool IsAdmin
        {
            get { return _IsAdmin; }
            set
            {
                _IsAdmin = value;
                NotifyPropertyChanged("IsAdmin");
            }
        }

        private string _Email;
        public string Email
        {
            get { return _Email; }
            set
            {
                _Email = value;
                NotifyPropertyChanged("Email");
            }
        }

        private string _Password;
        public string Password
        {
            get { return _Password; }
            set
            {
                _Password = value;
                NotifyPropertyChanged("Password");
            }
        }

        private string _EmployeeRole;
        public string EmployeeRole
        {
            get { return _EmployeeRole; }
            set
            {
                _EmployeeRole = value;
                NotifyPropertyChanged("EmployeeRole");
            }
        }

        public string Error
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsNewItem { get; internal set; }

        public string this[string columnName]
        {
            get
            {
                string result = string.Empty;
                switch (columnName)
                {
                    case "EmployeeID":
                        if (string.IsNullOrEmpty(this.EmpId))
                            result = "Employee ID cannot be empty";
                        break;
                    case "FirstName":
                        if (string.IsNullOrEmpty(this.Name))
                            result = "Name cannot be empty";
                        break;
                    case "Phone":
                        if (Regex.IsMatch(this.Phone, @"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$") == false)
                        {
                            result = "Please enter a valid Phone Number";
                        }
                        if (string.IsNullOrEmpty(this.Phone))
                            result = "Phone Number cannot be empty";
                        break;
                    case "Email":
                        if (Regex.IsMatch(this.Email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase) == false)
                            result = "Please enter a valid Email Address";
                        if (string.IsNullOrEmpty(this.Email))
                            result = "Email cannot be empty";
                        break;
                    case "Password":
                        if (string.IsNullOrEmpty(this.Password))
                            result = "Password cannot be empty";
                        break;
                }
                return result;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                IsRowchnaged = true;
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
