using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Threading;
using System.Windows.Forms.VisualStyles;

namespace VideoAutomation
{
    class Program
    {
        private const string DOWNLOAD_DIR = "M:\\Downloads";
        private const string COMPLETE_DIR = "M:\\Complete";
        private const string UNPACKING_DIR = "M:\\Unpacking";
        private const string UNPACKED_DIR = "M:\\Unpacking\\Unpacked";
        private const string CONTENT_DIR = "M:\\Unpacking\\Content";
        private const string CONVERTING_DIR = "M:\\Converting";
        private const string CONVERTED_DIR = "M:\\Converting\\Converted";
        private const string PROCESSING_DIR = "M:\\Processing";

        private const string PROCESSING_MOVIES = "M:\\Processing\\Movies";
        private const string PROCESSING_TV = "M:\\Processing\\TV";
        private const string PROCESSING_ANIME = "M:\\Processing\\Anime";

        private const string TEMP_DIR = "M:\\Holding";
        private const string DESTINATION_DIR = "Y:\\";

        enum ProcessingMode
        {
            Unknown = 0,
            Move_Completed = 1,
            Move_Archives = 2,
            Unpack_Archives = 3,
            Move_Unpacked = 4,
            Convert_Videos = 5,
            Move_Converted = 6,
            Process_Videos = 7,
            Clean_Directories = 8,
            Sync_Libraries = 9
        }

