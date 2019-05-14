using System;
using System.Diagnostics;

namespace Launcher
{
    class Launch
    {
        public static void Start()
        {
            // Create a new process.
            ProcessStartInfo start = new ProcessStartInfo();

            // Set our process' file to our path.
            start.FileName = string.Format("Program/{0}", Config.I.Path);

            // Start our program.
            Console.WriteLine("Launching {0}.", Config.I.Name);
            Process.Start(start);
        }
    }
}
