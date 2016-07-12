//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.IO.Ports;

//namespace SupervisorioCheetah
//{
//    class Serial //: IDisposable
//    {
//        #region Variáveis
//        private SerialPort conexao;
//        public Queue<double>[] dados { get; set; }
//        int numeroCanais = 0;
//        //public Queue<Dados> listaDados { get; set; }
//        //Dados d = new Dados();

//        private string porta = string.Empty;
//        private int baundRate = 0;

//        #endregion

//        public enum canais
//        {
//            tempo,
//            acelX, acelY, acelZ,
//            yaw, pitch, roll,
//            latitude, longitude,
//            posicaoX, posicaoY,
//            anguloVolante,
//            velocidade1, velocidade2, velocidade3, velocidade4, velocidadeCarro, velocidadeGPS,
//            freioDianteiro, freioTraseiro,
//            rotacaoMotor
//        };

//        #region Construtores
//        public Serial ()
//        {
//            this.porta = string.Empty;
//            this.baundRate = 0;
//            this.numeroCanais = 0;

//            Constructor();
//        }
//        public Serial(int numeroCanais)
//        {
//            this.numeroCanais = numeroCanais;

//            Constructor();
//        }
//        public Serial(int numeroCanais, string porta)
//        {
//            this.porta = porta;
//            this.numeroCanais = numeroCanais;

//            Constructor();
//        }

//        public Serial(int numeroCanais, string porta, int baudRate)
//        {
//            this.porta = porta;
//            this.baundRate = baudRate;
//            this.numeroCanais = numeroCanais;

//            Constructor();
//        }
        
//        private void Constructor ()
//        {
//            conexao = new SerialPort(this.porta, this.baundRate);
//            conexao.DataReceived += new SerialDataReceivedEventHandler(SerialReceiving);

//            dados = new Queue<double>[this.numeroCanais];


//            listaDados = new Queue<Dados>();
//            d = new Dados();
//        }
//        #endregion
        
//        /// <summary>
//        /// Abre a conexão serial.
//        /// Se a conexão já está aberta, abre de novo com os novos parametros.
//        /// Caso haja erro, lança exceção - InvalidOperationException.
//        /// </summary>
//        /// <param name="_nome"></param>
//        /// <param name="_frequencia"></param>
//        public bool abrirComunicação(string _nome, int _frequencia)
//        {
//            try
//            {
//                if (!conexao.IsOpen)
//                {
//                    conexao = new SerialPort(_nome, _frequencia);
//                    conexao.Open();
//                }
//                else
//                {
//                    conexao.Close();
//                    conexao.PortName = _nome;
//                    conexao.BaudRate = _frequencia;
//                    conexao.Open();
//                }

//                this.porta = _nome;
//                this.baundRate = _frequencia;
                
//            }
//            catch(InvalidOperationException e)
//            {

//                return false;
//            }

//            return true;
//        }

//        public string[] getNomes()
//        {
//            return SerialPort.GetPortNames();
//        }

//        public void fecharComunicação()
//        {
//            conexao.Close();
//        }
        
//        public void Dispose()
//        {
//            conexao.Close();

//            if(conexao != null)
//            {
//                conexao.Dispose();
//                conexao = null;
//            }
//        }

//        /// <summary>
//        /// Recebe os dados do buffer da serial aberta
//        /// </summary>
//        public void recebeDados()
//        {
//            #region Variáveis Maquina de estado
//            int tmpRead = 0;
//            int dadoParcial = 0;
//            int dado = 0;
//            int canal = 0;
//            int estado = 0;
//            #endregion

//            try
//            {
//                if (conexao.IsOpen)
//                {
//                    //listaDados = new Queue<Dados>();
//                    while (conexao.BytesToRead > 0)
//                    {
//                        #region Nova Maquina de Estado
//                        tmpRead = conexao.ReadByte();

//                        switch (estado)
//                        {
//                            case 0:
//                                if ((tmpRead & 0xE0) == 0x80)
//                                {
//                                    canal = tmpRead & 0x7F;
//                                    dado = 0;
//                                    estado = 1;
//                                }
//                                break;

//                            case 1:
//                                if ((tmpRead & 0xE0) == 0x00) // Verifica se é dado e se é o primeiro
//                                {
//                                    dadoParcial = (tmpRead & 0x1F);
//                                    if (((dadoParcial & 0x10) == 0x10) && dado == 0) // Verifica o sinal
//                                    // o dado tem que ser zero e o primeiro bit do dado parcial tem que ser 1
//                                    {
//                                        dadoParcial |= 0xF0; // nega os primeiros 3 bits do dado temporário
//                                        dado = -1; // faz todos os bits iguais a 1
//                                    }
//                                    dado = dadoParcial << 25; // Recebe os 5 digitos menos significativos

