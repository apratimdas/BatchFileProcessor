using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BatchFramework;


namespace TotalTextFilesLength
{
    class Program
    {
        static long sTotalTextFilesLength = 0;
        static void testNotify(string msg)
        {
            Console.WriteLine(msg);
        }
        static void AccumulateLength(IFileAccessLogic lo,  FileInfo fi)
        {
            sTotalTextFilesLength += fi.Length;
        }

        static void Main(string[] args)
        {
            //if(args.Length < 1)
            //{
            //    Console.WriteLine("Missing file or folder path:" + 
            //        "\nUsage: TotalTextFilesLength filepath\\folderPath");
            //    return;
            //}
            // string directoryPath = args[0];
            string directoryPath = ChooseDirectory();
            FileAccessLogic logic = new FileAccessLogic();
            logic.Recursive = true;
            logic.FilePattern = "*.txt";
            logic.onProcess += new FileAccessProcessEventHandler(OnProcessSimpleList);
            logic.onNotify += new FileAccessNotifyEventHandler(OnNotify);

            Console.WriteLine("");
            Console.WriteLine("Processing file or folder " + directoryPath);
            Console.WriteLine("Press any key to start:");
            Console.ReadKey();
            Console.WriteLine("************************************");
            logic.Execute(directoryPath);
            Console.WriteLine("Total length of all text files is {0} bytes", sTotalTextFilesLength);
            Console.WriteLine("************************************");
            Console.WriteLine("");
            Console.ReadKey();

        }

        private static void OnProcessSimpleList(object sender,
            ProcessEventArgs e)
        {
            if(e.Logic.Cancelled)
                return;
            e.Logic.Notify(String.Format("Listing \t{0} \t{1} \t{2}", String.Format("{0,-50}",e.FileInfo.FullName) , e.FileInfo.Length + " bytes",File.GetLastWriteTime(e.FileInfo.FullName)));
            AccumulateLength(e.Logic, e.FileInfo);
        }
        private static void OnNotify(object sedner, NotifyEventArgs e)
        {
            //Console.WriteLine(e.Message);
            testNotify(e.Message);
        }

        private static string ChooseDirectory()
        {
            bool validChoice = false;
            string directoryChoice = "";
            
            while(validChoice == false)
            {
                Console.WriteLine("Please specify top level directory in full:");
                directoryChoice = Console.ReadLine().Trim();
                if (Directory.Exists(directoryChoice))
                    validChoice = true;
                else
                {
                    Console.WriteLine("Invalid directory. Try again!");
                    Console.WriteLine("");
                }
            }

            return directoryChoice;
        }
    }
}
