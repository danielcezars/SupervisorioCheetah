using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Windows;
using System.Collections.ObjectModel;

namespace SupervisorioCheetah
{
   public class ChartSection : ConfigurationSection
    {
        private static ChartSection instance;

        private ChartSection() { }

        public static void addChart(List<SingleChart> chartList)
        {
            foreach (SingleChart s in chartList)
            {
                addChart(s);
            }
        }

        public static void addChart(SingleChart singleChart)
        {
            string path = System.Reflection.Assembly.GetEntryAssembly().Location;

            if (path.EndsWith(".config", StringComparison.InvariantCultureIgnoreCase))
            { path = path.Remove(path.Length - 7); }

            Configuration config = ConfigurationManager.OpenExeConfiguration(path);

            int index = 0;
            while (config.Sections["chart" + index.ToString()] != null)
            {
                index++;
            }

            addSensors(singleChart.listaSensores);

            if (config.Sections["chart" + index.ToString()] == null)
            {
                config.Sections.Add("chart" + index.ToString(), instance);
                config.Save(ConfigurationSaveMode.Modified);
            }
            instance = (ChartSection)config.Sections["chart" + index.ToString()];
        }
        

        public static void addSensors(ObservableCollection<BoolStringClass> lista)
        {
            chartElement ch;
            instance = new ChartSection();

            foreach (BoolStringClass b in lista)
            {
                ch = new chartElement();
                ch.sensor = b.TheText;
                ch.isSelected = b.IsSelected.ToString();

                instance.myChart.Add(ch);
            }
        }

        public static void removeAllCharts()
        {
            string path = System.Reflection.Assembly.GetEntryAssembly().Location;

            if (path.EndsWith(".config", StringComparison.InvariantCultureIgnoreCase))
            { path = path.Remove(path.Length - 7); }

            Configuration config = ConfigurationManager.OpenExeConfiguration(path);

            int index = 0;
            while (config.Sections["chart" + index.ToString()] != null)
            {
                config.Sections.Remove("chart" + index.ToString());
                index++;
            }

            config.Save(ConfigurationSaveMode.Modified);
        }
        
        public static SingleChart getSingleChart(ChartSection instance)
        {
            ObservableCollection<BoolStringClass> lista = new ObservableCollection<BoolStringClass>();

            foreach (chartElement cht in instance.myChart)
            {
                foreach (Sensores s in Enum.GetValues(typeof(Sensores)))
                {
                    if (cht.sensor == s.ToString())
                    {
                        lista.Add(new BoolStringClass { IsSelected = Convert.ToBoolean(cht.isSelected), TheText = cht.sensor });
                        break;
                    }
                }
            }
            return new SingleChart(lista);
        }

        public static List<SingleChart> getAllCharts()
        {
            string path = System.Reflection.Assembly.GetEntryAssembly().Location;

            if (path.EndsWith(".config", StringComparison.InvariantCultureIgnoreCase))
            { path = path.Remove(path.Length - 7); }

            Configuration config = ConfigurationManager.OpenExeConfiguration(path);
            List<SingleChart> charts = new List<SingleChart>();

            int index = 0;
            while (config.Sections["chart" + index.ToString()] != null)
            {
                charts.Add( getSingleChart((ChartSection)config.Sections["chart" + index.ToString()]));
                index++;
            }
            return charts;
        }

        [ConfigurationProperty("chart")]
        public ChartsCollection myChart
        {
            get { return ((ChartsCollection)base["chart"]); }
            set { base["chart"] = value; }
        }
    }

    public class chartElement : ConfigurationElement
    { 
        [ConfigurationProperty("sensor", DefaultValue = "", IsKey = true, IsRequired = false)]
        public string sensor
        {
            get { return ((string)(base["sensor"])); }
            set { base["sensor"] = value; }
        }

        [ConfigurationProperty("isSelected", DefaultValue = "", IsKey = false, IsRequired = true)]
        public string isSelected
        {
            get { return ((string)(base["isSelected"])); }
            set { base["isSelected"] = value; }
        }
    }

    [ConfigurationCollection(typeof(chartElement))]
    public class ChartsCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new chartElement();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((chartElement)(element)).sensor;
        }
        public chartElement this[int idx]
        {
            get { return (chartElement)BaseGet(idx); }
            set
            {
                if (BaseGet(idx) != null)
                { BaseRemoveAt(idx); }
                BaseAdd(idx, value);
            }
        }
        public void Add(chartElement ch)
        {
            BaseAdd(ch);
        }
        public void Clear()
        {
            BaseClear();
        }
        public void RemoveAt(int idx)
        {
            BaseRemoveAt(idx);
        }
    }
}
