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

        string path = "";

        Queue<double>[] dados;
        StreamWriter arquivoEscrita;
        StreamReader arquivoLeitura;

        List<SingleChart> charts = new List<SingleChart>();

        // try to change might be lower or higher than the rendering interval
        private const int UpdateInterval = 150;
        private bool disposed;
        private readonly System.Threading.Timer timer;
        public int TotalNumberOfPoints { get; private set; }
        public event PropertyChangedEventHandler PropertyChanged;

        enum barraEstados
        {
            Carrengando, 
            Erro,
            Pronto,
            Recebendo,
            Esperando
        }

        public MainWindow()
        {
            InitializeComponent();
            sttbrEstado.Text = barraEstados.Carrengando.ToString();
            numeroCanais = Enum.GetNames(typeof(Sensores)).Length;
            //plot1.ActualController.UnbindAll();

            charts = ChartSection.getAllCharts();

            path = Directory.GetCurrentDirectory() + "\\dados";
            if(!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            sttbarArquivo.Text = path;

            abrirArquivosEscrita();
            
            //string path = System.Reflection.Assembly.GetEntryAssembly().Location + ".config";

            //System.Diagnostics.Process proc = new System.Diagnostics.Process();
            //proc.StartInfo = new System.Diagnostics.ProcessStartInfo("Notepad.exe", '\"' + path + '\"');
            //proc.Start();

            // inicia uma lista de lista de dados com o tamanho do enum de sensores
            dados = new Queue<double>[numeroCanais];
            for (int i = 0; i < dados.Length; i++)
            {
                dados[i] = new Queue<double>();
            }
            conexao = new SerialPort(this.porta, this.baundRate);
            abrirComunicação(this.porta, this.baundRate);

            this.timer = new System.Threading.Timer(OnTimerElapsed);

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
            sttbrEstado.Text = barraEstados.Pronto.ToString();
            sttbrConexao.Text = "-";
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
                    sttbrEstado.Text = barraEstados.Recebendo.ToString();
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

                                    arquivos[canal].WriteLine(dado);
                                }
                                estado = 0;
                                dado = 0;
                                canal = 0;
                                break;
                        }
                        #endregion
                    }
                }
                else
                {
                    sttbrEstado.Text = barraEstados.Erro.ToString();
                }
            }
            catch
            {
                //sttbrEstado.Text = barraEstados.Erro.ToString();

            }
        }

        void abrirArquivosEscrita()
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                foreach (Sensores s in Enum.GetValues(typeof(Sensores)))
                {
                    arquivoEscrita = new StreamWriter(File.OpenWrite(path + "\\dados.cvs"));
                }
            }
            catch { }
        }

        void fecharArquivosEscrita()
        {
            try
            {
                foreach (StreamWriter s in arquivosEscrita)
                {
                    s.Close();
                }
            }
            catch { }
        }

        void abirArquivosLeitura()
        {
            string[] f = Directory.GetFiles(path, "*.txt");

            foreach (string s in f)
            {
                //arquivosLeitura[]
            }
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

                sttbrConexao.Text = conexao.PortName;
                sttbrEstado.Text = barraEstados.Pronto.ToString();
            }
            catch
            {
                sttbrEstado.Text = barraEstados.Erro.ToString();
                sttbrConexao.Text = "-";
                return false;
            }
            return true;
        }
        
        public void fecharComunicação()
        {
            conexao.Close();

            sttbrEstado.Text = barraEstados.Pronto.ToString();
            sttbrConexao.Text = "-";
        }
        
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            sttbrEstado.Text = barraEstados.Esperando.ToString();
            var t = new ConfigurarGraficos(charts);
            t.ShowDialog();

            sttbrEstado.Text = barraEstados.Carrengando.ToString();
            charts = t.charts;

            ChartSection.removeAllCharts();
            ChartSection.addChart(charts);

            sttbrEstado.Text = barraEstados.Pronto.ToString();

            //string path = System.Reflection.Assembly.GetEntryAssembbly().Location + ".config";
            //System.Diagnostics.Process proc = new System.Diagnostics.Process();
            //proc.StartInfo = new System.Diagnostics.ProcessStartInfo("Notepad.exe", '\"' + path + '\"');
            //proc.Start();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {

            sttbrEstado.Text = barraEstados.Esperando.ToString();
            var t = new WindowChooseSerialPort();
            t.ShowDialog();

            sttbrEstado.Text = barraEstados.Carrengando.ToString();
            abrirComunicação(t.nameConexao, baundRate);
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            sttbrEstado.Text = barraEstados.Esperando.ToString();
            var t = new FolderBrowserDialog();
            t.ShowDialog();

            path = t.SelectedPath;

            sttbrEstado.Text = barraEstados.Pronto.ToString();
            sttbarArquivo.Text = path;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            fecharArquivosEscrita();
            fecharComunicação();
        }
    }
}