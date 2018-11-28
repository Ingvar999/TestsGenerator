using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp;
using TestsGeneratorLibrary.Structures;
using TestsGeneratorLibrary;

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
                IEnumerable<FileSource> result = testTemplatesGenerator.GetTestTemplates()
                    .Select(template => new FileSource(Encoding.Default.GetBytes(template.TestClassData), template.TestClassName));
                return result;
            });
        }
    }
}
