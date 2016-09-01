namespace CoordinateTransformations.Data
{
    using Enums;
    using Contracts;
    using Geometry.Point;

    public class Point : IPoint
    {
        private readonly string pointName;
        private readonly PointType pointType;
        private readonly Point2D sourcePoint;
        private readonly Point2D targetPoint;
        private double offsetX;
        private double offsetY;

        public Point(string pointName, Point2D sourcePoint)
        {
            this.pointName = pointName;
            this.pointType = PointType.ObservationPoint;
            this.sourcePoint = new Point2D(sourcePoint.PositionX, sourcePoint.PositionY);
            this.targetPoint = new Point2D(0.0, 0.0);
        }

        public Point(string pointName, Point2D sourcePoint, Point2D targetPoint)
        {
            this.pointName = pointName;
            this.pointType = PointType.ControlPoint;
            this.sourcePoint = new Point2D(sourcePoint.PositionX, sourcePoint.PositionY);
            this.targetPoint = new Point2D(targetPoint.PositionX, targetPoint.PositionY);
        }

        public string PointName
        {
            get
            {
                return this.pointName;
            }
        }

        public PointType PointType
        {
            get
            {
                return this.pointType;
            }
        }

        public Point2D SourcePoint
        {
            get
            {
                return this.sourcePoint;
            }
        }

        public Point2D TargetPoint
        {
            get
            {
                return this.targetPoint;
            }
        }

        public double OffsetX
        {
            get
            {
                return this.offsetX;
            }
            set
            {
                this.offsetX = value;
            }
        }

        public double OffsetY
        {
            get
            {
                return this.offsetY;
            }
            set
            {
                this.offsetY = value;
            }
        }
    }
}