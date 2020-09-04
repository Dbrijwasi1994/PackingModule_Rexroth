using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackingModule_Rexroth.Models
{
    class MonthlyScheduleMasterEntity : INotifyPropertyChanged
    {
        public MonthlyScheduleMasterEntity()
        {
            DispatchQty = 0;
        }

        public MonthlyScheduleMasterEntity(int slNo, string woNumber, string pumpPartNo, string custName, string packaging_Type, int dispatchQuantity, int priority, DateTime date, string customermodel, int monthSchedule, int scheduleWeek1, int scheduleWeek2, int scheduleWeek3, int scheduleWeek4, int ScheduleWeek5, int one, int two, int three, int four, int five, int six, int seven, int eight, int nine, int ten, int eleven, int twelve, int thirteen, int fourteen, int fifteen, int sixteen, int seventeen, int eighteen, int ninteen, int twenty, int twentyone, int twentytwo, int twentythree, int twentyfour, int twentyfive, int twentysix, int twentyseven, int twentyeight, int twentynine, int thirty, int thirtyone)
        {
            this.SerialNum = slNo;
            this.WorkOrderNumber = woNumber;
            this.PumpPartNumber = pumpPartNo;
            this.CustomerName = custName;
            this.PackagingType = packaging_Type;
            this.DispatchQty = dispatchQuantity;
            this.Priority = priority;
            this.Date = date;
            this.CustomerModel = customermodel;
            this.MonthSchedule = monthSchedule;
            this.ScheduleWeek1 = scheduleWeek1;
            this.ScheduleWeek2 = scheduleWeek2;
            this.ScheduleWeek3 = scheduleWeek3;
            this.ScheduleWeek4 = scheduleWeek4;
            this.ScheduleWeek5 = scheduleWeek5;
            this.One = one;
            this.Two = two;
            this.Three = three;
            this.Four = four;
            this.Five = five;
            this.Six = six;
            this.Seven = seven;
            this.Eight = eight;
            this.Nine = nine;
            this.Ten = ten;
            this.Eleven = eleven;
            this.Twelve = twelve;
            this.Thirteen = thirteen;
            this.Fourteen = fourteen;
            this.Fifteen = fifteen;
            this.Sixteen = sixteen;
            this.Seventeen = seventeen;
            this.Eighteen = eighteen;
            this.Nineteen = ninteen;
            this.Twenty = twenty;
            this.Twentyone = twentyone;
            this.Twentytwo = twentytwo;
            this.Twentythree = twentythree;
            this.Twentyfour = twentyfour;
            this.Twentyfive = twentyfive;
            this.Twentysix = twentysix;
            this.Twentyseven = twentyseven;
            this.Twentyeight = twentyeight;
            this.Twentynine = twentynine;
            this.Thirty = thirty;
            this.ThirtyOne = thirtyone;
        }

        internal bool IsRowChanged = false;
        private int serialNum;
        public int SerialNum
        {
            get { return serialNum; }
            set
            {
                serialNum = value;
                NotifyPropertyChanged("SerialNum");
            }
        }

        private string workOrderNumber;
        public string WorkOrderNumber
        {
            get { return workOrderNumber; }
            set
            {
                workOrderNumber = value;
                NotifyPropertyChanged("WorkOrderNumber");
            }
        }

        private string pumpPartNumber;
        public string PumpPartNumber
        {
            get { return pumpPartNumber; }
            set
            {
                pumpPartNumber = value;
                NotifyPropertyChanged("PumpPartNumber");
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

        private string packagingType;
        public string PackagingType
        {
            get { return packagingType; }
            set
            {
                packagingType = value;
                NotifyPropertyChanged("PackagingType");
            }
        }

        private int dispatchQty;
        public int DispatchQty
        {
            get { return dispatchQty; }
            set
            {
                dispatchQty = value;
                NotifyPropertyChanged("DispatchQty");
            }
        }

        private string customermodel;
        public string CustomerModel
        {
            get { return customermodel; }
            set
            {
                customermodel = value;
                NotifyPropertyChanged("CustomerModel");
            }
        }

        private DateTime date;
        public DateTime Date
        {
            get { return date; }
            set
            {
                date = value;
                NotifyPropertyChanged("Date");
            }
        }

        private int priority;
        public int Priority
        {
            get { return priority; }
            set
            {
                priority = value;
                NotifyPropertyChanged("Priority");
            }
        }

        private int monthSchedule;
        public int MonthSchedule
        {
            get { return monthSchedule; }
            set
            {
                monthSchedule = value;
                NotifyPropertyChanged("MonthSchedule");
            }
        }

        private int scheduleWeek1;
        public int ScheduleWeek1
        {
            get { return scheduleWeek1; }
            set
            {
                scheduleWeek1 = value;
                NotifyPropertyChanged("ScheduleWeek1");
            }
        }

        private int scheduleWeek2;
        public int ScheduleWeek2
        {
            get { return scheduleWeek2; }
            set
            {
                scheduleWeek2 = value;
                NotifyPropertyChanged("ScheduleWeek2");
            }
        }

        private int scheduleWeek3;
        public int ScheduleWeek3
        {
            get { return scheduleWeek3; }
            set
            {
                scheduleWeek3 = value;
                NotifyPropertyChanged("ScheduleWeek3");
            }
        }

        private int scheduleWeek4;
        public int ScheduleWeek4
        {
            get { return scheduleWeek4; }
            set
            {
                scheduleWeek4 = value;
                NotifyPropertyChanged("ScheduleWeek4");
            }
        }

        private int scheduleWeek5;
        public int ScheduleWeek5
        {
            get { return scheduleWeek5; }
            set
            {
                scheduleWeek5 = value;
                NotifyPropertyChanged("ScheduleWeek5");
            }
        }

        #region Days
        private int one;
        public int One
        {
            get { return one; }
            set
            {
                one = value;
                NotifyPropertyChanged("One");
            }
        }

        private int two;
        public int Two
        {
            get { return two; }
            set
            {
                two = value;
                NotifyPropertyChanged("Two");
            }
        }

        private int three;
        public int Three
        {
            get { return three; }
            set
            {
                three = value;
                NotifyPropertyChanged("Three");
            }
        }

        private int four;
        public int Four
        {
            get { return four; }
            set
            {
                four = value;
                NotifyPropertyChanged("Four");
            }
        }

        private int five;
        public int Five
        {
            get { return five; }
            set
            {
                five = value;
                NotifyPropertyChanged("Five");
            }
        }

        private int six;
        public int Six
        {
            get { return six; }
            set
            {
                six = value;
                NotifyPropertyChanged("Six");
            }
        }

        private int seven;
        public int Seven
        {
            get { return seven; }
            set
            {
                seven = value;
                NotifyPropertyChanged("Seven");
            }
        }

        private int eight;
        public int Eight
        {
            get { return eight; }
            set
            {
                eight = value;
                NotifyPropertyChanged("Eight");
            }
        }

        private int nine;
        public int Nine
        {
            get { return nine; }
            set
            {
                nine = value;
                NotifyPropertyChanged("Nine");
            }
        }

        private int ten;
        public int Ten
        {
            get { return ten; }
            set
            {
                ten = value;
                NotifyPropertyChanged("Ten");
            }
        }

        private int eleven;
        public int Eleven
        {
            get { return eleven; }
            set
            {
                eleven = value;
                NotifyPropertyChanged("Eleven");
            }
        }

        private int twelve;
        public int Twelve
        {
            get { return twelve; }
            set
            {
                twelve = value;
                NotifyPropertyChanged("Twelve");
            }
        }

        private int thirteen;
        public int Thirteen
        {
            get { return thirteen; }
            set
            {
                thirteen = value;
                NotifyPropertyChanged("Thirteen");
            }
        }

        private int fourteen;
        public int Fourteen
        {
            get { return fourteen; }
            set
            {
                fourteen = value;
                NotifyPropertyChanged("Fourteen");
            }
        }

        private int fifteen;
        public int Fifteen
        {
            get { return fifteen; }
            set
            {
                fifteen = value;
                NotifyPropertyChanged("Fifteen");
            }
        }

        private int sixteen;
        public int Sixteen
        {
            get { return sixteen; }
            set
            {
                sixteen = value;
                NotifyPropertyChanged("Sixteen");
            }
        }

        private int seventeen;
        public int Seventeen
        {
            get { return seventeen; }
            set
            {
                seventeen = value;
                NotifyPropertyChanged("Seventeen");
            }
        }

        private int eighteen;
        public int Eighteen
        {
            get { return eighteen; }
            set
            {
                eighteen = value;
                NotifyPropertyChanged("Eighteen");
            }
        }

        private int ninteen;
        public int Nineteen
        {
            get { return ninteen; }
            set
            {
                ninteen = value;
                NotifyPropertyChanged("Ninteen");
            }
        }

        private int twenty;
        public int Twenty
        {
            get { return twenty; }
            set
            {
                twenty = value;
                NotifyPropertyChanged("Twenty");
            }
        }

        private int twentyone;
        public int Twentyone
        {
            get { return twentyone; }
            set
            {
                twentyone = value;
                NotifyPropertyChanged("Twentyone");
            }
        }

        private int twentytwo;
        public int Twentytwo
        {
            get { return twentytwo; }
            set
            {
                twentytwo = value;
                NotifyPropertyChanged("Twentytwo");
            }
        }
        private int twentythree;
        public int Twentythree
        {
            get { return twentythree; }
            set
            {
                twentythree = value;
                NotifyPropertyChanged("Twentythree");
            }
        }
        private int twentyfour;
        public int Twentyfour
        {
            get { return twentyfour; }
            set
            {
                twentyfour = value;
                NotifyPropertyChanged("Twentyfour");
            }
        }
        private int twentyfive;
        public int Twentyfive
        {
            get { return twentyfive; }
            set
            {
                twentyfive = value;
                NotifyPropertyChanged("Twentyfive");
            }
        }
        private int twentysix;
        public int Twentysix
        {
            get { return twentysix; }
            set
            {
                twentysix = value;
                NotifyPropertyChanged("Twentysix");
            }
        }
        private int twentyseven;
        public int Twentyseven
        {
            get { return twentyseven; }
            set
            {
                twentyseven = value;
                NotifyPropertyChanged("Twentyseven");
            }
        }
        private int twentyeight;
        public int Twentyeight
        {
            get { return twentyeight; }
            set
            {
                twentyeight = value;
                NotifyPropertyChanged("Twentyeight");
            }
        }

        private int twentynine;
        public int Twentynine
        {
            get { return twentynine; }
            set
            {
                twentynine = value;
                NotifyPropertyChanged("Twentynine");
            }
        }
        private int thirty;
        public int Thirty
        {
            get { return thirty; }
            set
            {
                thirty = value;
                NotifyPropertyChanged("Thirty");
            }
        }
        private int thirtyone;
        public int ThirtyOne
        {
            get { return thirtyone; }
            set
            {
                thirtyone = value;
                NotifyPropertyChanged("ThirtyOne");
            }
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                IsRowChanged = true;
            }
        }
    }

    public class MonthEntity
    {
        public string MonthName { get; set; }
        public int MonthVal { get; set; }
    }
}
