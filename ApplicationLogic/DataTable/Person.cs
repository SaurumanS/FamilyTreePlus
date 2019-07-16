using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLogic.DataTable
{
    public class Person : INotifyPropertyChanged //Класс, описывающий какого либо человека
    {
        public Person()
        {
            PersonFamilies = new List<int>();
        }

        private string firstName;
        private string secondName;
        private string patronymic;
        private string dateOfBirth;
        private string placeOfBirth;
        private string dateOfDeath;
        private string placeOfDeath;
        private string gender;
        private string parents; // @FID@
        private List<string> personFamilies;//@FID@
        private string description;
        private string pathPhoto;

        public string Id { get; set; }
        public string FirstName
        {
            get { return firstName; }
            set
            {
                firstName = value;
                OnPropertyChanged("FirstName");
            }
        } 
        public string SecondName
        {
            get { return secondName; }
            set
            {
                secondName = value;
                OnPropertyChanged("SecondName");
            }
        }
        public string Patronymic
        {
            get { return patronymic; }
            set
            {
                patronymic = value;
                OnPropertyChanged("Patronymic");
            }
        }
        public string DateOfBirth
        {
            get { return dateOfBirth; }
            set
            {
                dateOfBirth = value;
                OnPropertyChanged("DateOfBirth");
            }
        }
        public string PlaceOfBirth
        {
            get { return placeOfBirth; }
            set
            {
                placeOfBirth = value;
                OnPropertyChanged("PlaceOfBirth");
            }
        }
        public string DateOfDeath
        {
            get { return dateOfDeath; }
            set
            {
                dateOfDeath = value;
                OnPropertyChanged("DateOfDeath");
            }
        }
        public string PlaceOfDeath
        {
            get { return placeOfDeath; }
            set
            {
                placeOfDeath = value;
                OnPropertyChanged("PlaceOfDeath");
            }
        }
        public string Gender
        {
            get { return gender; }
            set
            {
                gender = value;
                OnPropertyChanged("Gender");
            }
        }
        public string Parents
        {
            get { return parents; }
            set
            {
                parents = value;
                OnPropertyChanged("Parents");
            }
        }
        public List<string> PersonFamilies
        {
            get { return personFamilies; }
            set
            {
                personFamilies = value;
                OnPropertyChanged("PersonFamilies");
            }
        }
        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                OnPropertyChanged("Description");
            }
        }
        public string PathPhoto
        {
            get { return pathPhoto; }
            set
            {
                pathPhoto = value;
                OnPropertyChanged("PathPhoto");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
