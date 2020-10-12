using System;

namespace Contour_line
{
    class CFunction
    {
        private int index = 5;
        private double[] GlobalCoords = new double[2]; //Координаты глобального минимума
        private double C1 = 0;
        private double C2 = 0;
        private double M1 = 0;
        private double M2 = 0;
        private double[] W1 = new double[2];
        private double[] W2 = new double[2];

        public double GetValue(double[] _x)
        {
            switch (index)
            {
                case 1: // -1.5*y1^2* Exp[1 - y1^2 - 20.25*(y1 - y2)^2] - (0.5*(y1 - 1)*(y2 - 1))^4* Exp[2 - (0.5*(y1 - 1))^4 - (y2 - 1)^4]
                    return (-1.5 * _x[0] * _x[0] * Math.Exp(1 - _x[0] * _x[0] - 20.25 * (_x[0] - _x[1]) * (_x[0] - _x[1])) - Math.Pow(0.5 * (_x[0] - 1) * (_x[1] - 1), 4) * Math.Exp(2 - Math.Pow(0.5 * (_x[0] - 1), 4) - Math.Pow(_x[1] - 1, 4)));
                case 2: //(4 - 2.1*y1^2 + y1^4/3)*y1^2 + y1*y2 + (4*y2^2 - 4)*y2^2
                    return ((4 - 2.1 * _x[0] * _x[0] + Math.Pow(_x[0], 4) / 3) * _x[0] * _x[0] + _x[0] * _x[1] + (4 * _x[1] * _x[1] - 4) * _x[1] * _x[1]);
                case 3: //0.01*(y1*y2 + (y1 - \[Pi])^2 + 3*(y2^2 - \[Pi])^2) - (Sin[y1]*Sin[2*y2])^2
                    return 0.01 * (_x[0] * _x[1] + Math.Pow(_x[0] - Math.PI, 2) + 3 * Math.Pow(_x[1] * _x[1] - Math.PI, 2)) - Math.Pow(Math.Sin(_x[0]) * Math.Sin(2 * _x[1]), 2);
                case 4://(y1^2 - Cos[18*y1^2]) + (y2^2 - Cos[18*y2^2])
                    return (_x[0] * _x[0] - Math.Cos(18 * _x[0] * _x[0])) + (_x[1] * _x[1] - Math.Cos(18 * _x[1] * _x[1]));
                case 5: //\[Pi]*(10*(Sin[\[Pi]*(1 + (y1 - 1)/4)])^2 + ((y1 - 1)/4)^2*(1 + 10*(Sin[\[Pi]*(1 + (y2 - 1)/4)])^2) + (1 + (y2 - 1)/4)^2)/2
                    return Math.PI / 2 * (Math.Pow(10 * (Math.Sin(Math.PI * (1 + (_x[0] - 1) / 4))), 2) + Math.Pow((_x[0] - 1) / 4, 2) * (1 + 10 * Math.Pow(Math.Sin(Math.PI * (1 + (_x[1] - 1) / 4)), 2)) + Math.Pow((_x[1] - 1) / 4, 2));
                case 6:
                    {
                        double x = 5.55 + 0.34 / 2 * ((_x[0] - 9.46) * (_x[0] - 9.46) + (_x[1] - 10.49) * (_x[1] - 10.49));
                        double y = 5.71 + 0.71 / 2 * ((_x[0] + 9.16) * (_x[0] + 9.16) + (_x[1] + 4.55) * (_x[1] + 4.55));
                        return Math.Max(x, y);
                    }
                case 7:
                    {
                        double x = C1 + M1 / 2 * ((_x[0] - W1[0]) * (_x[0] - W1[0]) + (_x[1] - W1[1]) * (_x[1] - W1[1]));
                        double y = C2 + M2 / 2 * ((_x[0] - W2[0]) * (_x[0] - W2[0]) + (_x[1] - W2[1]) * (_x[1] - W2[1]));
                        return Math.Max(x, y);
                    }

                default: return 0; // тут мы вернем 0;
            }
        }
        public void set(int _ind)
        {
            this.index = _ind;
        } //Выбрать функцию;

        public void set_func(double m1, double m2, double c1, double c2, double[] w1, double[] w2)
        {
            this.M1 = m1;
            this.M2 = m2;
            this.C1 = c1;
            this.C2 = c2;
            this.W1 = w1;
            this.W2 = w2;

        } //Задать функцию;
    }


}
