namespace AffineTransformations
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Windows;
    using CoordinateTransformations.Data.Enums;
    using Microsoft.Win32;
    using Helpers;
    using CoordinateTransformations.Data.Geometry.Point;
    using CoordinateTransformations;
    using Databases;

    public partial class MainWindow : Window
    {
        private string productVersion = "1.1.0";
        private string outputLog = string.Empty;

        public MainWindow()
        {
            LanguageHelper.SetLanguage(CultureInfo.CurrentCulture.Name);

            this.InitializeComponent();
        }

        private void OpenFile(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog()
                {
                    DefaultExt = ".txt",
                    Filter = Properties.Resources.OpenFileDialogFilter
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    if (File.Exists(openFileDialog.FileName) == false)
                    {
                        throw new FileNotFoundException(string.Format(Properties.Resources.ExceptionFileNotFound,
                            openFileDialog.FileName));
                    }

                    DefaultDatabase.GetInstance().Points.Clear();

                    using (var reader = new StreamReader(openFileDialog.FileName, Encoding.UTF8))
                    {
                        int currentRowNumber = 0;

                        while (reader.EndOfStream == false)
                        {
                            string[] line = Regex.Replace(reader.ReadLine().Trim(), "\\s\\s+", " ").Split(' ');

                            switch (line.Length)
                            {
                                case 3:
                                    DefaultDatabase.GetInstance().Points.Add(new CoordinateTransformations.Data.Point(
                                        line[0],
                                        new Point2D(double.Parse(line[1]), double.Parse(line[2]))));

                                    break;
                                case 5:
                                    DefaultDatabase.GetInstance().Points.Add(new CoordinateTransformations.Data.Point(
                                        line[0],
                                        new Point2D(double.Parse(line[1]), double.Parse(line[2])),
                                        new Point2D(double.Parse(line[3]), double.Parse(line[4]))));

                                    break;
                                default:
                                    throw new ArgumentException(
                                        string.Format(Properties.Resources.ExceptionInvalidRowData, currentRowNumber));
                            }

                            currentRowNumber++;
                        }
                    }

                    int controlPointsCount =
                        DefaultDatabase.GetInstance().Points.Count(p => p.PointType == PointType.ControlPoint);
                    int observationPointsCount =
                        DefaultDatabase.GetInstance().Points.Count(p => p.PointType == PointType.ObservationPoint);
                    int allPointsCount = DefaultDatabase.GetInstance().Points.Count;

                    string message = string.Format(
                        Properties.Resources.OpenFileDialogMessageSuccess,
                        allPointsCount,
                        controlPointsCount,
                        observationPointsCount,
                        Environment.NewLine);

                    MessageBox.Show(message, Properties.Resources.MessageBoxTitleInformation, MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    this.transformButton.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Properties.Resources.MessageBoxTitleError, MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void Transform(object sender, RoutedEventArgs e)
        {
            try
            {
                this.uxTextBoxLog.Clear();

                AffineTransformation affineTransformation =
                    new AffineTransformation(DefaultDatabase.GetInstance().Points);
                affineTransformation.Transform();

                this.outputLog = LogHelper.CreateLog(affineTransformation);

                MessageBox.Show("Успешна трансформация на координати!", Properties.Resources.MessageBoxTitleInformation,
                    MessageBoxButton.OK, MessageBoxImage.Information);

                if (this.displayLogCheckBox.IsChecked == true)
                {
                    this.uxTextBoxLog.Text = this.outputLog;
                }

                this.uxButtonSaveFile.IsEnabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace, Properties.Resources.MessageBoxTitleError, MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void SaveFile(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog()
                {
                    DefaultExt = ".txt",
                    Filter = Properties.Resources.OpenFileDialogFilter,
                    CheckFileExists = false
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName))
                    {
                        writer.Write(this.outputLog);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Properties.Resources.MessageBoxTitleError, MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void ViewHelp(object sender, RoutedEventArgs e)
        {
            this.uxTextBoxLog.Clear();

            StringBuilder output = new StringBuilder();
            output.AppendFormat(Properties.Resources.ProductNameTitle, Properties.Resources.ProductNameValue);
            output.AppendLine();
            output.AppendFormat(Properties.Resources.ProductVersionTitle, this.productVersion);
            output.AppendLine();
            output.AppendFormat(Properties.Resources.ProductDescriptionTitle,
                Properties.Resources.ProductDescriptionValue);
            output.AppendLine();
            output.AppendFormat(Properties.Resources.AuthorNameTitle, Properties.Resources.AuthorNameValue);
            output.AppendLine();
            output.AppendFormat(Properties.Resources.AuthorHomepageTitle, Properties.Resources.AuthorHomepageValue);

            this.uxTextBoxLog.Text = output.ToString();
        }
    }
}