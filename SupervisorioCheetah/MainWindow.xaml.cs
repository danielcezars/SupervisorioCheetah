using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;

using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Windows;
using System.IO.Ports;
using System.Collections.Generic;

namespace SupervisorioCheetah
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SerialPort conexao { get; set; }
        //public Queue<double>[] dados { get; set; }
        int numeroCanais = 0;
        Dados d = new Dados();
        Random r;

        List<Queue<double>> dados;

        private string porta = "COM3";
        private int baundRate = 9600;


        public enum canais
        {
            acelX, acelY, acelZ,
            yaw, pitch, roll,
            latitude, longitude,
            posicaoX, posicaoY,
            anguloVolante,
            velocidade1, velocidade2, velocidade3, velocidade4, velocidadeCarro, velocidadeGPS,
            freioDianteiro, freioTraseiro,
            rotacaoMotor
        };

        public MainWindow()
        {
            InitializeComponent();
            // this.DataContext = new MainViewModel();
            this.DataContext = this;
            plot1.ActualController.UnbindAll();


            numeroCanais = Enum.GetNames(typeof(canais)).Length;
            dados = new List<Queue<double>>(numeroCanais);
            for(int i = 0; i < dados.Count; i++)
            {
                dados[i] = new Queue<double>();
            }

            

            r = new Random();

            this.timer = new Timer(OnTimerElapsed);

            this.SetupModel();

            conexao = new SerialPort(this.porta, this.baundRate);

            //dados = new Queue<double>[this.numeroCanais];

            d = new Dados();
            abrirComunicação(this.porta, this.baundRate);
        }


        // try to change might be lower or higher than the rendering interval
        private const int UpdateInterval = 200;

        private bool disposed;
        private readonly Timer timer;
        private int numberOfSeries;
        private Queue<Dados> listaDados;

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
            lock (this.PlotModel.SyncRoot)
            {
                this.getData();
            }

            this.PlotModel.InvalidatePlot(true);
        }

        private void getData()
        {
            #region
            int tmpRead = 0;
            int dadoParcial = 0;
            int dado = 0;
            int canal = 0;
            int estado = 0;


            try
            {
                if (conexao.IsOpen)
                {
                    listaDados = new Queue<Dados>();
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

                                    #region Verifica qual o canal para adicionar na classe
                                    switch (canal)
                                    {
                                        case ((int)canais.acelX):
                                            if (d._acelX)
                                            {
                                                updateChart();
                                                //listaDados.Enqueue(d);
                                                d = new Dados();
                                            }
                                            d.AcelX = (double)dado;
                                            d._acelX = true;
                                            break;
                                        case ((int)canais.acelY):
                                            if (d._acelY)
                                            {
                                                updateChart();
                                                //listaDados.Enqueue(d);
                                                d = new Dados();
                                            }
                                            d.AcelY = (double)dado;
                                            d._acelY = true;
                                            break;
                                        case ((int)canais.acelZ):
                                            if (d._acelZ)
                                            {
                                                updateChart();
                                                // listaDados.Enqueue(d);
                                                d = new Dados();
                                            }
                                            d.AcelZ = (double)dado;
                                            d._acelZ = true;
                                            break;
                                        case ((int)canais.anguloVolante):
                                            if (d._anguloVolante)
                                            {
                                                updateChart();
                                                // listaDados.Enqueue(d);
                                                d = new Dados();
                                            }
                                            d._anguloVolante = true;
                                            d.AnguloVolante = dado;
                                            break;
                                        case ((int)canais.freioDianteiro):
                                            if (d._freioDianteiro)
                                            {
                                                updateChart();
                                                // listaDados.Enqueue(d);
                                                d = new Dados();
                                            }
                                            d._freioDianteiro = true;
                                            d.FreioDianteiro = dado;
                                            break;
                                        case ((int)canais.freioTraseiro):
                                            if (d._freioTraseiro)
                                            {
                                                updateChart();
                                                //listaDados.Enqueue(d);
                                                d = new Dados();
                                            }
                                            d._freioTraseiro = true;
                                            d.FreioTraseiro = dado;
                                            break;
                                        case ((int)canais.latitude):
                                            if (d._Latitude)
                                            {
                                                updateChart();
                                                //listaDados.Enqueue(d);
                                                d = new Dados();
                                            }
                                            d._Latitude = true;
                                            d.Latitude = (double)dado;
                                            break;
                                        case ((int)canais.longitude):
                                            if (d._Longitude)
                                            {
                                                updateChart();
                                                //listaDados.Enqueue(d);
                                                d = new Dados();
                                            }
                                            d._Longitude = true;
                                            d.Longitude = dado;
                                            break;

                                        case ((int)canais.pitch):
                                            if (d._pitch)
                                            {
                                                updateChart();
                                                // listaDados.Enqueue(d);
                                                d = new Dados();
                                            }
                                            d._pitch = true;
                                            d.Pitch = (double)dado;
                                            break;
                                        case ((int)canais.posicaoX):
                                            if (d._posicaoX)
                                            {
                                                updateChart();
                                                // listaDados.Enqueue(d);
                                                d = new Dados();
                                            }
                                            d._posicaoX = true;
                                            d.PosicaoX = (double)dado;
                                            break;
                                        case ((int)canais.posicaoY):
                                            if (d._posicaoY)
                                            {
                                                updateChart();
                                                // listaDados.Enqueue(d);
                                                d = new Dados();
                                            }
                                            d._posicaoY = true;
                                            d.PosicaoY = (double)dado;
                                            break;
                                        case ((int)canais.roll):
                                            if (d._roll)
                                            {
                                                updateChart();
                                                //  listaDados.Enqueue(d);
                                                d = new Dados();
                                            }
                                            d._roll = true;
                                            d.Roll = (double)dado;
                                            break;
                                        case ((int)canais.rotacaoMotor):
                                            if (d._rotacaoMotor)
                                            {
                                                updateChart();
                                                // listaDados.Enqueue(d);
                                                d = new Dados();
                                            }
                                            d._rotacaoMotor = true;
                                            d.RotacaoMotor = dado;
                                            break;
                                        case ((int)canais.velocidade1):
                                            if (d._velocidade1)
                                            {
                                                updateChart();
                                                //  listaDados.Enqueue(d);
                                                d = new Dados();
                                            }
                                            d._velocidade1 = true;
                                            d.Velocidade1 = dado;
                                            break;
                                        case ((int)canais.velocidade2):
                                            if (d._velocidade2)
                                            {
                                                updateChart();
                                                // listaDados.Enqueue(d);
                                                d = new Dados();
                                            }
                                            d._velocidade2 = true;
                                            d.Velocidade2 = dado;
                                            break;
                                        case ((int)canais.velocidade3):
                                            if (d._velocidade3)
                                            {
                                                updateChart();
                                                //  listaDados.Enqueue(d);
                                                d = new Dados();
                                            }
                                            d._velocidade3 = true;
                                            d.Velocidade3 = dado;
                                            break;
                                        case ((int)canais.velocidade4):
                                            if (d._velocidade4)
                                            {
                                                updateChart();
                                                //  listaDados.Enqueue(d);
                                                d = new Dados();
                                            }
                                            d._velocidade4 = true;
                                            d.Velocidade4 = dado;
                                            break;
                                        case ((int)canais.velocidadeCarro):
                                            if (d._velocidadeCarro)
                                            {
                                                // listaDados.Enqueue(d);
                                                updateChart();
                                                d = new Dados();
                                            }
                                            d._velocidadeCarro = true;
                                            d.VelocidadeCarro = dado;
                                            break;
                                        case ((int)canais.velocidadeGPS):
                                            if (d._velocidadeGPS)
                                            {
                                                updateChart();
                                                //listaDados.Enqueue(d);
                                                d = new Dados();
                                            }
                                            d._velocidadeGPS = true;
                                            d.VelocidadeGPS = dado;
                                            break;
                                        case ((int)canais.yaw):
                                            if (d._yaw)
                                            {
                                                updateChart();
                                                // listaDados.Enqueue(d);
                                                d = new Dados();
                                            }
                                            d._yaw = true;
                                            d.Yaw = dado;
                                            break;
                                    }
                                    #endregion


                                }
                                estado = 0;
                                d.Tempo = DateTime.Now;
                                break;
                        }
                        #endregion
                        #endregion
                    }
                }
            }
            catch { }
            #endregion

        }

        private void plotChart()
        {
            int n = 0;

            for (int i = 0; i < PlotModel.Series.Count; i++)
            {
                var s = (LineSeries)PlotModel.Series[i];


                double x = s.Points.Count > 0 ? s.Points[s.Points.Count - 1].X + 1 : 0;
                if (s.Points.Count >= 200)
                    s.Points.RemoveAt(0);
                double y = 0;
                int m = 80;
                for (int j = 0; j < m; j++)
                    y += Math.Cos(0.001 * x * j * j);
                y /= m;
                s.Points.Add(new DataPoint(x, r.Next(0, 50)));


                n += s.Points.Count;
            }

            if (this.TotalNumberOfPoints != n)
            {
                this.TotalNumberOfPoints = n;
                this.RaisePropertyChanged("TotalNumberOfPoints");
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
            catch (InvalidOperationException e)
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
    }
}