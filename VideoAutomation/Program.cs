using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;

namespace VideoAutomation
{
    class Program
    {
        //private const string COMPLETE_DIR = "Y:\\Downloads\\Complete";
        private const string COMPLETE_DIR = "G:\\Complete";
        
        //private const string CONVERTING_DIR = "Y:\\Downloads\\Converting";
        private const string CONVERTING_DIR = "H:\\Converting";

        //private const string PROCESSING_DIR = "Y:\\Downloads\\Processing";
        private const string PROCESSING_DIR = "Y:\\Processing";

        //private const string CONVERTED_DIR = "Y:\\Downloads\\Converting\\Converted";
        private const string CONVERTED_DIR = "G:\\Converting\\Converted";

        private const string TEMP_DIR = "Y:\\Downloads\\Temp";

        enum ProcessingMode
        {
            Unknown = 0,
            Move_Complete = 1,
            Move_Converted = 2,
            Process_Files = 3,
            Cleanup_Complete = 5,
            Cleanup_Converting = 6,
            Cleanup_Processing = 7,
            Cleanup_Temp = 8
        }

        //private const string FILEBOT = "C:\\Users\\Brian\\AppData\\Local\\Microsoft\\WindowsApps\\filebot.exe";

        static void Main(string[] args)
        {
            bool continueLoop = true;

            while (continueLoop)
            {
                Console.WriteLine("1. Move Complete (" + COMPLETE_DIR + ") to Converting (" + CONVERTING_DIR + ")");
                Console.WriteLine("2. Move Converted (" + CONVERTED_DIR + ") to Processing (" + PROCESSING_DIR + ")");
                Console.WriteLine("3. Process Files ( " + PROCESSING_DIR + ")");
                //Console.WriteLine("5. Clean Up Complete (" + COMPLETE_DIR + ")");
                //Console.WriteLine("6. Clean Up Converting (" + CONVERTING_DIR + ")");
                //Console.WriteLine("7. Clean Up Processing (" + PROCESSING_DIR + ")");
                //Console.WriteLine("8. Clean Up Temp (" + TEMP_DIR + ")");
                Console.WriteLine("9. Exit");

                ProcessingMode myMode = ProcessingMode.Unknown;

                while (myMode == ProcessingMode.Unknown)
                {
                    var response = Console.ReadKey();
                    Console.Clear();

                    switch (response.KeyChar)
                    {
                        case '1':
                            myMode = ProcessingMode.Move_Complete;
                            Console.WriteLine("Moving files from COMPLETE to CONVERTING");
                            break;
                        case '2':
                            myMode = ProcessingMode.Move_Converted;
                            Console.WriteLine("Moving files from CONVERTED to PROCESSING");
                            break;
                        case '3':
                            myMode = ProcessingMode.Process_Files;
                            Console.WriteLine("Processing files in PROCESSING for Plex");
                            break;
                        case '5':
                            myMode = ProcessingMode.Cleanup_Complete;
                            Console.WriteLine("Cleaning up COMPLETE");
                            break;
                        case '6':
                            myMode = ProcessingMode.Cleanup_Converting;
                            Console.WriteLine("Cleaning up CONVERTING");
                            break;
                        case '7':
                            myMode = ProcessingMode.Cleanup_Processing;
                            Console.WriteLine("Cleaning up PROCESSING");
                            break;
                        case '8':
                            myMode = ProcessingMode.Cleanup_Temp;
                            Console.WriteLine("Cleaning up TEMP");
                            break;
                        case '9':
                            continueLoop = false;
                            break;
                        default:
                            myMode = ProcessingMode.Unknown;
                            break;
                    }
                }

                if (myMode == ProcessingMode.Unknown)
                {
                    Console.WriteLine("You must select an option from 1 to 9.");
                    Console.Clear();
                }
                else
                {
                    if (myMode == ProcessingMode.Move_Complete)
                    {
                        MoveVideos(COMPLETE_DIR, CONVERTING_DIR);
                    }
                    else if (myMode == ProcessingMode.Move_Converted)
                    {
                        MoveVideos(CONVERTED_DIR, PROCESSING_DIR);
                    }
                    else if (myMode == ProcessingMode.Process_Files)
                    {
                        ProcessVideos(PROCESSING_DIR);
                    }
                    else
                    {
                        Console.WriteLine("not yet implemented");
                    }
                }

                Console.WriteLine("Operation completed. Returning to menu.");
                Thread.Sleep(5000);
            }
        }