//                                    estado = 2; // muda o estado
//                                }
//                                else
//                                {
//                                    estado = 0; // Volta a esperar um canal
//                                }
//                                break;

//                            case 2:
//                                if ((tmpRead & 0xE0) == 0xA0) // Verifica se é dado e se é o segundo
//                                {
//                                    dadoParcial = (tmpRead & 0x1F);
//                                    if (((dadoParcial & 0x10) == 0x10) && dado == 0) // Verifica o sinal
//                                    {
//                                        dadoParcial |= 0xF0;
//                                        dado = -1;
//                                    }
//                                    dado |= dadoParcial << 20; // Recebe os 5 digitos menos significativos

//                                    estado = 3; // muda o estado
//                                }
//                                else
//                                {
//                                    estado = 0; // Volta a esperar um canal
//                                }
//                                break;

//                            case 3:
//                                if ((tmpRead & 0xE0) == 0xC0) // Verifica se é dado e se é o terceiro
//                                {
//                                    dadoParcial = (tmpRead & 0x1F);
//                                    if (((dadoParcial & 0x10) == 0x10) && dado == 0) // Verifica o sinal
//                                    {
//                                        dadoParcial |= 0xF0;
//                                        dado = -1;
//                                    }
//                                    dado |= dadoParcial << 15; // Recebe os 5 digitos menos significativos

//                                    estado = 4; // muda o estado
//                                }
//                                else
//                                {
//                                    estado = 0; // Volta a esperar um canal
//                                }
//                                break;
//                            case 4:
//                                if ((tmpRead & 0xE0) == 0x60) // Verifica se é dado e se é o quarto
//                                {
//                                    dadoParcial = (tmpRead & 0x1F);
//                                    if (((dadoParcial & 0x10) == 0x10) && dado == 0) // Verifica o sinal
//                                    {
//                                        dadoParcial |= 0xF0;
//                                        dado = -1;
//                                    }
//                                    dado |= dadoParcial << 10; // Recebe os 5 digitos menos significativos

//                                    estado = 5; // muda o estado
//                                }
//                                else
//                                {
//                                    estado = 0; // Volta a esperar um canal
//                                }
//                                break;

//                            case 5:
//                                if ((tmpRead & 0xE0) == 0x40) // Verifica se é dado e se é o quinto
//                                {
//                                    dadoParcial = (tmpRead & 0x1F);
//                                    if (((dadoParcial & 0x10) == 0x10) && dado == 0) // Verifica o sinal
//                                    {
//                                        dadoParcial |= 0xF0;
//                                        dado = -1;
//                                    }
//                                    dado |= dadoParcial << 5; // Recebe os 5 digitos menos significativos

//                                    estado = 6; // muda o estado
//                                }
//                                else
//                                {
//                                    estado = 0; // Volta a esperar um canal
//                                }
//                                break;

//                            case 6:
//                                if ((tmpRead & 0xE0) == 0x20) // Verifica se é dado e se é o sexto
//                                {
//                                    dado |= (tmpRead & 0x1F); // Recebe os 5 digitos menos significativos

//                                    #region Verifica qual o canal para adicionar na classe
//                                    switch (canal)
//                                    {
//                                        case ((int)canais.acelX):
//                                            if (d._acelX)
//                                            {
//                                                listaDados.Enqueue(d);
//                                                d = new Dados();
//                                            }
//                                            d.acelX = (double)dado;
//                                            d._acelX = true;
//                                            break;
//                                        case ((int)canais.acelY):
//                                            if (d._acelY)
//                                            {
//                                                listaDados.Enqueue(d);
//                                                d = new Dados();
//                                            }
//                                            d.acelY = (double)dado;
//                                            d._acelY = true;
//                                            break;
//                                        case ((int)canais.acelZ):
//                                            if (d._acelZ)
//                                            {
//                                                listaDados.Enqueue(d);
//                                                d = new Dados();
//                                            }
//                                            d.acelZ = (double)dado;
//                                            d._acelZ = true;
//                                            break;
//                                        case ((int)canais.anguloVolante):
//                                            if (d._anguloVolante)
//                                            {
//                                                listaDados.Enqueue(d);
//                                                d = new Dados();
//                                            }
//                                            d._anguloVolante = true;
//                                            d.anguloVolante = dado;
//                                            break;
//                                        case ((int)canais.freioDianteiro):
//                                            if (d._freioDianteiro)
//                                            {
//                                                listaDados.Enqueue(d);
//                                                d = new Dados();
//                                            }
//                                            d._freioDianteiro = true;
//                                            d.freioDianteiro = dado;
//                                            break;
//                                        case ((int)canais.freioTraseiro):
//                                            if (d._freioTraseiro)
//                                            {
//                                                listaDados.Enqueue(d);
//                                                d = new Dados();
//                                            }
//                                            d._freioTraseiro = true;
//                                            d.freioTraseiro = dado;
//                                            break;
//                                        case ((int)canais.latitude):
//                                            if (d._Latitude)
//                                            {
//                                                listaDados.Enqueue(d);
//                                                d = new Dados();
//                                            }
//                                            d._Latitude = true;
//                                            d.Latitude = (double)dado;
//                                            break;
//                                        case ((int)canais.longitude):
//                                            if (d._Longitude)
//                                            {
//                                                listaDados.Enqueue(d);
//                                                d = new Dados();
//                                            }
//                                            d._Longitude = true;
//                                            d.Longitude = dado;
//                                            break;
                                        
