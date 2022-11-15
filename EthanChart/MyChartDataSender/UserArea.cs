using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyChartDataSender
{
    public enum AxisData
    {
        CommandPos,
        ActualPos,
        CommandVelocity,
        ActualVelocity,
    }

    public class DataSenderAxisItem : IDataSenderItem
    {
        public int KeyCount { get; private set; }

        /// <summary>
        /// Head Name, Item Names
        /// </summary>
        Dictionary<string, string[]> senderItems { get; }

        public string[] KeyNames => senderItems.Keys.ToArray();

        public DataSenderAxisItem()
        {
            senderItems = new Dictionary<string, string[]>();

            var axisItem = new string[8];
            for (int i = 0; i < 8; i++)
            {
                axisItem[i] = "Axis " + i.ToString("00");
            }
            senderItems.Add("Axis", axisItem);
            senderItems.Add(nameof(AxisData), Enum.GetNames(typeof(AxisData)));

            KeyCount = senderItems.Count;
        }

        public string[] ItemList(string keyName)
        {
            if (senderItems.ContainsKey(keyName))
            {
                return senderItems[keyName];
            }
            else
            {
                return null;
            }
        }

        public void Update()
        {
            
        }
    }

    public class SenderConfig
    {
        byte maxLevel;
        byte[] _lvItemCount;
        LevelItem a;
        IDataSenderItem senderItem;
        Thread tUpdate;
        bool bUpdate;

        public ushort UpdateTime_ms { get; private set; }
        public bool IsOpen => senderItem != null;
        public bool IsConfigReady { get; private set; }
        public bool IsConfig { get; private set; }
        
        public bool Open(IDataSenderItem senderItem)
        {
            if (senderItem == null) return false;

            this.senderItem = senderItem;
            var keyName = senderItem.KeyNames;

            if (keyName == null || keyName.Length == 0)            
            {
                this.senderItem = null;
                return false;
            }

            maxLevel = (byte)keyName.Length;
            _lvItemCount = new byte[maxLevel];

            for (int i = 0; i < maxLevel; i++)
            {
                _lvItemCount[i] = (byte)(senderItem.ItemList(keyName[i]).Length);
                if (_lvItemCount[i] == 0)
                {
                    this.senderItem = null;
                    return false;
                }
            }
            return true;
        }
        
        public bool Config(List<ISeriesData> seriesList)
        {
            if (seriesList == null) return false;
            if (seriesList.Count == 0) return false;

            StartConfig();
            for (int i = 0; i < seriesList.Count; i++)
            {
                Config(seriesList[i].ValueReceived, seriesList[i].Indices);
            }
            EndConfig();
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="updateTime_ms">10~1000</param>
        /// <returns></returns>
        public bool Start(ushort updateTime_ms = 50)
        {
            if (!IsOpen) return false;            

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
            bUpdate = true;
            tUpdate.Start();
            return true;
        }

        public void Stop()
        {
            bUpdate = false;
        }

        public void Update()
        {

        }

        public void Set(double value, params int[] index)
        {
            if (!IsConfig) return;
            if (index == null || index.Length != maxLevel) return;

            LevelItem ab = a;
            for (int i = 0; i < maxLevel; i++)
            {
                if (i != maxLevel - 1)
                {
                    ab = GetSub(ab, index[i]);
                    if (ab == null) break;
                }
                else
                {
                    ab.Set(value);
                }
            }
        }

        //-------------------------------

        private void StartConfig()
        {
            if (!IsOpen) return;

            if (a == null) a = new LevelItem(-1, _lvItemCount[0]);
            IsConfigReady = true;
            IsConfig = false;
        }

        private void Config(Action<double> act, params int[] index)
        {
            if (!IsConfigReady) return;

            if (index == null || index.Length != maxLevel)
            {
                IsConfigReady = false;
                return;
            }

            LevelItem _a = a;
            for (int i = 0; i < maxLevel; i++)
            {
                if (i != maxLevel - 1)
                {
                    CreateInst(_a, index[i], _lvItemCount[i + 1]);
                    _a = _a[index[i]];
                }
                else
                {
                    CreateInst(_a, index[i], act);
                }
            }
        }

        private void EndConfig()
        {
            if (IsConfigReady)
            {
                IsConfigReady = false;
                IsConfig = true;
            }
        }        

        private void CreateInst(LevelItem ab, int abIndex, byte count)
        {
            if (ab[abIndex] == null)
            {
                if (count > 0)
                {
                    ab[abIndex] = new LevelItem(abIndex, count);
                }
                else
                {
                    
                }
            }
        }

        private void CreateInst(LevelItem ab, int abIndex, Action<double> act)
        {
            if (ab[abIndex] == null)
            {
                ab[abIndex] = new LevelItem(abIndex, act);
            }
        }

        private LevelItem GetSub(LevelItem main, int index)
        {
            if (main[index] != null) return main[index];
            else return null;
        }
    }

    internal class LevelItem
    {
        LevelItem[] items;
        public int Index { get; private set; }
        public LevelItem this[int inx]
        {
            get => items[inx];
            set => items[inx] = value;
        }
        Action<double> act;

        public LevelItem(int index, Action<double> act)
        {
            this.Index = index;
            this.act = act;
        }

        public LevelItem(int index, int itemCount)
        {
            this.Index = index;
            items = new LevelItem[itemCount];
        }

        public void Set(double val)
        {
            if (act != null)
            {
                act(val);
            }
        }
    }
}