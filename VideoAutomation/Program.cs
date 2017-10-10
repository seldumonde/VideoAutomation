using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace VideoAutomation
{
    class Program
    {
        private const string COMPLETE_DIR = "Y:\\Downloads\\Complete";
        private const string PROCESSING_DIR = "Y:\\Downloads\\Processing";
        private const string CONVERTING_DIR = "Y:\\Downloads\\Converting";
        private const string CONVERTED_DIR = "Y:\\Downloads\\Converting\\Converted";
        private const string TEMP_DIR = "Y:\\Downloads\\Temp";

        enum ProcessingMode
        {
            Unknown = 0,
            Move_Complete = 1,
            Convert_Files = 2,
            Move_Converted = 3,
            Process_Files = 4,
            Cleanup_Complete = 5,
            Cleanup_Converting = 6,
            Cleanup_Processing = 7,
            Cleanup_Temp = 8
        }

        //private const string FILEBOT = "C:\\Users\\Brian\\AppData\\Local\\Microsoft\\WindowsApps\\filebot.exe";

        static void Main(string[] args)
        {
            //Console.WriteLine("Select option: ");

            Console.WriteLine("1. Move Complete (" + COMPLETE_DIR + ") to Converting (" + CONVERTING_DIR + ")");
            Console.WriteLine("2. Convert Files ( " + CONVERTING_DIR + ")");
            Console.WriteLine("3. Move Converted (" + CONVERTED_DIR + ") to Processing (" + PROCESSING_DIR + ")");
            Console.WriteLine("4. Process Files ( " + PROCESSING_DIR + ")");
            Console.WriteLine("5. Clean Up Complete (" + COMPLETE_DIR + ")");
            Console.WriteLine("6. Clean Up Converting (" + CONVERTING_DIR + ")");
            Console.WriteLine("7. Clean Up Processing (" + PROCESSING_DIR + ")");
            Console.WriteLine("8. Clean Up Temp (" + TEMP_DIR + ")");

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
                        myMode = ProcessingMode.Convert_Files;
                        Console.WriteLine("Converting files in CONVERTING to standardized MP4");
                        break;
                    case '3':
                        myMode = ProcessingMode.Move_Converted;
                        Console.WriteLine("Moving files from CONVERTED to PROCESSING");
                        break;
                    case '4':
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
                    default:
                        Console.WriteLine("You must enter a number from 1 to 6.");

                        Console.WriteLine("1. Move Complete (" + COMPLETE_DIR + ") to Converting (" + CONVERTING_DIR + ")");
                        Console.WriteLine("2. Convert Files ( " + CONVERTING_DIR + ")");
                        Console.WriteLine("3. Process Files ( " + PROCESSING_DIR + ")");
                        Console.WriteLine("4. Clean Up Complete (" + COMPLETE_DIR + ")");
                        Console.WriteLine("5. Clean Up Converting (" + CONVERTING_DIR + ")");
                        Console.WriteLine("6. Clean Up Processing (" + PROCESSING_DIR + ")");

                        break;
                }
            }

            //Console.WriteLine("Press a or A to enter auto mode. Anything else will enter manual mode.");
            //var modeKey = Console.ReadKey(true);

            //bool AUTO_MODE = false;

            //if (modeKey.KeyChar == 'a' || modeKey.KeyChar == 'A') AUTO_MODE = true;

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
                var folders = Directory.GetDirectories(PROCESSING_DIR, "*", SearchOption.TopDirectoryOnly);

                foreach (var folder in folders)
                {
                    bool hasSomething = false;

                    if (folder.StartsWith(".")) continue;

                    Console.WriteLine("Looking into folder " + folder);

                    var contents = Directory.GetFiles(folder, "*.rar", SearchOption.AllDirectories);

                    //Console.WriteLine("Showing all RAR files... ");

                    //foreach (var file in contents)
                    //{
                    //    if (file.ToLower().Contains("sample"))
                    //    {
                    //        File.Delete(file);
                    //        continue;
                    //    }

                    //    hasSomething = true;
                    //    Console.WriteLine("-- " + file);
                    //    ProcessRAR(file, AUTO_MODE);
                    //}

                    contents = Directory.GetFiles(folder, "*.mp4", SearchOption.AllDirectories);

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
                        ProcessVideo(file);
                    }

                    contents = Directory.GetFiles(folder, "*.mkv", SearchOption.AllDirectories);

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
                        ProcessVideo(file);
                    }

                    contents = Directory.GetFiles(folder, "*.avi", SearchOption.AllDirectories);

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
                        ProcessVideo(file);
                    }

                    if (!hasSomething)
                    {
                        Console.WriteLine(folder + " has nothing of interest. Contents below:");

                        contents = Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories);

                        foreach (var file in contents)
                        {
                            Console.WriteLine("-- " + file);
                        }

                        Console.WriteLine("Clear and delete folder " + folder +
                                          "? Enter or y for yes, any other key for no.");

                        var response = Console.ReadKey(true); ;

                        if (response.Key == ConsoleKey.Enter || response.KeyChar == 'y' || response.KeyChar == 'Y')
                        {
                            Console.WriteLine("Deleting " + folder);
                            Directory.Delete(folder, true);
                        }
                        else
                        {
                            Console.WriteLine("Not clearing this folder.");
                        }
                    }
                }

                string rootFolder = PROCESSING_DIR;

                var rootContents = Directory.GetFiles(rootFolder, "*.rar", SearchOption.AllDirectories);

                //Console.WriteLine("Showing all RAR files... ");

                //foreach (var file in rootContents)
                //{
                //    if (file.ToLower().Contains("sample"))
                //    {
                //        File.Delete(file);
                //        continue;
                //    }

                //    Console.WriteLine("-- " + file);
                //    ProcessRAR(file, AUTO_MODE);
                //}

                rootContents = Directory.GetFiles(rootFolder, "*.mp4", SearchOption.AllDirectories);

                Console.WriteLine("Showing all MP4 files... ");

                foreach (var file in rootContents)
                {
                    if (file.ToLower().Contains("sample"))
                    {
                        File.Delete(file);
                        continue;
                    }

                    Console.WriteLine("-- " + file);
                    ProcessVideo(file);
                }

                rootContents = Directory.GetFiles(rootFolder, "*.mkv", SearchOption.AllDirectories);

                Console.WriteLine("Showing all MKV files... ");

                foreach (var file in rootContents)
                {
                    if (file.ToLower().Contains("sample"))
                    {
                        File.Delete(file);
                        continue;
                    }

                    Console.WriteLine("-- " + file);
                    ProcessVideo(file);
                }

                rootContents = Directory.GetFiles(rootFolder, "*.avi", SearchOption.AllDirectories);

                Console.WriteLine("Showing all AVI files... ");

                foreach (var file in rootContents)
                {
                    if (file.ToLower().Contains("sample"))
                    {
                        File.Delete(file);
                        continue;
                    }

                    Console.WriteLine("-- " + file);
                    ProcessVideo(file);
                }

                rootContents = Directory.GetFiles(rootFolder, "*.mov", SearchOption.AllDirectories);

                Console.WriteLine("Showing all MOV files... ");

                foreach (var file in rootContents)
                {
                    if (file.ToLower().Contains("sample"))
                    {
                        File.Delete(file);
                        continue;
                    }

                    Console.WriteLine("-- " + file);
                    ProcessVideo(file);
                }
            }
            //else if (pathToLook == PROCESSING_DIR)
            //{
            //    var folder = PROCESSING_DIR;

            //    var contents = Directory.GetFiles(folder, "*.mp4", SearchOption.AllDirectories);

            //    Console.WriteLine("Showing all MP4 files... ");

            //    foreach (var file in contents)
            //    {
            //        if (file.ToLower().Contains("sample"))
            //        {
            //            File.Delete(file);
            //            continue;
            //        }

            //        Console.WriteLine("-- " + file);
            //        RenameVideo(file, AUTO_MODE);
            //    }

            //    contents = Directory.GetFiles(folder, "*.mkv", SearchOption.AllDirectories);

            //    Console.WriteLine("Showing all MKV files... ");

            //    foreach (var file in contents)
            //    {
            //        if (file.ToLower().Contains("sample"))
            //        {
            //            File.Delete(file);
            //            continue;
            //        }

            //        Console.WriteLine("-- " + file);
            //        RenameVideo(file, AUTO_MODE);
            //    }

            //    contents = Directory.GetFiles(folder, "*.avi", SearchOption.AllDirectories);

            //    Console.WriteLine("Showing all AVI files... ");

            //    foreach (var file in contents)
            //    {
            //        if (file.ToLower().Contains("sample"))
            //        {
            //            File.Delete(file);
            //            continue;
            //        }

            //        Console.WriteLine("-- " + file);
            //        RenameVideo(file, AUTO_MODE);
            //    }

            //    folder = PROCESSING_DIR;

            //    contents = Directory.GetFiles(folder, "*.mp4", SearchOption.AllDirectories);

            //    Console.WriteLine("Showing all MP4 files... ");

            //    foreach (var file in contents)
            //    {
            //        if (file.ToLower().Contains("sample"))
            //        {
            //            File.Delete(file);
            //            continue;
            //        }

            //        Console.WriteLine("-- " + file);
            //        RenameVideo(file, AUTO_MODE);
            //    }

            //    contents = Directory.GetFiles(folder, "*.mkv", SearchOption.AllDirectories);

            //    Console.WriteLine("Showing all MKV files... ");

            //    foreach (var file in contents)
            //    {
            //        if (file.ToLower().Contains("sample"))
            //        {
            //            File.Delete(file);
            //            continue;
            //        }

            //        Console.WriteLine("-- " + file);
            //        RenameVideo(file, AUTO_MODE);
            //    }

            //    contents = Directory.GetFiles(folder, "*.avi", SearchOption.AllDirectories);

            //    Console.WriteLine("Showing all AVI files... ");

            //    foreach (var file in contents)
            //    {
            //        if (file.ToLower().Contains("sample"))
            //        {
            //            File.Delete(file);
            //            continue;
            //        }

            //        Console.WriteLine("-- " + file);
            //        RenameVideo(file, AUTO_MODE);
            //    }
            //}

            //Console.WriteLine("Done");

            //if (!AUTO_MODE) Console.ReadLine();
        }

        private static void MoveVideos(string foldername, string destFolder)
        {
            var folders = Directory.GetDirectories(foldername);

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
                Directory.Delete(foldername);
            }
            else
            {
                foreach (var file in files)
                {
                    if (file.Length == 0) continue;

                    if (file.ToLower().Contains("sample") || file.ToLower().Contains("trailer"))
                    {
                        File.Delete(file);
                        continue;
                    }

                    var extension = Path.GetExtension(file).ToLower();

                    if (extension == ".mov" || extension == ".mkv" || extension == ".avi" || extension == ".mp4")
                    {
                        string destFile = destFolder + "\\" + Path.GetFileName(file);

                        if (!File.Exists(destFile))
                        {
                            File.Move(file, destFile);
                        }
                        else
                        {
                            File.Delete(file);
                        }
                    }
                    else
                    {
                        string destFile = TEMP_DIR + "\\" + Path.GetFileName(file);

                        File.Copy(file, destFile, true);
                        File.Delete(file);
                    }
                }
            }
        }

        private static void ProcessVideo(string file, bool AUTO_MODE = false)
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
