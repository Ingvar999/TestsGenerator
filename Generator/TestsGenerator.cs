using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp;

namespace Generator
{
    public class TestsGenerator
    {
        public Task<List<FileSource>> GetGenerator(FileSource source)
        {
            return new Task<List<FileSource>>(GenerateTests, source);
        }

        private List<FileSource> GenerateTests(object state)
        {
            List<FileSource> result = new List<FileSource>();
            return result;
        }
    }
}
