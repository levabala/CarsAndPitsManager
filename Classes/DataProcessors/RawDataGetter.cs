using CarsAndPitsWPF2.Classes.DataTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarsAndPitsWPF2.Classes.DataProcessors
{
    class RawDataGetter
    {
        public static void getFromDirectory(            
            string folder,
            ProgressChangedEventHandler processHandler,
            RunWorkerCompletedEventHandler completeHandler)
        {
            List<string> directories = new List<string>();
            List<string> files = new List<string>();
            foreach (string path in Directory.GetDirectories(folder))
            {
                if (Directory.Exists(path)) directories.Add(path);
                else if (File.Exists(path)) files.Add(path);
            }

            List<CPRawDataGeo> data = new List<CPRawDataGeo>();

            Dictionary<SensorType, CPRawData> dictionary;

            BackgroundWorker bw = new BackgroundWorker();
            bw.WorkerReportsProgress = true;
            bw.DoWork += (sender, e) =>
            {
                double i = 0;
                foreach (string path in directories)
                {
                    dictionary = CPRawData.fromDirectory(path);
                    if (dictionary.ContainsKey(SensorType.ACCELEROMETER) && dictionary.ContainsKey(SensorType.GPS))
                        data.Add(new CPRawDataGeo(dictionary[SensorType.ACCELEROMETER], dictionary[SensorType.GPS]));
                    i++;

                    bw.ReportProgress((int)(i / directories.Count * 100));
                }

                dictionary = CPRawData.fromDirectory(folder);
                if (dictionary.ContainsKey(SensorType.ACCELEROMETER) && dictionary.ContainsKey(SensorType.GPS))
                    data.Add(new CPRawDataGeo(dictionary[SensorType.ACCELEROMETER], dictionary[SensorType.GPS]));
            };
            bw.ProgressChanged += processHandler;
            bw.RunWorkerCompleted += completeHandler;
            bw.RunWorkerAsync();
        }
    }
}
