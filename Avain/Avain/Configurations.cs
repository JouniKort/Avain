using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace Avain
{
    public partial class Configurations : Form
    {

        public DataTable DT { set; get; }

        public Configurations()
        {
            InitializeComponent();
        }

        private void Configurations_Load(object sender, EventArgs e)
        {
            dataGridView1.DataSource = DataBase.Komennot;
            dataGridView1.Columns[0].Width = (dataGridView1.Width-dataGridView1.RowHeadersWidth) / 4;
            dataGridView1.Columns[1].Width = (dataGridView1.Width-dataGridView1.RowHeadersWidth) / 4 * 3;
        }

        public DataTable GetContentAsDataTable(bool IgnoreHideColumns = false)
        {
            DataGridView dgv = dataGridView1;
            try
            {
                if (dgv.ColumnCount == 0) return null;
                DataTable dtSource = new DataTable();
                foreach (DataGridViewColumn col in dgv.Columns)
                {
                    if (IgnoreHideColumns & !col.Visible) continue;
                    if (col.Name == string.Empty) continue;
                    dtSource.Columns.Add(col.Name, col.ValueType);
                    dtSource.Columns[col.Name].Caption = col.HeaderText;
                }
                if (dtSource.Columns.Count == 0) return null;
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    DataRow drNewRow = dtSource.NewRow();
                    foreach (DataColumn col in dtSource.Columns)
                    {
                        drNewRow[col.ColumnName] = row.Cells[col.ColumnName].Value;
                    }
                    dtSource.Rows.Add(drNewRow);
                }
                return dtSource;
            }
            catch { return null; }
        }

        private void Configurations_FormClosed(object sender, FormClosedEventArgs e)
        {
            DataTable uusi = new DataTable();
            uusi.Columns.Add("Hotkey");
            uusi.Columns.Add("Komento");
            foreach(DataGridViewRow rivi in dataGridView1.Rows)
            {
                if (rivi.Cells[0].Value == null)
                    continue;
                DataRow DR = uusi.NewRow();
                DR[0] = rivi.Cells[0].Value.ToString();
                DR[1] = rivi.Cells[1].Value.ToString();
                uusi.Rows.Add(DR);
            }
            DataBase.DT = uusi;
            DataBase.Update();
        }
    }
}