        private static void ProcessVideos(string processDir)
        {
            Console.WriteLine("Press M or m to begin manual mode. Any other key will enter auto mode.");

            var autoKey = Console.ReadKey(true);

            bool autoMode = true;

            if (autoKey.KeyChar == 'm' || autoKey.KeyChar == 'M')
                autoMode = false;

            var folders = Directory.GetDirectories(processDir);

            if (folders.Length > 0)
            {
                foreach (var folder in folders)
                {
                    Console.WriteLine("Looking into folder " + folder);

                    ProcessVideos(folder);
                }
            }

            bool hasSomething = false;
            var contents = Directory.GetFiles(processDir, "*.mp4");

            Console.WriteLine("Showing all MP4 files... ");

            foreach (var file in contents)
            {
                if (file.ToLower().Contains("sample"))
                {
                    File.Delete(file);
                    continue;
                }

                hasSomething = true;
                Console.WriteLine("-- " + file);
                RenameVideo(file, autoMode);
            }

            contents = Directory.GetFiles(processDir, "*.mkv");

            Console.WriteLine("Showing all MKV files... ");

            foreach (var file in contents)
            {
                if (file.ToLower().Contains("sample"))
                {
                    File.Delete(file);
                    continue;
                }

                hasSomething = true;
                Console.WriteLine("-- " + file);
                RenameVideo(file, autoMode);
            }

            contents = Directory.GetFiles(processDir, "*.avi");

            Console.WriteLine("Showing all AVI files... ");

            foreach (var file in contents)
            {
                if (file.ToLower().Contains("sample"))
                {
                    File.Delete(file);
                    continue;
                }

                hasSomething = true;
                Console.WriteLine("-- " + file);
                RenameVideo(file, autoMode);
            }

            contents = Directory.GetFiles(processDir, "*.mov");

            Console.WriteLine("Showing all MOV files... ");

            foreach (var file in contents)
            {
                if (file.ToLower().Contains("sample"))
                {
                    File.Delete(file);
                    continue;
                }

                hasSomething = true;
                Console.WriteLine("-- " + file);
                RenameVideo(file, autoMode);
            }

            if (!hasSomething)
            {
                Console.WriteLine(processDir + " has nothing of interest. Cleaning it out and deleting it.");

                contents = Directory.GetFiles(processDir, "*.*", SearchOption.AllDirectories);

                foreach (var file in contents)
                {
                    string destFile = TEMP_DIR + "\\" + Path.GetFileName(file);

                    if (File.Exists(destFile))
                    {
                        File.Delete(file);
                    }
                    else
                    {
                        File.Move(file, destFile);
                    }
                }

                Directory.Delete(processDir);
            }
        }

        private static void MoveVideos(string foldername, string destFolder)
        {
            if (!Directory.Exists(foldername)) return;

            var folders = Directory.GetDirectories(foldername);
            
            Console.WriteLine("Scanning folder " + foldername);

            if (folders.Length > 0)
            {
                foreach (var folder in folders)
                {
                    if (folder.ToLower().Contains(".sync")) continue;

                    MoveVideos(folder, destFolder);
                }
            }

            var files = Directory.GetFiles(foldername);

            if (files.Length == 0 && folders.Length == 0)
            {
                Console.WriteLine("Deleting folder " + foldername);
                Directory.Delete(foldername);
            }
            else
            {
                foreach (var file in files)
                {
                    if (file.Length == 0) continue;

                    var fileInfo = new FileInfo(file);

                    if (fileInfo.Length == 0) continue;

                    if (file.ToLower().Contains("sample") || file.ToLower().Contains("trailer"))
                    {
                        File.Delete(file);
                        continue;
                    }

                    var extension = Path.GetExtension(file).ToLower();

                    if (extension == ".mov" || extension == ".mkv" || extension == ".avi" || extension == ".mp4")
                    {
                        string destFile = destFolder + "\\" + Path.GetFileName(file);
                        string ignoreFile = foldername + "\\" +  Path.GetFileNameWithoutExtension(file) + ".unknown.ignore";

                        if (File.Exists(ignoreFile))
                        {
                            Console.WriteLine("File not totally downloaded. Not moving.");

                        }
                        else if (!File.Exists(destFile))
                        {
                            try
                            {
                                File.Move(file, destFile);
                                Console.WriteLine("Moved " + file + ".");
                            }
                            catch
                            {
                                Console.WriteLine("Cannot move " + file + " as it is in use.");
                            }
                        }
                        else
                        {
                            File.Delete(file);
                        }
                    }
                    else if (extension != ".ignore")
                    {
                        string destFile = TEMP_DIR + "\\" + Path.GetFileName(file);

                        try
                        {
                            File.Copy(file, destFile, true);
                            File.Delete(file);
                        }
                        catch
                        {
                            Console.WriteLine("Cannot move " + file + " as it is in use.");
                        }
                    }
                }
            }
        }

