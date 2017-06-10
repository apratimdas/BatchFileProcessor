
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
        public static ProcessEventArgs temp = null;
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
            
            string directoryPath = Directory.GetCurrentDirectory();
            Console.WriteLine("Enter file formats space separated. Example( txt log exe )");
            FileAccessLogic logic = new FileAccessLogic();
            string userpatterns = Console.ReadLine();
            string[] patterns = userpatterns.Split(' ');
            logic.Recursive = true;
            logic.onProcess += new FileAccessProcessEventHandler(OnProcessSimpleList);
            logic.onNotify += new FileAccessNotifyEventHandler(OnNotify);
            Console.WriteLine("Processing current folder " + directoryPath + " for latest files of type \"" + userpatterns + "\"");
            Console.WriteLine("Press any key to start:");
            Console.ReadKey();
            Console.WriteLine("************************************");
            foreach (string pattern in patterns)
            {
                logic.FilePattern = "*." + pattern;
                logic.Execute(directoryPath);
                try
                {
                    temp.Logic.Notify(String.Format("Listing \t{0} \t{1} \t{2}", String.Format("{0,-110}", temp.FileInfo.FullName), temp.FileInfo.Length + " bytes", File.GetLastWriteTime(temp.FileInfo.FullName)));
                }
                catch(Exception e)
                {
                    Console.WriteLine("File of type {0} does not exist", pattern);
                }
                temp = null;
            }
            
            Console.WriteLine("************************************");
            Console.WriteLine("");
            Console.ReadKey();

        }

        private static void OnProcessSimpleList(object sender,
            ProcessEventArgs e)
        {
            if(e.Logic.Cancelled)
                return;
            if (temp == null) temp = e;
            else if (DateTime.Compare(File.GetLastWriteTime(temp.FileInfo.FullName), File.GetLastWriteTime(e.FileInfo.FullName))<=0) temp = e;

            AccumulateLength(e.Logic, e.FileInfo);
        }
        private static void OnNotify(object sedner, NotifyEventArgs e)
        {
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
