using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Windows.Forms.VisualStyles;

namespace VideoAutomation
{
    public class ThreadedExecutor
    {
        private int MAX_THREADS = 8;
        private string EXECUTABLE;
        private List<string> ParamList;
        private List<string> FileList;
        private List<bool> ThreadLocks;
        private int CURR_PROC = 0;
        
        public ThreadedExecutor(string executable)
        {
            EXECUTABLE = executable;
            ParamList = new List<string>();
            FileList = new List<string>();
            //ThreadLocks = new List<bool>();

//            for (int i = 0; i < MAX_THREADS; i++)
//            {
//                ThreadLocks.Add(false);
//            }
        }

        public void AddToQueue(string parameters, string filename)
        {
            ParamList.Add(parameters);
            FileList.Add(filename);
        }

        public void StartExecution()
        {
            int totalProcs = ParamList.Count;
            int currProc = 0;

            int threadCount = 0;
            
            if (totalProcs < MAX_THREADS)
            {
                threadCount = totalProcs;
            }
            else
            {
                threadCount = MAX_THREADS;
            }
            
            ThreadLocks = new List<bool>(threadCount);

            for (int i = 0; i < threadCount - 1; i++)
            {
                ThreadLocks.Add(false);
            }

            while (CURR_PROC < totalProcs - 1)
            {
                for (int i = 0; i < threadCount - 1 ; i++)
                {
                    if (ThreadLocks[i] == false)
                    {
                        CURR_PROC++;
                        ThreadLocks[i] = true;
                        Thread newThread = new Thread(new ParameterizedThreadStart(ExecuteProcess));
                        newThread.Start(i);
                    }   
                }
            }
        }

        void ExecuteProcess(object threadNum)
        {
            var parameters = ParamList[CURR_PROC];
            
            System.Diagnostics.Process.Start(EXECUTABLE, parameters).WaitForExit();

            int threadNumInt = (int) threadNum;
            ThreadLocks[threadNumInt] = false;

            var currPath = Path.GetDirectoryName(FileList[CURR_PROC]);
            var converted = currPath + "_" + Path.GetFileName(FileList[CURR_PROC]);
            
            File.Move(FileList[CURR_PROC], converted);
        }
    }
}