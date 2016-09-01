namespace Seo.Helpers
{
    using System;
    using System.Linq;
    using System.Text;

    internal class LogHelper
    {
        public static string CreateLog(CoordinateTransformations.AffineTransformation affineTransformation)
        {
            StringBuilder log = new StringBuilder();

            log.AppendLine("АФИННА ТРАНСФОРМАЦИЯ НА КООРДИНАТИ");
            log.AppendLine();
            log.AppendLine("ПАРАМЕТРИ НА ТРАНСФОРМАЦИЯТА");
            log.AppendFormat("a0 = {0}; a1 = {1}; a2 = {2};", affineTransformation.A0, affineTransformation.A1, affineTransformation.A2);
            log.AppendLine();
            log.AppendFormat("b0 = {0}; b1 = {1}; b2 = {2};", affineTransformation.B0, affineTransformation.B1, affineTransformation.B2);
            log.AppendLine();
            log.AppendLine();
            log.AppendFormat("{0,8} {1,15} {2,15} {3,15} {4,15} {5,15} {6,15}", "Точка", "X[м]", "Y[м]", "x[м]", "y[м]", "vx[мм]", "vy[мм]");
            log.AppendLine();

            double offsetXSquaredSum = 0;
            double offsetYSquaredSum = 0;

            var sortedPoints = affineTransformation.Points.OrderBy(p => p.PointType);

            foreach (var point in sortedPoints)
            {
                switch (point.PointType)
                {
                    case CoordinateTransformations.Data.Enums.PointType.CommonPoint:
                        offsetXSquaredSum += point.OffsetX * point.OffsetX;
                        offsetYSquaredSum += point.OffsetY * point.OffsetY;

                        log.AppendFormat(
                            "{0,8} {1,15:0.000} {2,15:0.000} {3,15:0.000} {4,15:0.000} {5,15:0.0} {6,15:0.0}",
                            point.PointName,
                            point.SourcePoint.PositionX,
                            point.SourcePoint.PositionY,
                            point.TargetPoint.PositionX,
                            point.TargetPoint.PositionY,
                            point.OffsetX,
                            point.OffsetY);
                        break;
                    case CoordinateTransformations.Data.Enums.PointType.NewPoint:
                        log.AppendFormat(
                            "{0,8} {1,15:0.000} {2,15:0.000} {3,15:0.000} {4,15:0.000}",
                            point.PointName,
                            point.SourcePoint.PositionX,
                            point.SourcePoint.PositionY,
                            point.TargetPoint.PositionX,
                            point.TargetPoint.PositionY);
                        break;
                }

                log.AppendLine();
            }

            double mx = Math.Sqrt(offsetXSquaredSum / affineTransformation.CommonPointsCount);
            double my = Math.Sqrt(offsetYSquaredSum / affineTransformation.CommonPointsCount);
            double ms = Math.Sqrt((offsetXSquaredSum + offsetYSquaredSum) / affineTransformation.CommonPointsCount);
            double m0 = Math.Sqrt((offsetXSquaredSum + offsetYSquaredSum) / ((2 * affineTransformation.CommonPointsCount) + 6));
            double mp = m0 * Math.Sqrt(2);

            log.AppendLine();
            log.AppendLine("ОЦЕНКА НА ТОЧНОСТТА");
            log.AppendFormat("mx = {0:0.0} мм; my = {1:0.0} мм; ms = {2:0.0} мм;", mx, my, ms);
            log.AppendLine();
            log.AppendFormat("m0 = {0:0.0} мм; mp = {1:0.0} мм;", m0, mp);

            return log.ToString();
        }
    }
}
