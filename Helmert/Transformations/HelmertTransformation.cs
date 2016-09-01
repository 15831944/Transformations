using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Globalization;
using System.Windows;
using System.Linq;
using AffineTransformations.Geometry.Point;

namespace AffineTransformations.Transformations
{
    class HelmertTransformation
    {
        private readonly List<XYPoint> sourcePoints;
        private readonly List<XYPoint> destinationPoints;
        private readonly List<XYPoint> transformedPoints;
        private int allPointsCount = 0;
        private int commonPointsCount = 0;
        private readonly StringBuilder outputLog;

        public HelmertTransformation()
        {
            this.sourcePoints = new List<XYPoint>();
            this.destinationPoints = new List<XYPoint>();
            this.transformedPoints = new List<XYPoint>();
            this.outputLog = new StringBuilder();
        }

        public void ReadFile(string path)
        {
            try
            {
                if (!File.Exists(path))
                {
                    throw new FileNotFoundException(string.Format("Файлът {0} не може да бъде намерен!", path));
                }
                using (StreamReader reader = new StreamReader(path, Encoding.UTF8))
                {
                    int currentRowNumber = 0;
                    while (!reader.EndOfStream)
                    {
                        string[] line = reader.ReadLine().Split(' ');
                        switch (line.Length)
                        {
                            case 3:
                                sourcePoints.Add(new XYPoint(line[0], double.Parse(line[1]), double.Parse(line[2])));
                                break;
                            case 5:
                                sourcePoints.Add(new XYPoint(line[0], double.Parse(line[1]), double.Parse(line[2])));
                                destinationPoints.Add(new XYPoint(line[0], double.Parse(line[3]), double.Parse(line[4])));
                                commonPointsCount++;
                                break;
                            default:
                                throw new ArgumentException(string.Format("Невалиден формат на данните на ред [{0}]!", currentRowNumber));
                        }
                        allPointsCount++;
                        currentRowNumber++;
                    }
                }
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "Грешка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Грешка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void Transform()
        {
            try
            {
                if (commonPointsCount < 2)
                {
                    throw new ArgumentException(string.Format("Броят на общите точки в двете координатни системи [{0}] е по-малък от [{1}]!", commonPointsCount, 2));
                }

                double ax1 = sourcePoints.GetRange(0, commonPointsCount).Sum(f => f.X) / commonPointsCount;
                double ay1 = sourcePoints.GetRange(0, commonPointsCount).Sum(f => f.Y) / commonPointsCount;
                double ax2 = destinationPoints.GetRange(0, commonPointsCount).Sum(f => f.X) / commonPointsCount;
                double ay2 = destinationPoints.GetRange(0, commonPointsCount).Sum(f => f.Y) / commonPointsCount;

                double a = ax2 - ax1;
                double b = ay2 - ay1;

                double dx1, dy1, dx2, dy2;
                double dx, dy;
                double p1 = 0, p2 = 0, p3 = 0, p4 = 0;
                double dxSquaredSum = 0;
                double dySquaredSum = 0;
                for (int i = 0; i < commonPointsCount; i++)
                {
                    dx1 = sourcePoints[i].X - ax1;
                    dy1 = sourcePoints[i].Y - ay1;
                    dx2 = destinationPoints[i].X - ax2;
                    dy2 = destinationPoints[i].Y - ay2;
                    dx = dx2 - dx1;
                    dy = dy2 - dy1;
                    p1 += dx1 * dy;
                    p2 += dy1 * dx;
                    p3 += dy1 * dy;
                    p4 += dx1 * dx;
                    dxSquaredSum += Math.Pow(dx1, 2);
                    dySquaredSum += Math.Pow(dy1, 2);
                }

                double c = (p1 - p2) / (dxSquaredSum + dySquaredSum);
                double d = (p3 + p4) / (dxSquaredSum + dySquaredSum);

                outputLog.AppendLine("ХЕЛМЕРТОВА ТРАНСФОРМАЦИЯ НА КООРДИНАТИ");
                outputLog.AppendLine();
                outputLog.AppendLine("ПАРАМЕТРИ НА ТРАНСФОРМАЦИЯТА");
                outputLog.AppendFormat("a = {0}; b = {1}; c = {2}; d = {3};", a, b, c, d);
                outputLog.AppendLine();
                outputLog.AppendLine();
                outputLog.AppendFormat("{0,8} {1,15} {2,15} {3,15} {4,15} {5,15} {6,15}", "Точка", "X[м]", "Y[м]", "x[м]", "y[м]", "vx[мм]", "vy[мм]");
                outputLog.AppendLine();

                double vxSquaredSum = 0;
                double vySquaredSum = 0;
                for (int i = 0; i < allPointsCount; i++)
                {
                    double x = sourcePoints[i].X + a - (sourcePoints[i].Y - ay1) * c + (sourcePoints[i].X - ax1) * d;
                    double y = sourcePoints[i].Y + b + (sourcePoints[i].X - ax1) * c + (sourcePoints[i].Y - ay1) * d;
                    if (i < commonPointsCount)
                    {
                        double vx = (destinationPoints[i].X - x) * 1000;
                        double vy = (destinationPoints[i].Y - y) * 1000;
                        vxSquaredSum += vx * vx;
                        vySquaredSum += vy * vy;

                        outputLog.AppendFormat("{0,8} {1,15} {2,15} {3,15} {4,15} {5,15} {6,15}",
                            sourcePoints[i].ID,
                            Math.Round(sourcePoints[i].X, 3).ToString("N3", CultureInfo.CurrentCulture),
                            Math.Round(sourcePoints[i].Y, 3).ToString("N3", CultureInfo.CurrentCulture),
                            Math.Round(destinationPoints[i].X, 3).ToString("N3", CultureInfo.CurrentCulture),
                            Math.Round(destinationPoints[i].Y, 3).ToString("N3", CultureInfo.CurrentCulture),
                            Math.Round(vx, 3).ToString("N1", CultureInfo.CurrentCulture),
                            Math.Round(vy, 3).ToString("N1", CultureInfo.CurrentCulture));
                    }
                    else
                    {
                        outputLog.AppendFormat("{0,8} {1,15} {2,15} {3,15} {4,15}",
                            sourcePoints[i].ID,
                            Math.Round(sourcePoints[i].X, 3).ToString("N3", CultureInfo.CurrentCulture),
                            Math.Round(sourcePoints[i].Y, 3).ToString("N3", CultureInfo.CurrentCulture),
                            Math.Round(x, 3).ToString("N3", CultureInfo.CurrentCulture),
                            Math.Round(y, 3).ToString("N3", CultureInfo.CurrentCulture));
                    }
                    transformedPoints.Add(new XYPoint(string.Empty, x, y));

                    outputLog.AppendLine();
                }

                double mx = Math.Sqrt(vxSquaredSum / commonPointsCount);
                double my = Math.Sqrt(vySquaredSum / commonPointsCount);
                double ms = Math.Sqrt((vxSquaredSum + vySquaredSum) / commonPointsCount);

                outputLog.AppendLine();
                outputLog.AppendLine("ОЦЕНКА НА ТОЧНОСТТА");
                outputLog.AppendFormat("mx = {0} мм; my = {1} мм; ms = {2} мм;", Math.Round(mx, 1), Math.Round(my, 1), Math.Round(ms, 1));
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Стоп!", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
            catch (Exception)
            {
                MessageBox.Show("Възникнала е непредвидена грешка по време на трансформацията на координатите!", "Стоп!", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        public string OutputLog
        {
            get { return outputLog.ToString(); }
        }

        public List<XYPoint> SourcePoints
        {
            get { return sourcePoints; }
        }

        public List<XYPoint> DestinationPoints
        {
            get { return destinationPoints; }
        }

        public List<XYPoint> TransformedPoints
        {
            get { return transformedPoints; }
        }
    }
}