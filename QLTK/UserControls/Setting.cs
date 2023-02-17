using LitJson;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLTK.UserControls
{
    public partial class Setting : UserControl
    {
        public Setting()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var nv = new bool[7];
            for (int i = 0; i < 7; i++)
            {
                nv[i] = checkedListBox1.GetItemChecked(i);
            }
            File.WriteAllText("Data//task.json", JsonMapper.ToJson(nv));
        }

        public void Setting_Load()
        {
            try
            {
                var listbool = JsonMapper.ToObject<bool[]>(File.ReadAllText("Data//task.json"));
                for (int i = 0; i < 7; i++)
                {
                    checkedListBox1.SetItemChecked(i, listbool[i]);
                }
            }
            catch { }
        }
    }
}
