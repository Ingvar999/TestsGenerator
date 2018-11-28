using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Generator;

namespace ConsoleApp
{
    class GenerateLayer
    {
        private const int timeout = 500;
        private int maxCount;
        private CommunicationSet<FileSource> outputSet;
        private CommunicationSet<FileSource> inputSet;
        private TestsGenerator generator;

        public GenerateLayer(int maxThreadsCount, CommunicationSet<FileSource> inputSet, CommunicationSet<FileSource> outputSet)
        {
            maxCount = maxThreadsCount;
            this.outputSet = outputSet;
            this.inputSet = inputSet;
            generator = new TestsGenerator();
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
            IEnumerable<FileSource> tests;
            while (inputSet.Sem.WaitOne(timeout))
            {
                FileSource source;
                inputSet.Queue.TryDequeue(out source);
                tests = await generator.GetGenerator(source);
                foreach (FileSource item in tests)
                {
                    outputSet.Queue.Enqueue(item);
                    outputSet.Sem.Release();
                    Console.WriteLine("Generated " + item.FileName);
                }
            }
        }
    }
}
