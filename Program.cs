using System;
using System.IO;
using System.Diagnostics;

namespace launcher
{
    class Program
    {
        static void Err(string errorMsg, int exitcode = 1)
        {
            Console.WriteLine($"ERROR: {errorMsg}");
            Console.ReadLine();
            Environment.Exit(exitcode);
        }
        static int Exec(string program, string arguments = null, bool waitForExit = true)
        {
            using(Process p = new Process())
            {
                p.StartInfo = new ProcessStartInfo
                {
                    FileName = program,
                    Arguments = arguments
                };
                
                p.Start();

                // when we're waiting for the process to exit
                // return the exitcode
                if(waitForExit)
                {
                    p.WaitForExit();
                    return p.ExitCode;
                }
            }

            // return 0 when we're not waiting for the process to exit
            return 0;
        }
        static void Main(string[] args)
        {
            string configFile = "launcher.txt";

            string exeFile = null, dataDir = null;

            // if config doesn't exist, error and exit
            if(!File.Exists(configFile))
            {
                Err($"missing config file: {configFile}");
            }

            string configText = File.ReadAllText(configFile);

            foreach(string line in configText.Split(Environment.NewLine))
            {
                if(line.StartsWith("exe="))
                {
                    exeFile = line.Split("exe=")[1];
                    if(!File.Exists(exeFile))
                        Err($"file {exeFile} doesn't exist!");
                }
                else if(line.StartsWith("data="))
                {
                    dataDir = line.Split("data=")[1];
                    if(!Directory.Exists(dataDir))
                       Err($"directory {dataDir} doesn't exist!");
                }
            }

            if(exeFile == null || dataDir == null)
                Err("invalid config file!");
            
            int exitcode = Exec(exeFile, $"--workDir {dataDir}");
        }
    }
}
