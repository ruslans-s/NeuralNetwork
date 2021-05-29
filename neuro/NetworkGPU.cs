using Amplifier;
using Amplifier.Extensions;
using Amplifier.OpenCL;
using System;
using System.IO;
using System.Threading;

namespace neuro
{
    struct LayerT
    {
        public Vector x; // вход слоя
        public Vector z; // активированный выход слоя
        public Vector df; // производная функции активации слоя
    }

    class SimpleKernels : OpenCLFunctions
    {
        [OpenCLKernel]
        void AddData([Global, Input] float[] a, [Global] float[] b, [Global, Output] float[] r)
        {
            int i = get_global_id(0);
            b[i] = 0.5f * b[i];
            r[i] = a[i] + b[i];
            a[i] += 2; // result will not copy out
        }

        [OpenCLKernel]
        void AddHalf([Global, Input] half[] a, [Global] half[] b)
        {
            int i = get_global_id(0);
            float af = vload_half(i, a);
            float bf = vload_half(i, b);
            vstore_half(af + bf, i, b);
        }

        [OpenCLKernel]
        void Fill([Global] float[] x, float value)
        {
            int i = get_global_id(0);

            x[i] = value;
        }

        [OpenCLKernel]
        void SAXPY([Global] float[] x, [Global] float[] y, float a)
        {
            int i = get_global_id(0);

            y[i] += a * x[i];
        }
    }

    public class SGEMMKernals : OpenCLFunctions
    {
        [OpenCLKernel]
        void Sigmoid([Global] float[] v, [Global] float[] df)
        {
            int i = get_global_id(0);
            v[i] = v[i] * df[i];
        }

        [OpenCLKernel]
        void Replacement([Global] float[] v, [Global] float[] df)
        {
            int i = get_global_id(0);
            v[i] = df[i];
        }


        [OpenCLKernel]
        void UpdateWeughts([Global] float[] weig, [Global] float[] x, [Global] float[] delt, float alph)
        {
            int i = get_global_id(0);
            int k = get_global_id(2);
            weig[i] = weig[i] - alph * delt[k] * x[i];
        }

        [OpenCLKernel]
        void CalculationY([Global] float[] v, [Global] float[] Lx, float y)
        {
            int i = get_global_id(0);

            y = v[i] * Lx[i];
        }

    }



