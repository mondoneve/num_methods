using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Diffik
{
    class ResultData {
        public int Nstep;
        public bool syst;
        public int dbcount;
        public int divcount;
        public int[] i;
        public double[] xi;
        public double[] hi;
        public double[] vi;
        public double[] v2i;
        public double[] minus;
        public double[] olp;
        public int[] div;  // Divide counter
        public int[] db;  // Double counter
        public double[] ui;
        public double[] minus2;

        public double[,] vii;
        public double[,] v2ii;
        public double[,] minusi;

        public ResultData(int n)
        {
            syst = false;
            dbcount = 0; divcount = 0;
            i = new int[n];
            xi = new double[n];
            hi = new double[n];
            vi = new double[n];
            v2i = new double[n];
            minus = new double[n];
            olp = new double[n];
            div = new int[n];
            db = new int[n];
            ui = new double[n];
            minus2 = new double[n];

            vii = new double[n,2];
            v2ii = new double[n,2];
            minusi = new double[n,2];
        }

        public void Insert(int _i, double _xi, double _hi, double _vi, double _v2i, double _minus, double _olp, int _div, int _db) {
            i[_i] = _i;
            xi[_i] = _xi;
            hi[_i] = _hi;
            vi[_i] = _vi;
            v2i[_i] = _v2i;
            minus[_i] = _minus;
            olp[_i] = _olp;
            div[_i] = _div;
            db[_i] = _db;
            return;
        }

        public void Insert(int _i, double _xi, double _hi, double[] _vi, double[] _v2i, double[] _minus, double _olp, int _div, int _db)
        {
            syst = true;
            i[_i] = _i;
            xi[_i] = _xi;
            hi[_i] = _hi;
            vi[_i] = 0;
            v2i[_i] = 0;
            minus[_i] = 0;
            olp[_i] = _olp;
            div[_i] = _div;
            db[_i] = _db;

            vii[_i,0] = _vi[0]; vii[_i, 1] = _vi[1];
            v2ii[_i,0] = _v2i[0]; v2ii[_i, 1] = _v2i[1];
            minusi[_i,0] = _minus[0]; minusi[_i, 1] = _minus[1];
            return;
        }

        public void fillTest(int n, Koshi task)
        {
            for (int j = 0; j < n; j++)
            {
                ui[j] = task.getTestValue(xi[j]);
                minus2[j] = Math.Abs(ui[j] - vi[j]);
            }
        }

    }
}
