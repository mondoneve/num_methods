using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace The_Diffik
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            setdef();
        }

        private void setdef() {
            groupBox2.Visible = false;
            tabPage1.Text = "СЕТКА";
            tabPage2.Text = "ГРАФИК";
            tabPage3.Text = "ПОРТРЕТ";
            radioButton2.Checked = true;
            tabPage3.Hide();
            groupBox3.Visible = true;
            textBox1.Text = "1";
            textBox2.Text = "0";
            textBox3.Text = "0.01";
            textBox4.Text = "0.01";
            textBox5.Text = "100";
            textBox6.Text = "0.0001";
            textBox7.Text = "1";
            textBox8.Text = "0.4";
            textBox9.Text = "0.8";
            textBox10.Text = "0.5";
            textBox11.Text = "1";


            
        }

        private void ff() {
            label12.Text = "MAX. |u-v|: ";
            label13.Text = "ШАГОВ: ";
            label14.Text = "КРАЙ: ";
            label15.Text = "МАХ. ОЛП: ";
            label16.Text = "Х2: ";
            label17.Text = "Х0.5: ";
            label18.Text = "МАХ. Ш: ";
            label19.Text = "МИН. Ш: ";

            tabPage3.Hide();
            chart1.Series.Clear();
            chart2.Series.Clear();
            chart1.Series.Add("МОЁ РЕШЕНИЕ");
            //label12.Text = " ";
            dataGridView1.ClearSelection();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ff();
            // Task selection
            int t = 0;
            if (radioButton3.Checked) t = 1;
            if (radioButton4.Checked) t = 2;
            // Task selection

            // Reckoning
            Koshi task = new Koshi(t, Convert.ToDouble(textBox2.Text), Convert.ToDouble(textBox1.Text));
            if (task.type == 2) { task.a = Convert.ToDouble(textBox8.Text); task.b = Convert.ToDouble(textBox9.Text); task.c = Convert.ToDouble(textBox10.Text); task.v0 = Convert.ToDouble(textBox11.Text); }
            Method meth = new Method(checkBox1.Checked, Convert.ToDouble(textBox6.Text), Convert.ToInt32(textBox5.Text), Convert.ToDouble(textBox3.Text), Convert.ToDouble(textBox4.Text), Convert.ToDouble(textBox7.Text));
            ResultData result = meth.ProcessTask(task);
            // Reckoning

            // Table print
            //int p = 1;
            //if (result.i.Length > 1000) p = 10 * result.i.Length/1000;
            groupBox3.Visible = true;
            int edge = (result.Nstep + 1 > 100) ? 100 : result.Nstep;
            for (int i = 0; i < edge; i++)
            {
                dataGridView1.Rows.Add();
                if (result.syst) {
                    String v = '(' + Convert.ToString(result.vii[i, 0]) + ", " + Convert.ToString(result.vii[i, 1]) + ')';
                    String v2 = '(' + Convert.ToString(result.v2ii[i, 0]) + ", " + Convert.ToString(result.v2ii[i, 1]) + ')';
                    String m = '(' + Convert.ToString(result.minusi[i, 0]) + ", " + Convert.ToString(result.minusi[i, 1]) + ')';
                    dataGridView1.Rows[i].SetValues(result.i[i], result.xi[i], v, v2, m, result.olp[i], result.hi[i], result.div[i], result.db[i], result.ui[i], result.minus2[i]);
                }
                else dataGridView1.Rows[i].SetValues(result.i[i], result.xi[i], result.vi[i], result.v2i[i], result.minus[i], result.olp[i], result.hi[i], result.div[i], result.db[i], result.ui[i], result.minus2[i]);
            }
            // Table print

            label13.Text += Convert.ToString(result.Nstep);
            label14.Text += Convert.ToString(result.xi[result.Nstep-1]);
            label15.Text += Convert.ToString(result.olp.Max());
            label16.Text += Convert.ToString(result.db.Sum());
            label17.Text += Convert.ToString(result.div.Sum());
            label18.Text += Convert.ToString(result.hi.Max());
            label19.Text += Convert.ToString(result.hi.Min());

            if (!result.syst)
            {
                //label12.Text = Convert.ToString(result.minus2.Max());
                chart1.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                for (int i = 0; i < result.Nstep; i++) chart1.Series[0].Points.AddXY(result.xi[i], result.vi[i]);
                chart1.Series[0].BorderWidth = 8;
                chart1.Series.Add("НАСТОЯЩЕЕ РЕШЕНИЕ");
                chart1.Series[1].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                chart1.Series[1].BorderWidth = 3;
                chart1.Series[1].ShadowOffset = 1;
                chart1.Series[1].BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
                for (int i = 0; i < result.Nstep; i++) chart1.Series[1].Points.AddXY(result.xi[i], result.ui[i]);
                if (task.type == 0) label12.Text += Convert.ToString(result.minus2.Max());
            }
            else
            {
                tabPage3.Show();
                chart1.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                for (int i = 1; i < result.Nstep; i++) chart1.Series[0].Points.AddXY(result.xi[i], result.vii[i,0]);
                chart1.Series[0].BorderWidth = 4;
                chart2.Series.Add("МОЯ ТРАЕКТОРИЯ");
                chart2.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                chart2.Series[0].BorderWidth = 4;
                for (int i = 1; i < result.Nstep; i++) chart2.Series[0].Points.AddXY(result.vii[i,0], result.vii[i, 1]);


            }
            //setdef();
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton4.Checked) groupBox2.Visible = true;
            if (!radioButton4.Checked) groupBox2.Visible = false;
        }

        private void label15_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox3.Text = Convert.ToString(Convert.ToDouble(textBox3.Text) * 10);
            textBox4.Text = Convert.ToString(Convert.ToDouble(textBox4.Text) * 10);
            textBox5.Text = Convert.ToString(Convert.ToDouble(textBox5.Text) / 10.0);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox3.Text = Convert.ToString(Convert.ToDouble(textBox3.Text) / 10.0);
            textBox4.Text = Convert.ToString(Convert.ToDouble(textBox4.Text) / 10.0);
            textBox5.Text = Convert.ToString(Convert.ToDouble(textBox5.Text) * 10);
        }

        private void tabPage3_Click(object sender, EventArgs e)
        {

        }
    }
}
