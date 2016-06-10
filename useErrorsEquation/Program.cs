using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using errors_equation;
using System.IO;

namespace useErrorsEquation
{
    class Program
    {
        private struct observation
        {
            public string Be;
            public string En;
            public double H;
            public double S;
        }

        private static observation[] my_observation;

        private static string[] Pname;

        private static double[] HKnown;

        private static int[,] MatrixB;
        private static double[,] MatrixL;
        private static double[,] MatrixP;

        static void Main(string[] args)
        {
            Int16 Hn = 0;
            Int16 Un = 0;
            Int16 Kn = 0;

            string strData = String.Empty;
            string[] strTemp;
            int dataIdx = 0;

            FileStream aFile = new FileStream("示例数据2.txt", FileMode.Open);
            StreamReader streamReader = new StreamReader(aFile);

            while (streamReader.Peek() != -1)
            {
                dataIdx++;

                //"strData"存储剔除空格逐行读取文档的当前行所有字符，
                // "strTemp"以数组形式存储当前行，以逗号分隔
                strData = streamReader.ReadLine().Trim();
                strTemp = strData.Trim().Split(',');

                if (dataIdx == 1)
                {
                    Kn = Convert.ToInt16(strTemp[0]);
                    Un = Convert.ToInt16(strTemp[1]);
                    Hn = Convert.ToInt16(strTemp[2]);

                    Pname = new string[Kn + Un];
                    HKnown = new double[Kn];
                    my_observation = new observation[Hn];

                    MatrixB = new int[Hn, Un];
                    MatrixL = new double[1,Hn];
                    MatrixP = new double[Hn, Hn];

                }

                else if (dataIdx == 2)
                {
                    for (int i = 0; i < Kn + Un; i++)
                    {
                        Pname[i] = Convert.ToString(strTemp[i]);
                    }
                }

                else if (dataIdx == 3)
                {
                    for (int i = 0; i < Kn; i++)
                    {
                        HKnown[i] = Convert.ToDouble(strTemp[i]);
                    }
                }

                else
                {
                    my_observation[dataIdx - 4].Be = Convert.ToString(strTemp[0]);
                    my_observation[dataIdx - 4].En = Convert.ToString(strTemp[1]);
                    my_observation[dataIdx - 4].H = Convert.ToDouble(strTemp[2]);
                    my_observation[dataIdx - 4].S = Convert.ToDouble(strTemp[3]);
                }
            }

            Console.WriteLine(" 数据读入成功 ！！！ ");

            Console.WriteLine("/////////////////////////////////////////////////////////////");

            Console.WriteLine(" Hn = {0}, Kn = {1}, Un = {2} ", Hn, Kn, Un);

            Console.WriteLine("/////////////////////////////////////////////////////////////");

            for (int i = 0; i < Kn + Un; i++)
            {
                Console.WriteLine("第 {0} 点 ：{1}", i + 1, Pname[i]);
            }

            Console.WriteLine("/////////////////////////////////////////////////////////////");

            for (int i = 0; i < Kn; i++)
            {
                Console.WriteLine("第 {0} 点高程 ：{1}", i + 1, HKnown[i].ToString("0.000"));
            }

            Console.WriteLine("/////////////////////////////////////////////////////////////");

            for (int i = 0; i < Hn; i++)
            {
                Console.WriteLine("起点 ：{0} 终点 ：{1} 高差：{2} 距离 ：{3}", my_observation[i].Be, my_observation[i].En, my_observation[i].H.ToString("0.0000"), my_observation[i].S.ToString("0.0000"));
            }

            Console.WriteLine("/////////////////////////////////////////////////////////////");

            string[] singlePath = new string[Kn + Un];
            string[,] mixPath = new string[Kn + Un, Kn + Un];

            double[] singleDistance = new double[Kn + Un];
            double[,] mixDistance = new double[Kn + Un, Kn + Un];

            errors_equation.errors_equation my_errors_equation = new errors_equation.errors_equation(Hn, Kn + Un, Kn, Pname, HKnown);

            for (int i = 0; i < Hn; i++)
            {
                my_errors_equation.createObservation(i, my_observation[i].Be, my_observation[i].En, my_observation[i].H, my_observation[i].S);
            }

            my_errors_equation.crateErrorsEquation();

            MatrixB = my_errors_equation.getMatrixB();

            MatrixL = my_errors_equation.getMatrixL();

            MatrixP = my_errors_equation.getMatrixP();

            Console.WriteLine("/////////////////////////////////////////////////////////////");

            Console.WriteLine("MatrixB：");

            for (int i = 0; i < Hn; i++)
            {
                for (int j = 0; j < Un; j++)
                {
                    Console.Write("{0}     ", MatrixB[i,j]);
                    if (j == Un - 1)
                    {
                        Console.WriteLine("\n");
                    }
                }
            }

            Console.WriteLine("/////////////////////////////////////////////////////////////");

            Console.WriteLine("MatrixL：");

            for (int i = 0; i < Hn; i++)
            {
                Console.Write("{0}     ", MatrixL[0,i].ToString("0"));
            }

            Console.WriteLine("\n");

            Console.WriteLine("/////////////////////////////////////////////////////////////");

            Console.WriteLine("MatrixP：");

            for (int i = 0; i < Hn; i++)
            {
                for (int j = 0; j < Hn; j++)
                {
                    Console.Write("{0}     ", MatrixP[i, j].ToString("0.000"));
                    if (j == Hn - 1)
                    {
                        Console.WriteLine("\n");
                    }
                }
            }
            Console.WriteLine("/////////////////////////////////////////////////////////////");

            Console.ReadKey();
            Console.ReadKey();
            Console.ReadKey();
        }
    }
}
