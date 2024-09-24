using azs.Data;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Navigation;

namespace azs
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int UserID { get; set; }
        public MainWindow()
        {
            InitializeComponent();

            ManagerPages.MainFrame = MainFrame;
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            ManagerPages.MainFrame.GoBack();
        }

        private void MainFrame_ContentRendered(object sender, EventArgs e)
        {

            if (MainFrame.CanGoBack)
            {
                BtnBack.Visibility = Visibility.Visible;
            }
            else
            {
                BtnBack.Visibility = Visibility.Hidden;
            }




        }
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            this.Width = 800; // фиксированная ширина окна
            this.Height = 450; // фиксированная высота окна
            base.OnRenderSizeChanged(sizeInfo);
        }
        private void MainFrame_Navigated(object sender, NavigationEventArgs e)
        {

        }





        private void idButton_Click(object sender, RoutedEventArgs e)
        {
            string login = loginTextBox.Text;
            string password = passwordTextBox.Text;

            using (var context = new FuelTrakEntities()) 
            {
                var user = Core.GetContext().Users.FirstOrDefault(u => u.Login == login && u.Password == password);

                if (user != null)
                {
                    MessageBox.Show("Авторизация успешна!");
                    rez.Visibility = Visibility.Visible;
                    klient.Visibility = Visibility.Visible;
                    zapravka.Visibility = Visibility.Visible;

                    loginTextBox.Text = "";
                    passwordTextBox.Text = "";
                    UserID = user.ID;
                    rez.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

                }
                else
                {
                    MessageBox.Show("Ошибка: Неверный логин или пароль.");
                }
            }
        }
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Page1());
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Page2());
        }
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Page3());
        }
    }
}
