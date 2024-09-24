using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
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
using System.Windows.Shapes;

namespace azs
{
    /// <summary>
    /// Логика взаимодействия для PaymentWindow.xaml
    /// </summary>
    public partial class PaymentWindow : Window
    {
        public PaymentWindow(double liters, string fuelType, string bonusCardNumber, string discount, double cost, Bitmap qrCodeBitmap)
        {
            InitializeComponent(); 

            // Устанавливаем значения текстовых полей с информацией о заправке
            textBox_1.Text = liters.ToString(); // Объем заправки
            textBox_1.IsReadOnly = true;

            textBox_2.Text = fuelType; // Вид топлива
            textBox_2.IsReadOnly = true;

            textBox_3.Text = bonusCardNumber; // Номер бонусной карты
            textBox_3.IsReadOnly = true;

            textBox_4.Text = discount; // Скидка
            textBox_4.IsReadOnly = true;

            textBox_5.Text = cost.ToString(); // Сумма к оплате
            textBox_5.IsReadOnly = true;

            // Отображаем QR-код на окне оплаты
            System.Windows.Controls.Image qrCodeImageControl = new System.Windows.Controls.Image(); // Создаем объект Image для QR-кода
            qrCodeImageControl.Source = ConvertBitmapToBitmapImage(qrCodeBitmap); // Задаем изображение QR-кода

            qrCodeImage.Source = qrCodeImageControl.Source; // Отображаем QR-код в контроле Image на форме
        }

        // Конвертация Bitmap в BitmapImage (для отображения в WPF)
        private BitmapImage ConvertBitmapToBitmapImage(Bitmap bitmap)
        {
            MemoryStream memoryStream = new MemoryStream();
            bitmap.Save(memoryStream, ImageFormat.Png);
            memoryStream.Position = 0;

            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.StreamSource = memoryStream;
            bitmapImage.EndInit();

            return bitmapImage;
        }

        private void QrCodeImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start("https://one-qr.ru/v?AZMWW27uOQO653H7");
        }
    }
}
