using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Device.Location;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarsAndPitsWPF2.Classes.DataTypes
{
    public class CPRawDataGeo
    {
        public long startTime;
        public string deviceId;
        public double totalValueAbs;
        public DataTuplyaGeo[] geoData;
        public SensorType sensor;
        public CPRawDataGeo(CPRawData sensorCPRawData, CPRawData geoCPRawData)            
        {            
            startTime = sensorCPRawData.startTime;
            deviceId = sensorCPRawData.deviceId;
            totalValueAbs = sensorCPRawData.totalValueAbs;
            sensor = sensorCPRawData.sensor;

            List<DataTuplyaGeo> geoData = new List<DataTuplyaGeo>();            
            int sensorIndex = 0;
            if (sensorCPRawData.data.Length > 0)
                for (int i = 1; i < geoCPRawData.data.Length; i++)                
                    while(
                        sensorCPRawData.data[sensorIndex].timeOffset <= geoCPRawData.data[i].timeOffset &&
                        sensorIndex < sensorCPRawData.data.Length-1)
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

            this.geoData = geoData.ToArray();
        }

        private static Dictionary<SensorType, CPRawDataGeo> fromDirectory(string path)
        {
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException("No such directory!");

            Dictionary<SensorType, CPRawDataGeo> data = new Dictionary<SensorType, CPRawDataGeo>();
            
            string[] files = Directory.GetFiles(path);

            int GPSIndex = Array.FindIndex(files, (str) => { return SensorType.fromPath(str) == SensorType.GPS; });
            if (GPSIndex == -1)
                return new Dictionary<SensorType, CPRawDataGeo>();

            CPRawData dataGeo = new CPRawData(files[GPSIndex]);
            files[GPSIndex] = null;
            foreach (string p in files)
            {
                if (p == null)
                    continue;
                CPRawDataGeo cpd = new CPRawDataGeo(new CPRawData(p), dataGeo);
                data.Add(cpd.sensor, cpd);
            }

            return data;
        }

        private static Dictionary<SensorType, CPRawDataGeo> fromDirectory(string path, Action<double> progressCallback)
        {
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException("No such directory!");

            Dictionary<SensorType, CPRawDataGeo> data = new Dictionary<SensorType, CPRawDataGeo>();

            string[] files = Directory.GetFiles(path);

            int GPSIndex = Array.FindIndex(files, (str) => { return SensorType.fromPath(str) == SensorType.GPS; });
            if (GPSIndex == -1)
                return new Dictionary<SensorType, CPRawDataGeo>();

            CPRawData dataGeo = new CPRawData(files[GPSIndex]);
            files[GPSIndex] = null;
            int i = 0;
            foreach (string p in files)
            {
                i++;
                if (p == null)
                    continue;
                CPRawDataGeo cpd = new CPRawDataGeo(new CPRawData(p), dataGeo);
                data.Add(cpd.sensor, cpd);                
                progressCallback(i / files.Length);
            }

            return data;
        }

        private static Dictionary<SensorType, CPRawDataGeo>[] fromDirOfDirs(string path)
        {
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException("No such directory!");

            List<Dictionary<SensorType, CPRawDataGeo>> output = new List<Dictionary<SensorType, CPRawDataGeo>>();

            string[] folders = Directory.GetDirectories(path);
            foreach (string folder in folders)
                if (Directory.Exists(folder))
                    output.Add(fromDirectory(folder));
            return output.ToArray();
        }

        private static Dictionary<SensorType, CPRawDataGeo>[] fromDirOfDirs(
            string path,
            Action<double> progressCallback)
        {
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException("No such directory!");

            List<Dictionary<SensorType, CPRawDataGeo>> output = new List<Dictionary<SensorType, CPRawDataGeo>>();

            double i = 0;
            string[] dirs = Directory.GetDirectories(path);
            foreach (string folder in dirs)
            {
                i++;
                if (Directory.Exists(folder))
                    output.Add(fromDirectory(folder));
                progressCallback(i / dirs.Length);
            }
            return output.ToArray();
        }

        public static Dictionary<SensorType, CPRawDataGeo>[] getByPath(string path, ParseMode parseMode)
        {
            switch (parseMode)
            {
                case ParseMode.FolderOfFiles:
                    return new Dictionary<SensorType, CPRawDataGeo>[] { fromDirectory(path) };
                case ParseMode.FolderOfFolders:
                    return fromDirOfDirs(path);
                default:
                    return new Dictionary<SensorType, CPRawDataGeo>[] { };
            }
        }

        public static Dictionary<SensorType, CPRawDataGeo>[] getByPath(
            string path,
            ParseMode parseMode,
            Action<double> progressCallback)
        {
            switch (parseMode)
            {
                case ParseMode.FolderOfFiles:
                    return new Dictionary<SensorType, CPRawDataGeo>[] { fromDirectory(path, progressCallback) };
                case ParseMode.FolderOfFolders:
                    return fromDirOfDirs(path, progressCallback);
                default:
                    return new Dictionary<SensorType, CPRawDataGeo>[] { };
            }
        }

        public static void getByPath(string path, ParseMode parseMode,            
            ProgressChangedEventHandler processHandler,
            RunWorkerCompletedEventHandler completeHandler)
        {            
            BackgroundWorker bw = new BackgroundWorker();
            bw.WorkerReportsProgress = true;
            bw.DoWork += (sender, e) =>
            {
                Action<double> callback = (progress) =>
                {
                    bw.ReportProgress((int)(progress * 100));
                };
                e.Result = getByPath(path, parseMode, callback);                
            };
            bw.ProgressChanged += processHandler;
            bw.RunWorkerCompleted += completeHandler;
            bw.RunWorkerAsync();
        }

        private double lineIntr(double x1, double x2, double y1, double y2, double x)
        {
            return y1 + (x - x1) * (y2 - y1) / (x2 - x1);
        }
    }

    public class DataTuplyaGeo : DataTuplya
    {        
        public GeoCoordinate coordinate;

        public DataTuplyaGeo(int timeOffset, double[] values, GeoCoordinate coordinate)
            :base(timeOffset, values)
        {            
            this.coordinate = coordinate;
        }
    }
}
