using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class WriteLayer
    {
        private const int timeout = 1000;
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
                if (inputSet.Sem.WaitOne(timeout))
                {
                    FileSource source;
                    inputSet.Queue.TryDequeue(out source);
                    FileStream stream = new FileStream(outputFolder + source.FileName, FileMode.OpenOrCreate);
                    var context = new Context<WriteLayer>(this, source, stream);
                    stream.BeginWrite(source.Data, 0, source.Data.Length, WriteCompletion, context);
                }
                else
                {
                    break;
                }
            }
        }

        private static void WriteCompletion(IAsyncResult result)
        {
            var context = (Context<WriteLayer>)result.AsyncState;
            context.Stream.EndWrite(result);
            context.Stream.Close();

            if (context.Obj.inputSet.Sem.WaitOne(timeout))
            {
                FileSource source;
                context.Obj.inputSet.Queue.TryDequeue(out source);
                FileStream stream = new FileStream(context.Obj.outputFolder + source.FileName, FileMode.OpenOrCreate);
                var newContext = new Context<WriteLayer>(context.Obj, source, stream);
                stream.BeginWrite(source.Data, 0, source.Data.Length, WriteCompletion, newContext);
            }
        }
    }
}
