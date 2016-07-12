using OxyPlot;
using OxyPlot.Series;
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

namespace ChartTests
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public IList<PlotModel> Plots { get; set; }
        Queue<double>[] dados;


        public MainWindow()
        {
            InitializeComponent();



            Plots = new List<PlotModel>();
            for (int i = 0; i < 10; i++)
            {
                var p = new PlotModel { Title = "Plot " + i };
                p.Series.Add(new FunctionSeries(x => Math.Cos(x * i), 0, 10, 0.01));
                Plots.Add(p);
            }
            (this.Content as FrameworkElement).DataContext = this;
        }
    }
}