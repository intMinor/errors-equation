using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shortest_path
{
    public class shortest_path
    {
        private const double INFINITY = 999999.00;               //定义无限远

        private string[] mixPath;

        private string singlePath = "";

        private double[] mixDistance;

        private bool[] tagPoint;

        private struct graph
        {
            public int pathNum;
            public int pointAllNum;
            public int pointKnownNum;
            public string[] pointName;
            public double[,] pathDisdance;
        }

        private struct observation
        {
            public string starName;
            public string stopName;
            public double length;
        }

        private graph my_graph;

        private observation[] my_observation;

        //构造函数
        public shortest_path(int pathNum, int pointAllNum, int pointKnownNum, string[] Pname)
        {
            mixPath = new string[pointAllNum];
            Array.Clear(mixPath, 0, pointAllNum);

            mixDistance = new double[pointAllNum];
            Array.Clear(mixDistance, 0, pointAllNum);

            tagPoint = new bool[pointAllNum];
            Array.Clear(tagPoint, 0, pointAllNum);

            my_graph.pathNum = pathNum;
            my_graph.pointAllNum = pointAllNum;
            my_graph.pointKnownNum = pointKnownNum;
            my_graph.pointName = new string[pointAllNum];
            my_graph.pathDisdance = new double[pointAllNum, pointAllNum];
            Array.Copy(Pname, my_graph.pointName, pointAllNum);

            my_observation = new observation[pathNum];
            Array.Clear(my_observation, 0, pathNum);

            for (int i = 0; i < pointAllNum; i++)
                for (int j = 0; j < pointAllNum; j++)
                    if (i == j)
                        my_graph.pathDisdance[i, j] = 0.0;
                    else
                        my_graph.pathDisdance[i, j] = INFINITY;
        }

        public void createObservation(int i, string starName, string stopName, double length)
        {
            my_observation[i].starName = starName;
            my_observation[i].stopName = stopName;
            my_observation[i].length = length;
        }

        public void createGraph()
        {
            int starIndex = 0;
            int stopIndex = 0;

            for (int i = 0; i < my_graph.pathNum; i++)
            {
                starIndex = Array.IndexOf(my_graph.pointName, my_observation[i].starName);
                stopIndex = Array.IndexOf(my_graph.pointName, my_observation[i].stopName);

                my_graph.pathDisdance[starIndex, stopIndex] = my_observation[i].length;
                my_graph.pathDisdance[stopIndex, starIndex] = my_observation[i].length;
            }
        }

        public void Dijkstra(int currentIndex)
        {
            int[] tempPath = new int[my_graph.pointAllNum];
            Array.Clear(tempPath, 0, my_graph.pointAllNum);

            double tempMix;
            int Transit;

            for (int i = 0; i < my_graph.pointAllNum; i++)
            {
                mixDistance[i] = my_graph.pathDisdance[currentIndex,i];

                tagPoint[i] = false;

                if (my_graph.pathDisdance[currentIndex, i] < INFINITY)
                    tempPath[i] = currentIndex;
                else
                    tempPath[i] = -1;
            }

            tagPoint[currentIndex] = true;
            tempPath[currentIndex] = 0;

            for (int i = 0; i < my_graph.pointAllNum - 1; i++)
            {
                tempMix = INFINITY;
                Transit = -1;

                for (int j = 0; j < my_graph.pointAllNum; j++)
                {
                    if (tagPoint[j] == false && mixDistance[j] < tempMix)
                    {
                        Transit = j;
                        tempMix = mixDistance[j];
                    }
                }

                tagPoint[Transit] = true;

                for (int j = 0; j < my_graph.pointAllNum; j++)
                {
                    if (tagPoint[j] == false)
                    {
                        if (my_graph.pathDisdance[Transit, j] < INFINITY && mixDistance[Transit] + my_graph.pathDisdance[Transit, j] < mixDistance[j])
                        {
                            mixDistance[j] = mixDistance[Transit] + my_graph.pathDisdance[Transit, j];
                            tempPath[j] = Transit;
                        }
                    }
                }
            }
            tidyPath(tempPath, currentIndex);
        }

        private string[] tidyPath (int[] tempPath, int currentIndex)
        {
            for (int i = 0; i < my_graph.pointAllNum; i++)
            {
                if (i != currentIndex)
                {
                    if (tagPoint[i] == true && mixDistance[i] < INFINITY)
                    {
                        recursive(tempPath, i, currentIndex);
                        mixPath[i] += my_graph.pointName[currentIndex];
                        mixPath[i] += ",";
                        mixPath[i] += singlePath;
                        mixPath[i] += my_graph.pointName[i];
                        singlePath = "";
                    }
                    else
                    {
                        mixPath[i] = "";
                    }
                }
            }

            Array.Clear(tempPath, 0, my_graph.pointAllNum);

            return mixPath;
        }

        private void recursive(int[] tempPath, int i, int currentIndex)
        {
            if (tempPath[i] == currentIndex)
                return;
            recursive(tempPath, tempPath[i], currentIndex);
            singlePath += my_graph.pointName[tempPath[i]];
            singlePath += ",";
        }

        public string[] getmixPath()
        {
            return mixPath;
        }

        public double[] getmixDistance()
        {
            return mixDistance;
        }
    }
}
