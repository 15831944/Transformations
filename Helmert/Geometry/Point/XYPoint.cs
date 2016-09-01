using System;

namespace Seo.Geometry.Point
{
    class XYPoint
    {
        private readonly string id;
        private readonly double x;
        private readonly double y;

        public XYPoint(double x, double y) : this(string.Empty, x, y)
        {
            this.x = x;
            this.y = y;
        }

        public XYPoint(string id, double x, double y)
        {
            this.id = id;
            this.x = x;
            this.y = y;
        }

        public string ID
        {
            get { return this.id; }
        }

        public double X
        {
            get { return this.x; }
        }

        public double Y
        {
            get { return this.y; }
        }
    }
}