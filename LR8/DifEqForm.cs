using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.InteropServices;

namespace LR8
{
    public partial class DifEqForm : Form  // y'(x) = (-(5x^2 + 3)y^3 + 12y) / 8x; y(1) = sqrt(2); h = 0.1
    {
        int method, targetParts = 10;
        double left, right, eps, yInit = Math.Sqrt(2);

        List<(double, double)> tempData, RungeTempData;

        public DifEqForm()
        {
            InitializeComponent();

            tempData = new List<(double, double)>();
            RungeTempData = new List<(double, double)>();
        }

        private void buttonRectangleMethod_Click(object sender, EventArgs e)
        {
            inputSection(1);
        }

        private void buttonTrapezMethod_Click(object sender, EventArgs e)
        {
            inputSection(2);
        }

        private void buttonParabolaMethod_Click(object sender, EventArgs e)
        {
            inputSection(3);
        }

        private void buttonExecInput_Click(object sender, EventArgs e)
        {
            if (textBoxLeftData.Text == string.Empty || textBoxRightData.Text == string.Empty || textBoxEps.Text == string.Empty)
            {
                labelErrorInput.Visible = true;
                return;
            }

            left = double.Parse(textBoxLeftData.Text);
            right = double.Parse(textBoxRightData.Text);
            eps = double.Parse(textBoxEps.Text);

            if (left > right)
            {
                labelLeftData.ForeColor = Color.Red;
                labelRightData.ForeColor = Color.Red;
                labelErrorInput.Visible = true;
                return;
            }

            //setResult();
            //targetParts = 1;

            if (method == 1) EulerMethod();
            else if (method == 2) Cauchy_EulerMethod();
            else if (method == 3) Runge_KuttaMethod();
        }

        private void methodSection()
        {
            buttonParabolaMethod.Visible = true;
            buttonRectangleMethod.Visible = true;
            buttonTrapezMethod.Visible = true;

            labelMethod.Text = "Выберите метод решения:";
            labelLeftData.Visible = false;
            labelRightData.Visible = false;
            labelEps.Visible = false;
            labelErrorInput.Visible = false;

            textBoxLeftData.Visible = false;
            textBoxRightData.Visible = false;
            textBoxEps.Visible = false;

            buttonExecInput.Visible = false;
            buttonReturn.Visible = false;
        }

        private void inputSection(int choice)
        {
            method = choice;

            buttonParabolaMethod.Visible = false;
            buttonRectangleMethod.Visible = false;
            buttonTrapezMethod.Visible = false;

            labelLeftData.ForeColor = Color.Black;
            labelRightData.ForeColor = Color.Black;
            labelEps.ForeColor = Color.Black;

            labelMethod.Text = "Выберите отрезок функции:";

            labelLeftData.Visible = true;
            labelRightData.Visible = true;
            labelEps.Visible = true;
            labelEps.Text = "- точность";
            textBoxLeftData.Visible = true;
            textBoxRightData.Visible = true;
            textBoxEps.Visible = true;

            buttonExecInput.Visible = true;
            buttonReturn.Visible = true;
        }

