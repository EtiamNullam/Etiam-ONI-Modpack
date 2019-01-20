using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Common
{
    public static class LoadCommon
    {
        public static void OnLoad()
        {
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            string currentAssemblyName = currentAssembly.GetName().Name;
            string resourceName =  currentAssemblyName + ".Common.Common.dll";

            using (var stream = currentAssembly.GetManifestResourceStream(resourceName))
            {
                if (stream != null)
                {
                    byte[] assemblyData = new byte[stream.Length];

                    stream.Read(assemblyData, 0, assemblyData.Length);

                    Assembly.Load(assemblyData);
                }
                else
                {
                    throw new Exception(currentAssemblyName + ": Couldn't load Embedded Resource: " + resourceName);
                }
            }
        }
    }
}
