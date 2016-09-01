namespace CoordinateTransformations.Contracts
{
    using Data.Enums;
    using Data.Geometry.Point;

    public interface IPoint
    {
        string PointName { get; }

        PointType PointType { get; }

        Point2D SourcePoint { get; }

        Point2D TargetPoint { get; }

        double OffsetX { get; set; }

        double OffsetY { get; set; }
    }
}