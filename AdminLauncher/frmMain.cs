using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AdminLauncher.Properties;

namespace AdminLauncher
{
    public partial class frmMain : Form
    {
        private NotifyIcon trayIcon;
        private ContextMenu trayMenu = new ContextMenu();
        private bool allowshowdisplay = false;

        //private StartProgram[] startPrograms =
        //{
        //    new StartProgram {Name="Visual Studio 2013", Program = "devenv.exe", WorkingDirectory = @"C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\"},
        //    new StartProgram {Name="Visual Studio 2015", Program = "devenv.exe", WorkingDirectory = @"C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\"},
        //    new StartProgram {Name="Powershell", Program = "powershell.exe", WorkingDirectory = @"%SystemRoot%\system32\WindowsPowerShell\v1.0\"},
        //    new StartProgram {Name="Command prompt", Program = "cmd.exe", WorkingDirectory = @"%HOMEDRIVE%%HOMEPATH%"},
        //    new StartProgram {Name="Windows Explorer", Program = "explorer.exe", WorkingDirectory = @"%HOMEDRIVE%%HOMEPATH%"},
        //};

        private IEnumerable<StartProgram> startPrograms;

        protected override void SetVisibleCore(bool value)
        {
            base.SetVisibleCore(allowshowdisplay ? value : allowshowdisplay);
        }

        public frmMain()
        {
            InitializeComponent();

            startPrograms = StartProgram.Read();

            UpdateMenuItems();

            trayIcon = new NotifyIcon();
            trayIcon.Text = "AdminLauncher";

            trayIcon.Icon = Properties.Resources.IconImage;
//            trayIcon.Icon = new Icon(SystemIcons.Application, 40, 40);

            trayIcon.MouseUp += trayIcon_MouseUp;
            // Add menu to tray icon and show it.
            trayIcon.ContextMenu = trayMenu;
            trayIcon.Visible = true;


        }

        private void trayIcon_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                MethodInfo mi = typeof (NotifyIcon).GetMethod("ShowContextMenu",
                    BindingFlags.Instance | BindingFlags.NonPublic);
                mi.Invoke(trayIcon, null);
            }
        }

        private void UpdateMenuItems()
        {
            trayMenu.MenuItems.Clear();
            foreach (StartProgram startProgram in startPrograms)
            {
                MenuItem menuItem = new MenuItem(startProgram.Name, MenuChoice);
                menuItem.Tag = startProgram;
                trayMenu.MenuItems.Add(menuItem);
            }
            trayMenu.MenuItems.Add("Admin", OnShowForm);
        }

        private void MenuChoice(object sender, EventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            if (menuItem == null)
            {
                log("Anrop saknar Menyval!");
                return;
            }

            StartProgram startProgram = menuItem.Tag as StartProgram;
            if (startProgram == null)
            {
                log("MenuItem saknar StartProgram!");
                return;
            }

            log(string.Format("Startar {0}", startProgram.Name));
            run(startProgram);
        }


        private void OnShowForm(object sender, EventArgs e)
        {
            allowshowdisplay = true;
            Visible = true;
            WindowState = FormWindowState.Normal;
            BringToFront();
        }


        private void log(string data)
        {
            string text = string.Format("{0}: {1}\r\n", DateTime.Now.ToShortTimeString(), data);
            tbLog.AppendText(text);
        }

        private void run(StartProgram startProgram)
        {
            Process process = new Process
            {
                StartInfo =
                {
                    FileName = startProgram.Program,
                    Arguments = startProgram.Arguments ?? "",
                    WorkingDirectory = startProgram.WorkingDirectory
                }
            };
            process.Start();
        }

        protected override void OnLoad(EventArgs e)
        {
            Visible = false; // Hide form window.
            ShowInTaskbar = false; // Remove from taskbar.

            base.OnLoad(e);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            startPrograms = StartProgram.Read();
            UpdateMenuItems();
            log("Meny är uppdaterad!");
        }

        private void btnHide_Click(object sender, EventArgs e)
        {
            Hide();
        }
    }


}
