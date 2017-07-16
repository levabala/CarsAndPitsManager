using CarsAndPitsWPF2.Classes;
using CarsAndPitsWPF2.Classes.DataTypes;
using CarsAndPitsWPF2.Classes.Nets;
using CarsAndPitsWPF2.Classes.Visualizers;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CarsAndPitsWPF2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();            
            MyWindow.Loaded += MyWindow_Loaded;            
        }

        private void MyWindow_Loaded(object sender, RoutedEventArgs e)
        {
            MyWindow.KeyDown += MyWindow_KeyDown;

            CPManager manager = new CPManager(new Net());
            String folder = selectFolder();
            if (folder == "null")
                return;
            
            CPRawDataGeo.getByPath(
                folder, 
                ParseMode.FolderOfFolders,                
                (s,o) =>
                {
                    //progress changed
                    updateProgress(o.ProgressPercentage);
                },
                (s,o) =>
                {
                    //end
                    Dictionary<SensorType, CPRawDataGeo>[] data = (Dictionary<SensorType, CPRawDataGeo>[])o.Result;

                    List<CPRawDataGeo> list = new List<CPRawDataGeo>();
                    foreach (Dictionary<SensorType, CPRawDataGeo> dict in data)
                        foreach (KeyValuePair<SensorType, CPRawDataGeo> pair in dict)
                            if (pair.Value.geoData.Length > 0 && pair.Value.geoData[0].values.Length >= 3)
                                list.Add(pair.Value);

                    updateProgress(0);
                    manager.addData(
                        list.ToArray(),
                        (ss, oo) =>
                        {
                            //progress changed
                            updateProgress(oo.ProgressPercentage);
                        },
                        (ss, oo) =>
                        {
                            manager.addVisualizer(MyVisualizer);
                            Title = "Done";
                            updateProgress(100);
                        });
                    
                });            
        }

        private void updateProgress(double percentage)
        {
            LoadProgressBar.Value = percentage;
        }

        private void MyWindow_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    Application.Current.Shutdown();
                    break;
            }
        }

        public object CPVisualizer { get; private set; }

        private string selectFolder()
        {
            VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();
            if (dialog.ShowDialog().Value)
            {
                Properties.Settings.Default.LastFolder = dialog.SelectedPath;
                Properties.Settings.Default.Save();
                return dialog.SelectedPath;
            }
            else return "null";
        }

    }
}
