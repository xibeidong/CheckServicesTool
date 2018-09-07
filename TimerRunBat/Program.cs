using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
namespace TimerRunBat
{
    class Program
    {


        static void Main(string[] args)
        {

            while (true)
            {
                Thread.Sleep(60000);
                Process proc = null;
                try
                {
                    string targetDir = AppDomain.CurrentDomain.BaseDirectory;
                    // string targetDir = string.Format(@"C:\Users\patrick\Desktop\");//this is where testChange.bat lies
                    proc = new Process();
                    proc.StartInfo.WorkingDirectory = targetDir;
                    proc.StartInfo.FileName = "启动services.bat";
                    proc.StartInfo.Arguments = string.Format("10");//this is argument
                    //proc.StartInfo.CreateNoWindow = true;
                    proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;//这里设置DOS窗口不显示，经实践可行
                    proc.Start();
                    proc.WaitForExit();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception Occurred :{0},{1}", ex.Message, ex.StackTrace.ToString());
                }

            }
          
            
        }
    }
}
