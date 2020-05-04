using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FileCopyMultithreading;

namespace MultithreadCopy
{
    class Program
    {
        public static void Main(string[] args)
        {
            FileCopyMultithreaded fcm = new FileCopyMultithreaded();

            string sourceDirectory = @"D:\sourceDirectory";
            string targetDirectory = @"D:\targetDirectory";

            if (!Directory.Exists(sourceDirectory))
            {
                Console.WriteLine("The specified source directory does not exist!");
                Environment.Exit(-1);
            }
            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }

            foreach (string sourceFile in Directory.GetFiles(sourceDirectory))
            {
                string targetFile = Path.Combine(targetDirectory, Path.GetFileName(sourceFile));
                fcm.EnqueueFiles(new KeyValuePair<string, string>(sourceFile, targetFile));
            }

            
            if (fcm != null) 
                fcm.StartCopy();


            Console.WriteLine( "Press enter to end");
            Console.ReadKey(); // We need to wait for threads to finish their copying.
        }
    }
}
