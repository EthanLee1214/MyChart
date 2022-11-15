using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Data;

namespace MyChartDataSender
{
    public class MyDataSender
    {
        readonly string NameCmn = "Name";
        readonly string ValueCmn = "Value";

        DataTable _dt;
        IDataSenderItem senderItem;
        Thread tUpdate;
        bool bUpdateFlag = false;
        public ushort UpdateTime_ms { get; private set; }

        public IDataSenderItem SenderItem => senderItem;

        public MyDataSender()
        {
            senderItem = new DataSenderAxisItem();
            _dt = new DataTable();

            var keys = senderItem.KeyNames;
            var keyCmn = new List<DataColumn>(keys.Length);

            for (int i = 0; i < keys.Length; i++)
            {
                var cmn = new DataColumn();
                cmn.ColumnName = keys[i];
                cmn.DataType = typeof(int);
                _dt.Columns.Add(cmn);
                keyCmn.Add(cmn);
            }
            _dt.PrimaryKey = keyCmn.ToArray();
            _dt.Columns.Add(ValueCmn, typeof(double));
        }

        public bool ConfigItem(List<ISeriesData> seriesList)
        {
            _dt.Rows.Clear();

            for (int i = 0; i < seriesList.Count; i++)
            {
                var row = _dt.Rows.Add();

                for (int key = 0; key < senderItem.KeyCount; key++)
                {
                    try
                    {
                        row[key] = seriesList[i].Indices[key];
                    }
                    catch(Exception ex)
                    {
                        // 아마 Key 중복 예외
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="updateTime_ms">10~1000</param>
        /// <returns></returns>
        public bool Start(ushort updateTime_ms = 50)
        {
            if (_dt == null) return false;
            if (_dt.Rows.Count == 0) return false;

            if (updateTime_ms > 1000)
            {
                UpdateTime_ms = 1000;
            }
            else if (updateTime_ms < 10)
            {
                UpdateTime_ms = 10;
            }
            else
            {
                UpdateTime_ms = updateTime_ms;
            }            

            tUpdate = new Thread(Update);
            tUpdate.IsBackground = true;
            bUpdateFlag = true;
            tUpdate.Start();
            return true;
        }

        public void Stop()
        {
            bUpdateFlag = false;
        }

        private void Update()
        {
            while(bUpdateFlag)
            {

                Thread.Sleep(UpdateTime_ms);
            }
        }
    }
}
