using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace SupervisorioCheetah
{
    /// <summary>
    /// Interaction logic for ConfigurarGraficos.xaml
    /// </summary>
    public partial class ConfigurarGraficos : Window
    {
        public List<SingleChart> charts = new List<SingleChart>();
        public ObservableCollection<BoolStringClass> listaSensores = new ObservableCollection<BoolStringClass>();

        public ConfigurarGraficos(List<SingleChart> charts)
        {
            InitializeComponent();
            this.charts = charts;
            listaSensores = new ObservableCollection<BoolStringClass>();
            listaSensores = this.charts[0].listaSensores;

            ItensListBox.ItensListBox.ItemsSource = listaSensores;
        }

    }
}