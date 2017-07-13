using CarsAndPitsWPF2.Classes.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarsAndPitsWPF2.Classes.Nets
{
    public class NetSquare
    {
        public readonly int level;
        public readonly double lat;
        public readonly double lng;
        public readonly int[] path;

        public double intensity;
        public NetSquare[] children;        

        public NetSquare(double lat, double lng, int level, double intensity)
        {
            this.level = level;
            this.lat = lat;
            this.lng = lng;
            this.intensity = intensity;
            path = new int[0];
            children = new NetSquare[4];
        }

        public NetSquare(NetSquare parent, int index, double intensity)
        {
            level = parent.level + 1;
            lat = parent.lat + ((index > 1) ? 180 / Math.Pow(2, parent.level) / 2 : 0);
            lng = parent.lng + ((index == 1 || index == 3) ? 360 / Math.Pow(2, parent.level) / 2 : 0);
            children = new NetSquare[4];

            //path remembering
            path = new int[parent.path.Length + 1];
            parent.path.CopyTo(path, 0);
            path[path.Length - 1] = index;

            this.intensity = intensity;
        }                
    }

    public class NetSquareCPData : NetSquare
    {
        //device-id -> sensor -> sequence
        public Dictionary<string, Dictionary<SensorType, CPDataSequence>> data
            = new Dictionary<string, Dictionary<SensorType, CPDataSequence>>();

        public NetSquareCPData(double lat, double lng, int level, double intensity)
            : base(lat, lng, level, intensity) { }
        public NetSquareCPData(NetSquare parent, int index, double intensity)
            : base(parent, index, intensity) { }

        public void putData(CPRawData rawData)
        {
            if (!data.ContainsKey(rawData.deviceId))
            {
                Dictionary<SensorType, CPDataSequence> dictionary = new Dictionary<SensorType, CPDataSequence>()
                {
                    { rawData.sensor, new CPDataSequence(rawData.sensor, rawData.startTime) }
                };
                data.Add(rawData.deviceId, dictionary);
            }
            else if (!data[rawData.deviceId].ContainsKey(rawData.sensor))
                data[rawData.deviceId].Add(rawData.sensor, new CPDataSequence(rawData.sensor, rawData.startTime));

            CPDataSequence sequense = data[rawData.deviceId][rawData.sensor];
            CPVectorAbs[] vectorsAbs = CPVectorAbs.fromArray(rawData.data, rawData.startTime);
            foreach (CPVectorAbs vectorAbs in vectorsAbs)
                sequense.addVector(vectorAbs);
        }      
        
        public void putData(CPVectorAbs vectorAbs, string deviceId, SensorType sensor)
        {
            if (!data.ContainsKey(deviceId))
            {
                Dictionary<SensorType, CPDataSequence> dictionary = new Dictionary<SensorType, CPDataSequence>()
                {
                    { sensor, new CPDataSequence(sensor, vectorAbs.absoluteTime) }
                };
                data.Add(deviceId, dictionary);
            }
            else if (!data[deviceId].ContainsKey(sensor))
                data[deviceId].Add(sensor, new CPDataSequence(sensor, vectorAbs.absoluteTime));

            CPDataSequence sequense = data[deviceId][sensor];
            sequense.addVector(vectorAbs);
        }        
    }
}
