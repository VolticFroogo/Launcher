using System;
using System.Threading;

namespace Launcher
{
    class Program
    {
        static void Main(string[] args)
        {
            // Initialise our config.
            Config.Init();

            // Aesthetic changes.
            Console.Title = String.Format("{0} Launcher", Config.I.Name);
            Console.CursorVisible = false;

            // Check for any updates on the update server.
            var msg = Network.Get();

            switch (msg.ID)
            {
                // If we need an update:
                case Message.Types.Update:
                    Console.WriteLine("Currently on version {0}, downloading version {1}.", Config.I.Version, msg.Version);

                    // Set some new data in our config.
                    Config.I.Name = msg.Name;
                    Config.I.Path = msg.Path;

                    // Start updating the game.
                    Updater.Start(msg.Download, msg.Exceptions);

                    // Change the config version to our new version.
                    Config.I.Version = msg.Version;

                    // Save our newly edited config.
                    Config.I.Save();

                    break;

                // If we are up to date:
                case Message.Types.Latest:
                    Console.WriteLine("Still on latest version ({0}).", Config.I.Version);

                    break;
            }

            // Launch our program.
            Launch.Start();

            // Wait three seconds so we can read what everything said, instead of instantly closing.
            Thread.Sleep(3000);
        }

        private static void Error()
        {
            Console.Write("The launcher has failed, press any key to close the launcher...");
            Console.ReadKey();
        }
    }
}
