using System;
using System.Threading;

namespace Random_Generator
{
    class Random_Generator
    {
        static void Main(string[] args)
        {
            Console.WriteLine(" <enter> to quit ");
            new RandomGenerator().Run();
            string response = Console.ReadLine();

        }

        public class RandomGenerator
        {

            public void Run()
            {
                /* open memory window and show how ints/uints are stored lsb ... msb on the stack */
                /* ints and other primitives go on the stack; strings and objects go on the heap */
                int x1 = 1; uint x2 = 1; int x3 = -1; int x4 = 2;
                int y1 = x1 >> 8; int y2 = x1 << 8;
                /*
                In memory
                x1 -> 01 00 00 00;      x2 -> 01 00 00 00     x3 -> ff ff ff ff; 
                y1 -> 00 00 00 00;      y2 -> 00 01 00 00 = 256
                */

                // DateTime.Now.Ticks returns a long ( signed 64-bit )
                // One tick represents one hundred nanoseconds or one ten-millionth of a second
                // It takes 10 ticks to make up a millisecond.
                // Random(int seed) takes a 32-bit signed int
                // c# max int is constant 2,147,483,647 

                long a1 = System.DateTime.Now.Ticks; long a2 = System.DateTime.Now.Ticks;
                Console.WriteLine("(long)Ticks = " + a1 + "  /  " + a2);
                Console.WriteLine("  Notice that the difference in vals is somewhere in the tens/hundreds of msec positions. ");
                Console.WriteLine();

                int b1 = (int)System.DateTime.Now.Ticks; int b2 = (int)System.DateTime.Now.Ticks;
                Console.WriteLine("(int)Ticks = " + b1 + "  /  " + b2);
                Console.WriteLine("(uint)Ticks = " + (uint)b1 + "  /  " + (uint)b2);
                Console.WriteLine(" Notice that the cast to returns the same seed val despite two different tick counts.");
                Console.WriteLine();

                int c1 = (int)System.DateTime.Now.Ticks; Thread.Sleep(1); int c2 = (int)System.DateTime.Now.Ticks;
                Console.WriteLine("(int)Ticks (sleep 1) = " + c1 + "  /  " + c2);
                Console.WriteLine("  Waiting one sec btwn gettting the seed val provides different seeds.  ");
                Console.WriteLine();

                /*
                (long)Ticks = 636142976***1572***54525  /  636142976***1632***54525
                (int)Ticks = 1965982141  /  1973642141
                (uint)Ticks = 1965982141  /  1973642141
                (int)Ticks (sleep 1) = -2129375155  /  -2129365155                
                */

                /*
                (int)Ticks = -1397562247  /  -1397562247
                (uint)Ticks = 2897405049  /  2897405049
                */

                Single m1 = ((Single)(new Random((int)System.DateTime.Now.Ticks)).NextDouble());
                Thread.Sleep(1);
                Single m2 = ((Single)(new Random((int)System.DateTime.Now.Ticks)).NextDouble());
                Console.WriteLine(m1 + " sleep(1) /  " + m2);
                Console.WriteLine("  With the sleep statement, we get different random number.  ");
                Console.WriteLine();


                m1 = ((Single)(new Random((int)System.DateTime.Now.Ticks)).NextDouble());
                m2 = ((Single)(new Random((int)System.DateTime.Now.Ticks)).NextDouble());
                Console.WriteLine(m1 + " /  " + m2);
                Console.WriteLine("  Without the sleep statement, we get the same 'random number'.  ");
                Console.WriteLine();

            }   // public void Run()

        }   // public class RandomGenerator

    }   // class Random_Generator
}   // namespace Random_Generator
