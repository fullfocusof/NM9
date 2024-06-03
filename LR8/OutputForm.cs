using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LR8
{
    public partial class OutputForm : Form
    {
        List<(double, double)> dataToOut;
        double epsToOut;

        public OutputForm(List<(double, double)> data, double eps)
        {
            InitializeComponent();
            dataToOut = data;
            epsToOut = eps;
        }

        private void OutputForm_Load(object sender, EventArgs e)
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Номер узла", typeof(int));
            dataTable.Columns.Add("Значение x", typeof(double));
            dataTable.Columns.Add("Значение y", typeof(double));

            dataTable.Rows.Add(1, 1, Math.Sqrt(2));
            for (int i = 0; i < dataToOut.Count; i++)
            {
                dataTable.Rows.Add(i + 2, dataToOut[i].Item1, dataToOut[i].Item2);
            }

            labelInfo.Text = "\n\nКоличество узлов - " + (dataToOut.Count + 1).ToString() + "\nШаг - " + (dataToOut[1].Item1 - dataToOut[0].Item1).ToString() + "\nТочность - " + epsToOut.ToString();

            DGV.DataSource = dataTable;
        }

        private void buttonDrawGraph_Click(object sender, EventArgs e)
        {
            GraphForm graph = new GraphForm(dataToOut);
            graph.ShowDialog();
        }       
    }
}
