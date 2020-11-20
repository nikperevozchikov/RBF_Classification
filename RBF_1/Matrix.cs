using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBF_1
{
    public class Matrix
    {
        double[,] array;
        int row, column;
        private static Random rnd = new Random();

        public int Row { get { return row; } }
        public int Column { get { return column; } }

        public Matrix(double[,] matrix)
        {
            array = matrix;
            row = matrix.GetLength(0);
            column = matrix.GetLength(1);
        }

        public Matrix(int row, int colunm)
        {
           this.row = row;
            this.column = colunm;
            array = new double[row, column];
        }
        public double Get(int i, int j)
        {
            return array[i, j];
        }
        public double Set(int i, int j, double value)
        {
            return array[i, j] = value;
        }

        public override string ToString()
        {
            string str = "";

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    str += array[i, j] + "\t";
                }
                str += "\n";
            }

            return str;
        }

        //public static Matrix operator -(Matrix m1, Matrix m2)
        //{
        //    if (m1.row != m2.row || m1.column != m2.column)
        //    {
        //        throw new Exception("Вычитание невозможно");
        //    }

        //    Matrix m = new Matrix(m1.row, m1.column);

        //    for (int i = 0; i < m1.row; i++)
        //    {
        //        for (int j = 0; j < m1.column; j++)
        //        {
        //            m.array[i, j] = m1.array[i, j] - m2.array[i, j];
        //        }
        //    }
        //    m.row = m1.row;
        //    m.column = m1.column;
        //    return m;
        //}

        //public static Matrix operator +(Matrix m1, Matrix m2)
        //{
        //    if (m1.row != m2.row || m1.column != m2.column)
        //    {
        //        throw new Exception("Сложение невозможно");
        //    }

        //    Matrix m = new Matrix(m1.row, m1.column);

        //    for (int i = 0; i < m1.row; i++)
        //    {
        //        for (int j = 0; j < m1.column; j++)
        //        {
        //            m.array[i, j] = m1.array[i, j] + m2.array[i, j];
        //        }
        //    }
        //    m.row = m1.row;
        //    m.column = m1.column;
        //    return m;
        }
    }