//                                        case ((int)canais.pitch):
//                                            if (d._pitch)
//                                            {
//                                                listaDados.Enqueue(d);
//                                                d = new Dados();
//                                            }
//                                            d._pitch = true;
//                                            d.pitch = (double)dado;
//                                            break;
//                                        case ((int)canais.posicaoX):
//                                            if (d._posicaoX)
//                                            {
//                                                listaDados.Enqueue(d);
//                                                d = new Dados();
//                                            }
//                                            d._posicaoX = true;
//                                            d.posicaoX = (double)dado;
//                                            break;
//                                        case ((int)canais.posicaoY):
//                                            if (d._posicaoY)
//                                            {
//                                                listaDados.Enqueue(d);
//                                                d = new Dados();
//                                            }
//                                            d._posicaoY = true;
//                                            d.posicaoY = (double)dado;
//                                            break;
//                                        case ((int)canais.roll):
//                                            if (d._roll)
//                                            {
//                                                listaDados.Enqueue(d);
//                                                d = new Dados();
//                                            }
//                                            d._roll = true;
//                                            d.roll = (double)dado;
//                                            break;
//                                        case ((int)canais.rotacaoMotor):
//                                            if (d._rotacaoMotor)
//                                            {
//                                                listaDados.Enqueue(d);
//                                                d = new Dados();
//                                            }
//                                            d._rotacaoMotor = true;
//                                            d.rotacaoMotor = dado;
//                                            break;
//                                        case ((int)canais.velocidade1):
//                                            if (d._velocidade1)
//                                            {
//                                                listaDados.Enqueue(d);
//                                                d = new Dados();
//                                            }
//                                            d._velocidade1 = true;
//                                            d.velocidade1 = dado;
//                                            break;
//                                        case ((int)canais.velocidade2):
//                                            if (d._velocidade2)
//                                            {
//                                                listaDados.Enqueue(d);
//                                                d = new Dados();
//                                            }
//                                            d._velocidade2 = true;
//                                            d.velocidade2 = dado;
//                                            break;
//                                        case ((int)canais.velocidade3):
//                                            if (d._velocidade3)
//                                            {
//                                                listaDados.Enqueue(d);
//                                                d = new Dados();
//                                            }
//                                            d._velocidade3 = true;
//                                            d.velocidade3 = dado;
//                                            break;
//                                        case ((int)canais.velocidade4):
//                                            if (d._velocidade4)
//                                            {
//                                                listaDados.Enqueue(d);
//                                                d = new Dados();
//                                            }
//                                            d._velocidade4 = true;
//                                            d.velocidade4 = dado;
//                                            break;
//                                        case ((int)canais.velocidadeCarro):
//                                            if (d._velocidadeCarro)
//                                            {
//                                                listaDados.Enqueue(d);
//                                                d = new Dados();
//                                            }
//                                            d._velocidadeCarro = true;
//                                            d.velocidadeCarro = dado;
//                                            break;
//                                        case ((int)canais.velocidadeGPS):
//                                            if (d._velocidadeGPS)
//                                            {
//                                                listaDados.Enqueue(d);
//                                                d = new Dados();
//                                            }
//                                            d._velocidadeGPS = true;
//                                            d.velocidadeGPS = dado;
//                                            break;
//                                        case ((int)canais.yaw):
//                                            if (d._yaw)
//                                            {
//                                                listaDados.Enqueue(d);
//                                                d = new Dados();
//                                            }
//                                            d._yaw = true;
//                                            d.yaw = dado;
//                                            break;
//                                    }
//                                    #endregion
//                                }
//                                estado = 0;
//                                d.tempo = DateTime.Now;
//                                break;
//                        }
//                        #endregion
//                    }
//                }
//            }
//            catch { }
//        }
        

