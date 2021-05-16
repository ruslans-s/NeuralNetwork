using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace neuro
{
    struct TheardStruk
    {
        internal Vector fileVector;
        internal Vector Y;
    };

    public partial class Form1 : Form
    {

         Network network = new Network(new int[] { 784, 16, 10 }); // создаём сеть с двумя входами, тремя нейронами в скрытом слое и одним выходом

        public Form1()
        {
            InitializeComponent();
            network.ReadFromFile();
            DirectoryInfo dir = new DirectoryInfo(@"D:\repos\neuro\train\");

            foreach (FileInfo file in dir.GetFiles())
            {
                fileName.Add(file.Name);
            }
            listBox1.Items.Add(fileName.Count);
            
        }


        List<string> fileName = new List<string>();
        private void button1_Click(object sender, EventArgs e)
        {

            progressBar1.Maximum = fileName.Count;

            for (int l = 0; l < fileName.Count; l++)
            {
                network.ReadFromFile();
                Bitmap newImage = new Bitmap(@"D:\repos\neuro\train\" + fileName[l]);

                Vector fileVector = new Vector(784);
                Vector Y = new Vector(10);

                for (int j = 0; j < newImage.Width; j++)
                    for (int i = 0; i < newImage.Height; i++)
                    {
                        //listBox1.Items.Add(Convert.ToInt32(newImage.GetPixel(i, i).R));
                        fileVector[j * newImage.Width + i] = 0.00390625 * Convert.ToInt32(newImage.GetPixel(i, i).R);
                    }
                //Запись ответа упростить!!!!
                string lineForRegex = Regex.Replace(fileName[l], ".*num", "");
                lineForRegex = Regex.Replace(lineForRegex, ".png", "");
                Y[Convert.ToInt32(lineForRegex)] = 1;

                
                //Пробуем в потоки
                int theardCount = 15;
                List<Thread> threads = new List<Thread>();
                TheardStruk threadsStruct = new TheardStruk();
                threadsStruct.fileVector = fileVector;
                threadsStruct.Y = Y;

                for (int i = 0; i < theardCount; i++)
                {
                    threads.Add(new Thread(() =>
                        {
                            fornewthread(fileVector, Y, network);
                        }
                        ));
                    threads[i].Start();
                }

                listBox1.Items.Add("file");
                

                network.Train(fileVector, Y, 1 , 1e-7, 666); // запускаем обучение сети 
                progressBar1.Value = progressBar1.Value + 1;
                if (l == 200) break;

            }
            listBox1.Items.Add("Обучение закончено, записываем данные в файл");
            network.WriteToFile();
        }

        static object locker = new object();

        void fornewthread(Vector fileVector, Vector Y, Network network)
        {

            /*  if (obj.GetType() != typeof(TheardStruk))
                  return;
              TheardStruk tS = (TheardStruk)obj;*/

            network.Train(fileVector, Y, 1, 1e-7, 666); // запускаем обучение сети 

        }

        int jk = 0;
        private void button2_Click(object sender, EventArgs e)
        {

            Bitmap newImage2 = new Bitmap(@"D:\repos\neuro\train\" + fileName[jk]);

            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.Load(@"D:\repos\neuro\train\" + fileName[jk]);

            Vector fileVector3 = new Vector(784);

            for (int j = 0; j < newImage2.Width; j++)
                for (int i = 0; i < newImage2.Height; i++)
                {
                    fileVector3[j * newImage2.Width + i] = 0.00390625 * Convert.ToInt32(newImage2.GetPixel(i, i).R);
                }

            lock (locker)
            {
                Vector fileVector4 = network.Forward(fileVector3);

                listBox1.Items.Clear();

                listBox1.Items.Add(Convert.ToString(fileName[jk]));
                double sample = 0,
                    samplePos = 0;
                for (int j = 0; j < 10; j++)
                {
                    listBox1.Items.Add(j + ": " + fileVector4[j]);
                    if (fileVector4[j] > sample)
                    {
                        sample = fileVector4[j];
                        samplePos = j;
                    }
                }
                listBox1.Items.Add("Это: " + samplePos + " ? " + sample);
            }
            jk++;
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void загрузитьВесыИзФайлаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            network.ReadFromFile();
            listBox1.Items.Add("Параметры записаны в файл");
        }

        private void записатьВесыВФайлToolStripMenuItem_Click(object sender, EventArgs e)
        {
            network.WriteToFile();
            listBox1.Items.Add("Параметры загружены из файла");
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }


    class Vector
    {
        public double[] v; // значения вектора
        public int n; // длина вектора

        // конструктор из длины
        public Vector(int n)
        {
            this.n = n; // копируем длину
            v = new double[n]; // создаём массив
        }

        // создание вектора из вещественных значений
        public Vector(params double[] values)
        {
            n = values.Length;
            v = new double[n];

            for (int i = 0; i < n; i++)
                v[i] = values[i];
        }

        // обращение по индексу
        public double this[int i]
        {
            get { return v[i]; } // получение значение
            set { v[i] = value; } // изменение значения
        }
    }


    class Matrix
    {
        double[][] v; // значения матрицы
        public int n, m; // количество строк и столбцов

        // создание матрицы заданного размера и заполнение случайными числами из интервала (-0.5, 0.5)
        public Matrix(int n, int m, Random random)
        {
            this.n = n;
            this.m = m;

            v = new double[n][];

            for (int i = 0; i < n; i++)
            {
                v[i] = new double[m];

                for (int j = 0; j < m; j++)
                    v[i][j] = random.NextDouble() - 0.5; // заполняем случайными числами
            }
        }

        // обращение по индексу
        public double this[int i, int j]
        {
            get { return v[i][j]; } // получение значения
            set { v[i][j] = value; } // изменение значения
        }
    }

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

        public void ReadFromFile()
        {
            StreamReader sw = new StreamReader("scales.txt");
            //Запись слоев
            // sw.WriteLine(layersN);
            layersN = Convert.ToInt32(sw.ReadLine());

            //Запись весов
            for (int j = 0; j < weights.Length; j++)
            {
                for (int l = 0; l < weights[j].n; l++)
                {
                    for (int m = 0; m < weights[j].m; m++)
                    {
                        //  sw.WriteLine(weights[j][l, m]);
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
                    // sw.WriteLine(deltas[j][i]);
                }
            }

            sw.Close();
        }

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

                //  Console.WriteLine("epoch: {0}, error: {1}", epoch, error); // выводим в консоль номер эпохи и величину ошибку

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

                //  Console.WriteLine("epoch: {0}, error: {1}", epoch, error); // выводим в консоль номер эпохи и величину ошибку



                epoch++; // увеличиваем номер эпохи
            } while (epoch <= epochs && error > eps);
        }

    }
}


