using System;
using System.ComponentModel;
using System.Threading;

using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Windows;
using System.IO.Ports;
using System.Collections.Generic;
using System.Configuration;

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
        private int numberOfSeries;

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            plot1.ActualController.UnbindAll();


            charts.Add(new SingleChart());
            charts[0].listaSensores[0].IsSelected = true;
            charts[0].listaSensores[2].IsSelected = true;
            charts[0].listaSensores[4].IsSelected = true;
            charts[0].listaSensores[7].IsSelected = true;
            charts.Add(new SingleChart());
            charts[1].listaSensores[3].IsSelected = true;
            charts[1].listaSensores[8].IsSelected = true;
            charts[1].listaSensores[6].IsSelected = true;
            charts[1].listaSensores[5].IsSelected = true;

            ChartSection.addChart(charts);


            charts = ChartSection.getAllCharts();


            string path = System.Reflection.Assembly.GetEntryAssembly().Location + ".config";

            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo = new System.Diagnostics.ProcessStartInfo("Notepad.exe", '\"' + path + '\"');
            proc.Start();

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
            this.SetupModel();
        }


        private void SetupModel()
        {
            this.timer.Change(Timeout.Infinite, Timeout.Infinite);

            PlotModel = new PlotModel();
            PlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Minimum = -2, Maximum = 55 });




            this.numberOfSeries = 1;

            for (int i = 0; i < this.numberOfSeries; i++)
            {
                PlotModel.Series.Add(new LineSeries { LineStyle = LineStyle.Solid });
            }

            this.RaisePropertyChanged("PlotModel");

            this.timer.Change(500, UpdateInterval);
        }

        public int TotalNumberOfPoints { get; private set; }

        public PlotModel PlotModel { get; private set; }

        private void OnTimerElapsed(object state)
        {
            getData();

            lock (this.PlotModel.SyncRoot)
            {
                this.plotChart();
            }

            this.PlotModel.InvalidatePlot(true);
        }

        private void getData()
        {
            int tmpRead = 0;
            int dadoParcial = 0;
            int dado = 0;
            int canal = 0;
            int estado = 0;

            try
            {
                if (conexao.IsOpen)
                {
                    while (conexao.BytesToRead > 0)
                    {
                        #region Nova Maquina de Estado
                        tmpRead = conexao.ReadByte();

                        #region Verifica estado
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

                                    this.dados[canal].Enqueue(dado);
                                }
                                estado = 0;
                                dado = 0;
                                canal = 0;
                                break;
                        }
                        #endregion
                        #endregion
                    }
                }
            }
            catch { }
        }

        private void plotChart()
        {
            if (dados[0].Count > 0)
            {
                for (int i = 0; i < PlotModel.Series.Count; i++)
                {
                    var s = (LineSeries)PlotModel.Series[i];

                    // Coloca limite no eixo horizontal
                    double x = s.Points.Count > 0 ? s.Points[s.Points.Count - 1].X + 1 : 0;
                    if (s.Points.Count >= 500)
                        s.Points.RemoveAt(0);

                    // Adiciona valor ao gráfico -------------------------------------------------------------------------
                    s.Points.Add(new DataPoint(x, dados[0].Dequeue()));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string property)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(property));
            }
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

        public string[] getNomes()
        {
            return SerialPort.GetPortNames();
        }

        public void fecharComunicação()
        {
            conexao.Close();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.timer.Dispose();
                }
            }

            this.disposed = true;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var t = new ConfigurarGraficos(charts);
            t.ShowDialog();
        }
    }
}