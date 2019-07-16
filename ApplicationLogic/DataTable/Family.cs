using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLogic.DataTable
{
    public class Family : INotifyPropertyChanged //Family description
    {
        public Family()
        {
            ChildrenID = new List<string>();
            theyWasMarried = true;
        }


        private string husbandID;
        private string wifeID;
        private List<string> childrenID;
        bool theyWasMarried;
        public string ID { get; set; }//@F{ID}@
        public string HusbandID
        {
            get { return husbandID; }
            set
            {
                husbandID = value;
                OnPropertyChanged("HusbandID");
            }
        }
        public string WifeID
        {
            get { return wifeID; }
            set
            {
                wifeID = value;
                OnPropertyChanged("WifeID");
            }
        }
        public List<string> ChildrenID
        {
            get { return childrenID; }
            set
            {
                childrenID = value;
                OnPropertyChanged("ChildrenID");
            }
        }
        public bool TheyWasMarried
        {
            get { return theyWasMarried;}
            set
            {
                theyWasMarried = value;
                OnPropertyChanged("TheyWasMarried");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
