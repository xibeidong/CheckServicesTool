using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.ServiceProcess;
using System.Diagnostics;

namespace CheckServicrsTools
{
    public partial class Form1 : Form
    {
       
        public Form1()
        {
            InitializeComponent();
            aTimer = new System.Timers.Timer();
            //到时间的时候执行事件  
            aTimer.Elapsed += new System.Timers.ElapsedEventHandler(check);
            aTimer.Interval = 60000;
            aTimer.AutoReset = true;//执行一次 false，一直执行true  
            //是否执行System.Timers.Timer.Elapsed事件  
            aTimer.Enabled = true;
            
          //  
        }

        private void check(object source, System.Timers.ElapsedEventArgs e)
        {
           bool a =  CheckServerState("DeviceNodeService");
           bool b = CheckServerState("ResourceService");
           if(!b) //只有Res服务停止的情况
           {
               Process proc = null;
               try
               {
                   string targetDir = AppDomain.CurrentDomain.BaseDirectory;
                   // string targetDir = string.Format(@"C:\Users\patrick\Desktop\");//this is where testChange.bat lies
                   proc = new Process();
                   proc.StartInfo.WorkingDirectory = targetDir;
                   proc.StartInfo.FileName = "ReStartAll.bat";
                   proc.StartInfo.Arguments = string.Format("10");//this is argument
                   //proc.StartInfo.CreateNoWindow = true;
                   proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;//这里设置DOS窗口不显示，经实践可行
                   proc.Start();
                   proc.WaitForExit();
                   Console.WriteLine(System.DateTime.Now + "  全部服务已重新启动...");
               }
               catch (Exception ex)
               {
                   Console.WriteLine("Exception Occurred :{0},{1}", ex.Message, ex.StackTrace.ToString());
               }
           } 
           else if (!a) //只有Node服务停止的情况
           {
               Process proc = null;
               try
               {
                   string targetDir = AppDomain.CurrentDomain.BaseDirectory;
                   // string targetDir = string.Format(@"C:\Users\patrick\Desktop\");//this is where testChange.bat lies
                   proc = new Process();
                   proc.StartInfo.WorkingDirectory = targetDir;
                   proc.StartInfo.FileName = "ReStartNodeOnly.bat";
                   proc.StartInfo.Arguments = string.Format("10");//this is argument
                   //proc.StartInfo.CreateNoWindow = true;
                   proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;//这里设置DOS窗口不显示，经实践可行
                   proc.Start();
                   proc.WaitForExit();
                   Console.WriteLine( System.DateTime.Now+ "  Node服务已重新启动..." );
               }
               catch (Exception ex)
               {
                   Console.WriteLine("Exception Occurred :{0},{1}", ex.Message, ex.StackTrace.ToString());
               }
           }
           
        }
        //实例化Timer类  
        System.Timers.Timer aTimer;   

        public const int WM_DEVICE_CHANGE = 0x219;
        public const int DBT_DEVICEARRIVAL = 0x8000;
        public const int DBT_DEVICE_REMOVE_COMPLETE = 0x8004;
        public const UInt32 DBT_DEVTYP_PORT = 0x00000003;
        [StructLayout(LayoutKind.Sequential)]
        struct DEV_BROADCAST_HDR
        {
            public UInt32 dbch_size;
            public UInt32 dbch_devicetype;
            public UInt32 dbch_reserved;
        }


        [StructLayout(LayoutKind.Sequential)]
        protected struct DEV_BROADCAST_PORT_Fixed
        {
            public uint dbcp_size;
            public uint dbcp_devicetype;
            public uint dbcp_reserved;
            // Variable?length field dbcp_name is declared here in the C header file.
        }


        /// <summary>
        /// 检测USB串口的拔插
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_DEVICE_CHANGE)        // 捕获USB设备的拔出消息WM_DEVICECHANGE
            {
                switch (m.WParam.ToInt32())
                {
                    case DBT_DEVICE_REMOVE_COMPLETE:    // USB拔出  
                        Console.WriteLine("USB 被拔出  "+ System.DateTime.Now.ToString());
                        break;
                    case DBT_DEVICEARRIVAL:             // USB插入获取对应串口名称
                        DEV_BROADCAST_HDR dbhdr = (DEV_BROADCAST_HDR)Marshal.PtrToStructure(m.LParam, typeof(DEV_BROADCAST_HDR));
                        if (dbhdr.dbch_devicetype == DBT_DEVTYP_PORT)
                        {
                            string portName = Marshal.PtrToStringUni((IntPtr)(m.LParam.ToInt32() + Marshal.SizeOf(typeof(DEV_BROADCAST_PORT_Fixed))));
                            Console.WriteLine("Port '" + portName + "' arrived." + System.DateTime.Now.ToString());
                            
                        }
                        break;
                }
            }
            base.WndProc(ref m);
        }

        public bool CheckServerState(string ServiceName)
        {
            ServiceController[] service = ServiceController.GetServices();
            bool isStart = false;
            bool isExite = false;
            for (int i = 0; i < service.Length; i++)
            {
                if (service[i].DisplayName.ToUpper().Equals(ServiceName.ToUpper()))
                {
                    isExite = true;
                    ServiceController server = service[i];
                    if (service[i].Status == ServiceControllerStatus.Running)
                    {
                        isStart = true;
                        break;
                    }
                }
            }

            if (!isExite)
            {
                Console.WriteLine(ServiceName+"不存在此服务");
            }
            else
            {


                if (isStart)
                {
                  // Console.WriteLine(ServiceName+"服务已启动..."+System.DateTime.Now);
                }
                else
                {
                    Console.WriteLine(System.DateTime.Now +"  "+ ServiceName + "服务没启动...");
                    return false;
                }
            }
            return true;
        }

        //窗体不显示直接运行
        private void Form1_Load(object sender, EventArgs e)
        {
            Form theFrm = (Form)sender;
            theFrm.ShowInTaskbar = false;
            theFrm.Opacity = 0D;

        }

    }
}
