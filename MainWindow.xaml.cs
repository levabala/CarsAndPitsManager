using CarsAndPitsWPF2.Classes;
using CarsAndPitsWPF2.Classes.DataTypes;
using CarsAndPitsWPF2.Classes.Nets;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
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

            CPManager manager = new CPManager(new Net());

            String folder = selectFolder();
            if (folder == "null")
                return;

            Dictionary<SensorType, CPRawData>[] data = CPRawData.getByPath(folder, ParseMode.FolderOfFiles);
            List<CPRawData> list = new List<CPRawData>();
            foreach (Dictionary<SensorType, CPRawData> dict in data)
                foreach (KeyValuePair<SensorType, CPRawData> pair in dict)
                    list.Add(pair.Value);

            manager.addData(list.ToArray());
        }

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
