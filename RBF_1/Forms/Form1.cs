using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;


namespace RBF_1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private static int countRow = 120;
        private static int numInput = 4;
        private static int numHidden;// = 3;
        private static int numOutput = 3;
        private static int countIter = 1000;
        private string filename = "iris_train.txt";

        private static Matrix weight; //= new Matrix(numHidden, numOutput);
        private static Matrix centroids;// = new Matrix(numHidden, numInput);        //Центры
        private static Matrix x;
        private static double[] y = new double[numOutput];
        private static Random rnd = new Random();
        private static double er;//= 0; // new double[countRow];      //105
        private static double errTest;
        double[,] D = new double[countRow, numOutput];
        private static double n;
        private static RBFNetwork rn;
        public static int temp;// = 0;

        public void SetC()
        {
            for (int i = 0; i < centroids.Row; i++)
            {
                for (int j = 0; j < centroids.Column; j++)
                {
                    centroids.Set(i, j, rnd.NextDouble());
                }
            }
        }

        private void button_Sample_Click(object sender, EventArgs e)
        {
            label_err.Text = "Learning error: ";
           
            label_Test.Text = "Testing error: ";

          
            n = Convert.ToDouble(textBox_n.Text);

            //Объем обучающей выборки
            countRow = Convert.ToInt32(textBox_X.Text);
            numHidden = Convert.ToInt32(textBox_C.Text);
            countIter = Convert.ToInt32(textBox1.Text);
         
            x = ReaderWriter.ReadMatrix(filename, countRow, numInput);
            double[] d = ReaderWriter.ReadVector(filename, countRow, numInput);
            D = new double[countRow, numOutput];
            RecD(d);

            //Ин центроид
            centroids = ReaderWriter.ReadMatrixC(filename, countRow, numInput, numHidden, d);
            Console.WriteLine(centroids);
            rn = new RBFNetwork(countRow, numInput, numHidden, numOutput, x, centroids, n, temp, er);
           // Console.WriteLine(rn);
            rn.countIter = countIter;
            //Ин весов
            rn.SetWeight();
           
            rn.TrainNetwork(x, centroids, countRow, numInput, numHidden, numOutput, D);
           // Console.WriteLine(rn);
            er = rn.errTrain;
            label_err.Text = "Learning error: " + er;
            temp = rn.temp;
          

          //  button_Sample.Enabled = false;
            button_test.Enabled = true;
        }

        void RecD(double[] d)
        {
            int numClass = 0;
            for (int i = 0; i < D.GetLength(0); i++)
            {
                numClass = Convert.ToInt32(d[i]);
                D[i, numClass] = 1;
            }
        }


        private void button_test_Click(object sender, EventArgs e)
        {
            label_Test.Text = "Testing error: ";

           // button_Sample.Enabled = true;
          //  button_test.Enabled = false;

            countRow = 30;

            // чтение входных данных
            x = ReaderWriter.ReadMatrix("iris_test.txt", countRow, numInput);

            double[] d = ReaderWriter.ReadVector("iris_test.txt", countRow, numInput);
            D = new double[countRow, numOutput];
            RecD(d);

            // Тестирование сети
            errTest = rn.TestNetwork(x, D, countRow, numInput, numOutput, numHidden);

            Matrix result = rn.result;
            listBox1.Items.Clear();
            for(int i=0; i<result.Row; i++)
            {
                String resultD = "";
                String resultY = "";
                for (int j=0; j<result.Column; j++)
                {
                    resultY += result.Get(i,j).ToString() + " ";
                    resultD += D[i, j].ToString() + " ";
                }
                listBox1.Items.Add(resultD + " -----------------------------------------------------------------------------------> " + resultY);
            }
            label_Test.Text = "Testing error: " + errTest;
        }

        private void textBox_n_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox_C_TextChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            double dou = 0;
            listBox1.Items.Clear();
            for (int i = 0; i < 10; i++)
            {
                //Коэффициент обучение сети
                n = Convert.ToDouble(textBox_n.Text);

                //Объем обучающей выборки
                countRow = Convert.ToInt32(textBox_X.Text);
                numHidden = Convert.ToInt32(textBox_C.Text);
                countIter = Convert.ToInt32(textBox1.Text);
                // чтение входных данных
                x = ReaderWriter.ReadMatrix(filename, countRow, numInput);
                double[] d = ReaderWriter.ReadVector(filename, countRow, numInput);
                D = new double[countRow, numOutput];
                RecD(d);

                //Инициализация центроид
                centroids = ReaderWriter.ReadMatrixC(filename, countRow, numInput, numHidden, d);

                rn = new RBFNetwork(countRow, numInput, numHidden, numOutput, x, centroids, n, temp, er);
                rn.countIter = countIter;
                //Инициализация весов
                rn.SetWeight();

                // Обучение сети
                rn.TrainNetwork(x, centroids, countRow, numInput, numHidden, numOutput, D);

                er = rn.errTrain;


                countRow = 30;
                // чтение входных данных
                x = ReaderWriter.ReadMatrix("iris_test.txt", countRow, numInput);
                d = ReaderWriter.ReadVector("iris_test.txt", countRow, numInput);
                D = new double[countRow, numOutput];
                RecD(d);

                // Тестирование сети
                errTest = rn.TestNetwork(x, D, countRow, numInput, numOutput, numHidden);
                dou += errTest;
                listBox1.Items.Add(errTest);
            }
            listBox1.Items.Add("Result: " + dou/10);
        }

        private void label_err_Click(object sender, EventArgs e)
        {

        }
    }
}


