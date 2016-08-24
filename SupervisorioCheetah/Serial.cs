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
        /// <summary>
        /// Abre a conexão serial.
        /// Se a conexão já está aberta, abre de novo com os novos parametros.
        /// Caso haja erro, lança exceção - InvalidOperationException.
        /// 
        /// Se abrir mostra o nome da conexão na barra de estados
        /// Se não abrir mostra erro
        /// </summary>
        /// <param name="nome"></param>
        /// <param name="frequencia"></param>
        /// <param name="conexao">SerialPort deve ser uma instancia, não um valor nulo</param>
        /// <returns></returns>
        public static bool abrirComunicação(string nome, int frequencia, SerialPort conexao)
        {
            try
            {
                isNull(conexao);

                if (!conexao.IsOpen)
                {
                    conexao.PortName = nome;
                    conexao.BaudRate = frequencia;
                    conexao.Open();
                }
                else
                {
                    conexao.Close();
                    conexao.PortName = nome;
                    conexao.BaudRate = frequencia;
                    conexao.Open();
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// Verifica se a porta é nula, se sim inicia uma nova instancia
        /// </summary>
        /// <param name="conexao"></param>
        private static void isNull(SerialPort conexao)
        {
            if (conexao == null)
            {
                conexao = new SerialPort();
            }
        }
        /// <summary>
        /// Fecha a comuniação aberta
        /// </summary>
        /// <param name="conexao">Porta que deseja fechar</param>
        public static void fecharComunicação(SerialPort conexao)
        {
            conexao.Close();
        }

    }
}