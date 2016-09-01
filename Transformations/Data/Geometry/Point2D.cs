namespace CoordinateTransformations.Data.Geometry
{
    using System;

    public class Point2D
    {
        private double positionX;
        private double positionY;

        public Point2D(double x, double y)
        {
            this.positionX = x;
            this.positionY = y;
        }

        public double PositionX
        {
            get
            {
                return this.positionX;
            }

            set
            {
                this.positionX = value;
            }
        }

        public double PositionY
        {
            get
            {
                return this.positionY;
            }

            set
            {
                this.positionY = value;
            }
        }
    }
}