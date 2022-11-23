using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EthChartDef.App
{
    public interface ISeriesData
    {
        /// <summary>
        /// Length == IDataSenderItem - KeyNames Length,
        /// Array Index == IDataSenderItem - KeyName - ItemList Array Index
        /// </summary>
        int[] Indices { get; }
        Action<double> ValueReceived { get; }
        void Clear();
    }
}
