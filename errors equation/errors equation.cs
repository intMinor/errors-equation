using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using shortest_path;
using shortest_path;

namespace errors_equation
{
    public class errors_equation
    {
        private struct point
        {
            public int index;
            public string name;
            public double Hexact;
            public double Hcirca;
        }

        private struct observation
        {
            public string starName;
            public string stopName;
            public double Height;
            public double length;
        }

        private struct polynomial
        {
            public int[] single;
            public double constant;
        }
            
        private point[] my_point;
        private observation[] my_observation;
        private polynomial[] my_polynomial;

        private int pathNum;
        private int pointNum;
        private int pointNum_K;

        private string[] pointName;
        private double[] ponintHknown;

        private string[] singlePath;
        private string[,] mixPath;

        private double[] singleDistance;
        private double[,] mixDistance;

        private int[] indexMix;
        private string[] pathMix;

        private short[,] singlePolynomial;
        private double[] constantPolynomial;

        private int[,] MatrixB;
        private double[,] MatrixL;
        private double[,] MatrixP;

        public errors_equation(int my_pathNum, int my_pointNum, int my_pointNum_K, string[] my_pointName, double[] my_ponintHknown)
        {
            pathNum = my_pathNum;
            pointNum = my_pointNum;
            pointNum_K = my_pointNum_K;

            my_point = new point[pointNum];
            my_observation = new observation[pathNum];
            my_polynomial = new polynomial[pathNum];

            singlePath = new string[pointNum];
            Array.Clear(singlePath, 0, my_pointNum);
            mixPath = new string[pointNum - pointNum_K, pointNum_K];

            singleDistance = new double[pointNum];
            Array.Clear(singleDistance, 0, my_pointNum);
            mixDistance = new double[pointNum - pointNum_K, pointNum_K];

            indexMix = new int[(pointNum - my_pointNum_K)];
            Array.Clear(indexMix, 0, pointNum - pointNum_K);
            pathMix = new string[(pointNum - pointNum_K)];
            Array.Clear(pathMix, 0, pointNum - pointNum_K);

            pointName = new string[pointNum];
            Array.Copy(my_pointName, pointName, 0);
            ponintHknown = new double[pointNum];
            Array.Copy(my_ponintHknown, ponintHknown, 0);

            singlePolynomial = new short[pathNum, pointNum - pointNum_K];
            constantPolynomial = new double[pathNum];
            Array.Clear(constantPolynomial, 0, pathNum);

            MatrixB = new int[pathNum, pointNum - pointNum_K];
            MatrixL = new double[1,pathNum];
            MatrixP = new double[pathNum, pathNum];

            for (int i = 0; i < pointNum; i++)
            {
                my_point[i].index = i;
                my_point[i].name = my_pointName[i];

                pointName[i] = my_pointName[i];

                my_polynomial[i].constant = 0.0;
                my_polynomial[i].single = new int[pointNum - pointNum_K];

                for (int j = 0; j < pointNum - pointNum_K; j++)
                {
                    my_polynomial[i].single[j] = 0;
                    singlePolynomial[i, j] = 0;
                }

                if (i < pointNum_K)
                {
                    ponintHknown[i] = my_ponintHknown[i];
                    my_point[i].Hexact = ponintHknown[i];
                    my_point[i].Hcirca = ponintHknown[i];
                }
                else
                {
                    my_point[i].Hexact = 0;
                    my_point[i].Hcirca = 0;
                }
            }
        }

        public void createObservation(int i, string starName, string stopName, double Height, double length)
        {
            my_observation[i].starName = starName;
            my_observation[i].stopName = stopName;
            my_observation[i].Height = Height;
            my_observation[i].length = length;
        }

