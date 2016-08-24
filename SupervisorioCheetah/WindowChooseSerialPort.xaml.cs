using System;
using System.Collections.Generic;
using System.IO.Ports;
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
    /// Interaction logic for WindowChooseSerialPort.xaml
    /// </summary>
    public partial class WindowChooseSerialPort : Window
    {
        public string nameConexao { get; set; }
        SerialPort conexao;
        int frequencia;

        public WindowChooseSerialPort(int frequencia, SerialPort conexao)
        {
            InitializeComponent();
            this.conexao = conexao;
            this.frequencia = frequencia;
            cmbxList.ItemsSource = SerialPort.GetPortNames();
            cmbxList.SelectedIndex = 0;
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            cmbxList.ItemsSource = SerialPort.GetPortNames();
            cmbxList.SelectedIndex = 0;
        }

        private void btnConect_Click(object sender, RoutedEventArgs e)
        {
            if ((string)cmbxList.SelectedValue != string.Empty)
            {
                nameConexao = (string)cmbxList.SelectedValue;
                // Tenta conectar, altera os textos da barra de estados dependendo da resposta ------------------
                if (Serial.abrirComunicação(nameConexao, frequencia, conexao))
                {
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Não foi possível abrir a comunicação!");
                }
                // ----------------------------------------------------------------------------------------------
            }
            else
            { MessageBox.Show("Escolha uma porta serial!"); }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
