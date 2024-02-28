using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace TestSleep1ms
{
    internal class Program
    {
        private SemaphoreSlim _locker = new SemaphoreSlim(1, 1);
        static void Main(string[] args)
        {
            //_ = new Program().TestSleep();
            new Program().TestSpin();
            // new Program().TestSleep0();
            //_ = new Program().TestYeild();
            Console.WriteLine("Hello, World!");
            Console.ReadKey();
        }

        private async Task TestSleep()
        {
            var sw = new Stopwatch();
            sw.Start();
            while (true)
            {
                await _locker.WaitAsync();
                Console.WriteLine($"{sw.ElapsedTicks}");
                _locker.Release();
                sw.Restart();
            }

        }


        private async Task TestYeild()
        {
            var sw = new Stopwatch();
            sw.Start();
            while (true)
            {
                await Task.Delay(1);
                Console.WriteLine($"{sw.ElapsedTicks}");
                sw.Restart();
            }

        }

        private void TestSleep0()
        {
            Task.Run(() =>
            {
                var sw = new Stopwatch();
                sw.Start();
                while (true)
                {
                    Thread.Sleep(0);
                    Console.WriteLine($"{sw.ElapsedTicks}");
                    sw.Restart();
                }
            });
        }


        private void TestSpin()
        {
            Task.Run(() =>
            {

                var sw = new Stopwatch();
                    var sp = new SpinWait();
                sw.Start();
                while (true)
                {
                    //SpinWait.SpinUntil(() =>
                    //{
                    //    //Console.WriteLine($"执行输出{sw.ElapsedTicks}");
                    //    return sw.ElapsedMilliseconds == 2L;
                    //}, 1);
                    sp.SpinOnce();

                    //Thread.SpinWait(200);
                    //sp.SpinOnce();
                    //sp.SpinOnce();
                    Console.WriteLine($"执行中{sw.ElapsedTicks}");
                    if (sp.Count==int.MaxValue)
                    {
                        Console.WriteLine("!!!!!!!!!!!!!!");
                        Console.ReadKey();
                    }
                    //sp.Reset();
                    //Console.WriteLine($"自旋次数{sp.Count}");
                    //Console.WriteLine($"执行中{sw.ElapsedMilliseconds}");
                    //Console.WriteLine($"结果~~~~~~~{sw.ElapsedMilliseconds}");
                    sw.Restart();
                }
            });
        }
    }
}
