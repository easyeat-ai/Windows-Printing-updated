using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using WebSocketSharp;
using WebSocketSharp.Server;
using Microsoft.Win32;
namespace ReceiptPrint
{
    static class Program
    { 
        [STAThread]
        static void Main(string[] args)
        {
            //WebSocketServer wssv = new WebSocketServer("ws://127.0.0.1:7980");
            //wssv.AddWebSocketService<Print>("/print");
            //wssv.Start();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            RegistryKey reg = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            reg.SetValue("Windows Offline", Application.ExecutablePath.ToString());
            Application.Run(new EasyEats(args));
        }

    }
}
