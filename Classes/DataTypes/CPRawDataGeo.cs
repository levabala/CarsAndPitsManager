using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarsAndPitsWPF2.Classes.DataTypes
{
    class CPRawDataGeo
    {
        public long startTime;
        public string deviceId;        
        public SensorType sensor;
        public DataTuplyaGeo[] geoData;

        public CPRawDataGeo(CPRawData sensorCPRawData, CPRawData geoCPRawData)            
        {
            startTime = sensorCPRawData.startTime;
            deviceId = sensorCPRawData.deviceId;

            List<DataTuplyaGeo> geoData = new List<DataTuplyaGeo>();            
            int sensorIndex = 0;
            for (int i = 1; i < geoCPRawData.data.Length; i++)
            {
                while(sensorCPRawData.data[sensorIndex].timeOffset <= geoCPRawData.data[i].timeOffset)
                {
                    double sLat = geoCPRawData.data[i - 1].values[0];
                    int sTime = geoCPRawData.data[i - 1].timeOffset;
                    double eLat = geoCPRawData.data[i].values[0];
                    int eTime = geoCPRawData.data[i].timeOffset;                
                    int nowTime = sensorCPRawData.data[sensorIndex].timeOffset;
                    double nowLat = lineIntr(sTime, eTime, sLat, eLat, nowTime);

                    double sLng = geoCPRawData.data[i - 1].values[1];                    
                    double eLng = geoCPRawData.data[i].values[1];                    
                    double nowLng = lineIntr(sTime, eTime, sLng, eLng, nowTime);

                    geoData.Add(new DataTuplyaGeo(nowTime, sensorCPRawData.data[sensorIndex].values, new GeoCoordinate(nowLat, nowLng)));

                    sensorIndex++;
                }
            }

            this.geoData = geoData.ToArray();
        }

        private double lineIntr(double x1, double x2, double y1, double y2, double x)
        {
            return y1 + (x - x1) * (y2 - y1) / (x2 - x1);
        }
    }

    struct DataTuplyaGeo
    {
        public int timeOffset;
        public double[] values;
        public GeoCoordinate coordinate;

        public DataTuplyaGeo(int timeOffset, double[] values, GeoCoordinate coordinate)
        {
            this.timeOffset = timeOffset;
            this.values = values;
            this.coordinate = coordinate;
        }
    }
}
