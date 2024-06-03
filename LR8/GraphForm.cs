using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using ZedGraph;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace LR8
{
    public partial class GraphForm : Form
    {
        List<(double, double)> points;

        public GraphForm(List<(double, double)> data)
        {
            InitializeComponent();

            points = data;

            ZedGraphControl zedGraphControl = new ZedGraphControl();
            GraphPane myPane = zedGraphControl.GraphPane;

            int width = ClientSize.Width;
            int height = ClientSize.Height;

            myPane.CurveList.Clear();
            myPane.GraphObjList.Clear();
            myPane.Title.Text = "График";
            myPane.XAxis.Title.Text = "X";
            myPane.YAxis.Title.Text = "Y";

            PointPairList pointPairList = new PointPairList();
            pointPairList.Add(1, Math.Sqrt(2));

            // Координаты узлов
            foreach (var node in points)
            {
                pointPairList.Add(node.Item1, node.Item2);

                //TextObj label = new TextObj($"({node.Item1.ToString("0.00")}, {node.Item2.ToString("0.00")})", node.Item1, node.Item2);
                //label.FontSpec.Size = 6;
                //label.FontSpec.FontColor = Color.Black;
                //label.Location.AlignH = AlignH.Right;
                //label.Location.AlignV = AlignV.Top;
                //myPane.GraphObjList.Add(label);
            }

            // Красная вертикальная линия y(1) = sqrt(2)
            LineObj line = new LineObj(Color.Red, 1, 0, 1, Math.Sqrt(2));
            line.Line.Style = System.Drawing.Drawing2D.DashStyle.Dot;
            line.IsClippedToChartRect = true;
            myPane.GraphObjList.Add(line);

            // Легенда функции
            LineItem myCurve = myPane.AddCurve("y'(x) = (-(5x^2 + 3)y^3 + 12y) / 8x", pointPairList, Color.Blue, SymbolType.Circle);
            myCurve.Symbol.Fill = new Fill(Color.Blue);
            myCurve.Line.IsVisible = true;

            // Легенда X*
            PointPairList markerList = new PointPairList();
            markerList.Add(1, 0);
            LineItem markerCurve = myPane.AddCurve("y(1) = sqrt(2)", markerList, Color.Red, SymbolType.XCross);
            markerCurve.Symbol.Fill = new Fill(Color.Red);
            markerCurve.Line.IsVisible = false;

            zedGraphControl.Size = new Size(width, height);

            Controls.Add(zedGraphControl);
        }
    }
}
