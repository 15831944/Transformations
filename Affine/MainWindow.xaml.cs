namespace Seo
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Windows;
    using CoordinateTransformations.Data.Enums;
    using Microsoft.Win32;
    using Seo.Helpers;

    public partial class MainWindow : Window
    {
        private string productVersion = "1.1.0";
        private ICollection<CoordinateTransformations.Data.Point> points;
        private string outputLog = string.Empty;

        public MainWindow()
        {
            this.SetLanguage();
            this.InitializeComponent();

            this.points = new List<CoordinateTransformations.Data.Point>();
        }

        private void SetLanguage()
        {
            switch (CultureInfo.CurrentCulture.Name)
            {
                case "bg-BG":
                    Properties.Resources.Culture = new CultureInfo("bg-BG");

                    break;
                default:
                    Properties.Resources.Culture = new CultureInfo("en-US");

                    break;
            }
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
                        throw new FileNotFoundException(string.Format(Properties.Resources.ExceptionFileNotFound, openFileDialog.FileName));
                    }

                    this.points.Clear();

                    using (var reader = new StreamReader(openFileDialog.FileName, Encoding.UTF8))
                    {
                        int currentRowNumber = 0;

                        while (reader.EndOfStream == false)
                        {
                            string[] line = Regex.Replace(reader.ReadLine().Trim(), "\\s\\s+", " ").Split(' ');

                            switch (line.Length)
                            {
                                case 3:
                                    this.points.Add(new CoordinateTransformations.Data.Point(
                                        line[0],
                                        new CoordinateTransformations.Data.Geometry.Point2D(double.Parse(line[1]), double.Parse(line[2]))));

                                    break;
                                case 5:
                                    this.points.Add(new CoordinateTransformations.Data.Point(
                                        line[0],
                                        new CoordinateTransformations.Data.Geometry.Point2D(double.Parse(line[1]), double.Parse(line[2])),
                                        new CoordinateTransformations.Data.Geometry.Point2D(double.Parse(line[3]), double.Parse(line[4]))));

                                    break;
                                default:
                                    throw new ArgumentException(string.Format(Properties.Resources.ExceptionInvalidRowData, currentRowNumber));
                            }

                            currentRowNumber++;
                        }
                    }

                    int commonPointsCount = this.points.Where(p => p.PointType == PointType.CommonPoint).Count();
                    int newPointsCount = this.points.Where(p => p.PointType == PointType.NewPoint).Count();
                    int allPointsCount = this.points.Count;

                    string message = string.Format(
                        Properties.Resources.OpenFileDialogMessageSuccess,
                        allPointsCount,
                        commonPointsCount,
                        newPointsCount,
                        Environment.NewLine);

                    MessageBox.Show(message, Properties.Resources.MessageBoxTitleInformation, MessageBoxButton.OK, MessageBoxImage.Information);

                    this.transformButton.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Properties.Resources.MessageBoxTitleError, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Transform(object sender, RoutedEventArgs e)
        {
            this.uxTextBoxLog.Clear();

            try
            {
                CoordinateTransformations.AffineTransformation affineTransformation = new CoordinateTransformations.AffineTransformation(this.points);
                affineTransformation.TransformCoordinates();

                this.outputLog = LogHelper.CreateLog(affineTransformation);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Properties.Resources.MessageBoxTitleError, MessageBoxButton.OK, MessageBoxImage.Error);
            }

            MessageBox.Show("Успешна трансформация на координати!", Properties.Resources.MessageBoxTitleInformation, MessageBoxButton.OK, MessageBoxImage.Information);

            if (this.displayLogCheckBox.IsChecked == true)
            {
                this.uxTextBoxLog.Text = this.outputLog;
            }

            this.uxButtonSaveFile.IsEnabled = true;
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
                MessageBox.Show(ex.Message, Properties.Resources.MessageBoxTitleError, MessageBoxButton.OK, MessageBoxImage.Error);
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
            output.AppendFormat(Properties.Resources.ProductDescriptionTitle, Properties.Resources.ProductDescriptionValue);
            output.AppendLine();
            output.AppendFormat(Properties.Resources.AuthorNameTitle, Properties.Resources.AuthorNameValue);
            output.AppendLine();
            output.AppendFormat(Properties.Resources.AuthorHomepageTitle, Properties.Resources.AuthorHomepageValue);

            this.uxTextBoxLog.Text = output.ToString();
        }
    }
}