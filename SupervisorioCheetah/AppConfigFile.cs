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
    class AppConfigFile
    {
        Configuration config;
        private ObservableCollection<BoolStringClass> listaSensores;
        public AppConfigFile()
        {
            config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            ChartSection s = ChartSection.open("chartsCollection");
            //ChartsConfigSection g = (ChartsConfigSection)config.GetSection("chartsCollection");
        }

        public void addKey(string key, string value)
        {
            config.AppSettings.Settings.Add(key, value);
        }

        public void removeKey(string key)
        {
            config.AppSettings.Settings.Remove(key);
        }

        public void addChartKeys(SingleChart singleChart)
        {
            foreach (BoolStringClass s in singleChart.listaSensores)
            {
                addKey(s.TheText, s.IsSelected.ToString());
            }
        }

        public void addChartKeys(List<SingleChart> chart)
        {
            foreach (SingleChart s in chart)
            {
                addChartKeys(s);
            }
        }

    }


    public class ChartSection : ConfigurationSection
    {
        private static ChartSection instance;
        private ChartSection() { }

        public static ChartSection open(string section)
        {
            return open(System.Reflection.Assembly.GetEntryAssembly().Location, section);
        }
        public static ChartSection open(string path, string section)
        {
            if ((object)instance == null)
            {
                if (path.EndsWith(".config", StringComparison.InvariantCultureIgnoreCase))
                { path = path.Remove(path.Length - 7); }

                Configuration config = ConfigurationManager.OpenExeConfiguration(path);

                if (config.Sections[section] == null)
                {
                    instance = new ChartSection();
                    config.Sections.Add(section, instance);
                    config.Save(ConfigurationSaveMode.Modified);
                }
                else
                { instance = (ChartSection)config.Sections[section]; }
            }
            return instance;
        }

        [ConfigurationProperty("charts")]
        public ChartsCollection Members
        {
            get { return ((ChartsCollection)base["charts"]); }
            set { base["charts"] = value; }
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
}