        static void Main(string[] args)
        {
            bool continueLoop = true;

            while (continueLoop)
            {
                Console.WriteLine("1. Move Completed Downloads");
                Console.WriteLine("2. Move Completed Archives");
                Console.WriteLine("3. Unpack Archives");
                Console.WriteLine("4. Move Unpacked Videos");
                Console.WriteLine("5. Convert Videos");
                Console.WriteLine("6. Move Converted Videos");
                Console.WriteLine("7. Process Videos");
                Console.WriteLine("8. Clean Directories");
                Console.WriteLine("9. Sync Libraries");

                Console.WriteLine("To exit, close this window or press ESC.");

                ProcessingMode myMode = ProcessingMode.Unknown;

                while (myMode == ProcessingMode.Unknown)
                {
                    var response = Console.ReadKey();
                    Console.Clear();

                    if (response.Key == ConsoleKey.Escape)
                    {
                        continueLoop = false;
                        break;
                    }

                    switch (response.KeyChar)
                    {
                        case '1':
                            myMode = ProcessingMode.Move_Completed;
                            Console.WriteLine("Moving completed videos...");
                            break;
                        case '2':
                            myMode = ProcessingMode.Move_Archives;
                            Console.WriteLine("Moving completed archives...");
                            break;
                        case '3':
                            myMode = ProcessingMode.Unpack_Archives;
                            Console.WriteLine("Unpacking archives...");
                            break;
                        case '4':
                            myMode = ProcessingMode.Unknown;
                            Console.WriteLine("Moving unpacked videos...");
                            break;
                        case '5':
                            myMode = ProcessingMode.Unknown;
                            Console.WriteLine("Convert videos...");
                            break;
                        case '6':
                            myMode = ProcessingMode.Unknown;
                            Console.WriteLine("Moving converted videos...");
                            break;
                        case '7':
                            myMode = ProcessingMode.Unknown;
                            Console.WriteLine("Process videos...");
                            break;
//                        case '8':
//                            myMode = ProcessingMode.Move_Completed;
//                            Console.WriteLine("Moving completed videos...");
//                            break;
//                        case '9':
//                            myMode = ProcessingMode.Move_Completed;
//                            Console.WriteLine("Moving completed videos...");
//                            break;
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
                    switch (myMode)
                    {
                        case ProcessingMode.Move_Completed:
                            Console.Clear();
                            MoveVideos(DOWNLOAD_DIR);
                            break;
                        case ProcessingMode.Move_Archives:
                            Console.Clear();
                            MoveArchives(DOWNLOAD_DIR);
                            break;
                        case ProcessingMode.Unpack_Archives:
                            Console.Clear();
                            UnpackArchives();
                            break;
                    }
                }

                Console.WriteLine("Operation completed. Press ENTER to return to menu.");

                Console.ReadKey();

                Console.Clear();
            }
        }

        private static string CalcDepth(char depthChar, int depth)
        {
            string output = "";

            for (int x = 0; x < depth; x++)
            {
                output += depthChar;
            }

            return output;
        }

        private static void MoveVideos(string processDir, int depth = 0)
        {
            var folders = Directory.GetDirectories(processDir);

            if (folders.Length > 0)
            {
                foreach (var folder in folders)
                {
                    Console.WriteLine(CalcDepth('-', depth) + folder);

                    MoveVideos(folder, depth + 1);
                }
            }

            var contents = Directory.GetFiles(processDir, "*");

//            if (contents.Length == 0)
//            {
//    //                Directory.Delete(processDir);    
//            }

            foreach (var content in contents)
            {
                var filename = Path.GetFileName(content);
                Console.WriteLine(CalcDepth('*', depth) + filename);

                var extension = Path.GetExtension(content).ToLower();

                if (extension == ".mp4")
                {
                    var destFileName = COMPLETE_DIR + "\\" + filename;

                    if (File.Exists(destFileName))
                    {
                        FileInfo destFile = new FileInfo(destFileName);
                        FileInfo sourceFile = new FileInfo(content);

                        if (destFile.Length < sourceFile.Length)
                        {
                            File.Delete(destFileName);
                            File.Move(content, destFileName);
                        }
                        else
                        {
                            File.Delete(content);
                        }
                    }
                    else
                    {
                        File.Move(content, destFileName);
                    }
                }
                else if (extension == ".avi" || extension == ".mov" || extension == ".mkv" || extension == ".wmv")
                {
                    var destFileName = CONVERTING_DIR + "\\" + filename;

                    if (File.Exists(destFileName))
                    {
                        FileInfo destFile = new FileInfo(destFileName);
                        FileInfo sourceFile = new FileInfo(content);

                        if (destFile.Length < sourceFile.Length)
                        {
                            File.Delete(destFileName);
                            File.Move(content, destFileName);
                        }
                        else
                        {
                            File.Delete(content);
                        }
                    }
                    else
                    {
                        File.Move(content, destFileName);
                    }
                }
                else
                {
                    var destFileName = TEMP_DIR + "\\" + filename;

                    if (File.Exists(destFileName))
                    {
                        FileInfo destFile = new FileInfo(destFileName);
                        FileInfo sourceFile = new FileInfo(content);

                        if (destFile.Length < sourceFile.Length)
                        {
                            File.Delete(destFileName);
                            File.Move(content, destFileName);
                        }
                        else
                        {
                            File.Delete(content);
                        }
                    }
                    else
                    {
                        try
                        {
                            File.Move(content, destFileName);
                        }
                        catch
                        {
                            Console.WriteLine("File in use, cannot move");
                        }
                    }
                }
            }
        }

        private static void MoveArchives(string processDir, int depth = 0)
        {
            var folders = Directory.GetDirectories(processDir);

            if (folders.Length > 0)
            {
                foreach (var folder in folders)
                {
                    Console.WriteLine(CalcDepth('-', depth) + folder);

                    MoveArchives(folder, depth + 1);
                }
            }

            var contents = Directory.GetFiles(processDir, "*");

            //            if (contents.Length == 0)
            //            {
            //    //                Directory.Delete(processDir);    
            //            }

            foreach (var content in contents)
            {
                var filename = Path.GetFileName(content);
                Console.WriteLine(CalcDepth('*', depth) + filename);

                var extension = Path.GetExtension(content).ToLower();

                if (extension.StartsWith(".r"))
                {
                    var destFileName = UNPACKING_DIR + "\\" + filename;

                    if (File.Exists(destFileName))
                    {
                        FileInfo destFile = new FileInfo(destFileName);
                        FileInfo sourceFile = new FileInfo(content);

                        if (destFile.Length < sourceFile.Length)
                        {
                            File.Delete(destFileName);
                            File.Move(content, destFileName);
                        }
                        else
                        {
                            File.Delete(content);
                        }
                    }
                    else
                    {
                        try
                        {
                            File.Move(content, destFileName);
                        }
                        catch
                        {
                            Console.WriteLine("File in use, cannot move");
                        }
                    }
                }
            }
        }

        private static void UnpackArchives()
        {
            var archives = Directory.GetFiles(UNPACKING_DIR, "*.rar");

            var parameters = "-o=\"" + CONTENT_DIR + "\" e ";

            foreach (var archive in archives)
            {
                var filename = Path.GetFileNameWithoutExtension(archive);

                var myParams = parameters + "\"" + archive + "\"";

                var extractProc = new Process();
                extractProc.StartInfo.FileName = "7z";
                extractProc.StartInfo.Arguments = myParams;

                extractProc.Start();
                extractProc.WaitForExit();

                if (extractProc.ExitCode == 0)
                {
                    var subFiles = Directory.GetFiles(UNPACKING_DIR + filename + ".r*");

                    foreach (var subfile in subFiles)
                    {
                        File.Move(subfile, UNPACKED_DIR + "\\" + Path.GetFileName(subfile));
                    }
                }
                else
                {
                    Console.WriteLine("Could not extract file " + Path.GetFileName(filename));
                }
            }
        }
    }
}
