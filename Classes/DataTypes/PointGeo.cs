using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarsAndPitsWPF2.Classes.DataTypes
{
    public struct PointGeo
    {
        public double Lat, Lng, Alt;

        public PointGeo(double lat, double lng, double alt = 0)
        {
            Lat = lat;
            Lng = lng;
            Alt = alt;
        }

        public static explicit operator PointGeo(GeoCoordinate coord)
        {
            return new PointGeo(coord.Latitude, coord.Longitude, coord.Altitude);
        }
    }
}
