using Gma.System.MouseKeyHook;
using System.Data;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Text;

namespace Avain
{
    static class KeyListenercs
    {
        public static DataTable DT { set; get; }
        public static bool Copy = false;
        public static bool Write = false;
        public static NotifyIcon NI { set; get; }
        private static IKeyboardMouseEvents m_events;
        private static bool altGear = false;
        private static string sana = "";
        private static KeysConverter kc;

        private const int bScanLeftControl = 0x9d;
        private const int bScanV = 0x2f;
        private const int ExtendedKey = 0x0001;
        private const int keyUp = 0x0002;
        private const int LeftControl = 0xA2;
        private const int V_ = 0x56;
        private const int Alt_ = 0xa4;
        private const int bScanAlt = 0xb8;

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        public static void Start()
        {
            kc = new KeysConverter();
            m_events = Hook.GlobalEvents();
            SubscribeGlobal();
        }

        private static void SubscribeGlobal()
        {
            Unsubscribe();
            SubscribeGlobal(Hook.GlobalEvents());
        }

        public static void Unsubscribe()
        {
            m_events.KeyDown -= onKeyDown;
            m_events.KeyUp -= onKeyUp;
        }

        public static void OnOff(bool p)
        {
            if (p)
                SubscribeGlobal();
            else
                Unsubscribe();
        }

        private static void SubscribeGlobal(IKeyboardMouseEvents events)
        {
            m_events = events;
            m_events.KeyDown += onKeyDown;
            m_events.KeyUp += onKeyUp;
        }

        private static void onKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.RMenu && !altGear)
                altGear = true;
        }

        private static void onKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.RMenu)
            {
                altGear = false;
                sana = sana.ToLower();
                AccessSQL.AccessHandler.Viesti("Kirjoitettu sana oli "+sana);
                TarkistaJaAvaa();
                sana = "";
            }
            if (altGear && !kc.ConvertToString(e.KeyCode).Equals("LControlKey"))
            {
                sana = sana + kc.ConvertToString(e.KeyCode);
            }
        }

        private static void TarkistaJaAvaa()
        {
            foreach(DataRow DR in DT.Rows)
            {
                string viesti = "Hotkey " + DR[0].ToString() + " Komento " + DR[1].ToString();
                AccessSQL.AccessHandler.Viesti(viesti);
                if (DR[0].ToString().ToLower().Equals(sana.ToLower()))
                {
                    if (Copy)
                    {
                        NI.BalloonTipText = "Kopioidaan kohteeseen " + DR[1].ToString();
                        NI.Visible = true;
                        NI.ShowBalloonTip(1);
                        var t = new Thread(() => Move(DR[1].ToString()));
                        t.Start();
                    }
                    else if (Write)
                    {
                        if (Path.GetExtension(DR[1].ToString()).Equals(".txt"))
                        {
                            NI.BalloonTipText = "Kirjoitetaan kohteesta " + DR[1].ToString();
                            NI.Visible = true;
                            NI.ShowBalloonTip(1);
                            string teksti = File.ReadAllText(DR[1].ToString(),encoding:Encoding.UTF8);
                            Clipboard.SetText(teksti);
                            //ControlC();
                        }
                    }
                    else
                    {
                        NI.BalloonTipText = "Avataan tiedosto " + DR[1].ToString();
                        NI.Visible = true;
                        NI.ShowBalloonTip(1);
                        Process.Start(DR[1].ToString());
                    }
                }
            }
        }

        private static void ControlC()
        {
            Unsubscribe();

            keybd_event(0xa5, 0, keyUp|ExtendedKey, 0);                 //AltGr ylös 0xA5 0 0x0002|0x0001 0

            Thread.Sleep(30);

            keybd_event(LeftControl, bScanLeftControl, ExtendedKey, 0); //Shift alas 0xA2 0x9D 0x0001 0
            keybd_event(V_,bScanV , 0, 0);                              //V alas 0x56 0x2f 0 0
            keybd_event(V_, 0, keyUp, 0);                               //V Ylös 0x56 0 0x0002 0
            keybd_event(LeftControl, 0, keyUp, 0);                      //Shift ylös 0xA2 0 0x0002 0

            Thread.Sleep(30);

            SubscribeGlobal();
        }

        private static void Move(string dest)
        {
            IntPtr handle = GetForegroundWindow();

            List<string> selected = new List<string>();
            var shell = new Shell32.Shell();
            foreach (SHDocVw.InternetExplorer window in shell.Windows())
            {
                if (window.HWND == (int)handle)
                {
                    Shell32.FolderItems items = ((Shell32.IShellFolderViewDual2)window.Document).SelectedItems();
                    foreach (Shell32.FolderItem item in items)
                    {
                        selected.Add(item.Path);
                        AccessSQL.AccessHandler.Viesti(item.Path);
                    }
                }
            }
            foreach(string path in selected)
            {
                File.Move(path, dest+"\\"+Path.GetFileName(path));
            }
        }
    }
}