using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading.Tasks;

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
            string fileName;
            for (int i = 0; i < maxCount; ++i)
            {
                if (files.TryDequeue(out fileName))
                {
                    FileStream stream = new FileStream(fileName, FileMode.Open);
                    var context = new Context<ReadLayer>(this, new FileSource(stream.Length, fileName), stream);
                    stream.BeginRead(context.Item.Data, 0, context.Item.Data.Length, ReadCompletion, context);
                }
                else
                {
                    break;
                }
            }
        }

        private static void ReadCompletion(IAsyncResult result)
        {
            var context = (Context<ReadLayer>)result.AsyncState;

            context.Stream.EndRead(result);
            context.Stream.Close();
            context.Obj.outputSet.Queue.Enqueue(context.Item);
            context.Obj.outputSet.Sem.Release();

            string fileName;
            if (context.Obj.files.TryDequeue(out fileName))
            {
                FileStream stream = new FileStream(fileName, FileMode.Open);
                var newContext = new Context<ReadLayer>(context.Obj, new FileSource(stream.Length, fileName), stream);
                stream.BeginRead(context.Item.Data, 0, context.Item.Data.Length, ReadCompletion, newContext);
            }
        }
    }
}
