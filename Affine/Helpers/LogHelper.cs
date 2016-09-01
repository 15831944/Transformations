namespace AffineTransformations.Helpers
{
    using System;
    using System.Linq;
    using System.Text;
    using CoordinateTransformations;
    using CoordinateTransformations.Data.Enums;

    internal class LogHelper
    {
        public static string CreateLog(AffineTransformation affineTransformation)
        {
            var output = new StringBuilder();

            output.AppendLine("АФИННА ТРАНСФОРМАЦИЯ НА КООРДИНАТИ");
            output.AppendLine();
            output.AppendLine("ПАРАМЕТРИ НА ТРАНСФОРМАЦИЯТА");
            output.AppendFormat("a0 = {0}; a1 = {1}; a2 = {2};", affineTransformation.A0, affineTransformation.A1,
                affineTransformation.A2);
            output.AppendLine();
            output.AppendFormat("b0 = {0}; b1 = {1}; b2 = {2};", affineTransformation.B0, affineTransformation.B1,
                affineTransformation.B2);
            output.AppendLine();
            output.AppendLine();
            output.AppendFormat("{0,8} {1,15} {2,15} {3,15} {4,15} {5,15} {6,15}", "Точка", "X[м]", "Y[м]", "x[м]",
                "y[м]", "vx[мм]", "vy[мм]");
            output.AppendLine();

            var offsetXSquaredSum = 0.0;
            var offsetYSquaredSum = 0.0;

            var points = affineTransformation.Points.OrderBy(p => p.PointType);

            foreach (var point in points)
            {
                switch (point.PointType)
                {
                    case PointType.ControlPoint:
                        offsetXSquaredSum += point.OffsetX * point.OffsetX;
                        offsetYSquaredSum += point.OffsetY * point.OffsetY;

                        output.AppendFormat(
                            "{0,8} {1,15:0.000} {2,15:0.000} {3,15:0.000} {4,15:0.000} {5,15:0.0} {6,15:0.0}",
                            point.PointName,
                            point.SourcePoint.PositionX,
                            point.SourcePoint.PositionY,
                            point.TargetPoint.PositionX,
                            point.TargetPoint.PositionY,
                            point.OffsetX,
                            point.OffsetY);

                        break;
                    case PointType.ObservationPoint:
                        output.AppendFormat(
                            "{0,8} {1,15:0.000} {2,15:0.000} {3,15:0.000} {4,15:0.000}",
                            point.PointName,
                            point.SourcePoint.PositionX,
                            point.SourcePoint.PositionY,
                            point.TargetPoint.PositionX,
                            point.TargetPoint.PositionY);

                        break;
                }

                output.AppendLine();
            }

            var controlPointsCount = affineTransformation.Points.Count(p => p.PointType == PointType.ControlPoint);

            var mx = Math.Sqrt(offsetXSquaredSum / controlPointsCount);
            var my = Math.Sqrt(offsetYSquaredSum / controlPointsCount);
            var ms = Math.Sqrt((offsetXSquaredSum + offsetYSquaredSum) / controlPointsCount);
            var m0 = Math.Sqrt((offsetXSquaredSum + offsetYSquaredSum) / (2 * controlPointsCount + 6));
            var mp = m0 * Math.Sqrt(2);

            output.AppendLine();
            output.AppendLine("ОЦЕНКА НА ТОЧНОСТТА");
            output.AppendFormat("mx = {0:0.0} мм; my = {1:0.0} мм; ms = {2:0.0} мм;", mx, my, ms);
            output.AppendLine();
            output.AppendFormat("m0 = {0:0.0} мм; mp = {1:0.0} мм;", m0, mp);

            return output.ToString();
        }
    }
}