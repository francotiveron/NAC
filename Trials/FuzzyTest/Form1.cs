using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Accord;
using Accord.Controls;

namespace FuzzyTest {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();

            m_chart.Width = 100;
            m_chart.Height = 100;
            m_chart.RangeX = new Range(0, 100);
            m_chart.RangeY = new Range(0, 100);
            double[,] se = new double[11, 2];
            for (int i = 0; i <= 10; i++) {
                se[i, 0] = i * 10;
                se[i, 1] = i * 10;
            }
            m_chart.AddDataSeries("SE", Color.Yellow, Chart.SeriesType.Line, 3);
            m_chart.UpdateDataSeries("SE", se);
        }

    }
}
