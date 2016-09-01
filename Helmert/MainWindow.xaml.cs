using System;
using System.Text;
using System.Windows;
using Microsoft.Win32;
using System.IO;
using AffineTransformations.Transformations;

namespace AffineTransformations
{
    public partial class MainWindow : Window
    {
        private string productName = "Хелмертова трансформация";
        private string productVersion = "1.0.1";
        private string productDescription = "Програма за трансформиране на XY координати чрез Хелмертова трансформация";
        private string authorName = "инж. Н. Гешков";
        private string homepage = "http://SixEightOne.eu/";
        private string filepath = string.Empty;
        private string saveDirectory = Directory.GetCurrentDirectory();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                filepath = openFileDialog.FileName;
                TransformButton.IsEnabled = true;
            }
        }

        private void Transform(object sender, RoutedEventArgs e)
        {
            HelmertTransformation helmertTransformation = new HelmertTransformation();
            helmertTransformation.ReadFile(filepath);
            helmertTransformation.Transform();
            Log.Text = helmertTransformation.OutputLog;
            SaveButton.IsEnabled = true;
        }

        private void SaveFile(object sender, RoutedEventArgs e)
        {
            DateTime now = DateTime.Now;
            string filename = string.Format("Хелмертова_трансформация_{0:ddMMyyyy_HHmmss}.txt", now);
            try
            {
                if (Log.Text.Length > 0)
                {
                    using (StreamWriter writer = new StreamWriter(string.Format("{0}{1}{2}", saveDirectory, System.IO.Path.DirectorySeparatorChar, filename)))
                    {
                        writer.Write(Log.Text);
                        MessageBox.Show(string.Format("Файлът {0} беше записан успешно!", filename), "Информация!", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Възникнала е грешка при записването на файла!", "Грешка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ViewHelp(object sender, RoutedEventArgs e)
        {
            Log.Clear();
            StringBuilder help = new StringBuilder();
            help.AppendFormat("Име на програма: {0}", productName);
            help.AppendLine();
            help.AppendFormat("Версия: {0}", productVersion);
            help.AppendLine();
            help.AppendFormat("Описание: {0}", productDescription);
            help.AppendLine();
            help.AppendFormat("Автор: {0}", authorName);
            help.AppendLine();
            help.AppendFormat("Страница: {0}", homepage);
            Log.Text = help.ToString();
        }
    }
}
