namespace Seo.Databases
{
    using System.Collections.Generic;
    using CoordinateTransformations.Contracts;
    
    internal class DefaultDatabase
    {
        private static readonly DefaultDatabase instance = new DefaultDatabase();
        private readonly ICollection<IPoint> points = new List<IPoint>();

        private DefaultDatabase()
        {
        }

        public ICollection<IPoint> Points
        {
            get
            {
                return this.points;
            }
        }

        public static DefaultDatabase GetInstance()
        {
            return DefaultDatabase.instance;
        }
    }
}