using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading;

namespace Launcher
{
    class Updater
    {
        private static bool Completed = false;

        public static void Start(string download, List<string> exceptions)
        {
            // Generate a temporary file name.
            var path = Path.GetTempFileName();

            Console.WriteLine("Starting download.");

            // Create a web client.
            using (var client = new WebClient())
            {
                // Setup asynchronous handlers.
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgressChanged);
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(Downloaded);

                // Start downloading our file asynchronously.
                client.DownloadFileAsync(new Uri(download), path);
            }

            // While we haven't completed our download.
            while (!Completed)
                // Wait for 100 milliseconds.
                Thread.Sleep(100);

            Console.WriteLine();
            Console.WriteLine("Download finished.");

            // Clean up our previous install.
            DeleteOld(exceptions);

            // Extract our new version and overwrite any old files (that weren't cleaned up).
            ZipFile.ExtractToDirectory(path, "Program", true);
            Console.WriteLine("Decompressed program.");

            // Delete our temporary files.
            File.Delete(path);
            Console.WriteLine("Cleaned up temporary files.");
        }

        private static void DeleteOld(List<string> exceptions)
        {
            // Check if the directory exists where we are trying to delete.
            if (!Directory.Exists("Program"))
                return;

            // Call a recursive function to clear the directory.
            CleanDirectory("Program", exceptions);
        }

        private static bool CleanDirectory(string directory, List<string> exceptions)
        {
            // Get all directories and keep track of how many we remove.
            var directories = Directory.GetDirectories(directory);
            var rd = 0;

            foreach (var dir in directories)
            {
                // As this is a recursive function, call ourself again on the new directory.
                if (CleanDirectory(dir, exceptions))
                    // If our directory was removed, increment removed directories.
                    rd++;
            }

            // Get all files and keep track of how many we remove.
            var files = Directory.GetFiles(directory);
            var rf = 0;

            foreach (var file in files)
            {
                // Define exempt with default false.
                var exempt = false;

                // Iterate through each exception:
                foreach (var exception in exceptions)
                {
                    // If our file or directory contains our exception:
                    if (file.Contains(exception))
                    {
                        // Set exempt to true and exit out of our exception loop.
                        exempt = true;

                        break;
                    }
                }

                // If our file is exempt:
                if (exempt)
                    // Continue to the next file.
                    continue;

                // Delete our file.
                File.Delete(file);

                // Increment removed files.
                rf++;
            }

            // If we removed every directory and file in our current directory:
            if (rd == directories.Length && rf == files.Length)
            {
                // Delete our current directory.
                Directory.Delete(directory);

                // Return true so our recursive function knows we cleared our directory.
                return true;
            }

            // Return false so our recursive function knows our directory still contains files.
            return false;
        }

        private static void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            // Convert longs to doubles.
            var received = (double)e.BytesReceived;
            var total = (double)e.TotalBytesToReceive;

            // Define a constant width for our progress bar.
            const int width = 50;

            // Calculate how much of our bar should be full and percentage completed.
            var fraction = received / total;
            var amount = (int)(fraction * width);
            var percentage = (int)(fraction * 100);

            // Define an empty string for our bar.
            string bar = string.Empty;

            // Iterate through each character of the bar:
            for (var i = 0; i < width; i++)
            {
                // If this current position of the bar should be filled in:
                if (amount >= i)
                    // If we are at the end of the bar:
                    if (amount == i || i == width - 1)
                        // Add a pointer to the end of the bar.
                        bar += ">";
                    // Otherwise:
                    else
                        // Add filler to the middle of the bar.
                        bar += "=";
                // Otherwise:
                else
                    // Add whitespace to make sure the bar is a constant width.
                    bar += " ";
            }

            // Convert our byte amounts to human readable strings.
            var recReadable = BytesToString(e.BytesReceived);
            var totReadable = BytesToString(e.TotalBytesToReceive);

            // Make sure our string is a constant width.
            while (true)
                if (recReadable.Length < 8)
                    recReadable += " ";
                else
                    break;

            // Format our percentage to a human readable string.
            var perReadable = string.Format("{0}%", percentage);

            // Make sure our string is a constant width.
            while (true)
                if (perReadable.Length < 4)
                    perReadable += " ";
                else
                    break;

            // Print all of our information to the console.
            Console.Write("\rDownload progress  [{0}]  {1}  {2} / {3}", bar, perReadable, recReadable, totReadable);
        }

        private static string BytesToString(long byteCount)
        {
            // Define our suffixes.
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };

            // If we have zero bytes, return 0B.
            if (byteCount == 0)
                return "0" + suf[0];

            // If our bytes are negative, flip that.
            var bytes = Math.Abs(byteCount);

            // Calculate the suffix using logarithm.
            var place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));

            // Calculate how many amount of our *suffix* are there using powers.
            var num = Math.Round(bytes / Math.Pow(1024, place), 1);

            // Set our sign again, convert our number to a string then append the suffix.
            var output = (Math.Sign(byteCount) * num).ToString() + suf[place];

            // Return our output.
            return output;
        }

        private static void Downloaded(object sender, AsyncCompletedEventArgs e)
        {
            // Once we have finished our download, set completed to true.
            // This will let our main thread continue.
            Completed = true;
        }
    }
}
