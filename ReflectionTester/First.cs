using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReflectionTester
{
    public class First
    {
        private int first_a = 16101975;
        protected int first_b = 1975;

        private int First_A
        {
            get { return first_a; }
            set { first_a = value; }
        }

        public delegate void TestDelegate(string a, int b);
        public event TestDelegate OnTestDelegate;

        private string Method1(string a, int b)
        {
            this.first_b = b;
            Console.WriteLine("Method1(a = " + a + ", b = " + b.ToString() + ") is executed with the result: " + a);

            if (OnTestDelegate != null)
            {
                OnTestDelegate(a, b);
            }

            return a;
        }
    }

    public class Second : First
    {
        private int second_c = 16101975;
        protected int second_d = 1975;

        private int First_C
        {
            get { return second_c; }
            set { second_c = value; }
        }
    }
}
