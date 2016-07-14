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
        public WindowChooseSerialPort()
        {
            InitializeComponent();
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
            nameConexao = (string)cmbxList.SelectedValue;
        }
    }
}
