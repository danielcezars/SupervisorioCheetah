using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SupervisorioCheetah
{
    /// <summary>
    /// Interaction logic for EscolherSensores.xaml
    /// </summary>
    public partial class EscolherSensores : UserControl, INotifyPropertyChanged
    {
        private ObservableCollection<BoolStringClass> listaSensores = new ObservableCollection<BoolStringClass>();

        public EscolherSensores()
        {
            InitializeComponent();

            ItensListBox.ItemsSource = ListaSensores;
        }

        public ObservableCollection<BoolStringClass> ListaSensores
        {
            get
            { return (ObservableCollection<BoolStringClass>)GetValue(sensorProperty); }
            set
            { SetValueDp(sensorProperty, value); }
        }
        public static readonly DependencyProperty sensorProperty = DependencyProperty.Register("listaSensores",
            typeof(ObservableCollection<BoolStringClass>), typeof(EscolherSensores), null);

        public event PropertyChangedEventHandler PropertyChanged;
        void SetValueDp(DependencyProperty property, object value, [System.Runtime.CompilerServices.CallerMemberName] String p = null)
        {
            SetValue(property, value);
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(p));
            }
        }
    }
}
