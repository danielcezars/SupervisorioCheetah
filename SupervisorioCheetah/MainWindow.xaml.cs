using System;
using System.ComponentModel;
using System.Threading;

using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Windows;
using System.IO.Ports;
using System.Collections.Generic;
using System.Windows.Data;

namespace SupervisorioCheetah
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SerialPort conexao { get; set; }
        private string porta = "COM3";
        private int baundRate = 9600;
        int numeroCanais = 0;

        Queue<double>[] dados;

        List<SingleChart> charts = new List<SingleChart>();

        // try to change might be lower or higher than the rendering interval
        private const int UpdateInterval = 150;
        private bool disposed;
        private readonly Timer timer;
        public int TotalNumberOfPoints { get; private set; }
        public event PropertyChangedEventHandler PropertyChanged;


        public MainWindow()
        {
            InitializeComponent();
            //plot1.ActualController.UnbindAll();
            


            charts = ChartSection.getAllCharts();
            

            //string path = System.Reflection.Assembly.GetEntryAssembly().Location + ".config";

            //System.Diagnostics.Process proc = new System.Diagnostics.Process();
            //proc.StartInfo = new System.Diagnostics.ProcessStartInfo("Notepad.exe", '\"' + path + '\"');
            //proc.Start();

            // inicia uma lista de lista de dados com o tamanho do enum de sensores
            numeroCanais = Enum.GetNames(typeof(Sensores)).Length;
            dados = new Queue<double>[numeroCanais];
            for (int i = 0; i < dados.Length; i++)
            {
                dados[i] = new Queue<double>();
            }
            conexao = new SerialPort(this.porta, this.baundRate);
            abrirComunicação(this.porta, this.baundRate);

            this.timer = new Timer(OnTimerElapsed);

            //Plots = new List<PlotModel>();
            //for (int i = 0; i < 3; i++)
            //{
            //    var p = new PlotModel { Title = "Plot " + i };
            //    p.Series.Add(new FunctionSeries(x => Math.Cos(x * i), 0, 10, 0.01));
            //    p.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Minimum = -2, Maximum = 2 });
            //    Plots.Add(p);
            //}

            this.SetupModel();
            this.DataContext = this;
        }

        public IList<PlotModel> Plots { get; set; }


        private void SetupModel()
        {
            this.timer.Change(Timeout.Infinite, Timeout.Infinite);

            
            Plots = new List<PlotModel>();

            foreach (SingleChart s in charts)
            {
                var p = new PlotModel() { Title = "chart" + charts.IndexOf(s) };
                p.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Minimum = -2, Maximum = 2 });
                
                foreach (BoolStringClass b in s.listaSensores)
                {
                    if (b.IsSelected)
                    {
                        p.Series.Add(new LineSeries
                        {
                            LineStyle = LineStyle.Solid,
                            Title = b.TheText
                        });
                    }
                }
                Plots.Add(p);
            }

            this.RaisePropertyChanged("Plots");
            this.timer.Change(500, UpdateInterval);
        }

        protected void RaisePropertyChanged(string property)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(property));
            }
        }

        
        private void updateChartObjects()
        {
            
        }

        

        private void OnTimerElapsed(object state)
        {
            
        }

        private void plotChart()
        {
            
        }
        
        /// <summary>
        /// Abre a conexão serial.
        /// Se a conexão já está aberta, abre de novo com os novos parametros.
        /// Caso haja erro, lança exceção - InvalidOperationException.
        /// </summary>
        /// <param name="_nome"></param>
        /// <param name="_frequencia"></param>
        public bool abrirComunicação(string _nome, int _frequencia)
        {
            try
            {
                if (!conexao.IsOpen)
                {
                    conexao = new SerialPort(_nome, _frequencia);
                    conexao.Open();
                }
                else
                {
                    conexao.Close();
                    conexao.PortName = _nome;
                    conexao.BaudRate = _frequencia;
                    conexao.Open();
                }

                this.porta = _nome;
                this.baundRate = _frequencia;

            }
            catch
            {
                return false;
            }
            return true;
        }
        
        public void fecharComunicação()
        {
            conexao.Close();
        }
        
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var t = new ConfigurarGraficos(charts);
            t.ShowDialog();
            charts = t.charts;

            ChartSection.removeAllCharts();
            ChartSection.addChart(charts);

            //string path = System.Reflection.Assembly.GetEntryAssembbly().Location + ".config";
            //System.Diagnostics.Process proc = new System.Diagnostics.Process();
            //proc.StartInfo = new System.Diagnostics.ProcessStartInfo("Notepad.exe", '\"' + path + '\"');
            //proc.Start();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            var t = new WindowChooseSerialPort();
            t.ShowDialog();
            abrirComunicação(t.nameConexao, baundRate);
        }
    }
}