        private static void RenameVideo(string file, bool AUTO_MODE)
        {
            var botProc = new Process();

            bool validinput = false;
            var result = new ConsoleKeyInfo();
            string conflict = "", database = "";

            if (AUTO_MODE)
            {
                conflict = "auto";
            }
            else
            {
                while (!validinput)
                {
                    Console.WriteLine("Conflict resolution: 1. Auto, 2. Override, 3. Skip (default 1):");
                    result = Console.ReadKey(true);

                    if (result.KeyChar == '1' || result.Key == ConsoleKey.Enter)
                    {
                        validinput = true;
                        conflict = "auto";
                    }
                    else if (result.KeyChar == '1')
                    {
                        validinput = true;
                        conflict = "override";
                    }
                    else if (result.KeyChar == '1')
                    {
                        validinput = true;
                        conflict = "skip";
                    }
                }
            }

            validinput = false;

            if (AUTO_MODE)
            {
                database = "";
            }
            else
            {
                while (!validinput)
                {
                    Console.WriteLine(
                        "Database: 1. Auto, 2. TheTVDB, 3. TheMovieDB, 4. AniDB, 5. OMDb, 6. AcoustID, 7. ID3  (default 1):");
                    result = Console.ReadKey(true);

                    if (result.KeyChar == '2')
                    {
                        validinput = true;
                        database = "--db TheTVDB";
                    }
                    else if (result.KeyChar == '3')
                    {
                        validinput = true;
                        database = "--db TheMovieDB";
                    }
                    else if (result.KeyChar == '4')
                    {
                        validinput = true;
                        database = "--db AniDB";
                    }
                    else if (result.KeyChar == '5')
                    {
                        validinput = true;
                        database = "--db OMDb";
                    }
                    else if (result.KeyChar == '6')
                    {
                        validinput = true;
                        database = "--db AcoustID";
                    }
                    else if (result.KeyChar == '7')
                    {
                        validinput = true;
                        database = "--db ID3";
                    }
                    else if (result.KeyChar == '1' || result.Key == ConsoleKey.Enter)
                    {
                        validinput = true;
                    }
                }
            }

            string query = "";

            if (!AUTO_MODE)
            {
                Console.WriteLine("Enter a specific query term? Leave blank to ignore.");
                query = Console.ReadLine();

                if (query != "") query = "--q \"" + query + "\"";
            }

            Console.WriteLine("Renaming with applied settings: " + Path.GetFileName(file));

            botProc.StartInfo = new ProcessStartInfo
            {
                FileName = "filebot",
                Arguments = " -rename --format Y:\\{plex} " + database + " -check -non-strict " + query + " --conflict " + conflict + " \"" + file + "\"",
                UseShellExecute = false,
                CreateNoWindow = false,
                WindowStyle = ProcessWindowStyle.Normal,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };

            int lineCount = 0, errorLineCount = 0;
            string output = "", errors = "";

            botProc.OutputDataReceived +=
                delegate (object sender, DataReceivedEventArgs e)
                {
                    if (!String.IsNullOrEmpty(e.Data))
                    {
                        lineCount++;
                        output += "\n" + lineCount + ": " + e.Data;
                    }
                };

            botProc.ErrorDataReceived +=
                delegate (object sender, DataReceivedEventArgs e)
                {
                    if (!String.IsNullOrEmpty(e.Data))
                    {
                        errorLineCount++;
                        errors += "\n" + errorLineCount + ": " + e.Data;
                    }
                };

            botProc.Start();

            botProc.BeginOutputReadLine();
            botProc.BeginErrorReadLine();

            botProc.WaitForExit();
            botProc.Close();

            Console.WriteLine(output);
            Console.WriteLine(errors);

            if (errors.Contains("exact copy") || output.Contains("already exists"))
            {
                if (AUTO_MODE)
                {
                    File.Delete(file);
                }
                else
                {
                    Console.WriteLine("Exact match found. Deleting new version.");

                    File.Delete(file);

                    //var responseKey = Console.ReadKey(true);

                    //                    if (responseKey.Key == ConsoleKey.Enter)
                    //                  {
                    //                    File.Delete(file);
                    //              }
                }
            }
            else if (output.Contains("Failed to identify or process any files"))
            {
                //Console.WriteLine("Renaming failed. Do you want to reprocess? Enter for yes, anything else for no.");
                //result = Console.ReadKey(true);

                //if (result.Key == ConsoleKey.Enter) CompleteRename(file, AUTO_MODE);
            }
        }


    }
}
