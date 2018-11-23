using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class ConsoleApp
    {
        static void Main(string[] args)
        {
            string inputPath = @"D:\git\СПП\TestsGenerator\Input\";
            string outputPath = @"D:\git\СПП\TestsGenerator\Output\";
            int maxReadTasksCount = 3;
            int maxWriteTasksCount = 3;
            int maxProcessesTasksCount = 3;

            var files = new ConcurrentQueue<string>(Directory.GetFiles(inputPath, "*.cs"));
            var readGenerateSet = new CommunicationSet<FileSource>(10);
            var writeGenerateSet = new CommunicationSet<FileSource>(10);
            ReadLayer reader = new ReadLayer(maxReadTasksCount, files, readGenerateSet);
            GenerateLayer generate = new GenerateLayer(maxProcessesTasksCount, readGenerateSet, writeGenerateSet);
            WriteLayer writer = new WriteLayer(maxWriteTasksCount, writeGenerateSet, outputPath);
            reader.Start();
            generate.Start();
            writer.Start();
            Console.ReadKey();
        }
    }
}
