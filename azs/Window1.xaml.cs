using azs.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
using System.Windows.Shapes;

namespace azs
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {

        public Window1(Drivers selectedDriver)
        {
            InitializeComponent();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var context = Core.GetContext();
            // Получаем список марок бензина из базы данных
            var marks = context.MarksOfGasoline.ToList();

            // Устанавливаем источник данных для ComboBox
            Combobox1.ItemsSource = marks;
            Combobox1.DisplayMemberPath = "Name";

        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

        }
        
    }
    

}

