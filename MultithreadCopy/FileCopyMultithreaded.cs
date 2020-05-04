using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileCopyMultithreading
{         
    public class FileCopyMultithreaded
    {
        private delegate void CompleteDelegate(string sourceFile);
        private static Queue<KeyValuePair<string, string>> CopyList = new Queue<KeyValuePair<string, string>>();
        private static Semaphore pool = new Semaphore(0, 3);

        public int BufferSizeInKB = 4 * 1024;
        private int count;

        private event CompleteDelegate FileCopyComplete;
        private void OnFileCopyComplete(string sourceFile)
        {
            FileCopyComplete?.Invoke(sourceFile);
        }

        public void EnqueueFiles(KeyValuePair<string, string> file)
        {
            CopyList.Enqueue(file);
        }

        public FileCopyMultithreaded() { }

       

        private void CopyJob(object job)
        {
            KeyValuePair<string, string> copyJob = (KeyValuePair<string, string>)job;
            CopyFile(copyJob);
            count = CopyList.Count;
        }

        public void StartCopy()
        {
                      
                        
            while (CopyList.Count > 0)
            {               
                
                
                Thread t = new Thread(Copy);
                t.Start(CopyList.Dequeue());
                pool.WaitOne();
            }

            
            while (count > 0)
            {
                Thread.Sleep(100);
            }
        }

        private void Copy(object x)
        {            
            if (x is KeyValuePair<string, string>)
            {
                var kvp = (KeyValuePair<string, string>) x;
                CopyFile(kvp);
                pool.Release();
                Interlocked.Decrement(ref count);
            }
        }

        private void CopyFile(KeyValuePair<string, string> copyJob)
        {
            byte[] buffer = new byte[BufferSizeInKB * 1024]; // 4K buffer

            using (FileStream source = new FileStream(copyJob.Key, FileMode.Open, FileAccess.Read))
            {
                using (FileStream destination = new FileStream(copyJob.Value, FileMode.Create, FileAccess.Write))
                {
                    int currentBlockSize = 0;

                    while ((currentBlockSize = source.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        destination.Write(buffer, 0, currentBlockSize);
                    }
                }
            }

            OnFileCopyComplete(copyJob.Key);
        }
    }
}