        private void textBoxLeftData_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
                (e.KeyChar != ',' || textBox.Text.Contains(",") || textBox.SelectionStart == 0) &&
                (e.KeyChar != '-' || textBox.Text.Contains("-") || textBox.SelectionStart != 0))
            {
                e.Handled = true;
            }
        }

        static private double getValueFunc(double x, double y)
        {
            return (-(5*Math.Pow(x, 2) + 3) * Math.Pow(y, 3) + 12*y) / 8*x;
        }

        private void buttonReturn_Click(object sender, EventArgs e)
        {
            methodSection();
        }

        private void formTable()
        {
            string table = "Номер узла".PadRight(5) + "\t" + "Значение x".PadRight(10) + "\t\t\t" + "Значение y".PadRight(10) + "\n";
            table += "1".PadRight(5) + "\t\t" + "1".PadRight(10) + "\t\t\t" + yInit.ToString().PadRight(10) + "\n";

            for (int i = 0; i < RungeTempData.Count; i++)
            {
                table += (i + 2).ToString() + "\t\t\t" + RungeTempData[i].Item1.ToString().PadRight(10) + "\t\t\t" + RungeTempData[i].Item2.ToString().PadRight(10) + "\n";
            }

            table += "\n\nКоличество узлов - " + RungeTempData.Count.ToString() + "\nШаг - " + (RungeTempData[1].Item1 - RungeTempData[0].Item1).ToString() + "\nТочность - " + eps.ToString();

            File.WriteAllText("E:\\Projects\\NumMethods\\LR8-9\\LR8\\dataOutput.txt", table); // поменять путь
            outputTable();
        }

        private void outputTable()
        {
            OutputForm tempForm = new OutputForm(RungeTempData, eps);
            tempForm.ShowDialog();
        }

        private void EulerMethod()
        {
            double tempEps;
            
            do
            {
                double step = (right - left) / targetParts, stepRunge = step / 2;
                double x0 = 1.0f, y0 = yInit;
                double x0Runge = 1.0f, y0Runge = yInit;

                for (int i = 0; i < targetParts; i++)
                {
                    double xTemp = x0 + step;
                    double yTemp = getValueFunc(x0, y0) * step + y0;
                    tempData.Add((xTemp, yTemp));
                    x0 = xTemp;
                    y0 = yTemp;
                }

                for (int i = 0; i < targetParts * 2; i++)
                {
                    double xTempRunge = x0Runge + stepRunge;
                    double yTempRunge = getValueFunc(x0Runge, y0Runge) * stepRunge + y0Runge;
                    RungeTempData.Add((xTempRunge, yTempRunge));
                    x0Runge = xTempRunge;
                    y0Runge = yTempRunge;
                }

                tempEps = Math.Abs(RungeTempData[targetParts * 2 - 2].Item2 - tempData[targetParts - 1].Item2);

                if (tempEps > eps)
                {
                    targetParts *= 2;
                    tempData.Clear();
                    RungeTempData.Clear();
                }

            } while (tempEps > eps);

            formTable();

            targetParts = 10;
            tempData.Clear();
            RungeTempData.Clear();
        }

        private void Cauchy_EulerMethod()
        {
            double tempEps = 0.0f;

            do
            {
                double step = (right - left) / targetParts, stepRunge = step / 2;
                double x0 = 1.0f, y0 = yInit;
                double x0Runge = 1.0f, y0Runge = yInit;

                for (int i = 0; i < targetParts; i++)
                {
                    double xTemp = x0 + step;
                    double fTemp = getValueFunc(x0, y0);
                    double yApprox = step * fTemp + y0;
                    double fApprox = getValueFunc(xTemp, yApprox);
                    double yTemp = y0 + step / 2 * (fApprox + fTemp);
                    tempData.Add((xTemp, yTemp));
                    x0 = xTemp;
                    y0 = yTemp;
                }

                for (int i = 0; i < targetParts * 2; i++)
                {
                    double xTempRunge = x0Runge + stepRunge;
                    double fTempRunge = getValueFunc(x0Runge, y0Runge);
                    double yApproxRunge = stepRunge * fTempRunge + y0Runge;
                    double fApproxRunge = getValueFunc(xTempRunge, yApproxRunge);
                    double yTempRunge = y0Runge + stepRunge / 2 * (fApproxRunge + fTempRunge);
                    RungeTempData.Add((xTempRunge, yTempRunge));
                    x0Runge = xTempRunge;
                    y0Runge = yTempRunge;
                }

                tempEps = Math.Abs(RungeTempData[targetParts * 2 - 1].Item2 - tempData[targetParts - 1].Item2);

                if (tempEps > eps)
                {
                    targetParts *= 2;
                    tempData.Clear();
                    RungeTempData.Clear();
                }

            } while (tempEps > eps);

            formTable();

            targetParts = 10;
            tempData.Clear();
            RungeTempData.Clear();
        }

        private void Runge_KuttaMethod()
        {
            double tempEps = 0.0f;

            do
            {
                double step = (right - left) / targetParts, stepRunge = step / 2;
                double x0 = 1.0f, y0 = yInit;
                double x0Runge = 1.0f, y0Runge = yInit;

                for (int i = 0; i < targetParts; i++)
                {
                    double xTemp = x0 + step;
                    double xTempHalf = x0 + step / 2;

                    double K1 = getValueFunc(x0, y0) * step;
                    double y1 = y0 + K1 / 2;
                    double K2 = getValueFunc(xTempHalf, y1) * step;
                    double y2 = y0 + K2 / 2;
                    double K3 = getValueFunc(xTempHalf, y2) * step;
                    double y3 = y0 + K3;
                    double K4 = getValueFunc(xTemp, y3) * step;
                    double yTemp = y0 + (K1 + 2 * K2 + 2 * K3 + K4) / 6;

                    tempData.Add((xTemp, yTemp));
                    x0 = xTemp;
                    y0 = yTemp;
                }

                for (int i = 0; i < targetParts * 2; i++)
                {
                    double xTempRunge = x0Runge + stepRunge;
                    double xTempHalfRunge = x0Runge + stepRunge / 2;

                    double K1 = getValueFunc(x0Runge, y0Runge) * stepRunge;
                    double y1 = y0Runge + K1 / 2;
                    double K2 = getValueFunc(xTempHalfRunge, y1) * stepRunge;
                    double y2 = y0Runge + K2 / 2;
                    double K3 = getValueFunc(xTempHalfRunge, y2) * stepRunge;
                    double y3 = y0Runge + K3;
                    double K4 = getValueFunc(xTempRunge, y3) * stepRunge;
                    double yTempRunge = y0Runge + (K1 + 2 * K2 + 2 * K3 + K4) / 6;

                    RungeTempData.Add((xTempRunge, yTempRunge));
                    x0Runge = xTempRunge;
                    y0Runge = yTempRunge;
                }

                tempEps = Math.Abs(RungeTempData[targetParts * 2 - 1].Item2 - tempData[targetParts - 1].Item2);

                if (tempEps > eps)
                {
                    targetParts *= 2;
                    tempData.Clear();
                    RungeTempData.Clear();
                }

            } while (tempEps > eps);

            formTable();

            targetParts = 10;
            tempData.Clear();
            RungeTempData.Clear();
        }
    }
}
