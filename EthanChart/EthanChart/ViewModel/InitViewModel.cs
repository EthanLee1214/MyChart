using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using EthChartDef.App;
using EthChartDef.User;

namespace EthanChart.ViewModel
{
    public class InitViewModel : ViewModelBase
    {
        string dllName;

        public string DLLName
        {
            get => dllName;
            set
            {
                dllName = value;
                OnPropertyChanged();
            }
        }
    }

    public class TempSeries : ISeriesData
    {
        List<double> values;

        public int[] Indices { get; private set; }

        public Action<double> ValueReceived { get; private set; }

        public List<double> Values => values;

        public TempSeries(int[] indices)
        {
            Indices = indices;
            values = new List<double>(1000);

            ValueReceived += val =>
            {
                values.Add(val);
            };
        }

        public TempSeries(int[] indices, Action<double> valueReceived)
        {
            Indices = indices;
            ValueReceived = valueReceived;
        }

        public void Clear()
        {

        }
    }
}
