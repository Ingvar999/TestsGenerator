using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Generator;

namespace ConsoleApp
{
    class WriteLayer
    {
        private const int timeout = 500;
        private int maxCount;
        private string outputFolder;
        private CommunicationSet<FileSource> inputSet;

        public WriteLayer(int maxThreadsCount, CommunicationSet<FileSource> inputSet, string folder)
        {
            maxCount = maxThreadsCount;
            this.inputSet = inputSet;
            outputFolder = folder;
        }

        public void Start()
        {
            for (int i = 0; i < maxCount; ++i)
            {
                StartTrhreadAsync();
            }
        }

        public async void StartTrhreadAsync()
        {
            FileStream stream;
            FileSource file;
            while (inputSet.Sem.WaitOne(timeout))
            {
                inputSet.Queue.TryDequeue(out file);
                stream = new FileStream(outputFolder + file.FileName, FileMode.OpenOrCreate);
                await stream.WriteAsync(file.Data, 0, file.Data.Length);
                stream.Close();
                Console.WriteLine("Wrote " + stream.Name);
            }
        }
    }
}
