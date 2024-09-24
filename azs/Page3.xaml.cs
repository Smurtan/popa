using azs.Data;
using Microsoft.Win32;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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



namespace azs
{
    /// <summary>
    /// Логика взаимодействия для Page3.xaml
    /// </summary>
    public partial class Page3 : Page
    {
        public Page3()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

            DGridProducts.ItemsSource = Core.GetContext().Refuellings.ToList();
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
            double totalRowsHeight = DGridProducts.Items.Count * (DGridProducts.RowHeight + 1);

            // Задаем новые размеры таблицы
            DGridProducts.Width = totalColumnsWidth + 25;
            DGridProducts.Height = totalRowsHeight + 25;
        }

        
        private void ExportButton_Click(object sender, RoutedEventArgs e)

        {
            // Получаем данные из таблицы Refuellings
            var refuellings = Core.GetContext().Refuellings.ToList();

            // Создаем диалоговое окно сохранения файла
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx";
            saveFileDialog.Title = "Save Excel File";
            saveFileDialog.FileName = "Refuellings.xlsx";

            if (saveFileDialog.ShowDialog() == true)
            {
                // Создаем новый файл Excel
                var fileInfo = new FileInfo(saveFileDialog.FileName);
                using (ExcelPackage package = new ExcelPackage(fileInfo))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Refuellings");

                    // Записываем данные только из таблицы Refuellings в файл Excel
                    worksheet.Cells.LoadFromCollection(refuellings.Select(r => new { r.ID, r.DriverID, r.MarkID, r.UserID, r.Cost, r.Litter, r.RefuellingDate }), true);

                    package.Save();
                }
            }
        }
    }
}
