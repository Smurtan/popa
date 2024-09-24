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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace azs
{
    /// <summary>
    /// Логика взаимодействия для Page1.xaml
    /// </summary>
    public partial class Page1 : Page
    {
        public Page1()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            DGridProducts.ItemsSource = Core.GetContext().MarksOfGasoline.ToList();
            UpdateDataGridSize();
        }
        private void UpdateDataGridSize()
        {
            double totalColumnsWidth = 0;

            // Вычисляем общую ширину колонок таблицы
            foreach (var column in DGridProducts.Columns)
            {
                totalColumnsWidth += column.ActualWidth;
            }

            // Вычисляем высоту таблицы на основе количества записей
            double totalRowsHeight = DGridProducts.Items.Count * (DGridProducts.RowHeight + 1); // Учитываем высоту заголовка

            // Задаем новые размеры таблицы
            DGridProducts.Width = totalColumnsWidth + 25; // 25 - для полосы прокрутки
            DGridProducts.Height = totalRowsHeight + 25; // 25 - для полосы прокрутки
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (DGridProducts.SelectedItem is MarksOfGasoline selectedMarksOfGasoline)
            {
                // Здесь можно добавить логику разрешения редактирования полей водителя, если требуется

                try
                {
                    Core.GetContext().Entry(selectedMarksOfGasoline).State = EntityState.Modified;
                    Core.GetContext().SaveChanges();
                    MessageBox.Show("Данные успешно отредактированы.");
                    ClearMarksOfGasolineData(); // Обновляем данные после редактирования
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении изменений: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Выберите один резервуар для редактирования.");
            }
        }
        private void ClearMarksOfGasolineData()
        {
            Core.GetContext().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
            DGridProducts.ItemsSource = Core.GetContext().MarksOfGasoline.ToList();
        }
    }
}
