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

namespace ChartExplorer
{

    public partial class MainWindow : Window
    {

        public List<CustomChart> customCharts;
        public MainWindow()
        {
            InitializeComponent();
            customCharts = new List<CustomChart>();
        }

        private void ButtonNewChart_Click(object sender, RoutedEventArgs e)
        {
            CustomChart customChart = new CustomChart();
            customChart.Name = "CustomChart_" + customCharts.Count; 
            customCharts.Add(customChart);
            customChart.Show();
        }
    }
}
