using LitJson;
using QLTK.Functions;
using QLTK.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace QLTK.UserControls
{
    public partial class Dashboard : UserControl
    {
        public Dashboard()
        {
            InitializeComponent();
            list_account = new BindingSource();
            list_account.DataSource = new List<Account>();
            dataAccount.DataSource = list_account;
            ServerView.Items.AddRange(Server.servers);
            ServerView.SelectedIndex = 0;
            this.dataAccount.MouseWheel += new MouseEventHandler(mousewheel);
        }
        private void Dashboard_Load(object sender, EventArgs e)
        {
            try
            {
                var dataText = File.ReadAllText("Data//dataAccount.json");
                var dataJson = JsonMapper.ToObject<Account[]>(dataText);
                foreach (var account in dataJson)
                    list_account.Add(account);
                new Thread(DragonServer.StartListen) { IsBackground = true }.Start();

                //list_account.ListChanged += List_account_ListChanged;
            }
            catch
            {
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            dataAccount.Update();
            dataAccount.Refresh();
            if (auto)
            {
                Auto();
                checkTimeUpPean();
            }
        }

        BindingSource list_account;
        void mousewheel(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Delta > 0 && dataAccount.FirstDisplayedScrollingRowIndex > 0)
                {
                    dataAccount.FirstDisplayedScrollingRowIndex--;
                }
                else if (e.Delta < 0)
                {
                    dataAccount.FirstDisplayedScrollingRowIndex++;
                }
            }
            catch
            {
            }
        }

        #region helper

        bool is_number(string str)
            => int.TryParse(str, out var a);

        

        Account get_account_intput_batch(int num) => new Account()
        {
            id = dataAccount.Rows.Count,
            username = AccView.Text.Replace("\'X\'", num.ToString()),
            password = PassView.Text,
            index_server = ServerView.SelectedIndex,
        };

        Account get_account_intput => new Account()
        {
            id = dataAccount.Rows.Count,
            username = AccView.Text,
            password = PassView.Text,
            index_server = ServerView.SelectedIndex,
        };
        public List<Account> get_list_account_connected
           => ((List<Account>)list_account.List).FindAll(acc => acc.socket != null && acc.socket.Connected);

        public List<Account> get_list_account_opened
           => ((List<Account>)list_account.List).FindAll(acc => acc.process != null && !acc.process.HasExited);

        List<Account> get_list_account_selected_and_opened
           => ((List<Account>)list_account.List).FindAll(acc => dataAccount.Rows[acc.id].Selected && acc.process != null && !acc.process.HasExited);

        List<Account> get_list_account_selected
            => ((List<Account>)list_account.List).FindAll(acc => dataAccount.Rows[acc.id].Selected);

        public List<Account> get_list_account
            => (List<Account>)list_account.List;

        bool exist_account(Account acc)
            => get_list_account.Any(a => (a.username == acc.username && a.password == acc.password && a.server == acc.server));

        Account get_account(string username) 
            => get_list_account.Find(a => a.username == username);

        public Account get_account_next
            => get_list_account.Find(a => (a.process == null || a.process.HasExited) && a.status != "Đã làm xong nv!" && !a.status.Contains("Login lại vào"));
        public List<Account> get_account_wait_pean
            => get_list_account.FindAll(a => a.status.Contains("Login lại vào"));
        #endregion
        #region method
        void checkTimeUpPean()
        {
            var a = get_account_wait_pean;
            foreach(var acc in a)
            {
                var time = TimeHelper.gI().DateTimeFromString(acc.status.Replace("Login lại vào: ", string.Empty)) - DateTime.Now;
                if (time.Seconds > 0)
                    acc.status = "Đã nâng đậu xong!";
            }
        }
        bool auto = false;
        async void Auto()
        {
            if(get_list_account_opened.Count < maxThread.Value)
            {
                await Login(get_account_next);
            }
        }
        void SortTab(List<Account> accounts)
        {
            var maxWidth = SystemParameters.PrimaryScreenWidth;
            var maxHeight = SystemParameters.PrimaryScreenHeight;


            int cx = 0, cy = 0;

            for (int i = 0; i < accounts.Count; i++)
            {
                if (!Utils.ExistedWindow(accounts[i], out IntPtr hWnd))
                    continue;

                if (!Utils.GetWindowRect(hWnd, out var rect))
                    continue;

                int width = rect.right - rect.left;
                int height = rect.bottom - rect.top;

                Utils.MoveWindow(hWnd, cx, cy, width, height, true);

                cx += width;
                if (cx + width > maxWidth)
                {
                    cx = 0;
                    cy += height - 5;
                }
                if (cy + height > maxHeight)
                {
                    cy = 0;
                }
            }
        }

        async Task doLogin(List<Account> accounts)
        {
            foreach (var account in accounts)
            {
                await Login(account);
            }
        }

        async Task Login(Account account)
        {
            if (account == null)
                return;
            if (account.process == null || account.process.HasExited)
            {
                account.status =  "Đang mở game!";
                //var ss = account.size.Split('x');
                account.process = Process.Start("123Tool.exe", $"-silent-crashes -disable-gpu-skinning -releaseCodeOptimization -disableManagedDebugger -accept-apiupdate -no-stereo-rendering -batchmode -nographics");// -batchmode -nographics   $"-screen-width {ss[0]} -screen-height {ss[1]}");

                //while (account.process.MainWindowHandle == IntPtr.Zero)
                //{
                    await Task.Delay(100);
                //}
                //Utils.SetWindowText(account.process.MainWindowHandle, $"[{account.id}]");
            }
        }

        #endregion

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(AccView.Text))
            {
                Utils.notification("Tài khoản không thể để trống!", MessageBoxIcon.Error);
                AccView.Focus();
                return;
            }
            if (string.IsNullOrEmpty(PassView.Text))
            {
                Utils.notification("Mật không thể để trống!", MessageBoxIcon.Error);
                PassView.Focus();
                return;
            }
            
            if (!cbAddBatch.Checked)
            {
                var a = get_account_intput;
                if (exist_account(a))
                {
                    Utils.notification("Tài khoản đã tồn tại!", MessageBoxIcon.Error);
                    return;
                }
                list_account.Add(a);
            }
            else
            {
                if(int.TryParse(addFrom.Text, out int f) && int.TryParse(addTo.Text, out int t))
                {
                    for(int i = f; i <= t; i++)
                    {
                        list_account.Add(get_account_intput_batch(i));
                    }
                }
                else
                {
                    Utils.notification("Vui lòng nhập đúng dữ liệu!", MessageBoxIcon.Error);
                }

            }
            File.WriteAllText("Data//dataAccount.json", JsonMapper.ToJson(get_list_account));
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            var list_edit = get_list_account_selected;
            if (list_edit.Count < 1)
            {
                Utils.notification("Vui lòng chọn tài khoản cần sửa!", MessageBoxIcon.Error);
                return;
            }
            if (list_edit.Count == 1)
            {
                if (Utils.Question($"Bạn có chắc chắn muốn sửa tài khoản {list_edit[0].username}!") == DialogResult.Yes)
                {
                    list_edit[0].username = AccView.Text;
                    list_edit[0].password = PassView.Text;
                    list_edit[0].index_server = ServerView.SelectedIndex;

                    File.WriteAllText("Data//dataAccount.json", JsonMapper.ToJson(get_list_account));
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var list_delete = get_list_account_selected;
            if (list_delete.Count < 1)
            {
                Utils.notification("Vui lòng chọn tài khoản cần xóa!", MessageBoxIcon.Error);
                return;
            }
            if(list_delete.Count == 1)
            {
                if (Utils.Question($"Bạn có chắc chắn muốn xóa tài khoản {list_delete[0].username}!") == DialogResult.Yes)
                {
                    list_account.Remove(list_delete[0]);
                    goto save;
                }
                return;
            }
            if (Utils.Question($"Bạn có chắc chắn muốn xóa {list_delete.Count} đang select!") == DialogResult.Yes)
            {
                foreach (var account in list_delete)
                {
                    list_account.Remove(account);
                }
                goto save;
            }
            return;
        save:
            int i = 0;
            foreach (var account in get_list_account)
            {
                account.id = i;
                i++;
            }
            File.WriteAllText("Data//dataAccount.json", JsonMapper.ToJson(get_list_account));
        }
        private void btnLogin_Click(object sender, EventArgs e)
        {
            //await doLogin(get_list_account_selected);
            //var a = StringCipher.Encrypt("qwertyuiopasdfghjklzxcvbnm1234567890");
            //var b = StringCipher.Decrypt(a);
            auto = !auto;
            if (auto)
                btnLogin.Text = "STOP";
            else
            {
                btnLogin.Text = "LOGIN";
                close();
            }
        }

      

        private void btnClose_Click(object sender, EventArgs e)
        {
            var l = get_list_account;
            l.ForEach(a => a.status = "-");
        }

        private void dataAccount_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            var acc = get_list_account[e.RowIndex];
            AccView.Text = acc.username;
            PassView.Text = acc.password;
            ServerView.SelectedIndex = acc.index_server;
            ServerView.Refresh();
        }
        public void close()
        {
            List<Account> accs = this.get_list_account_opened;
            accs.ForEach(delegate (Account a)
            {
                a.process.Kill();
            });
        }
    }
}
