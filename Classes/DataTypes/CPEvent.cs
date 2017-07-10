using CarsAndPitsWPF2.Classes.Other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarsAndPitsWPF2.Classes.DataTypes
{
    struct CPEvent
    {
        public readonly string Who;
        public readonly CPEventType What;
        public readonly PointLatLng Where;
        public readonly long When;
        public readonly Object How;

        public CPEvent(string who, CPEventType what, PointLatLng where, long when, Object how)
        {
            Who = who;
            What = what;
            Where = where;
            When = when;
            How = how;            
        }
    }

    enum CPEventType
    {

    }
}
