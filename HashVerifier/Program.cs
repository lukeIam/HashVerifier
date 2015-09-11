using System;
using System.IO;

namespace HashVerifier
{
    class Program
    {
        static void Main(string[] args)
        {
            EModus modus = EModus.Verify;
            string filename = "";
            string folder = "";
            int ignoreDepth = 0;

            foreach (string arg in args)
            {
                if (arg.StartsWith("--hashfile="))
                {
                    filename = arg.Replace("--hashfile=", "").Trim();
                }
               /* else if (arg.StartsWith("ignoreDeep="))
                {
                    ignoreDepth = Convert.ToInt32(arg.Replace("ignoreDeep=", "").Trim());
                }*/
                else if (arg.StartsWith("--directory="))
                {
                    folder = arg.Replace("--directory=", "").Trim();
                }
                else if (arg.StartsWith("--help"))
                {
                    Console.WriteLine("Usage:");
                    Console.WriteLine("HashVerifier [add] [--hashfile=FILE] [--directory=FOLDER] [--help]");
                    Environment.Exit(2);
                }
                else if (arg.Trim().Equals("add"))
                {
                    modus = EModus.AddSignatures;
                }
            }

            /*if (ignoreDepth < 0)
            {
                throw new ArgumentException("Invalid ignoreDeep");
            }*/

            if (String.IsNullOrEmpty(filename))
            {
                filename = "hashes.xml";
            }

            if (String.IsNullOrEmpty(folder))
            {
                folder = Directory.GetCurrentDirectory();
            }

            if (!Directory.Exists(folder))
            {
                throw new ArgumentException("Invalid directory");
            }

            string log = "";
            if (modus == EModus.Verify)
            {
                bool result = FileWalker.Check(new DirectoryInfo(folder), new FileInfo(filename), ignoreDepth, out log);
                Console.WriteLine(result ? "All files verified" : "Some files could not be verified");
                Console.WriteLine("#########################################");
                Console.WriteLine(log);
                if (!result)
                {
                    Environment.Exit(1);
                }
            }
            else if (modus == EModus.AddSignatures)
            {
                FileWalker.Add(new DirectoryInfo(folder), new FileInfo(filename), ignoreDepth, out log);
                Console.WriteLine(log);
            }
        }

        
    }
}
