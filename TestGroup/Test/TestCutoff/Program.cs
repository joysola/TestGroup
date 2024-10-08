﻿namespace TestCutoff
{
    internal class Program
    {
        static List<double> _runList =
            [
            99.33,
            101.2,
            99.42
            ];
        static List<double> _runList2 =
            [
            91.03,
            88.2,
            87.53
            ];
        static List<double> _runList3 =
            [
            0,
            0,
            0
            ];
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            List<double> testList = [.._runList2];
            var count = testList.Count;
            var sum = testList.Sum();
            var sum_square = testList.Select(x => Math.Pow(x, 2)).Sum();

            var avg = testList.Average();
            var varianceList = testList.Select(x => Math.Pow(x - avg, 2)).ToList();
            var variance = varianceList.Sum();
            var std = Math.Sqrt(variance / (count - 1));

            var desireStd = 3.0;
            var b = -2 * sum;
            var a = count;
            var c = sum_square - (count - 1) * Math.Pow(desireStd, 2);

            var result1 = (-b + Math.Sqrt(Math.Pow(b, 2) - 4 * a * c)) / (2 * a);
            var result2 = (-b - Math.Sqrt(Math.Pow(b, 2) - 4 * a * c)) / (2 * a);


            var varianceList2 = testList.Select(x => Math.Pow(x - result1, 2)).ToList();
            var variance2 = varianceList2.Sum();
            var std2 = Math.Sqrt(variance2 / (count - 1));


            var response3 = 100.0;
            var varianceList3 = testList.Select(x => Math.Pow(x - avg, 2)).ToList();
            var variance3 = varianceList3.Sum();
            var std3 = Math.Sqrt(variance3 / (count - 1));
            Console.ReadKey();
        }
    }
}
