using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Diffik
{
    class Koshi {

        public int type;  // Type of task: 0 - Test, 1 - Main1, 2 - Main2
        public double x0;  // x0
        public double u0;  // u0
        public double a;
        public double b;
        public double c;
        public double v0;

        public Koshi (int _type, double _x0, double _u0) { x0 = _x0; type = _type; u0 = _u0; }

        public double f(double x, double u) {
            if (type == 1) return ((Math.Pow(x, 3) + 1) / (Math.Pow(x, 5) + 1)) * Math.Pow(u, 2) + u - Math.Pow(u, 3) * Math.Sin(10 * x);
            if (type == 0) return 2*u;
            return 0;
        }

        public double[] f_syst(double[] uv)
        {
            double[] r = new double[2];
            r[0] = uv[1];
            r[1] = -a * uv[1] * Math.Abs(uv[1]) - b * uv[1] - c * uv[0];
            return r;
        }

        public double getTestValue(double x) { return u0 * Math.Exp(2 * x); }
    }
}
