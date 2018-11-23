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
        private const int timeout = 1000;
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

        public async void Start()
        {
            for (int i = 0; i < maxCount; ++i)
            {
                if (inputSet.Sem.WaitOne(timeout))
                {
                    FileSource source;
                    inputSet.Queue.TryDequeue(out source);
                    var t = generator.GetGenerator(source);
                    t.Start();
                    await t.ContinueWith(GenerateCompletion, this);
                }
                else
                {
                    break;
                }
            }
        }

        static async void GenerateCompletion(Task<List<FileSource>> t, object obj)
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
                var task = that.generator.GetGenerator(source);
                task.Start();
                await task.ContinueWith(GenerateCompletion, that);
            }
        }
    }
}