    class NetworkGPU
    {
        public float y;


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
                        weights[j][l, m] = (float)Convert.ToDouble(sw.ReadLine());
                    }
                }
            }

            // запись массивов значений на каждом слое
            for (int j = 0; j < L.Length; j++)
            {
                for (int i = 0; i < L[j].x.n; i++)
                {
                    L[j].x[i] = (float)Convert.ToDouble(sw.ReadLine());
                }
                for (int i = 0; i < L[j].z.n; i++)
                {
                    L[j].z[i] = (float)Convert.ToDouble(sw.ReadLine());
                }
                for (int i = 0; i < L[j].df.n; i++)
                {
                    L[j].df[i] = (float)Convert.ToDouble(sw.ReadLine());
                }
            }
            // запись дельт
            for (int j = 0; j < deltas.Length; j++)
            {
                for (int i = 0; i < deltas[j].n; i++)
                {
                    deltas[j][i] = (float)Convert.ToDouble(sw.ReadLine());
                }
            }
            sw.Close();
        }

        private OpenCLCompiler compiler1;
        private dynamic exec;

        //Инцилизация нейронки 
        public NetworkGPU(int[] sizes)
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

            /*
            var compiler1 = new OpenCLCompiler();
            compiler1.UseDevice(0);
            compiler1.CompileKernel(typeof(NNActivationKernels));
            */

             compiler1 = new OpenCLCompiler();
                   //Select a default device
            compiler1.UseDevice(0);
            //Compile the sample kernel
            compiler1.CompileKernel(typeof(SGEMMKernals), typeof(SimpleKernels));
             exec = compiler1.GetExec();
        }


        static void Replacement()
        { 
        
        }

            // прямое распространение
            public Vector Forward(Vector input)
        {
            for (int k = 0; k < layersN; k++)
            {
                if (k == 0)
                {
                    exec.Replacement(L[k].x.v, input.v);

                 //   L[k].x.v.AmplifyFor(compiler1, "Replacement", input.v);

                    /*   for (int i = 0; i < input.n; i++)
                           L[k].x[i] = input[i];*/
                }
                else
                {
                    exec.Replacement(L[k].x.v, L[k - 1].z.v);

                //    L[k].x.v.AmplifyFor(compiler1, "Replacement", L[k - 1].z.v);

                    /*  for (int i = 0; i < L[k - 1].z.n; i++)
                          L[k].x[i] = L[k - 1].z[i];*/
                }

               
                for (int i = 0; i < weights[k].n; i++)
                {
                    y = 0;

                    //CalculationY([Global] float[] v, [Global] float[] Lx, float y)

                   // e/xec.CalculationY(weights[k].v[i], L[k].x.v,y);

                   // y.AmplifyFor(compiler1, "CalculationY", weights[k].v[i], L[k].x.v);

                    for (int j = 0; j < weights[k].m; j++)
                        y += weights[k][i, j] * L[k].x[j];

               
                    // активация с помощью сигмоидальной функции
                       L[k].z[i] = (float)(1 / (1 + Math.Exp(-y)));
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

                deltas[last][i] = (float)(e * L[last].df[i]); // запоминаем дельту
                error += e * e / 2; // прибавляем к ошибке половину квадрата значения
            }

            float[] deltasToDArray = new float[0];
            float[] dfToDArray = new float[0];

            // вычисляем каждую предудущю дельту на основе текущей с помощью умножения на транспонированную матрицу
            for (int k = last; k > 0; k--)
            {
                for (int i = 0; i < weights[k].m; i++)
                {
                    deltas[k - 1][i] = 0;

                    for (int j = 0; j < weights[k].n; j++)
                        deltas[k - 1][i] += weights[k][j, i] * deltas[k][j];

                    Array.Resize<float>(ref deltasToDArray, deltasToDArray.Length + 1);
                    deltasToDArray[deltasToDArray.Length - 1] = deltas[k - 1].v[i];

                    Array.Resize<float>(ref dfToDArray, dfToDArray.Length + 1);
                    dfToDArray[dfToDArray.Length - 1] = L[k - 1].df.v[i];

                   // deltas[k - 1].v[i] *= L[k - 1].df.v[i]; // умножаем получаемое значение на производную предыдущего слоя
                }
            }

            exec.Sigmoid(deltasToDArray, dfToDArray);

            //******************

            //float[] delt = new float[0];
            float[] x = new float[0];
            float[] weig = new float[0];

            float alph = 0.5f;

            for (int k = 0; k < layersN; k++)
            {
                for (int i = 0; i < weights[k].n; i++)
                {
                /*    Array.Resize<float>(ref delt, delt.Length + 1);
                    delt[delt.Length - 1] = deltas[k][i];*/

                    for (int j = 0; j < weights[k].m; j++)
                    {
                        Array.Resize<float>(ref x, x.Length + 1);
                        x[x.Length - 1] = L[k].x[j];

                        Array.Resize<float>(ref weig, weig.Length + 1);
                        weig[weig.Length - 1] = weights[k][i, j];
                    }
                }
            }

            exec.UpdateWeughts(weig, x, deltasToDArray, alph);

            int counter = 0;
            for (int k = 0; k < layersN; k++)
            {
                for (int i = 0; i < weights[k].n; i++)
                {
                    for (int j = 0; j < weights[k].m; j++)
                    {
                        weights[k][i, j] = weig[counter];
                        counter++;
                    }
                }
            }

            //*****************

            int li = 0;
            for (int k = last; k > 0; k--)
            {
                for (int i = 0; i < weights[k].m; i++)
                {
                    deltas[k - 1][i] = deltasToDArray[li];
                    li++;
                }
            }
        }

        // обновление весовых коэффициентов, alpha - скорость обучения
        void UpdateWeights(double alpha)
        {
            float[] delt = new float[0];
            float[] x = new float[0];
            float[] weig = new float[0];

            float alph = (float)alpha;

            for (int k = 0; k < layersN; k++)
            {
                for (int i = 0; i < weights[k].n; i++)
                {
                    Array.Resize<float>(ref delt, delt.Length + 1);
                    delt[delt.Length - 1] = deltas[k][i];

                    for (int j = 0; j < weights[k].m; j++)
                    {
                        Array.Resize<float>(ref x, x.Length + 1);
                        x[x.Length - 1] = L[k].x[j];

                        Array.Resize<float>(ref weig, weig.Length + 1);
                        weig[weig.Length - 1] = weights[k][i, j];
                    }
                }
            }

            exec.UpdateWeughts(weig, x, delt, alph);

            int counter = 0;
            for (int k = 0; k < layersN; k++)
            {
                for (int i = 0; i < weights[k].n; i++)
                {
                    for (int j = 0; j < weights[k].m; j++)
                    {
                        weights[k][i, j] = weig[counter];
                        counter++;
                    }
                }
            }

            

            /*
            for (int k = 0; k < layersN; k++)
            {
                for (int i = 0; i < weights[k].n; i++)
                {
                    for (int j = 0; j < weights[k].m; j++)
                    {
                        weights[k][i, j] -= (float)(alpha * deltas[k][i] * L[k].x[j]);
                    }
                }
            }*/
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
                                               //     UpdateWeights(alpha); // обновление весовых коэффициентов
                    if (i > 500) break;
                }

                epoch++; // увеличиваем номер эпохи
            } while (epoch <= epochs && error > eps);
        }

    }
}
