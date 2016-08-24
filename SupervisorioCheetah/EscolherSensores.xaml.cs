using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace SupervisorioCheetah
{
    /// <summary>
    /// Interaction logic for EscolherSensores.xaml
    /// </summary>
    public partial class EscolherSensores : UserControl, INotifyPropertyChanged
    {
        public EscolherSensores()
        {
            InitializeComponent();

            ItensListBox.ItemsSource = ListaSensores;
        }

        public ObservableCollection<BoolStringClass> ListaSensores
        {
            get { return (ObservableCollection<BoolStringClass>)GetValue(sensorProperty); }
            set { SetValueDp(sensorProperty, value); }
        }

        public static readonly DependencyProperty sensorProperty = DependencyProperty.Register("listaSensores",
            typeof(ObservableCollection<BoolStringClass>), typeof(EscolherSensores), null);

        public event PropertyChangedEventHandler PropertyChanged;
        void SetValueDp(DependencyProperty property, object value, [System.Runtime.CompilerServices.CallerMemberName] String p = null)
        {
            SetValue(property, value);
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(p));
            }
        }
    }
}