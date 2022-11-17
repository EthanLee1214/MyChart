using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using EthChartDef;
using MyChartDataSender;

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

        public void Start()
        {
            DLLName = @"C:\Users\HP\Desktop\MyPortfolio\MyChart\EthanChart\exe\x64\UserChartSender_WMX.dll";

            var asm = Assembly.LoadFile(DLLName);
            var types = asm.GetExportedTypes();

            UserDefSender uds = null;

            for (int i = 0; i < types.Length; i++)
            {
                if (types[i].BaseType == typeof(UserDefSender))
                {
                    uds = Activator.CreateInstance(types[i]) as UserDefSender;
                    break;
                }
            }

            DataSender ds = new DataSender();
            ds.Open(uds);

            var series = new List<ISeriesData>(4);
            series.Add(new TempSeries(new int[] { 0, 0 }));
            series.Add(new TempSeries(new int[] { 0, 1 }));
            series.Add(new TempSeries(new int[] { 1, 0 }));
            series.Add(new TempSeries(new int[] { 1, 1 }));
            ds.ConfigSeries(series);

            ds.Start();

            System.Threading.Thread.Sleep(3000);

            ds.Stop();
            ;
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
    }
}
