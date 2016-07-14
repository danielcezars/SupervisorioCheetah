using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace SupervisorioCheetah
{
    public static class Serial //: IDisposable
    {
        public static void Dispose()
        {
            conexao.Close();

            if (conexao != null)
            {
                conexao.Dispose();
                conexao = null;
            }
        }

        /// <summary>
        /// Recebe os dados do buffer da serial aberta
        /// </summary>
        public static void getData(SerialPort p, Queue<double>[] d)
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
                if (p.IsOpen)
                {
                    while (p.BytesToRead > 0)
                    {
                        #region Nova Maquina de Estado
                        tmpRead = p.ReadByte();

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

                                    d[canal].Enqueue(dado);
                                }
                                estado = 0;
                                dado = 0;
                                canal = 0;
                                break;
                        }
                        #endregion
                    }
                }
            }
            catch { }
        }
        
        private static void SerialReceiving(object sender, SerialDataReceivedEventArgs e)
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
                                    this.dados[canal].Enqueue(dado); // adiciona no vetor
                                }
                                estado = 0;
                                break;
                        }
                        #endregion
                    }
                }
            }
            catch (TimeoutException) { }
        }
    }
}
