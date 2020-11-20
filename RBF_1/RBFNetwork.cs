using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace RBF_1
{
    public class RBFNetwork
    {
        private int countRow;
        private int numInput;
        private int numHidden;
        private int numOutput;
        private Matrix weight;
        private double[] w0;
        private Matrix centroids;
        private Matrix x;      //Входые сигналы
        private Random rnd = new Random();
        private static double R;
        private double n;//= 0.01;
        private double errPr;
        private double errTest;
        private double[,] sigmoida;
        private List<int> neuronWin = new List<int>();
        public int temp;
        public double errTrain;
        public Matrix result;
        public int countIter;

        public RBFNetwork() { }

        public RBFNetwork(int countRow, int numInput, int numHidden, int numOutput, Matrix x, Matrix centroids, double n, int temp, double errTrain)
        {
            this.countRow = countRow;
            this.numInput = numInput;
            this.numOutput = numOutput;
            this.numHidden = numHidden;
            this.x = x; //105x4
            this.centroids = centroids; //3x4
            this.n = n;
            this.temp = temp;
            this.errTrain = errTrain;

            sigmoida = new double[numHidden, numInput];

            weight = new Matrix(numHidden, numOutput);
            SetWeight();

            w0 = new double[numOutput];

            //Инициализация весов w0
            for (int i = 0; i < w0.Length; i++)
            {
                w0[i] = rnd.NextDouble();
            }
        }

        public void SetWeight()
        {
            for (int i = 0; i < weight.Row; i++)
            {
                for (int j = 0; j < weight.Column; j++)
                {
                    weight.Set(i, j, rnd.NextDouble());
                }
            }
        }


        //public int NeuralWin(int numVector)
        //{
        //    Dictionary<int, double> num = new Dictionary<int, double>();
        //    double sum = 0;
        //    double evk = 0;
        //    int indexWin = 0;

        //    for (int i = 0; i < centroids.Row; i++) //3
        //    {
        //        sum = 0;
        //        for (int j = 0; j < centroids.Column; j++) //4
        //        {
        //            sum += Math.Pow((x.Get(numVector, j) - centroids.Get(i, j)), 2);
        //        }
        //        evk = Math.Sqrt(sum);
        //        num.Add(i, evk);

        //    }
        //    var l = num.OrderBy(value => value.Value);
        //    num = l.ToDictionary((keyItem) => keyItem.Key, (valueItem) => valueItem.Value);

        //    indexWin = num.ElementAt(0).Key;
        //    //проверяем не побеждал ли нейрон
        //    while (neuronWin.Contains(indexWin))
        //    {
        //        num.Remove(indexWin);
        //        indexWin = num.ElementAt(0).Key;
        //    }
        //    neuronWin.Add(indexWin);

        //    return indexWin;
        //}

        public Matrix SetCentroidsWin(Matrix x, Matrix centroids)
        {
            //Стабилизация сi(0)
            double cent = 0;
            double sum = 0;
            double evklid = 0;
            int winnerIdx = -1;
            List<Double> cWinner = new List<Double>();
            bool epoch = true;
            int t = 0;
            double error = 0;
            List<Double> errEvkC = new List<Double>();
            //int[] masVin = new int[numHidden];
       
      
            while (epoch)
            {
                for (int ti = 0; ti < x.Row; ti++) 
                {

                    cWinner.Clear();
                    for (int i = 0; i < centroids.Row; i++) //3
                    {
                        sum = 0;
                        for (int j = 0; j < centroids.Column; j++) //4
                        {
                            sum += Math.Pow((x.Get(ti, j) - centroids.Get(i, j)), 2);
                        }
                        cent = Math.Sqrt(sum);
                        cWinner.Add(cent);

                    }
                    winnerIdx = cWinner.IndexOf(cWinner.Min());

                    evklid = 0;
                    error = 0;
                    for (int i = 0; i < centroids.Row; i++) //3
                    {
                        sum = 0;
                     
                        for (int j = 0; j < centroids.Column; j++) //4 13
                        {
                            if (i == winnerIdx)
                            {
                                centroids.Set(winnerIdx, j, centroids.Get(winnerIdx, j) + n * (x.Get(ti, j) - centroids.Get(winnerIdx, j)));

                                //Для центроиды победителя
                                //считаем евклидову норму
                                sum += Math.Pow((x.Get(ti, j) - centroids.Get(winnerIdx, j)), 2);
                            }
                        }

                        if (i == winnerIdx)
                        {
                            evklid = Math.Sqrt(sum);
                            error += evklid;

                        }
                    }
                }

                errEvkC.Add(0.5 * error);

                if (errEvkC.Count() > 1)
                {
                    if (Math.Round(errEvkC.ElementAt(errEvkC.Count - 1), 5) == Math.Round(errEvkC.ElementAt(errEvkC.Count - 2), 5))
                    {
                        epoch = false;
                    }
                }
             
                t++;
            }
            return centroids;
        }

        public void Sigmoida(Matrix centroids)
        {
            double[] m = new double[numInput];
            double temp = 0;

            for (int i = 0; i < centroids.Row; i++) //3
            {
                for (int j = 0; j < centroids.Column; j++) //4
                {
                    temp = 0;
                    for (int k = 0; k < centroids.Row; k++) //3
                    {
                        if (i != k)
                        {
                            temp += Math.Pow(centroids.Get(i, j) - centroids.Get(k, j), 2);
                        }
                    }
                    sigmoida[i,j] = Math.Sqrt((1 / R) * temp);
                }
               
            }
        }

        public void NOutputRBF(Matrix y)
        {
            int indexNeuronWin = -1;

            for (int i = 0; i < y.Row; i++)//105
            {
                double max = -1;
                //находим победителя
                for (int j = 0; j < y.Column; j++)//3
                {
                    if (max <= y.Get(i, j))
                    {
                        max = y.Get(i, j);
                        indexNeuronWin = j;
                    }
                }
                //корректируем выходы
                for (int j = 0; j < y.Column; j++)//3
                {
                    if (j != indexNeuronWin)
                    {
                        y.Set(i, j, 0);
                      
                    }
                    else y.Set(i, indexNeuronWin, 1);
                }
            }
        }

        public RBFNetwork TrainNetwork(Matrix x, Matrix centroids, int countRow, int numInput, int numHidden, int numOutput, double[,] d)
        {
            R = centroids.Row - 1;
            List<double> sko = new List<double>();
            double e = 0;
            double eSum = 0;
            double[] error = new double[countRow]; //105
            double[,] newX = new double[countRow, numOutput];
            double[,] newC = new double[centroids.Row, centroids.Column];
            double[,] newSi = new double[centroids.Row, centroids.Column];
            double m = numOutput;
            double p = countRow;
            bool flg = true;

            double sum = 0;

            double[,] gradient = new double[numHidden, numOutput];
            int t = 0;
            //k-means

            SetCentroidsWin(x, centroids);

            Sigmoida(centroids);

            double[,] u = new double[countRow, numHidden];  //3
            double[,] f = new double[countRow, numHidden];  //3 
            Matrix y = new Matrix(countRow, numOutput); //3
            double temp1 = 0;
            int r = countIter;

            while (r!=0)
            {
                 r--;
                eSum = 0;
                //Обучение сети
                for (int ti = 0; ti < x.Row; ti++) 
                {
                    for (int q = 0; q < y.Column; q++)
                    {
                        e = 0;
                        for (int i = 0; i < centroids.Row; i++) //3
                        {
                            sum = 0;
                            for (int j = 0; j < centroids.Column; j++) //4
                            {
                                sum += Math.Pow(x.Get(ti, j) - centroids.Get(i, j), 2)/ (Math.Pow(sigmoida[i,j], 2));

                                //newC[i,j] = x.Get(ti, j) - centroids.Get(i, j)/ (Math.Pow(sigmoida[i, j], 2));
                                //newSi[i, j] = x.Get(ti, j) - centroids.Get(i, j) / (Math.Pow(sigmoida[i, j], 3));

                            }

                            u[ti, i] = sum ; 

                            f[ti, i] = Math.Exp(-0.5 * u[ti, i]);
                            temp1 = 0;
                        }
                        for (int w = 0; w < weight.Column; w++)
                        {
                            temp1 += weight.Get(q, w) * f[ti, q];
                        }
                        //Console.WriteLine(temp1);
                        //выход
                        y.Set(ti, q, temp1);

                        //разность
                        newX[ti, q] = y.Get(ti, q) - d[ti, q];
                        
                        //for (int i = 0; i < centroids.Row; i++) //3
                        //{
                        //    for (int j = 0; j < centroids.Column; j++) //4
                        //    {

                        //        centroids.Set(i, j, centroids.Get(i, j) - n * newX[ti, q] * f[ti, q] * weight.Get(i, q) * (x.Get(ti, j) - centroids.Get(i, j) / (Math.Pow(sigmoida[i, j], 2))));
                        //        //    sigmoida[i, j] = sigmoida[i, j] - n * newX[ti, q] * f[ti, q] * weight.Get(i, q) * ((x.Get(ti, j) - centroids.Get(i, j) / (Math.Pow(sigmoida[i, j], 3))));

                        //    }
                        //}


                        for (int k = 0; k < weight.Column; k++)
                        {   
                            gradient[k, q] = newX[ti, q] * f[ti, q];
                            //пересчитываем вес
                            weight.Set(k, q, weight.Get(k, q) - n * gradient[k, q]);
                        }
                        //Console.WriteLine(weight);
                        e += Math.Pow(y.Get(ti, q) - d[ti, q], 2);
                    }
                    eSum += e;
                }
                sko.Add(Math.Sqrt((eSum / (m * p - 1))));
                //Console.WriteLine(sko.ElementAt(sko.Count - 1));
                t++;
            }
            NOutputRBF(y);
            
            for (int y1 = 0; y1 < y.Row; y1++)
            {
                for (int y2 = 0; y2 < y.Column; y2++)
                {
                    Console.Write(y.Get(y1, y2) + " ");
                }

               Console.WriteLine(t);

            }

            for (int y2 = 0; y2 < sko.Count(); y2++)
            {
                errPr = sko.ElementAt(y2);
                Console.WriteLine(errPr);

            }

            Console.WriteLine(sko.ElementAt(sko.Count - 1));

            temp = t;
            errTrain = Math.Round(errPr, 10);
            RBFNetwork nr = new RBFNetwork(countRow, numInput, numHidden, numOutput, x, centroids, n, temp, errTrain);

            return nr;
        }


        public double TestNetwork(Matrix x, double[,] d, int countRow, int numInput, int numOutput, int numHidden)
        {

            List<double> sko = new List<double>();
            double[] error = new double[countRow]; //45
            double[,] newX = new double[countRow, numOutput];
            double m = numOutput;
            double p = countRow;

            double sum = 0;

            double[,] gradient = new double[numOutput, numHidden];
            double[,] u = new double[countRow, numHidden];  //3
            double[,] f = new double[countRow, numHidden];  //3 
            Matrix y = new Matrix(countRow, numOutput); //3
            double temp = 0;

            //Обучение сети
            for (int ti = 0; ti < x.Row; ti++) //45
            {

                for (int q = 0; q < y.Column; q++)
                {
                    for (int i = 0; i < centroids.Row; i++) //3
                    {
                        sum = 0;

                        for (int j = 0; j < centroids.Column; j++) //4
                        {
                            sum += Math.Pow(x.Get(ti, j) - centroids.Get(i, j), 2) / (Math.Pow(sigmoida[i,j], 2));
                        }

                        u[ti, i] = sum; 

                        f[ti, i] = Math.Exp(-0.5 * u[ti, i]);
                    }
                    temp = 0;
                    for (int w = 0; w < weight.Column; w++)
                    {
                        temp += weight.Get(q, w) * f[ti, q];

                    }
                    //Выход
                    y.Set(ti, q, temp);
                    
                }
            }

            NOutputRBF(y);

            PrintOutput(y);


            errTest = ErrorTesting(y, d);
            result = y;
            return errTest;
        }

        public void PrintOutput(Matrix y)
        {
            StreamWriter str = new StreamWriter("y.txt");
            str.WriteLine(y);
            str.Close();
        }
        public double ErrorTesting(Matrix y, double[,] d)
        {
            double countErr = 0;
            double countRight = 0;

            //Ошибка тестирования сети
            for (int i = 0; i < y.Row; i++) //45
            {
                for (int j = 0; j < y.Column; j++) //3
                {
                    if (y.Get(i, j) != (d[i, j]))
                    {
                        countErr++;
                    }
                    else countRight++;
                }
            }

            return errTest = Math.Round((countErr / countRight), 7);
        }
    }
}


