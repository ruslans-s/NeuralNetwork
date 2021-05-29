using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
        Network network = new Network(new int[] { 784, 30, 10 });
        //  NetworkGPU network = new NetworkGPU(new int[] { 784, 16, 10 });

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
        /*   void fornewthread(Vector fileVector, Vector Y, Network network, int toch, int epoh)
           {
               network.Train(fileVector, Y, toch, 1e-7, epoh / theardCount); // запускаем обучение сети 
           }
           void fornewthread(Vector[] fileVector, Vector[] Y, Network network, double toch, int epoh)
           {
               network.Train(fileVector, Y, toch, 1e-7, epoh); // запускаем обучение сети 

           }*/

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
            //teachInt();
            teach();
            /*
            theardCount = Setting.txtBox1 - 1;

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
            sR.Close();*/

        }
        void teach()
        {
            theardCount = Setting.txtBox1 - 1;

            StreamReader sw = new StreamReader("option.ini");
            //Загружаем параметры для обучания
            int startPosition = Convert.ToInt32(sw.ReadLine()); // Позиция в папке 3016
            int countTeachImg = Setting.txtBox2; //Кол-во изображений для обучения
            sw.Close();


            //Считаем время для статистики 
            Stopwatch st = new Stopwatch();
            st.Start();

            //Считываем веса из файла
            network.ReadFromFile();

            Vector[] fileVector = new Vector[fileName.Count];
            Vector[] Y = new Vector[fileName.Count];
            
            
            for (int l = 0; l < fileName.Count; l++)
            {
                //Загружаем картинку
                Bitmap newImage = new Bitmap(@"train\" + fileName[l]);

                //Создаем вектора с данными 
                fileVector[l] = new Vector(784);
                Y[l] = new Vector(10);

                //Обрабатываем картинк
                  for (int j = 0; j < newImage.Height; j++)
                      for (int i = 0; i < newImage.Width; i++)
                      {
                          //Получаем цвет от 0 до 1 в градиенте белого 
                          fileVector[l][j * newImage.Height + i] = (float)(0.00390625 * ((Convert.ToInt32(newImage.GetPixel(i, i).R) + Convert.ToInt32(newImage.GetPixel(i, i).G) + Convert.ToInt32(newImage.GetPixel(i, i).B)) / 3));
                      }

                //Записываем данные в вектор ответа
                for (int r1 = 0; r1 < 10; r1++)
                {
                    Y[l][r1] = 0;
                }
                Y[l][Convert.ToInt32(Regex.Replace(Regex.Replace(fileName[l], ".*num", ""), ".png", ""))] = 1.0f;

            }

        
            //Запускаем в главном потоке обучение
            network.Train(fileVector, Y, 0.33f, 1e-20, Setting.txtBox3); // запускаем обучение сети 

            //Останавливаем таймер
            st.Stop();

            //Закончили 
            listBox1.Items.Add("Обучение закончено, записываем данные в файл");
            listBox1.Items.Add("Время выполнения: " + st.Elapsed.TotalSeconds + " C.");

            //Записываем веса в файл
            network.WriteToFile();

            //Запись итоговой позиций в файл
            StreamWriter sR = new StreamWriter("option.ini");
            sR.WriteLine(startPosition + countTeachImg);
            sR.Close();

        }

        public void test()
        {
            List<string> testFileName = new List<string>();
            int[] numberSuccess = new int[10];
            int[] numberFailure = new int[10];

        //    DirectoryInfo dir = new DirectoryInfo(@"train\");

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
                for (int j = 0; j < newImage.Height; j++)
                    for (int i = 0; i < newImage.Width; i++)
                    {
                        fileVector[j * newImage.Height + i] = (float)(0.00390625 * ((Convert.ToInt32(newImage.GetPixel(i, i).R) + Convert.ToInt32(newImage.GetPixel(i, i).G) + Convert.ToInt32(newImage.GetPixel(i, i).B)) / 3));
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
                if (samplePos != Convert.ToInt32(Regex.Replace(Regex.Replace(testFileName[tF], ".*num", ""), ".png", "")))
                {
                    failure++;
                    numberFailure[Convert.ToInt32(Convert.ToInt32(Regex.Replace(Regex.Replace(testFileName[tF], ".*num", ""), ".png", "")))]++;

                }
                else
                {
                    numberSuccess[Convert.ToInt32(Convert.ToInt32(Regex.Replace(Regex.Replace(testFileName[tF], ".*num", ""), ".png", "")))]++;
                    success++;
                }



            }

            listBox1.Items.Add("Успешных: " + success);
            listBox1.Items.Add("Неудач: " + failure);
            listBox1.Items.Add("Проецент верных: " + (success * 100 / testFileName.Count) + "%");
            for (int nS = 0; nS < 10; nS++)
            {
                listBox1.Items.Add(nS + ": успешно" + numberSuccess[nS] + " Неудач:" + numberFailure[nS]);
            }

        }
        private void тестированиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            test();
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
                    fileVector3[j * newImage2.Width + i] = (float)(0.00390625 * Convert.ToInt32(newImage2.GetPixel(i, i).R));
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


}


