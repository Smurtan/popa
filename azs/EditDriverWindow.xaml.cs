using azs.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace azs
{
    /// <summary>
    /// Логика взаимодействия для EditDriverWindow.xaml
    /// </summary>
    public partial class EditDriverWindow : Window
    {


        public EditDriverWindow()
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

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var context = Core.GetContext();

            if (string.IsNullOrEmpty(fullNameTextBox.Text) || string.IsNullOrEmpty(ageTextBox.Text))
            {
                MessageBox.Show("Ошибка: не все поля заполнены.");
                return;
            }

            if (!int.TryParse(ageTextBox.Text, out int count) || count > 100)
            {
                MessageBox.Show("Ошибка: количество должно быть числом и не более 100.");
                return;
            }
            if (Combobox1.SelectedItem is MarksOfGasoline selectedMark)
            {
                var MarkID = selectedMark.ID;

                 context = Core.GetContext();

                var mark = context.MarksOfGasoline.FirstOrDefault(m => m.ID == MarkID);
                if (mark != null)
                {
                    if (mark.TotalQuantity <= 0)
                    {
                        MessageBox.Show("Ошибка: Колонка пустая, выберите другую колонку!");
                        return;
                    }

                    mark.TotalQuantity -= count;

                    string bonusCard = GenerateUniqueBonusCard();

                    bonusCardNumberTextBox.Text = bonusCard;
                    bonusCardNumberTextBox.IsEnabled = false;

                    var costPerLiter = mark.Cost;
                    double sale = 0.0;
                    double cost = (double)(costPerLiter * count);
                    if (count > 30)
                    {
                        sale = (0.03 * (count - 30) * (double)costPerLiter) / cost;
                    }

                    var newDriver = new Drivers
                    {
                        DiscountCoefficient = (decimal)sale,
                        Name = fullNameTextBox.Text,
                        BonusCard = bonusCard,
                        Count = count
                    };

                    context.Drivers.Add(newDriver);
                    context.SaveChanges();

                    var mainWindow = (MainWindow)Application.Current.MainWindow;

                    var newRefuelling = new Refuellings
                    {
                        DriverID = newDriver.ID,
                        MarkID = MarkID,
                        UserID = mainWindow.UserID,
                        Cost = (decimal)(cost - (cost * sale)),
                        Litter = count,
                        RefuellingDate = DateTime.Now
                    };

                    context.Refuellings.Add(newRefuelling);
                    context.SaveChanges();

                    Page2 page2 = Application.Current.Windows.OfType<Page2>().FirstOrDefault();
                    if (page2 != null)
                    {
                        page2.DGridProducts.Items.Add(newDriver);
                    }

                    Page3 page3 = Application.Current.Windows.OfType<Page3>().FirstOrDefault();
                    if (page3 != null)
                    {
                        page3.DGridProducts.Items.Add(newRefuelling);
                    }


                    MessageBox.Show($"Водитель: {newDriver.Name} \n Номер бонусной карты: {newDriver.BonusCard} \n Количество завпрвленных литров: {newDriver.Count} \n Коэффициент скидки: {newDriver.DiscountCoefficient} \n Стоимость заправки: {newRefuelling.Cost}");
                }
                else
                {
                    MessageBox.Show("Ошибка: Выбранная колонка не найдена.");
                }
            }
            else
            {
                MessageBox.Show("Ошибка: Выберите один из элементов из списка.");
            }
        }



        private string GenerateUniqueBonusCard()
        {
            Random random = new Random();
            string uniqueCardNumber = random.Next(100000, 999999).ToString(); // Генерация 6-значного номера

            // Проверка на уникальность в базе данных
            while (IsBonusCardExist(uniqueCardNumber))
            {
                uniqueCardNumber = random.Next(100000, 999999).ToString();
            }
            return uniqueCardNumber;
        }

       
        private bool IsBonusCardExist(string cardNumber)
        {
            // Проверка наличия номера карты в базе данных
            return Core.GetContext().Drivers.Any(driver => driver.BonusCard == cardNumber);
        }


        





    }

}