        public void crateErrorsEquation()
        {
            shortest_path.shortest_path my_shortest_path = new shortest_path.shortest_path(pathNum, pointNum, pointNum_K, pointName);

            for (int i = 0; i < pathNum; i++)
            {
                my_shortest_path.createObservation(i, my_observation[i].starName, my_observation[i].stopName, my_observation[i].length);
            }

            my_shortest_path.createGraph();

            for (int i = 0; i < pointNum - pointNum_K; i++)
            {
                my_shortest_path.Dijkstra(i + pointNum_K);

                singlePath = my_shortest_path.getmixPath();
                singleDistance = my_shortest_path.getmixDistance();

                for (int j = 0; j < pointNum_K; j++)
                {
                    mixPath[i,j] = singlePath[j];
                    mixDistance[i,j] = singleDistance[j];

                    if (mixDistance[i, j] != 0.0)
                    {
                        Console.Write("mixPath[{0}]:{1}    ", j, mixPath[i, j]);
                        Console.Write("mixDistance[{0}]:{1}", j, mixDistance[i, j]);

                        Console.Write("\n");
                    }
                }
                Console.WriteLine("/////////////////////////////////////////////////////////////");
                Array.Clear(singlePath, 0, pointNum);
                Array.Clear(singleDistance, 0, pointNum);
            }

            Console.WriteLine("/////////////////////////////////////////////////////////////");

            for (int i = 0; i < pointNum - pointNum_K; i++)
            {
                double tempMix = mixDistance[i, 0];
                indexMix[i] = 0;
                if (pointNum_K != 1)
                {
                    for (int j = 0; j < pointNum_K - 1; j++)
                    {
                        if (mixDistance[i, j + 1] < tempMix)
                        {
                            indexMix[i] = j + 1;
                            tempMix = mixDistance[i, j + 1];
                        }
                    }
                }
                Console.WriteLine("indexMix[{0}]：{1}", i, indexMix[i]);
            }

            Console.WriteLine("/////////////////////////////////////////////////////////////");

            for (int i = 0; i < pointNum - pointNum_K; i++)
            {
                for (int j = 0; j < pointNum_K; j++)
                {
                    if (j == indexMix[i])
                        pathMix[i] = mixPath[i,j];
                }
                Console.WriteLine("pathMix[{0}]：{1}", i, pathMix[i]);
            }

            Console.WriteLine("/////////////////////////////////////////////////////////////");

            for (int i = 0; i < pointNum - pointNum_K; i++)
            {
                int[] digitPath = getDigitPath(pathMix[i]);

                for (int j = 0; j < digitPath.Length; j++)
                {
                    Console.WriteLine("digitPath[{0}]：{1}", j, digitPath[j]);
                }

                double Height = 0.0;
                my_point[i + pointNum_K].Hcirca = my_point[digitPath[digitPath.Length - 1]].Hexact;

                for (int j = digitPath.Length - 1; j > 0; j--)
                {
                    double singleHeight = 0.0;
                    for (int k = 0; k < pathNum; k++)
                    {
                        if (my_point[digitPath[j]].name == my_observation[k].starName && my_point[digitPath[j - 1]].name == my_observation[k].stopName)
                        {
                            singleHeight = my_observation[k].Height;
                            k = pathNum;
                        }
                        else if (my_point[digitPath[j]].name == my_observation[k].stopName && my_point[digitPath[j - 1]].name == my_observation[k].starName)
                        {
                            singleHeight = -(my_observation[k].Height);
                            k = pathNum;
                        }
                    }
                    Height += singleHeight;
                }

                my_point[i + pointNum_K].Hcirca += Height;

                Console.WriteLine("my_point[{0}]：{1}", i + pointNum_K, my_point[i + pointNum_K].Hcirca);

                Console.WriteLine("/////////////////////////////////////////////////////////////");
            }

            Console.WriteLine("/////////////////////////////////////////////////////////////");

            for (int i = 0; i < pathNum; i++)
            {
                int starIndex = Array.IndexOf(pointName, my_observation[i].starName);
                int stopIndex = Array.IndexOf(pointName, my_observation[i].stopName);

                for (int j = 0; j < pointNum - pointNum_K; j++)
                {
                    if (j + pointNum_K == starIndex || j + pointNum_K == stopIndex)
                    {
                        if (j + pointNum_K == starIndex)
                        {
                            singlePolynomial[i, j] = -1;
                            //my_polynomial[i].single[j] = -1;
                        }
                        else
                        {
                            singlePolynomial[i, j] = 1;
                            //my_polynomial[i].single[j] = 1;
                        }
                    }
                    //Console.WriteLine("my_polynomial[{0}].single[{1}]： {2}", i, j, my_polynomial[i].single[j]);
                    Console.WriteLine("singlePolynomial[{0}][{1}]： {2}", i, j, singlePolynomial[i, j]);
                }
                //my_polynomial[i].constant = -1000 * Math.Round((my_point[stopIndex].Hcirca - my_point[starIndex].Hcirca - my_observation[i].Height), 3);
                constantPolynomial[i] = -1000*(my_point[stopIndex].Hcirca - my_point[starIndex].Hcirca - my_observation[i].Height);

                //Console.WriteLine("my_polynomial[{0}].constant： {1}", i, my_polynomial[i].constant);
                Console.WriteLine("constantPolynomial[{0}]： {1}", i, constantPolynomial[i].ToString("0"));

                Console.WriteLine("/////////////////////////////////////////////////////////////");
            }

            Console.WriteLine("/////////////////////////////////////////////////////////////");

            for (int i = 0; i < pathNum; i++)
                for (int j = 0; j < pointNum - pointNum_K; j++)
                    MatrixB[i,j] = singlePolynomial[i, j];

            for (int i = 0; i < pathNum; i++)
                MatrixL[0,i] = constantPolynomial[i];

            for (int i = 0; i < pathNum; i++)
                for (int j = 0; j < pathNum; j++)
                {
                    if (j == i)
                    {
                        MatrixP[i,j] = 1.0/my_observation[i].length;
                    }
                    else
                        MatrixP[i,j] = 0.0;
                }

        }

        private int[] getDigitPath(string stringPath)
        {
            string[] strTemp = stringPath.Trim().Split(',');
            int l = strTemp.Length;
            int[] intPath = new int[l];
            for (int i = 0; i < l; i++)
            {
                intPath[i] = Array.IndexOf(pointName, strTemp[i]);
            }
            return intPath;
        }

        public int[,] getMatrixB()
        {
            return MatrixB;
        }

        public double[,] getMatrixL()
        {
            return MatrixL;
        }

        public double[,] getMatrixP()
        {
            return MatrixP;
        }
    }
}
