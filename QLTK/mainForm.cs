using Microsoft.Win32;
using QLTK.Functions;
using QLTK.UserControls;
using Siticone.Desktop.UI.WinForms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLTK
{
    public partial class mainForm : Form
    {
        public static mainForm gI;
        public mainForm()
        {
            InitializeComponent();
            new SiticoneDragControl(this);
            new SiticoneShadowForm(this);

            infomation = new Infomation();
            infomation.Dock = DockStyle.Fill;
            PanelSlider.Controls.Add(infomation);

            dashboard = new Dashboard();
            dashboard.Dock = DockStyle.Fill;
            PanelSlider.Controls.Add(dashboard);

            setting = new Setting();
            setting.Dock = DockStyle.Fill;
            PanelSlider.Controls.Add(setting);
            gI = this;
        }
        public Dashboard dashboard;
        public Setting setting;
        public Infomation infomation;

        public DateTime time_expired;

        

        #region font
        

        /// <summary>
        /// Installs font on the user's system and adds it to the registry so it's available on the next session
        /// Your font must be included in your project with its build path set to 'Content' and its Copy property
        /// set to 'Copy Always'
        /// </summary>
        /// <param name="contentFontName">Your font to be passed as a resource (i.e. "myfont.tff")</param>
        static void RegisterFont(string contentFontName)
        {
            // Creates the full path where your font will be installed
            var fontDestination = Path.Combine(System.Environment.GetFolderPath
                                          (System.Environment.SpecialFolder.Fonts), contentFontName);

            if (!File.Exists(fontDestination))
            {
                // Copies font to destination
                System.IO.File.Copy(Path.Combine(System.IO.Directory.GetCurrentDirectory(), contentFontName), fontDestination);

                // Retrieves font name
                // Makes sure you reference System.Drawing
                PrivateFontCollection fontCol = new PrivateFontCollection();
                fontCol.AddFontFile(fontDestination);
                var actualFontName = fontCol.Families[0].Name;

                //Add font
                Utils.AddFontResource(fontDestination);
                //Add registry entry   
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts",
        actualFontName, contentFontName, RegistryValueKind.String);
            }
        }
        #endregion
        void btnDashboard_Click(object sender, EventArgs e)
        {
            dashboard.BringToFront();
        }
        private void HistoryBtn_Click(object sender, EventArgs e)
        {
            infomation.BringToFront();
        }

        private void btnSetting_Click(object sender, EventArgs e)
        {
            setting.BringToFront();
        }
        void update_date_Tick(object sender, EventArgs e)
        {
            if (time_expired != null)
            {
                var timeSpan = TimeHelper.gI().calculator(time_expired);
                if (timeSpan.Days > 9999)
                {
                    lbDate.Text = "forever";
                    update_date.Enabled = false;
                    update_date.Stop();
                    return;
                }
                if (timeSpan.Days > 0)
                    lbDate.Text = $"{timeSpan.Days}d : {timeSpan.Hours}h : {timeSpan.Minutes}m : {timeSpan.Seconds}s";
                else if (timeSpan.Hours > 0)
                    lbDate.Text = $"{timeSpan.Hours}h : {timeSpan.Minutes}m : {timeSpan.Seconds}s";
                else if (timeSpan.Minutes > 0)
                    lbDate.Text = $"{timeSpan.Minutes}m : {timeSpan.Seconds}s";
                else if (timeSpan.Seconds > 0)
                    lbDate.Text = $"{timeSpan.Seconds}s";
                else
                {
                    dashboard.close();
                    lbDate.Text = "out of date";
                    update_date.Enabled = false;
                    update_date.Stop();
                    btnDashboard.Enabled = false;
                }
            }
            //TimeHelper.gI().DateTimeNow().ToString("dd/MM/yyyy HH:mm:ss");
        }

        
        void mainFrom_Load(object sender, EventArgs e)
        {
            this.btnDashboard.Enabled = AntiCracker.gI().check_key_license();
            setting.Setting_Load();
            if (btnDashboard.Enabled)
            {
                update_date.Enabled = true;
                btnDashboard.BringToFront();
            }
        }

        private void mainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            dashboard.close();
        }
    }
}
