namespace CoordinateTransformations.Data
{
    using System;
    using System.Linq;
    using CoordinateTransformations.Data.Enums;
    using CoordinateTransformations.Data.Geometry;

    public class Point
    {
        private readonly PointType pointType;
        private readonly Point2D sourcePoint;
        private readonly Point2D targetPoint;
        private readonly string pointName;
        private double offsetX;
        private double offsetY;

        public Point(string pointName, Point2D sourcePoint)
        {
            this.pointName = pointName;
            this.sourcePoint = new Point2D(sourcePoint.PositionX, sourcePoint.PositionY);
            this.targetPoint = new Point2D(0.0, 0.0);
            this.pointType = PointType.NewPoint;
        }

        public Point(string pointName, Point2D sourcePoint, Point2D targetPoint)
        {
            this.pointName = pointName;
            this.sourcePoint = new Point2D(sourcePoint.PositionX, sourcePoint.PositionY);
            this.targetPoint = new Point2D(targetPoint.PositionX, targetPoint.PositionY);
            this.pointType = PointType.CommonPoint;
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

        public PointType PointType
        {
            get
            {
                return this.pointType;
            }
        }

        public string PointName
        {
            get
            {
                return this.pointName;
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
    }
}