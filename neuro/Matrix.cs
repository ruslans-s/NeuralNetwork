using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace neuro
{
    class Matrix
    {
      public  float[][] v; // значения матрицы
        public int n, m; // количество строк и столбцов

        // создание матрицы заданного размера и заполнение случайными числами из интервала (-0.5, 0.5)
        public Matrix(int n, int m, Random random)
        {
            this.n = n;
            this.m = m;

            v = new float[n][];

            for (int i = 0; i < n; i++)
            {
                v[i] = new float[m];

                for (int j = 0; j < m; j++)
                    v[i][j] = (float)(random.NextDouble() - 0.5); // заполняем случайными числами
            }
        }

        // обращение по индексу
        public float this[int i, int j]
        {
            get { return v[i][j]; } // получение значения
            set { v[i][j] = value; } // изменение значения
        }
    }
}
