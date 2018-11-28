using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Generator;

namespace ConsoleApp
{
    public class ReadLayer
    {
        private int maxCount;
        private ConcurrentQueue<string> files;
        private CommunicationSet<FileSource> outputSet;

        public ReadLayer(int maxThreadsCount, ConcurrentQueue<string> sourceList, CommunicationSet<FileSource> outputSet)
        {
            maxCount = maxThreadsCount;
            files = sourceList;
            this.outputSet = outputSet;
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
            string fileName;
            FileStream stream;
            FileSource file;
            while (files.TryDequeue(out fileName))
            {
                stream = new FileStream(fileName, FileMode.Open);
                file = new FileSource(stream.Length, fileName);
                await stream.ReadAsync(file.Data, 0, file.Data.Length);
                Console.WriteLine("Read " + fileName);
                outputSet.Queue.Enqueue(file);
                outputSet.Sem.Release();
            }
        }
    }
}
