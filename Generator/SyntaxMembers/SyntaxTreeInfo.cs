using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generator
{
    public class SyntaxTreeInfo
    {
        public IEnumerable<ClassInfo> Classes { get; }

        public SyntaxTreeInfo(IEnumerable<ClassInfo> classes)
        {
            Classes = classes;
        }
    }
}
