using System;

namespace DotinSignSample
{
    class Program
    {
        static void Main(string[] args)
        {
           var sign = Utility.GenerateSign("json", "test", "test", "test", "test", "7c0b306bca793b5a9d4b5e92b8c87e967b");
        }
    }
}
