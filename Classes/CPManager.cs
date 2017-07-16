using CarsAndPitsWPF2.Classes.DataTypes;
using CarsAndPitsWPF2.Classes.Nets;
using CarsAndPitsWPF2.Classes.Visualizers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarsAndPitsWPF2.Classes
{
    public class CPManager
    {
        Net net;
        List<CPNetVisualizer> visualizers;

        public CPManager(Net net)
        {
            this.net = net;
            visualizers = new List<CPNetVisualizer>();
        }

        public void addData(CPRawDataGeo rawData)
        {
            net.putData(rawData);

            foreach (CPNetVisualizer visualizer in visualizers)
                visualizer.Invalidate();
        }

        public void addData(CPRawDataGeo[] rawData)
        {
            foreach (CPRawDataGeo data in rawData)
                net.putData(data);

            foreach (CPNetVisualizer visualizer in visualizers)
                visualizer.Invalidate();
        }

        public void addData(
            CPRawDataGeo[] rawData,
            ProgressChangedEventHandler processHandler,
            RunWorkerCompletedEventHandler completeHandler)
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.WorkerReportsProgress = true;
            bw.DoWork += (sender, e) =>
            {
                double workToDo = 0;
                Parallel.ForEach(rawData, data =>
                {
                    workToDo += data.geoData.Length;
                });

                double workDone = 0;
                foreach (CPRawDataGeo data in rawData)
                {                    
                    net.putData(data);
                    workDone += data.geoData.Length;
                    bw.ReportProgress((int)(workDone / workToDo * 100));
                }

                foreach (CPNetVisualizer visualizer in visualizers)
                    visualizer.Invalidate();
            };
            bw.ProgressChanged += processHandler;
            bw.RunWorkerCompleted += completeHandler;
            bw.RunWorkerAsync();            
        }

        public void addVisualizer(CPNetVisualizer visualizer)
        {
            visualizer.Net = net;
            visualizer.Invalidate();
            visualizers.Add(visualizer);            
        }
    }    
}
