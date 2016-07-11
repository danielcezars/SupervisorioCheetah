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
        public List<SingleChart> charts { get; set; }
        List<EscolherSensores> listaEscolherSensores;
        List<Button> listaBtnRemover;
        List<Grid> listaGrids;
        List<StackPanel> listaStackPanel;
        

        public ConfigurarGraficos(List<SingleChart> charts)
        {
            InitializeComponent();

            this.charts = charts;

            //ItensListBox.ItensListBox.ItemsSource = charts[0].listaSensores;
            //ItensListBox1.ItensListBox.ItemsSource = charts[1].listaSensores;
            //ItensListBox2.ItensListBox.ItemsSource = charts[2].listaSensores;
            //ItensListBox3.ItensListBox.ItemsSource = charts[3].listaSensores;

            loadCharts(charts);
        }

        public void saveCharts(List <SingleChart> charts)
        {
            ChartSection.removeAllCharts();
            ChartSection.addChart(charts);
        }

        public void loadCharts(List<SingleChart> charts)
        {
            listaEscolherSensores = new List<EscolherSensores>();
            listaBtnRemover = new List<Button>();
            listaGrids = new List<Grid>();
            listaStackPanel = new List<StackPanel>();

            foreach (SingleChart s in charts)
            {
                addChart(s);
            }
        }

        public void btn_Click(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            removeChart(listaBtnRemover.IndexOf(b));
        }
        
        public void addChart(SingleChart s)
        {
            listaGrids.Add(new Grid());
            listaGrids[listaGrids.Count - 1].ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            listaGrids[listaGrids.Count - 1].ColumnDefinitions.Add(new ColumnDefinition());

            listaBtnRemover.Add(new Button()
            {
                Height = 20,
                VerticalAlignment = VerticalAlignment.Top,
                Width = 20,
                Margin = new Thickness(10)
            });
            listaBtnRemover[listaBtnRemover.Count - 1].Click += new RoutedEventHandler(btn_Click);

            Grid.SetColumn(listaBtnRemover[listaGrids.Count - 1], 0);

            listaStackPanel.Add(new StackPanel());
            listaEscolherSensores.Add(new EscolherSensores());
            listaEscolherSensores[listaEscolherSensores.Count - 1].ItensListBox.ItemsSource = s.listaSensores;
            listaStackPanel[listaStackPanel.Count - 1].Children.Add(listaEscolherSensores[listaEscolherSensores.Count - 1]);
            Grid.SetColumn(listaStackPanel[listaStackPanel.Count - 1], 1);

            listaGrids[listaGrids.Count - 1].Children.Add(listaStackPanel[listaStackPanel.Count - 1]);
            listaGrids[listaGrids.Count - 1].Children.Add(listaBtnRemover[listaBtnRemover.Count - 1]);
            myPanel.Children.Add(listaGrids[listaGrids.Count - 1]);
        }
        
        public  void removeChart(int i)
        {
            listaBtnRemover.RemoveAt(i);
            listaEscolherSensores.RemoveAt(i);
            listaGrids.RemoveAt(i);
            listaStackPanel.RemoveAt(i);
            myPanel.Children.RemoveAt(i);
        }

        private void btnAdicionar_Click(object sender, RoutedEventArgs e)
        {
            addChart(new SingleChart());
            myScrowViewer.ScrollToBottom();
            listaEscolherSensores[listaEscolherSensores.Count - 1].Focus();
        }

        void receberCharts()
        {
            charts = new List<SingleChart>();
            
            foreach (EscolherSensores e in listaEscolherSensores)
            {
                charts.Add(new SingleChart((ObservableCollection<BoolStringClass>)e.ItensListBox.ItemsSource)) ;
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            receberCharts();
        }
    }
}