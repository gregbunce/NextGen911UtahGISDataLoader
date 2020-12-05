using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextGen911DataLoader.commands
{
    class ExecuteArcpyScript
    {
        public static void run_arcpy(string pythonFile, string arg1)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            //start.FileName = @"C:\Program Files\ArcGIS\Pro\bin\Python\envs\arcgispro-py3\python.exe";
            start.FileName = @"C:\Users\gbunce\AppData\Local\Programs\ArcGIS\Pro\bin\Python\envs\arcgispro-py3\python.exe";
            start.Arguments = string.Format("{0} {1}", pythonFile, arg1);
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    Console.Write(result);
                }
            }
        }

        // overload the method to allow three arguments (two of which will be passed into the python script as argv params (arg1 and arg2)).
        public static void run_arcpy(string pythonFile, string arg1, string arg2)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            //start.FileName = @"C:\Program Files\ArcGIS\Pro\bin\Python\envs\arcgispro-py3\python.exe";
            start.FileName = @"C:\Users\gbunce\AppData\Local\Programs\ArcGIS\Pro\bin\Python\envs\arcgispro-py3\python.exe";
            start.Arguments = string.Format("{0} {1} {2}", pythonFile, arg1, arg2);
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    Console.Write(result);
                }
            }
        }

    }
}
