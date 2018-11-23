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
        private const int timeout = 2000;
        private int maxCount;
        private ConcurrentQueue<string> files;
        private CommunicationSet<FileSource> outputSet;
        private CommunicationSet<FileSource> inputSet;
        private TestsGenerator generator;

        public GenerateLayer(int maxThreadsCount, ConcurrentQueue<string> sourceList, CommunicationSet<FileSource> inputSet, CommunicationSet<FileSource> outputSet)
        {
            maxCount = maxThreadsCount;
            files = sourceList;
            this.outputSet = outputSet;
            this.inputSet = inputSet;
            generator = new TestsGenerator();
        }

        public void Start()
        {
            for (int i = 0; i < maxCount; ++i)
            {
                if (inputSet.Sem.WaitOne(timeout))
                {
                    FileSource source;
                    inputSet.Queue.TryDequeue(out source);
                    generator.GetGenerator(source).ContinueWith(GenerateCompletion, this).Start();
                }
                else
                {
                    break;
                }
            }
        }

        static void GenerateCompletion(Task<List<FileSource>> t, object obj)
        {
            GenerateLayer that = (GenerateLayer)obj;
            foreach (FileSource item in t.Result)
            {
                that.outputSet.Queue.Enqueue(item);
                that.outputSet.Sem.Release();
            }
            if (that.inputSet.Sem.WaitOne(timeout))
            {
                FileSource source;
                that.inputSet.Queue.TryDequeue(out source);
                that.generator.GetGenerator(source).ContinueWith(GenerateCompletion, that).Start();
            }
        }
    }
}
