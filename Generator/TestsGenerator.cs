using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generator
{
    public class TestsGenerator
    {
        public Task<IEnumerable<FileSource>> GetGenerator(FileSource source)
        {
            return Task<IEnumerable<FileSource>>.Factory.StartNew(delegate {
                SyntaxTreeInfoBuilder syntaxTreeInfoBuilder = new SyntaxTreeInfoBuilder(Encoding.Default.GetString(source.Data));
                SyntaxTreeInfo syntaxTreeInfo = syntaxTreeInfoBuilder.GetSyntaxTreeInfo();

                TestClassTemplateGenerator testTemplatesGenerator = new TestClassTemplateGenerator(syntaxTreeInfo);
                return testTemplatesGenerator.GetTestTemplates();
            });
        }
    }
}
