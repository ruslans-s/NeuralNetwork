using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace neuro
{
    class Network
    {
        struct LayerT
        {
            public Vector x; // вход слоя
            public Vector z; // активированный выход слоя
            public Vector df; // производная функции активации слоя
        }


        Matrix[] weights; // матрицы весов слоя
        LayerT[] L; // значения на каждом слое
        Vector[] deltas; // дельты ошибки на каждом слое

        int layersN; // число слоёв

        //Запись в файл
        public void WriteToFile()
        {
            StreamWriter sw = new StreamWriter("scales.txt");
            //Запись слоев
            sw.WriteLine(layersN);

            //Запись весов
            for (int j = 0; j < weights.Length; j++)
            {
                for (int l = 0; l < weights[j].n; l++)
                {
                    for (int m = 0; m < weights[j].m; m++)
                    {
                        sw.WriteLine(weights[j][l, m]);
                    }
                }
            }

            // запись массивов значений на каждом слое
            for (int j = 0; j < L.Length; j++)
            {
                for (int i = 0; i < L[j].x.n; i++)
                {
                    sw.WriteLine(L[j].x[i]);
                }
                for (int i = 0; i < L[j].z.n; i++)
                {
                    sw.WriteLine(L[j].z[i]);
                }
                for (int i = 0; i < L[j].df.n; i++)
                {
                    sw.WriteLine(L[j].df[i]);
                }
            }
            // запись дельт
            for (int j = 0; j < deltas.Length; j++)
            {
                for (int i = 0; i < deltas[j].n; i++)
                {
                    sw.WriteLine(deltas[j][i]);
                }
            }

            sw.Close();
        }
        //Чтение из файла
        public void ReadFromFile()
        {
            StreamReader sw = new StreamReader("scales.txt");
            //Запись слоев

            layersN = Convert.ToInt32(sw.ReadLine());

            //Запись весов
            for (int j = 0; j < weights.Length; j++)
            {
                for (int l = 0; l < weights[j].n; l++)
                {
                    for (int m = 0; m < weights[j].m; m++)
                    {
                        weights[j][l, m] = Convert.ToDouble(sw.ReadLine());
                    }
                }
            }

            // запись массивов значений на каждом слое
            for (int j = 0; j < L.Length; j++)
            {
                for (int i = 0; i < L[j].x.n; i++)
                {
                    L[j].x[i] = Convert.ToDouble(sw.ReadLine());
                }
                for (int i = 0; i < L[j].z.n; i++)
                {
                    L[j].z[i] = Convert.ToDouble(sw.ReadLine());
                }
                for (int i = 0; i < L[j].df.n; i++)
                {
                    L[j].df[i] = Convert.ToDouble(sw.ReadLine());
                }
            }
            // запись дельт
            for (int j = 0; j < deltas.Length; j++)
            {
                for (int i = 0; i < deltas[j].n; i++)
                {
                    deltas[j][i] = Convert.ToDouble(sw.ReadLine());
                }
            }
            sw.Close();
        }

        //Инцилизация нейронки 
        public Network(int[] sizes)
        {
            Random random = new Random(DateTime.Now.Millisecond); // создаём генератор случайных чисел

            layersN = sizes.Length - 1; // запоминаем число слоёв

            weights = new Matrix[layersN]; // создаём массив матриц весовых коэффициентов
            L = new LayerT[layersN]; // создаём массив значений на каждом слое
            deltas = new Vector[layersN]; // создаём массив для дельт

            for (int k = 1; k < sizes.Length; k++)
            {
                weights[k - 1] = new Matrix(sizes[k], sizes[k - 1], random); // создаём матрицу весовых коэффициентов

                L[k - 1].x = new Vector(sizes[k - 1]); // создаём вектор для входа слоя
                L[k - 1].z = new Vector(sizes[k]); // создаём вектор для выхода слоя
                L[k - 1].df = new Vector(sizes[k]); // создаём вектор для производной слоя

                deltas[k - 1] = new Vector(sizes[k]); // создаём вектор для дельт
            }
        }

        // прямое распространение
        public Vector Forward(Vector input)
        {
            for (int k = 0; k < layersN; k++)
            {
                if (k == 0)
                {
                    for (int i = 0; i < input.n; i++)
                        L[k].x[i] = input[i];
                }
                else
                {
                    for (int i = 0; i < L[k - 1].z.n; i++)
                        L[k].x[i] = L[k - 1].z[i];
                }

                for (int i = 0; i < weights[k].n; i++)
                {
                    double y = 0;

                    for (int j = 0; j < weights[k].m; j++)
                        y += weights[k][i, j] * L[k].x[j];

                    // активация с помощью сигмоидальной функции
                    L[k].z[i] = 1 / (1 + Math.Exp(-y));
                    L[k].df[i] = L[k].z[i] * (1 - L[k].z[i]);

                    // активация с помощью гиперболического тангенса
                    // L[k].z[i] = Math.Tanh(y);
                    // L[k].df[i] = 1 - L[k].z[i] * L[k].z[i];

                    //активация с помощью ReLU
                    //  L[k].z[i] = y > 0 ? y : 0;
                    // L[k].df[i] = y > 0 ? 1 : 0;
                }
            }

            return L[layersN - 1].z; // возвращаем результат
        }

        // обратное распространение
        void Backward(Vector output, ref double error)
        {
            int last = layersN - 1;

            error = 0; // обнуляем ошибку

            for (int i = 0; i < output.n; i++)
            {
                double e = L[last].z[i] - output[i]; // находим разность значений векторов

                deltas[last][i] = e * L[last].df[i]; // запоминаем дельту
                error += e * e / 2; // прибавляем к ошибке половину квадрата значения
            }

            // вычисляем каждую предудущю дельту на основе текущей с помощью умножения на транспонированную матрицу
            for (int k = last; k > 0; k--)
            {
                for (int i = 0; i < weights[k].m; i++)
                {
                    deltas[k - 1][i] = 0;

                    for (int j = 0; j < weights[k].n; j++)
                        deltas[k - 1][i] += weights[k][j, i] * deltas[k][j];

                    deltas[k - 1][i] *= L[k - 1].df[i]; // умножаем получаемое значение на производную предыдущего слоя
                }
            }
        }

        // обновление весовых коэффициентов, alpha - скорость обучения
        void UpdateWeights(double alpha)
        {
            for (int k = 0; k < layersN; k++)
            {
                for (int i = 0; i < weights[k].n; i++)
                {
                    for (int j = 0; j < weights[k].m; j++)
                    {
                        double temp = alpha * deltas[k][i] * L[k].x[j];
                        weights[k][i, j] -= temp;
                    }
                }
            }
        }

        public void Train(Vector X, Vector Y, double alpha, double eps, int epochs)
        {
            int epoch = 1; // номер эпохи

            double error; // ошибка эпохи
          

            do
            {
                error = 0; // обнуляем ошибку
                // проходимся по всем элементам обучающего множества

                Forward(X); // прямое распространение сигнала
                Backward(Y, ref error); // обратное распространение ошибки
                UpdateWeights(alpha); // обновление весовых коэффициентов


                epoch++; // увеличиваем номер эпохи
            } while (epoch <= epochs && error > eps);
        }
        public void Train(Vector[] X, Vector[] Y, double alpha, double eps, int epochs)
        {
            int epoch = 1; // номер эпохи

            double error; // ошибка эпохи

            do
            {
                error = 0; // обнуляем ошибку

                // проходимся по всем элементам обучающего множества
                for (int i = 0; i < X.Length; i++)
                {
                    Forward(X[i]); // прямое распространение сигнала
                    Backward(Y[i], ref error); // обратное распространение ошибки
                    UpdateWeights(alpha); // обновление весовых коэффициентов
                }

                epoch++; // увеличиваем номер эпохи
            } while (epoch <= epochs && error > eps);
        }

    }
}
