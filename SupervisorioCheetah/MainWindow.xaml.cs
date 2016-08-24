using System;
using System.ComponentModel;
using System.Threading;

using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using System.Windows;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SupervisorioCheetah
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SerialPort conexao = new SerialPort();
        int numeroCanais = 0;
        private string porta = "COM3";
        private int baundRate = 9600;

        string path = "";

        Queue<double>[] dados;

        List<SingleChart> charts = new List<SingleChart>();
        public List<PlotModel> Plots { get; set; }

        // try to change might be lower or higher than the rendering interval
        private const int UpdateInterval = 150;
        private readonly System.Threading.Timer timer;
        public int TotalNumberOfPoints { get; private set; }
        public event PropertyChangedEventHandler PropertyChanged;

        

        public MainWindow()
        {
            InitializeComponent();
            //sttbrEstado.Text = barraEstados.Carrengando.ToString();
            //sttbrConexao.Text = "-";

            // inicia o arquivo de leitura e escrita -----------------------------------------------------------
            path = Directory.GetCurrentDirectory();
            if(!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            //sttbarArquivo.Text = path + "\\dados.cvs";
            // -------------------------------------------------------------------------------------------------

            // inicia uma lista de lista de dados com o tamanho do enum de sensores ----------------------------
            numeroCanais = Enum.GetNames(typeof(Sensores)).Length;
            dados = new Queue<double>[numeroCanais];
            for (int i = 0; i < dados.Length; i++)
            {
                dados[i] = new Queue<double>();
            }
            // -------------------------------------------------------------------------------------------------
            // inicia todos os parametro para o aquisição de dados ---------------------------------------------
            this.timer = new System.Threading.Timer(OnTimerElapsed);
            // inicia todos os parametro para os gráficos ------------------------------------------------------
            charts = ChartSection.getAllCharts();
            this.SetupModel();
            this.DataContext = this;
            // -------------------------------------------------------------------------------------------------
            
            #region testes
            //Plots = new List<PlotModel>();
            //for (int i = 0; i < 3; i++)
            //{
            //    var p = new PlotModel { Title = "Plot " + i };
            //    p.Series.Add(new FunctionSeries(x => Math.Cos(x * i), 0, 10, 0.01));
            //    p.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Minimum = -2, Maximum = 2 });
            //    Plots.Add(p);
            //}

            //string path = System.Reflection.Assembly.GetEntryAssembly().Location + ".config";
            //System.Diagnostics.Process proc = new System.Diagnostics.Process();
            //proc.StartInfo = new System.Diagnostics.ProcessStartInfo("Notepad.exe", '\"' + path + '\"');
            //proc.Start();
            #endregion
            //sttbrEstado.Text = barraEstados.Pronto.ToString();
            //sttbrConexao.Text = "-";
        }
        
        public void loadCharts(List<SingleChart> charts)
        {
            //listaAllCharts = new List<AllCharts>();
            //foreach(SingleChart s in charts)
            //{
            //    addChart(s);
            //}
        }

        public void addChart(SingleChart singleChart)
        {
            //listaAllCharts.Add(new AllCharts());

            //// Modifica as propridades do controle aqui --------------------------------------------------------
            ////listaAllCharts[listaAllCharts.Count - 1]
            //// -------------------------------------------------------------------------------------------------


            //myPanel.Children.Add(listaAllCharts[listaAllCharts.Count - 1]);
        }

        public PlotModel MeuModelo { get; private set; }
        private void SetupModel()
        {
            this.timer.Change(Timeout.Infinite, Timeout.Infinite);
            
            Plots = new List<PlotModel>();

            foreach (SingleChart s in charts)
            {
                MeuModelo = new PlotModel();

                MeuModelo.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Minimum = -100, Maximum = 100 });

                // Adiciona a série para cada sensor --------------------------------------------------------------
                foreach (BoolStringClass b in charts[0].listaSensores)
                {
                    if (b.IsSelected)
                    {
                        MeuModelo.Series.Add(new LineSeries { LineStyle = LineStyle.Solid, Title = b.TheText });
                    }
                }
                // -------------------------------------------------------------------------------------------------

                Plots.Add(MeuModelo);
            }

            #region

            //listaAllCharts = new List<AllCharts>();
            //listaGraficos = new ObservableCollection<PlotModel>();
            //Plots = new List<PlotModel>();

            //foreach (SingleChart s in charts)
            //{
            //    var p = new PlotModel() { Title = "chart" + charts.IndexOf(s) };
            //    p.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Minimum = -2, Maximum = 2 });

            //    // Adiciona a série para cada sensor --------------------------------------------------------------
            //    foreach (BoolStringClass b in s.listaSensores)
            //    {
            //        if (b.IsSelected)
            //        {
            //            p.Series.Add(new LineSeries
            //            {
            //                LineStyle = LineStyle.Solid,
            //                Title = b.TheText
            //            });
            //        }
            //    }
            //    // -------------------------------------------------------------------------------------------------

            //    listaGraficos.Add(p);
            //    Plots.Add(p);

            //    var v = (LineSeries)Plots[Plots.Count - 1].Series[0];
            //    v.Points.Add(new DataPoint(0, 0));
            //    v.Points.Add(new DataPoint(1, 1));
            //    v.Points.Add(new DataPoint(2, 2));
            //    v.Points.Add(new DataPoint(3, 3));

            //    listaAllCharts.Add(new AllCharts());
            //    listaAllCharts[listaAllCharts.Count - 1].ItensListBox.ItemsSource = listaGraficos;

            //    myPanel.Children.Add(listaAllCharts[listaAllCharts.Count - 1]);
            //}
            #endregion

            //graficos.ItensListBox.ItemsSource = Plots;
            //meusGraficos.Children.Add(graficos);

            this.RaisePropertyChanged("graficos");

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
            getData();

            lock (this.MeuModelo.SyncRoot)
            {
                this.plotChart();
            }

            this.MeuModelo.InvalidatePlot(true);
        }
        private void getData()
        {
            #region Variáveis Maquina de estado
            int tmpRead = 0;
            int dadoParcial = 0;
            int dado = 0;
            int canal = 0;
            int estado = 0;
            #endregion

            try
            {
                if (conexao.IsOpen)
                {
                    //sttbrEstado.Text = barraEstados.Recebendo.ToString();
                    while (conexao.BytesToRead > 0)
                    {
                        #region Nova Maquina de Estado
                        tmpRead = conexao.ReadByte();

                        switch (estado)
                        {
                            case 0:
                                if ((tmpRead & 0xE0) == 0x80)
                                {
                                    canal = tmpRead & 0x7F;
                                    dado = 0;
                                    estado = 1;
                                }
                                break;

                            case 1:
                                if ((tmpRead & 0xE0) == 0x00) // Verifica se é dado e se é o primeiro
                                {
                                    dadoParcial = (tmpRead & 0x1F);
                                    if (((dadoParcial & 0x10) == 0x10) && dado == 0) // Verifica o sinal
                                    // o dado tem que ser zero e o primeiro bit do dado parcial tem que ser 1
                                    {
                                        dadoParcial |= 0xF0; // nega os primeiros 3 bits do dado temporário
                                        dado = -1; // faz todos os bits iguais a 1
                                    }
                                    dado = dadoParcial << 25; // Recebe os 5 digitos menos significativos

                                    estado = 2; // muda o estado
                                }
                                else
                                {
                                    estado = 0; // Volta a esperar um canal
                                }
                                break;

                            case 2:
                                if ((tmpRead & 0xE0) == 0xA0) // Verifica se é dado e se é o segundo
                                {
                                    dadoParcial = (tmpRead & 0x1F);
                                    if (((dadoParcial & 0x10) == 0x10) && dado == 0) // Verifica o sinal
                                    {
                                        dadoParcial |= 0xF0;
                                        dado = -1;
                                    }
                                    dado |= dadoParcial << 20; // Recebe os 5 digitos menos significativos

                                    estado = 3; // muda o estado
                                }
                                else
                                {
                                    estado = 0; // Volta a esperar um canal
                                }
                                break;

                            case 3:
                                if ((tmpRead & 0xE0) == 0xC0) // Verifica se é dado e se é o terceiro
                                {
                                    dadoParcial = (tmpRead & 0x1F);
                                    if (((dadoParcial & 0x10) == 0x10) && dado == 0) // Verifica o sinal
                                    {
                                        dadoParcial |= 0xF0;
                                        dado = -1;
                                    }
                                    dado |= dadoParcial << 15; // Recebe os 5 digitos menos significativos

                                    estado = 4; // muda o estado
                                }
                                else
                                {
                                    estado = 0; // Volta a esperar um canal
                                }
                                break;
                            case 4:
                                if ((tmpRead & 0xE0) == 0x60) // Verifica se é dado e se é o quarto
                                {
                                    dadoParcial = (tmpRead & 0x1F);
                                    if (((dadoParcial & 0x10) == 0x10) && dado == 0) // Verifica o sinal
                                    {
                                        dadoParcial |= 0xF0;
                                        dado = -1;
                                    }
                                    dado |= dadoParcial << 10; // Recebe os 5 digitos menos significativos

                                    estado = 5; // muda o estado
                                }
                                else
                                {
                                    estado = 0; // Volta a esperar um canal
                                }
                                break;

                            case 5:
                                if ((tmpRead & 0xE0) == 0x40) // Verifica se é dado e se é o quinto
                                {
                                    dadoParcial = (tmpRead & 0x1F);
                                    if (((dadoParcial & 0x10) == 0x10) && dado == 0) // Verifica o sinal
                                    {
                                        dadoParcial |= 0xF0;
                                        dado = -1;
                                    }
                                    dado |= dadoParcial << 5; // Recebe os 5 digitos menos significativos

                                    estado = 6; // muda o estado
                                }
                                else
                                {
                                    estado = 0; // Volta a esperar um canal
                                }
                                break;

                            case 6:
                                if ((tmpRead & 0xE0) == 0x20) // Verifica se é dado e se é o sexto
                                {
                                    dado |= (tmpRead & 0x1F); // Recebe os 5 digitos menos significativos
                                    dados[canal].Enqueue(dado);
                                    
                                }
                                estado = 0;
                                dado = 0;
                                canal = 0;
                                break;
                        }
                        #endregion
                    }


                    //ArquivoTxt.salvaArquivo(path, dados);
                }
                else
                {
                    //sttbrEstado.Text = barraEstados.Recebendo.ToString();
                }
            }
            catch
            {
               // sttbrEstado.Text = barraEstados.Erro.ToString();
            }
        }
        
        private void plotChart()
        {
            if (dados[0].Count > 0)
            {
                for (int i = 0; i < MeuModelo.Series.Count; i++)
                {
                    var s = (LineSeries)MeuModelo.Series[i];

                    // Coloca limite no eixo horizontal
                    double x = s.Points.Count > 0 ? s.Points[s.Points.Count - 1].X + 1 : 0;
                    if (s.Points.Count >= 200)
                        s.Points.RemoveAt(0);

                    if (dados[0].Count > 0)
                    {
                        // Adiciona valor ao gráfico -------------------------------------------------------------------------
                        s.Points.Add(new DataPoint(x, dados[0].Dequeue()));
                    }
                }
            }
        }
        
        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            //sttbrEstado.Text = barraEstados.Esperando.ToString();
            var t = new WindowChooseSerialPort(baundRate, conexao);
            t.ShowDialog();

            porta = t.nameConexao;
            //sttbrEstado.Text = barraEstados.Pronto.ToString();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            //sttbrEstado.Text = barraEstados.Esperando.ToString();
            var t = new ConfigurarGraficos(charts);
            t.ShowDialog();

            //sttbrEstado.Text = barraEstados.Carrengando.ToString();
            charts = t.charts;

            ChartSection.removeAllCharts();
            ChartSection.addChart(charts);

            //sttbrEstado.Text = barraEstados.Pronto.ToString();

            //string path = System.Reflection.Assembly.GetEntryAssembbly().Location + ".config";
            //System.Diagnostics.Process proc = new System.Diagnostics.Process();
            //proc.StartInfo = new System.Diagnostics.ProcessStartInfo("Notepad.exe", '\"' + path + '\"');
            //proc.Start();
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            //sttbrEstado.Text = barraEstados.Esperando.ToString();
            // --------------------------------------------------------------------
            var t = new FolderBrowserDialog();
            t.ShowNewFolderButton = true;
            if (t.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
            {
                path = t.SelectedPath;
            }
            //---------------------------------------------------------------------
           // sttbrEstado.Text = barraEstados.Pronto.ToString();
            //sttbarArquivo.Text = path;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Serial.fecharComunicação(conexao);
        }
    }
}