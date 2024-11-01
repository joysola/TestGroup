using System;

class Program
{
    static void Main(string[] args)
    {
        Foo foo = null;
        Foo foo2 = new Foo();
        var n = 2 + foo?.N + foo2.N ?? 1;
        Console.WriteLine(n);
        //Foo foo = null;
        //int n = Lindexi() + foo?.N ?? 1;
        //Console.WriteLine(n);
    }

    private static int Lindexi()
    {
        Console.WriteLine("林德熙是");
        return 2;
    }

    class Foo
    {
        public int N
        {
            get
            {
                Console.WriteLine("Hi.");
                return 1;
            }
        }
    }
}

//class Foo
//{
//    public int N { get; } = 1;
//}




