using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EthChartDef;

namespace MyChartDataSender
{
    public class SenderConfig
    {
        
    }

    public class DataSender : IDataSenderForUser
    {
        public event Action Starting;
        public event Action Done;

        byte maxLevel;
        byte[] _lvItemCount;
        
        Thread tUpdate;
        bool bUpdate = false;

        public SenderListlItem ListItem { get; private set; }

        public ushort UpdateTime_ms { get; private set; }
        public bool IsOpen => UserSender != null;
        public bool IsConfigReady { get; private set; }
        public bool IsConfig { get; private set; }
        public UserDefSender UserSender { get; private set; }

        public bool Open(UserDefSender userSender)
        {
            if (userSender == null) return false;

            this.UserSender = userSender;
            this.UserSender.DataSender = this;
            var keyName = userSender.KeyNames;

            if (keyName == null || keyName.Length == 0)
            {
                this.UserSender = null;
                return false;
            }

            maxLevel = (byte)keyName.Length;
            _lvItemCount = new byte[maxLevel];

            for (int i = 0; i < maxLevel; i++)
            {
                _lvItemCount[i] = (byte)(userSender.ItemList(keyName[i]).Length);
                if (_lvItemCount[i] == 0)
                {
                    this.UserSender = null;
                    return false;
                }
            }
            return true;
        }

        public void Close()
        {
            bUpdate = false;
            if (tUpdate != null)
            {
                if (tUpdate.IsAlive)
                {
                    tUpdate.Join(250);
                }

                tUpdate = null;
            }

            IsConfigReady = false;
            IsConfig = false;

            Starting = null;
            Done = null;
            maxLevel = 0;
            _lvItemCount = null;
            UserSender = null;                        
            ListItem = null;
        }
        
        public bool ConfigSeries(IList<ISeriesData> seriesList)
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

            if (Starting != null) Starting();

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

        private void Update()
        {
            while(bUpdate)
            {
                UserSender.Update();
                Thread.Sleep(UpdateTime_ms);
            }

            if (Done != null) Done();
        }

        public void Set(double value, params int[] index)
        {
            if (!IsConfig) return;
            if (index == null || index.Length != maxLevel) return;

            var listItem = ListItem;
            for (int i = 0; i <= maxLevel; i++)
            {
                if (i != maxLevel)
                {
                    listItem = GetSub(listItem, index[i]);
                    if (listItem == null) break;
                }
                else
                {
                    listItem.Set(value);
                }
            }
        }

        //-------------------------------

        private void StartConfig()
        {
            if (!IsOpen) return;

            ListItem = new SenderListlItem(-1, _lvItemCount[0]);
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

            var listItem = ListItem;
            for (int i = 0; i < maxLevel; i++)
            {
                if (i != maxLevel - 1)
                {
                    CreateInst(listItem, index[i], _lvItemCount[i + 1]);
                    listItem = listItem[index[i]];
                }
                else
                {
                    CreateInst(listItem, index[i], act);
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

        private void CreateInst(SenderListlItem item, int index, byte count)
        {
            if (item[index] == null)
            {
                if (count > 0)
                {
                    item[index] = new SenderListlItem(index, count);
                }
                else
                {
                    
                }
            }
        }

        private void CreateInst(SenderListlItem item, int index, Action<double> act)
        {
            if (item[index] == null)
            {
                item[index] = new SenderListlItem(index, act);
            }
        }

        private SenderListlItem GetSub(SenderListlItem main, int index)
        {
            if (main[index] != null) return main[index];
            else return null;
        }
    }
}