using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Avain
{
    public class Tray : Form
    {

        public NotifyIcon trayIcon;
        private ContextMenu trayMenu;

        public Tray()
        {
            trayMenu = new ContextMenu();
            TeeMenu();

            KeyListenercs.Start();

            DataBase.Connect();

            trayIcon = new NotifyIcon();
            trayIcon.Text = "Avain";
            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Avain\\Key.ico"))
                trayIcon.Icon = new Icon(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Avain\\Key.ico");
            else
                trayIcon.Icon = new Icon("Resources\\Key.ico");
            KeyListenercs.NI = trayIcon;

            trayIcon.ContextMenu = trayMenu;
            trayIcon.Visible = true;
        }
        
        private void TeeMenu()
        {
            trayMenu.MenuItems.Add("Commands", Commands);
            trayMenu.MenuItems.Add("On", On);
            trayMenu.MenuItems[1].Checked = true;
            trayMenu.MenuItems.Add("Write", Write);
            trayMenu.MenuItems.Add("Copy", Copy);
            trayMenu.MenuItems.Add("Exit", OnExit);
        }

        private void Write(object sender, EventArgs e)
        {
            MenuItem it = sender as MenuItem;
            it.Checked = !it.Checked;
            KeyListenercs.Write = it.Checked;
            KeyListenercs.Copy = false;
            foreach (MenuItem Mi in trayMenu.MenuItems)
            {
                if (Mi.Text.Equals("Copy"))
                {
                    Mi.Checked = false;
                }
            }
        }

        private void On(object sender, EventArgs e)
        {
            MenuItem it = sender as MenuItem;
            it.Checked = !it.Checked;
            KeyListenercs.OnOff(it.Checked);
        }

        private void Commands(object sender, EventArgs e)
        {
            Configurations config = new Configurations();
            config.Show();
        }

        private void Copy(object sender, EventArgs e)
        {
            MenuItem it = sender as MenuItem;
            it.Checked = !it.Checked;
            KeyListenercs.Copy = it.Checked;
            KeyListenercs.Write = false;
            foreach(MenuItem Mi in trayMenu.MenuItems)
            {
                if (Mi.Text.Equals("Write"))
                {
                    Mi.Checked = false;
                }
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            Visible = false; // Hide form window.
            ShowInTaskbar = false; // Remove from taskbar.

            base.OnLoad(e);
        }

        private void OnExit(object sender, EventArgs e)
        {
            AccessSQL.AccessHandler.SuljeYhteys();
            KeyListenercs.Unsubscribe();
            Application.Exit();
        }

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                // Release the icon resource.
                trayIcon.Dispose();
            }

            base.Dispose(isDisposing);
        }
    }
}