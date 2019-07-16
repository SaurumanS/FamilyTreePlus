using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FamilyTreePlus
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            FileStream fileStream = new FileStream(@"c:\Users\User\source\repos\FamilyTreePlus\Family.ged", FileMode.Open);
            ApplicationLogic.GedcomFile.GedcomFileWriter fileWriter = new ApplicationLogic.GedcomFile.GedcomFileWriter(fileStream);
            InitializeComponent();
        }
    }
}
