using System;
using Nancy.Hosting.Self;

namespace ComicRackWebViewer
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                host.Start();
                Console.ReadLine();
            }
            finally
            {
                host.Stop();
            }
        }
    }
}
