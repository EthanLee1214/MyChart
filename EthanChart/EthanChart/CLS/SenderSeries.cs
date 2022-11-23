using LiveCharts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EthChartDef.App;
using LiveCharts.Definitions.Charts;
using System.Reflection;

namespace EthanChart.CLS
{
    public class SenderSeries : ISeriesData
    {
        public IChartValues CV { get; set; }

        public int[] Indices { get; set; }

        public Action<double> ValueReceived { get; private set; }

        public SenderSeries()
        {
            ValueReceived += val =>
            {
                if (CV == null) return;
                CV.Add(val);
            };
        }

        public void SetIndices(int[] val)
        {
            Indices = val;
        }

        public void Clear()
        {
            CV.Clear();
        }
    }
}
