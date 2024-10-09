using MathNet.Filtering.Median;
using MathNet.Numerics.LinearAlgebra.Factorization;
using System.Drawing;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace TestMedianFilter
{
    internal class Program
    {

        static List<double> _ruList =
        [
            0,
          91.03,
          99.33,
          0.2742,
          -0.5723,
          -2.506,
          -9.504,
          0,
          88.25,
          101.2,
          111.6,
          87.85,
          101.9,
          169.4,
          0,
          87.53,
          99.42,
        ];

        static List<double> _baselines =
            [
                7289.6,
                7267.4,
                7248.6,
                7229.3,
                7210.6,
                7192,
                7173.8,
                7156.2,
                7139.1,
                7122.1,
                7105.6,
                7089.5,
                7073.7,
                7058.1,
                7018.6,
                7002.9,
                6989.4
            ];

        static List<double> _bindings =
            [
                7290.6,
                7303.1,
                7312,
                7230.7,
                7212,
                7192.7,
                7171.1,
                7158,
                7174.8,
                7188.1,
                7144.2,
                7125.2,
                7140.1,
                7236.8,
                7020.1,
                7038.4,
                7054.7
            ];

        static List<double> _ruList2 = [0,
17.23       ,
18.83       ,
0.05207     ,
-0.1088     ,
-0.4773     ,
-1.813      ,
0           ,
16.88       ,
19.4        ,
21.41       ,
16.88       ,
19.61       ,
32.65       ,
0           ,
16.95       ,
19.28       ,
];

        static List<double> _ruNotZeroList =
            [
20.92  ,
92.91  ,
99.47  ,
21.06  ,
20.37  ,
18.81  ,
13.24  ,
20.74  ,
90.69  ,
101    ,
109.2  ,
90.36  ,
101.5  ,
155.1  ,
20.55  ,
90.09  ,
99.54  ,



            ];
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            var window = 5;
            //var addPointsCount = window / 2;
            var addPointsCount = window;
            var testList = _ruNotZeroList;


            var filter = new OnlineMedianFilter(window);
            var length = testList.Count;

            List<double> newRUs8 = [.. testList.Where(x => x <= 106.2 && x >= 17)];
            for (int i = 0; i < addPointsCount; i++)
            {
                newRUs8.Insert(0, testList[0]);

            }
            //for (int i = 0; i < addPointsCount; i++)
            //{
            //    //newRUs8.Add(_ruList[^1]);
            //    newRUs8.Add(_ruList[0]);
            //}
            var result0 = filter.ProcessSamples(newRUs8.ToArray());

            var result1 = ApplyMedianFilter([..newRUs8], window);

            var xx = 0;
           


            Console.ReadKey();
        }
        static double[] MedianFilter(double[] input, int windowSize)
        {
            int halfWindowSize = windowSize / 2;
            double[] output = new double[input.Length];

            for (int i = 0; i < input.Length; i++)
            {
                // Define the window's bounds
                int start = Math.Max(0, i - halfWindowSize);
                int end = Math.Min(input.Length - 1, i + halfWindowSize);

                // Extract the window
                double[] window = new double[end - start + 1];
                Array.Copy(input, start, window, 0, window.Length);

                // Sort the window to find the median
                Array.Sort(window);
                output[i] = window[window.Length / 2];
            }

            return output;
        }

        public static double[] ApplyMedianFilter(double[] input, int windowSize)
        {
            //if (windowSize % 2 == 0)
            //    throw new ArgumentException("Window size must be odd.");

            int padding = windowSize / 2;
            double[] output = new double[input.Length];

            for (int i = 0; i < input.Length; i++)
            {
                // Create a window
                int start = Math.Max(0, i - padding);
                int end = Math.Min(input.Length - 1, i + padding);
                int length = end - start + 1;

                double[] window = new double[length];
                Array.Copy(input, start, window, 0, length);

                // Sort the window and find the median
                Array.Sort(window);
                output[i] = window[length / 2]; // length is guaranteed to be odd
            }

            return output;
        }

      





        static double[] MedianFilter2(double[] signal, int windowSize)
        {
            double[] result = new double[signal.Length];
            int halfWindow = windowSize / 2;

            for (int i = 0; i < signal.Length; i++)
            {
                // Create a temporary array for the current window
                int start = Math.Max(0, i - halfWindow);
                int end = Math.Min(signal.Length - 1, i + halfWindow);
                int[] window = new int[end - start + 1];

                Array.Copy(signal, start, window, 0, end - start + 1);
                Array.Sort(window); // Sort the window to find the median

                // Get the median value
                double median = window[window.Length / 2];
                result[i] = median;
            }

            return result;
        }

        /// <summary>
        /// 对矩阵M进行中值滤波
        /// </summary>
        /// <param name="m">矩阵M</param>
        /// <param name="windowRadius">过滤半径</param>
        /// <returns>结果矩阵</returns>
        private static double[,] MedianFilterFunction(double[,] m, int windowRadius)
        {
            int width = m.GetLength(0);
            int height = m.GetLength(1);

            double[,] lightArray = new double[width, height];

            //开始滤波
            for (int i = 0; i <= width - 1; i++)
            {
                for (int j = 0; j <= height - 1; j++)
                {
                    //得到过滤窗口矩形
                    Rectangle rectWindow = new Rectangle(i - windowRadius, j - windowRadius, 2 * windowRadius + 1, 2 * windowRadius + 1);
                    if (rectWindow.Left < 0) rectWindow.X = 0;
                    if (rectWindow.Top < 0) rectWindow.Y = 0;
                    if (rectWindow.Right > width - 1) rectWindow.Width = width - 1 - rectWindow.Left;
                    if (rectWindow.Bottom > height - 1) rectWindow.Height = height - 1 - rectWindow.Top;
                    //将窗口中的颜色取到列表中
                    List<double> windowPixelColorList = new List<double>();
                    for (int oi = rectWindow.Left; oi <= rectWindow.Right - 1; oi++)
                    {
                        for (int oj = rectWindow.Top; oj <= rectWindow.Bottom - 1; oj++)
                        {
                            windowPixelColorList.Add(m[oi, oj]);
                        }
                    }
                    //排序
                    windowPixelColorList.Sort();
                    //取中值
                    double middleValue = 0;
                    if ((windowRadius * windowRadius) % 2 == 0)
                    {
                        //如果是偶数
                        middleValue = Convert.ToDouble((windowPixelColorList[windowPixelColorList.Count / 2] + windowPixelColorList[windowPixelColorList.Count / 2 - 1]) / 2);
                    }
                    else
                    {
                        //如果是奇数
                        middleValue = windowPixelColorList[(windowPixelColorList.Count - 1) / 2];
                    }
                    //设置为中值
                    lightArray[i, j] = middleValue;
                }
            }
            return lightArray;
        }
    }
}
