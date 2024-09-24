using azs.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
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
using System.Xml.Linq;
using ZXing;
using System.Drawing;
using System.ComponentModel;
using System.Runtime.Remoting.Contexts;

using QRCoder;
using System.Drawing.Imaging;
using System.IO;



namespace azs
{
    /// <summary>
    /// Логика взаимодействия для Page2.xaml
    /// </summary>
    public partial class Page2 : Page
    {


        public Page2()
        {
            InitializeComponent();

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

            DGridProducts.ItemsSource = Core.GetContext().Drivers.ToList();

        }

        private void ClearDriversData()
        {
            Core.GetContext().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
            DGridProducts.ItemsSource = Core.GetContext().Drivers.ToList();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            EditDriverWindow editWindow = new EditDriverWindow();
            editWindow.ShowDialog();
            ClearDriversData(); // Обновляем данные после редактирования
        }


        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (DGridProducts.SelectedItem is Drivers selectedDriver)
            {
                // Создаем новое окно и передаем выбранного водителя
                Window1 window = new Window1(selectedDriver);
                window.ShowDialog();

                // Проверяем, что введенное значение в текстовом поле является числом и не превышает 100
                if (!int.TryParse(window.textBox_Litter.Text, out int count) || count > 100)
                {
                    MessageBox.Show("Ошибка: количество должно быть числом и не более 100.");
                    return;
                }

                // Проверяем выбранную марку бензина
                if (window.Combobox1.SelectedItem is MarksOfGasoline selectedMark)
                {
                    var MarkID = selectedMark.ID;

                    // Проверяем, что выбранная колонка не превышает 6
                    if (MarkID >= 6)
                    {
                        MessageBox.Show("Ошибка: Выберите одну из 6 колонок.");
                        return;
                    }

                    // Уменьшаем общее количество бензина у выбранной марки
                    selectedMark.TotalQuantity -= count;

                    // Вычисляем общую стоимость всех заправок для выбранного водителя
                    double totalRefuellingsCost = (double)Core.GetContext().Refuellings
                        .Where(r => r.DriverID == selectedDriver.ID)
                        .Sum(r => r.Cost);

                    // Увеличиваем количество заправленного бензина у водителя
                    selectedDriver.Count += count;

                    double sale = 0.0;
                    var costPerLiter = Core.GetContext().MarksOfGasoline.FirstOrDefault(m => m.ID == MarkID)?.Cost ?? 0;

                    double cost = (double)(costPerLiter * count);
                    if (selectedDriver.Count > 30)
                    {
                        // Вычисляем скидку для водителя, если он заправил более 30 раз
                        sale = (double)(0.30 * (selectedDriver.Count - 30) * (double)costPerLiter / (cost + totalRefuellingsCost));
                    }
                    selectedDriver.DiscountCoefficient = (decimal)sale;

                    Core.GetContext().SaveChanges();
                    ClearDriversData();

                    // Находим дату первой заправки для выбранного водителя
                    var firstRefuellingDate = Core.GetContext().Refuellings
                        .Where(r => r.DriverID == selectedDriver.ID)
                        .OrderBy(r => r.RefuellingDate)
                        .Select(r => r.RefuellingDate)
                        .FirstOrDefault();

                    DateTime currentMonthStartDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                    if (firstRefuellingDate != null && firstRefuellingDate < currentMonthStartDate)
                    {
                        // Обнуляем скидку, если прошел уже месяц с первой заправки
                        sale = 0.0;
                    }

                    Core.GetContext().SaveChanges();
                    ClearDriversData();
                    MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

                    // Создаем новую запись о заправке и сохраняем ее
                    var newRefuelling = new Refuellings
                    {
                        DriverID = selectedDriver.ID,
                        MarkID = MarkID,
                        UserID = mainWindow.UserID,
                        Litter = count,
                        Cost = (decimal)(cost - (cost * sale)),
                        RefuellingDate = DateTime.Now
                    };

                    Core.GetContext().Refuellings.Add(newRefuelling);
                    Core.GetContext().SaveChanges();
                    ClearDriversData();

                    // Выводим информацию о заправке
                    MessageBox.Show($"Количество заправленных литров = {count} \n Коэффициент скидки = {(double)selectedDriver.DiscountCoefficient} \n Стоимость заправки = {Math.Round(cost - (cost * sale)),2}");
                }
                else
                {
                    MessageBox.Show("Ошибка: Выберите одну из колонок.");
                }
            }
            else
            {
                MessageBox.Show("Выберите одного водителя для редактирования.");
            }
        }
       private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            string searchText = filterBox.Text;

            foreach (var item in DGridProducts.Items)
            {
                if (item is Drivers dataItem && dataItem.BonusCard == searchText)
                {
                    DGridProducts.SelectedItem = item;
                    DGridProducts.ScrollIntoView(item);
                    return; // Найдено и выделено, завершаем цикл
                }
            }

            MessageBox.Show("Номер бонусной карты не найден.");
        }


        private void PayButton_Click(object sender, RoutedEventArgs e)
        {

            // Проверяем, выбран ли водитель в списке, чтобы продолжить с операцией
            if (DGridProducts.SelectedItem is Drivers selectedDriver)
            {
                // Получаем последнюю операцию заправки для выбранного водителя
                var refuelling = Core.GetContext().Refuellings
                    .Where(r => r.DriverID == selectedDriver.ID)
                    .OrderByDescending(r => r.RefuellingDate)
                    .FirstOrDefault();

                // Если данные о заправке получены, продолжаем операцию
                if (refuelling != null)
                {
                    var liters = refuelling.Litter; // Объем заправки
                    var cost = refuelling.Cost; // Сумма к оплате

                    // Выводим уведомление о необходимости оплаты и сумме
                    MessageBox.Show($"Чтобы оплатить заправку, \n необходимо осуществить перевод на сумму {cost}");

                    // Получаем информацию о виде топлива и карте
                    var fuelType = Core.GetContext().MarksOfGasoline.FirstOrDefault(m => m.ID == refuelling.MarkID)?.Name; // Вид топлива
                    var bonusCardNumber = selectedDriver.BonusCard; // Номер бонусной карты
                    var discount = selectedDriver.DiscountCoefficient.ToString(); // Скидка

                    // Создаем QR-код для оплаты
                    string qrLink = "https://one-qr.ru/v?AZMWW27uOQO653H7"; // Ссылка для QR-кода
                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrLink, QRCodeGenerator.ECCLevel.Q); // Создание QR-кода
                    QRCode qrCode = new QRCode(qrCodeData);
                    Bitmap qrCodeImage = qrCode.GetGraphic(20, System.Drawing.Color.Black, System.Drawing.Color.White, true); // Генерация изображения QR-кода

                    // Открываем окно оплаты с данными и QR-кодом
                    PaymentWindow window2 = new PaymentWindow((double)liters, fuelType, bonusCardNumber, discount, (double)cost, qrCodeImage);
                    window2.ShowDialog();
                }
            }
        }

        private void Button_Click3(object sender, RoutedEventArgs e)
        {

        }
    }
}
