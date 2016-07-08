using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupervisorioCheetah
{
    /// <summary>
    /// Exibição de cada sensor em cada gráfico
    /// </summary>
    public class SingleChart
    {
        public ObservableCollection<BoolStringClass> listaSensores { get; set; }

        public SingleChart()
        {
            listaSensores = new ObservableCollection<BoolStringClass>();
            foreach (Sensores s in Enum.GetValues(typeof(Sensores)))
            {
                listaSensores.Add(new BoolStringClass { IsSelected = false, TheText = s.ToString() });
            }
        }
    }

    public class BoolStringClass
    {
        public string TheText { get; set; }
        public bool IsSelected { get; set; }
    }

}