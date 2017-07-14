using CarsAndPitsWPF2.Classes.DataTypes;
using CarsAndPitsWPF2.Classes.Nets;
using CarsAndPitsWPF2.Classes.Visualizers;
using System;
using System.Collections.Generic;
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

        public void addVisualizer(CPNetVisualizer visualizer)
        {
            visualizer.Net = net;
            visualizer.Invalidate();
            visualizers.Add(visualizer);            
        }
    }    
}
