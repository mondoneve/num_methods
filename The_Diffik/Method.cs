using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Diffik
{
    class Method {
        double precision;  // Closeness to x-axis end value
        double xborder;  // X-axis end value
        bool control;  // Control / Not control error 
        double eps;  // Local error
        int N;  // Max number of steps
        double h;  // Step

        public Method() { }

        public Method(bool _control, double _eps, int _N, double _prescision, double _h, double _xborder) { h = _h; control = _control; eps = _eps; N = _N; precision = _prescision; xborder = _xborder; }

        private double[] min(double[] a, double[] b)
        {
            double[] r = new double[a.Length];
            for (int i = 0; i < r.Length; i++) r[i] = a[i] - b[i];
            return r;
        }

        private double[] mult(double[] a, double[] b)
        {
            double[] r = new double[a.Length];
            for (int i = 0; i < r.Length; i++) r[i] = a[i] * b[i];
            return r;
        }

        private double[] mult(double[] a, double b)
        {
            double[] r = new double[a.Length];
            for (int i = 0; i < r.Length; i++) r[i] = a[i] * b;
            return r;
        }

        private double[] div(double[] a, double b)
        {
            double[] r = new double[a.Length];
            for (int i = 0; i < r.Length; i++) r[i] = a[i] / b;
            return r;
        }

        private double[] add(double[] a, double[] b)
        {
            double[] r = new double[a.Length];
            for (int i = 0; i < r.Length; i++) r[i] = a[i] + b[i];
            return r;
        }

        private double GetNext(Koshi task, double x, double u, double h) {
            double k1 = task.f(x, u);
            double k2 = task.f(x + h * 0.5, u + h * k1 * 0.5);
            double k3 = task.f(x + h * 0.5, u + h * k2 * 0.5);
            double k4 = task.f(x + h, u + h * k3);

            return u + h * (k1 + 2 * k2 + 2 * k3 + k4) / 6.0;
        }

        private double[] GetNext_syst(Koshi task, double x, double[] u, double h)
        {
            double[] hh = new double[2];
            hh[0] = h; hh[1] = h;
            double[] k1 = task.f_syst(u);
            double[] k2 = task.f_syst(add(u, mult(mult(hh,k1), 0.5)));
            double[] k3 = task.f_syst(add(u,mult(mult(hh, k2),0.5)));
            double[] k4 = task.f_syst(add(u, mult(hh, k3)));

            return add(u, div(mult(hh, add(add(k1, mult(k2, 2)), add(mult(k3, 2), k4))), 6.0));
        }

        /*public ResultData ProcessTask(Koshi task) {
            ResultData result = new ResultData(N+1);
            int step = 1;  // Number of step
            double x = task.x0;
            double u = Koshi.u0;

            result.Insert(0, x, h, u, u, 0f, 0f, 0, 0);

            if (control)
                while ((step <= N) && (x - xborder < precision)) {
                    int div = 0;
                    int db = 0;
                    double uNext = GetNext(task, x, u, h);  // Vi+1
                    double uNextSecond = GetNext(task, x + h * 0.5, GetNext(task, x, u, h * 0.5), h * 0.5);  // Vi+1'
                    double err = Math.Abs(uNextSecond - uNext) / 15.0;

                    if (err > eps)
                        while (err > eps) {
                            div++;
                            h /= 2;
                            uNext = GetNext(task, x, u, h);
                            uNextSecond = GetNext(task, x + h * 0.5, GetNext(task, x, u, h * 0.5), h * 0.5);
                            err = Math.Abs(uNextSecond - uNext) / 15.0;
                        }
                    if (err < eps / 32.0) db++;

                    
                    x += h;
                    u = uNext;
                    result.Insert(step, x, h, uNext, uNextSecond, uNext - uNextSecond, err, div, db);
                    step++;
                    if (db == 1) h *= 2;
                }
            else
                while ((step < N) && (x - xborder < precision))
                {
                    double uNext = GetNext(task, x, u, h);  // Vi+1
                    double uNextSecond = GetNext(task, x + h * 0.5, GetNext(task, x, u, h * 0.5), h * 0.5);  // Vi+1'
                    double err = Math.Abs(uNextSecond - uNext) / 15.0;
                    result.Insert(step, x, h, uNext, uNextSecond, uNext - uNextSecond, err, 0, 0);
                    x += h;
                    step++;
                }


            if (task.type == 0) result.fillTest(N);

            return result;
        }*/

        public ResultData ProcessTask(Koshi task)
        {
            ResultData result = new ResultData(N + 1);
            result.Insert(0, task.x0, h, task.u0, task.u0, 0f, 0f, 0, 0);

            if (task.type == 2)
            {
                if (control) SolveWithControl_syst(task, ref result);
                else SolveNoControl_syst(task, ref result);
                return result;
            }

            if (control) SolveWithControl(task, ref result);
            else SolveNoControl(task, ref result);

            if (task.type == 0) result.fillTest(N + 1, task);

            return result;
        }


        private void SolveWithControl(Koshi task, ref ResultData data)
        {
            int step = 1;
            double x = task.x0;
            double u = task.u0;
            while ((step <= N) && (x - xborder < precision))
            {
                int div = 0;
                int db = 0;
                double uNext = GetNext(task, x, u, h);  // Vi+1
                double uNextSecond = GetNext(task, x + h * 0.5, GetNext(task, x, u, h * 0.5), h * 0.5);  // Vi+1'
                double err = Math.Abs(uNextSecond - uNext) / 15.0;

                if (err > eps)
                    while (err > eps)
                    {
                        div++;
                        h *= 0.5;
                        uNext = GetNext(task, x, u, h);
                        uNextSecond = GetNext(task, x + h * 0.5, GetNext(task, x, u, h * 0.5), h * 0.5);
                        err = Math.Abs(uNextSecond - uNext) / 15.0;
                    }
                if (err < eps / 32.0) db++;


                x += h;
                u = uNext;
                data.Insert(step, x, h, uNext, uNextSecond, uNext - uNextSecond, err, div, db);
                step++;
                if (db == 1) h *= 2;
            }

            if (x - xborder > precision)
            {
                step--;
                int div = 0;
                while (x - xborder > precision)
                {
                    x -= h;
                    h *= 0.5;
                    div++;
                    x += h;
                }
                double u1 = GetNext(task, x - h, u, h);
                double u2 = GetNext(task, x - h * 0.5, GetNext(task, x, u, h * 0.5), h * 0.5);
                data.Insert(step, x, h, u1, u2, u1 - u2, Math.Abs(u1 - u2) / 15.0, div, 0);
            }
            data.Nstep = step;
        }

        private void SolveNoControl(Koshi task, ref ResultData data)
        {
            int step = 1;
            double x = task.x0;
            double u = task.u0;
            while ((step <= N) && (x - xborder < precision))
            {
                u = GetNext(task, x, u, h);
                double uHalfHalf = GetNext(task, x + h * 0.5, GetNext(task, x, u, h * 0.5), h * 0.5);
                double err = Math.Abs(uHalfHalf - u) / 15.0;
                x += h;
                data.Insert(step, x, h, u, uHalfHalf, u - uHalfHalf, err, 0, 0);
                step++;
            }

            if (x - xborder > precision)
            {
                step--;
                int div = 0;
                while (x - xborder > precision)
                {
                    x -= h;
                    h *= 0.5;
                    div++;
                    x += h;
                }
                double u1 = GetNext(task, x - h, u, h);
                double u2 = GetNext(task, x - h * 0.5, GetNext(task, x, u, h * 0.5), h * 0.5);
                data.Insert(step, x, h, u1, u2, u1 - u2, Math.Abs(u1 - u2) / 15.0, div, 0);
            }
            data.Nstep = step;
        }

        private void SolveWithControl_syst(Koshi task, ref ResultData data)
        {
            int step = 1;
            double x = task.x0;
            double[] u = { task.u0, task.v0 };
            while ((step <= N) && (x - xborder < precision))
            {
                int div = 0;
                int db = 0;
                double[] uNext = GetNext_syst(task, x, u, h);  // Vi+1
                double[] uNextSecond = GetNext_syst(task, x + h * 0.5, GetNext_syst(task, x, u, h * 0.5), h * 0.5);  // Vi+1'
                double err = Math.Sqrt((uNext[0]-uNextSecond[0])* (uNext[0] - uNextSecond[0])+ (uNext[1] - uNextSecond[1])* (uNext[1] - uNextSecond[1])) / 15.0;
                // double err = Math.Abs(uNextSecond[0] - uNext[0]) / 15.0;
                //if (err < Math.Abs(uNextSecond[1] - u[1]) / 15.0) err = Math.Abs(uNextSecond[1] - u[1]) / 15.0;
                if (err > eps)
                    while (err > eps)
                    {
                        div++;
                        h /= 2;
                        uNext = GetNext_syst(task, x, u, h);
                        uNextSecond = GetNext_syst(task, x + h * 0.5, GetNext_syst(task, x, u, h * 0.5), h * 0.5);
                        err = Math.Abs(uNextSecond[0] - uNext[0]) / 15.0;
                        if (err < Math.Abs(uNextSecond[1] - u[1]) / 15.0) err = Math.Abs(uNextSecond[1] - u[1]) / 15.0;
                    }
                if (err < eps / 32.0) db++;


                x += h;
                u = uNext;
                data.Insert(step, x, h, uNext, uNextSecond, min(uNext, uNextSecond), err, div, db);
                step++;
                if (db == 1) h *= 2;
            }

            if (x - xborder > precision)
            {
                step--;
                int div = 0;
                while (x - xborder > precision)
                {
                    x -= h;
                    h *= 0.5;
                    div++;
                    x += h;
                }
                double[] u1 = GetNext_syst(task, x - h, u, h);
                double[] u2 = GetNext_syst(task, x - h * 0.5, GetNext_syst(task, x, u, h * 0.5), h * 0.5);
                data.Insert(step, x, h, u1, u2, min(u1, u2), Math.Abs(u1[0] - u2[0]) / 15.0, div, 0);
            }
            data.Nstep = step;
        }

        private void SolveNoControl_syst(Koshi task, ref ResultData data)
        {
            int step = 1;
            double x = task.x0;
            double[] u = { task.u0, task.v0 };
            while ((step <= N) && (x - xborder < precision))
            {
                u = GetNext_syst(task, x, u, h);
                double[] uHalfHalf = GetNext_syst(task, x + h * 0.5, GetNext_syst(task, x, u, h * 0.5), h * 0.5);
                double err = Math.Abs(uHalfHalf[0] - u[0]) / 15.0;
                if (err < Math.Abs(uHalfHalf[1] - u[1]) / 15.0) err = Math.Abs(uHalfHalf[1] - u[1]) / 15.0;
                x += h;
                data.Insert(step, x, h, u, uHalfHalf, min(u, uHalfHalf), err, 0, 0);
                step++;
            }

            if (x - xborder > precision)
            {
                step--;
                int div = 0;
                while (x - xborder > precision)
                {
                    x -= h;
                    h *= 0.5;
                    div++;
                    x += h;
                }
                double[] u1 = GetNext_syst(task, x - h, u, h);
                double[] u2 = GetNext_syst(task, x - h * 0.5, GetNext_syst(task, x, u, h * 0.5), h * 0.5);
                data.Insert(step, x, h, u1, u2, min(u1, u2), Math.Abs(u1[0] - u2[0]) / 15.0, div, 0);
            }
            data.Nstep = step;
        }
    }
}
