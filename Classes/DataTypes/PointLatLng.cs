using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarsAndPitsWPF2.Classes.Other
{
    struct PointLatLng
    {
        public double Lat, Lng;

        public PointLatLng(double lat, double lng)
        {
            Lat = lat;
            Lng = lng;
        }        
    }
}
