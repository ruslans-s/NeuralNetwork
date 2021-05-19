using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        //Создаем сеть с 784 входами, 16 скрытыми нейронами, и 10 выходами.
        Network network = new Network(new int[] { 784, 16, 10 });

        List<string> fileName = new List<string>();

        public Form1()
        {
            InitializeComponent();
            //Считываем веса
            network.ReadFromFile();

            //Создаем список обучаюших картинок
            DirectoryInfo dir = new DirectoryInfo(@"train\");
            foreach (FileInfo file in dir.GetFiles())
            {
                fileName.Add(file.Name);
            }
           // listBox1.Items.Add(fileName.Count);
        }


        private void button1_Click(object sender, EventArgs e)
        {

        }

        static object locker = new object();
        //Функция для обучения в многопоточности
        void fornewthread(Vector fileVector, Vector Y, Network network, int toch, int epoh)
        {
            network.Train(fileVector, Y, toch, 1e-7, epoh / theardCount); // запускаем обучение сети 
        }

        int jk = 0;
        //Обработка изображения для ответа
        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }
        //Загрузка весов
        private void загрузитьВесыИзФайлаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            network.ReadFromFile();
            listBox1.Items.Add("Параметры записаны в файл");
        }
        //Запись весов
        private void записатьВесыВФайлToolStripMenuItem_Click(object sender, EventArgs e)
        {
            network.WriteToFile();
            listBox1.Items.Add("Параметры загружены из файла");
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        int theardCount = 24; // Кол-во потоков + 1

        //Обучение
        private void обучениеToolStripMenuItem_Click(object sender, EventArgs e)
        {

            theardCount = Setting.txtBox2 - 1;

            StreamReader sw = new StreamReader("option.ini");
            //Загружаем параметры для обучания
            int startPosition = Convert.ToInt32(sw.ReadLine()); // Позиция в папке 3016

            int countTeachImg = Setting.txtBox2; //Кол-во изображений для обучения
            sw.Close();

            listBox1.Items.Add("Начинаем обучение, позиция: "+ startPosition);
            listBox1.Items.Add("циклов обучения: " + countTeachImg);
           
            //Прогресс бар для отслеживания 
            progressBar1.Maximum = countTeachImg+1;
            progressBar1.Value = 0;
            //Считаем время для статистики 
            Stopwatch st = new Stopwatch();
            st.Start();

            //Считываем веса из файла
            network.ReadFromFile();

            for (int l = startPosition; l < fileName.Count; l++)
            {
                //Загружаем картинку
                Bitmap newImage = new Bitmap(@"train\" + fileName[l]);

                //Создаем вектора с данными 
                Vector fileVector = new Vector(784);
                Vector Y = new Vector(10);

                //Обрабатываем картинк
                for (int j = 0; j < newImage.Width; j++)
                    for (int i = 0; i < newImage.Height; i++)
                    {
                        //Получаем цвет от 0 до 1 в градиенте белого 
                        fileVector[j * newImage.Width + i] = 0.00390625 * Convert.ToInt32(newImage.GetPixel(i, i).R);
                    }

                //Записываем данные в вектор ответа
                for(int r1=0; r1 < 10; r1++)
                {
                    Y[r1] = -1;
                }
                Y[Convert.ToInt32(Regex.Replace(Regex.Replace(fileName[l], ".*num", ""), ".png", ""))] = 1;


                List<Thread> threads = new List<Thread>(); // Список потоков

                //Запускаем потоки
                for (int i = 0; i < theardCount; i++)
                {
                    //Создаем поток и добавляем в него все данные
                    threads.Add(new Thread(() =>
                    {
                        fornewthread(fileVector, Y, network, 2, Setting.txtBox3);
                    }));
                    //Запускаем поток
                    threads[i].Start();
                }

                //Запускаем в главном потоке обучение
                network.Train(fileVector, Y, 2 , 1e-7, Setting.txtBox3 / theardCount); // запускаем обучение сети 

                //Увеличиваем прогресс бар
                progressBar1.Value = progressBar1.Value + 1;

                //Раннее завершение обучения
                if (l == startPosition + countTeachImg) break;

            }

            //Останавливаем таймер
            st.Stop();

            //Закончили 
            listBox1.Items.Add("Обучение закончено, записываем данные в файл");
            listBox1.Items.Add("Время выполнения: "+ st.Elapsed.TotalSeconds + " C.");

            //Записываем веса в файл
            network.WriteToFile();

            //Запись итоговой позиций в файл
            StreamWriter sR = new StreamWriter("option.ini");
            sR.WriteLine(startPosition+ countTeachImg);
            sR.Close();

        }

        private void тестированиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<string> testFileName = new List<string>();
            int[] numberSuccess = new int[10];
            int[] numberFailure = new int[10];

            DirectoryInfo dir = new DirectoryInfo(@"test\");
            foreach (FileInfo file in dir.GetFiles())
            {
                testFileName.Add(file.Name);
            }
            listBox1.Items.Add("Тестовых изображений: " + testFileName.Count);

            int success = 0,
                failure = 0;

            for (int tF = 0; tF < testFileName.Count; tF++)
            {
                Bitmap newImage = new Bitmap(@"test\" + testFileName[tF]);

                //Создаем вектора с данными 
                Vector fileVector = new Vector(784);
                Vector Y = new Vector(10);

                //Обработка изображения
                for (int j = 0; j < newImage.Width; j++)
                    for (int i = 0; i < newImage.Height; i++)
                    {
                        fileVector[j * newImage.Width + i] = 0.00390625 * Convert.ToInt32(newImage.GetPixel(i, i).R);
                    }

                //Прямой проход по сети
                Vector fileVector4 = network.Forward(fileVector);
               
                double sample = 0,
                    samplePos = 0;

                //Выводим ответ и ишем Max
                for (int j = 0; j < 10; j++)
                {
                    if (fileVector4[j] > sample)
                    {
                        sample = fileVector4[j];
                        samplePos = j;
                    }
                }
                //Пишем догадку
                if(samplePos != Convert.ToInt32(Regex.Replace(Regex.Replace(testFileName[tF], ".*num", ""), ".png", "")))
                {
                    failure++;
                    numberFailure[Convert.ToInt32(Convert.ToInt32(Regex.Replace(Regex.Replace(testFileName[tF], ".*num", ""), ".png", "")))]++;

                } else
                {
                    numberSuccess[Convert.ToInt32(Convert.ToInt32(Regex.Replace(Regex.Replace(testFileName[tF], ".*num", ""), ".png", "")))]++;
                    success++;
                }
            }

            listBox1.Items.Add("Успешных: " + success);
            listBox1.Items.Add("Неудач: " + failure);
            listBox1.Items.Add("Проецент верных: " + (success * 100 / testFileName.Count) + "%");
            for(int nS = 0; nS < 10; nS++)
            {
                listBox1.Items.Add(nS+": успешно" + numberSuccess[nS]+ " Неудач:" + numberFailure[nS]);
            }

           
        }

        private void загрузитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            //Загрузка изображения
            Bitmap newImage2 = new Bitmap(openFileDialog1.FileName);
            //Установка его в форму
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
         //   pictureBox1.Load(@"D:\repos\neuro\train\" + openFileDialog1.FileName);

            Vector fileVector3 = new Vector(784);
          
            //Обработка изображения
            for (int j = 0; j < newImage2.Width; j++)
                for (int i = 0; i < newImage2.Height; i++)
                {
                    fileVector3[j * newImage2.Width + i] = 0.00390625 * Convert.ToInt32(newImage2.GetPixel(i, i).R);
                }

            //Получаем ответ из нейросети 
            lock (locker)
            {
                //Прямой проход по сети
                Vector fileVector4 = network.Forward(fileVector3);
                //Очистка листбокса
                listBox1.Items.Clear();
                //Выводим имя файла
                listBox1.Items.Add(Convert.ToString(openFileDialog1.FileName));
                double sample = 0,
                    samplePos = 0;
                //Выводим ответ и ишем Max
                for (int j = 0; j < 10; j++)
                {
                    listBox1.Items.Add(j + ": " + fileVector4[j]);
                    if (fileVector4[j] > sample)
                    {
                        sample = fileVector4[j];
                        samplePos = j;
                    }
                }
                //Пишем догадку
                listBox1.Items.Add("Это: " + samplePos + " ? " + sample);
            }
        }

        Setting Setting = new Setting();

        private void параметрыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Setting.Show();
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


