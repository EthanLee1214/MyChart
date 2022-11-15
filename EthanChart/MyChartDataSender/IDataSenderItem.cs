using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyChartDataSender
{
    public interface IDataSenderItem
    {
        int KeyCount { get; }
        string[] KeyNames { get; }
        string[] ItemList(string keyName);
    }

    public interface ISeriesData
    {
        /// <summary>
        /// Length == IDataSenderItem - KeyNames Length,
        /// Array Index == IDataSenderItem - KeyName - ItemList Array Index
        /// </summary>
        int[] Indices { get; }
        Action<double> ValueReceived { get; }
    }

    public interface IDataSender
    {
        bool ConfigItem();
        bool Start();
        void Stop();
    }
}