//        private void SerialReceiving(object sender, SerialDataReceivedEventArgs e)
//        {
//            #region Variáveis Maquina de estado
//            int tmpRead = 0;
//            int dadoParcial = 0;
//            int dado = 0;
//            int canal = 0;
//            int estado = 0;
//            #endregion

//            try
//            {
//                if (conexao.IsOpen)
//                {
//                    while (conexao.BytesToRead > 0)
//                    {
//                        #region Nova Maquina de Estado
//                        tmpRead = conexao.ReadByte();
                        
//                        switch (estado)
//                        {
//                            case 0:
//                                if ((tmpRead & 0xE0) == 0x80)
//                                {
//                                    canal = tmpRead & 0x7F;
//                                    dado = 0;
//                                    estado = 1;
//                                }
//                                break;

//                            case 1:
//                                if ((tmpRead & 0xE0) == 0x00) // Verifica se é dado e se é o primeiro
//                                {
//                                    dadoParcial = (tmpRead & 0x1F);
//                                    if (((dadoParcial & 0x10) == 0x10) && dado == 0) // Verifica o sinal
//                                    // o dado tem que ser zero e o primeiro bit do dado parcial tem que ser 1
//                                    {
//                                        dadoParcial |= 0xF0; // nega os primeiros 3 bits do dado temporário
//                                        dado = -1; // faz todos os bits iguais a 1
//                                    }
//                                    dado = dadoParcial << 25; // Recebe os 5 digitos menos significativos

//                                    estado = 2; // muda o estado
//                                }
//                                else
//                                {
//                                    estado = 0; // Volta a esperar um canal
//                                }
//                                break;

//                            case 2:
//                                if ((tmpRead & 0xE0) == 0xA0) // Verifica se é dado e se é o segundo
//                                {
//                                    dadoParcial = (tmpRead & 0x1F);
//                                    if (((dadoParcial & 0x10) == 0x10) && dado == 0) // Verifica o sinal
//                                    {
//                                        dadoParcial |= 0xF0;
//                                        dado = -1;
//                                    }
//                                    dado |= dadoParcial << 20; // Recebe os 5 digitos menos significativos

//                                    estado = 3; // muda o estado
//                                }
//                                else
//                                {
//                                    estado = 0; // Volta a esperar um canal
//                                }
//                                break;

//                            case 3:
//                                if ((tmpRead & 0xE0) == 0xC0) // Verifica se é dado e se é o terceiro
//                                {
//                                    dadoParcial = (tmpRead & 0x1F);
//                                    if (((dadoParcial & 0x10) == 0x10) && dado == 0) // Verifica o sinal
//                                    {
//                                        dadoParcial |= 0xF0;
//                                        dado = -1;
//                                    }
//                                    dado |= dadoParcial << 15; // Recebe os 5 digitos menos significativos

//                                    estado = 4; // muda o estado
//                                }
//                                else
//                                {
//                                    estado = 0; // Volta a esperar um canal
//                                }
//                                break;
//                            case 4:
//                                if ((tmpRead & 0xE0) == 0x60) // Verifica se é dado e se é o quarto
//                                {
//                                    dadoParcial = (tmpRead & 0x1F);
//                                    if (((dadoParcial & 0x10) == 0x10) && dado == 0) // Verifica o sinal
//                                    {
//                                        dadoParcial |= 0xF0;
//                                        dado = -1;
//                                    }
//                                    dado |= dadoParcial << 10; // Recebe os 5 digitos menos significativos

//                                    estado = 5; // muda o estado
//                                }
//                                else
//                                {
//                                    estado = 0; // Volta a esperar um canal
//                                }
//                                break;

//                            case 5:
//                                if ((tmpRead & 0xE0) == 0x40) // Verifica se é dado e se é o quinto
//                                {
//                                    dadoParcial = (tmpRead & 0x1F);
//                                    if (((dadoParcial & 0x10) == 0x10) && dado == 0) // Verifica o sinal
//                                    {
//                                        dadoParcial |= 0xF0;
//                                        dado = -1;
//                                    }
//                                    dado |= dadoParcial << 5; // Recebe os 5 digitos menos significativos

//                                    estado = 6; // muda o estado
//                                }
//                                else
//                                {
//                                    estado = 0; // Volta a esperar um canal
//                                }
//                                break;

//                            case 6:
//                                if ((tmpRead & 0xE0) == 0x20) // Verifica se é dado e se é o sexto
//                                {
//                                    dado |= (tmpRead & 0x1F); // Recebe os 5 digitos menos significativos
//                                    this.dados[canal].Enqueue(dado); // adiciona no vetor
//                                }
//                                estado = 0;
//                                break;
//                        }
//                        #endregion
//                    }
//                }
//            }
//            catch (TimeoutException) { }
//        }
//    }
//}
