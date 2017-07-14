using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CarsAndPitsWPF2.Classes.DataTypes
{
    public struct RectGeo
    {
        public double Lat, Lng, WidthLng, HeightLat;
        public RectGeo(double lat1, double lng1, double lat2, double lng2)
        {
            Lat = lat1;
            Lng = lng1;
            WidthLng = lng2 - lng1;
            HeightLat = lat2 - lat1;
        }
        public RectGeo(PointGeo point, double latHeight, double lngWidth)
        {
            Lat = point.Lat;
            Lng = point.Lng;
            WidthLng = lngWidth;
            HeightLat = latHeight;
        }
        public RectGeo(Rect rect)
        {
            Lat = rect.Y;
            Lng = rect.X;
            WidthLng = rect.Width;
            HeightLat = rect.Height;
        }
        public RectGeo(PointGeo p1, PointGeo p2)
        {            
            Lat = p1.Lat;
            Lng = p1.Lng;
            WidthLng = p2.Lat - p1.Lat;
            HeightLat = p2.Lng - p2.Lng;
        }
    }
}
