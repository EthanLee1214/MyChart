using EthChartDef.Sender;
using EthChartDef.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace EthChartDef
{
    public static class Define
    {
        public const byte MaxItemNum = 3;
        public const byte MaxSeriesNum = 4;
    }

    public static class Util
    {
        public static T GetInstToAssembly<T>(string dllName) where T : class
        {
            Assembly asm = null;
            try
            {
                asm = Assembly.LoadFile(dllName);
            }
            catch (Exception ex)
            {
                return null;
            }
            var types = asm.GetExportedTypes();

            T t = null;

            for (int i = 0; i < types.Length; i++)
            {
                if (types[i].BaseType == typeof(T))
                {
                    t = Activator.CreateInstance(types[i]) as T;
                    break;
                }
            }
            return t;
        }

        public static T GetInterfaceToAssembly<T>(string dllName) where T : class
        {
            Assembly asm = null;
            try
            {
                asm = Assembly.LoadFile(dllName);
            }
            catch (Exception ex)
            {
                return null;
            }
            var types = asm.GetExportedTypes();

            T t = null;

            for (int i = 0; i < types.Length; i++)
            {   
                if (types[i].GetInterface(typeof(T).Name) == typeof(T))
                {
                    t = Activator.CreateInstance(types[i]) as T;
                    break;
                }
            }
            return t;
        }
    }
}