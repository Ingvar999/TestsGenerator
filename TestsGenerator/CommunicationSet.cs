using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;

namespace ConsoleApp
{
    public class CommunicationSet<T>
    {
        public Semaphore Sem { get; set; }
        public ConcurrentQueue<T> Queue { get; set; }
        public bool IsWorking { get; set; }

        public CommunicationSet(int maxValue)
        {
            Sem = new Semaphore(maxValue, maxValue);
            Queue = new ConcurrentQueue<T>();
            IsWorking = false;
        }
    }
}
