﻿using System.Text;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Generator;

namespace TestsGeneratorTest
{ 
    [TestClass]
    public class GeneratorTests
    {
        private CompilationUnitSyntax compilationUnitSyntax;

        public void SetUp(string name)
        {
            FileSource file = new FileSource(File.ReadAllBytes($"{name}.cs.testable"), "");
            var generator = new TestsGenerator();
            var t = generator.GetGenerator(file);
            t.Wait();
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(Encoding.Default.GetString(t.Result.First().Data));
            compilationUnitSyntax = syntaxTree.GetCompilationUnitRoot();
        }

        [TestMethod]
        public void UnitUsingDirectiveTest()
        {
            SetUp("SimpleClass");
            IEnumerable<UsingDirectiveSyntax> NUnitUsingDirective =
                from usingDirective in compilationUnitSyntax.DescendantNodes().OfType<UsingDirectiveSyntax>()
                where usingDirective.Name.ToString() == "Microsoft.VisualStudio.TestTools.UnitTesting"
                select usingDirective;

            Assert.IsNotNull(NUnitUsingDirective.FirstOrDefault());
        }

        [TestMethod]
        public void UnitUsingDirectiveTest2()
        {
            SetUp("SimpleClass");
            IEnumerable<UsingDirectiveSyntax> NUnitUsingDirective =
                from usingDirective in compilationUnitSyntax.DescendantNodes().OfType<UsingDirectiveSyntax>()
                where usingDirective.Name.ToString() == "ConsoleApp"
                select usingDirective;

            Assert.IsNotNull(NUnitUsingDirective.FirstOrDefault());
        }

        [TestMethod]
        public void SomeClassNamespaceTest()
        {
            SetUp("SimpleClass");
            IEnumerable<NamespaceDeclarationSyntax> Namespace =
                from namespaceDeclaration in compilationUnitSyntax.DescendantNodes().OfType<NamespaceDeclarationSyntax>()
                where namespaceDeclaration.Name.ToString() == "ConsoleApp.Tests"
                select namespaceDeclaration;

            Assert.IsNotNull(Namespace.FirstOrDefault());
        }

        [TestMethod]
        public void ClassMethodsCountTest()
        {
            SetUp("SimpleClass");
            IEnumerable<MethodDeclarationSyntax> methods =
                from methodDeclaration in compilationUnitSyntax.DescendantNodes().OfType<MethodDeclarationSyntax>()
                select methodDeclaration;

            Assert.IsTrue(methods.Count() == 2);
        }

        [TestMethod]
        public void ClassNameTest()
        {
            SetUp("SimpleClass");
            IEnumerable<ClassDeclarationSyntax> className =
                from classDeclaration in compilationUnitSyntax.DescendantNodes().OfType<ClassDeclarationSyntax>()
                where classDeclaration.Identifier.ValueText == "SimpleClassTests"
                select classDeclaration;

            Assert.IsNotNull(className.FirstOrDefault());
        }


        [TestMethod]
        public void MethodAssertFailTest()
        {
            SetUp("SimpleClass");
            IEnumerable<MethodDeclarationSyntax> method =
                from methodDeclaration in compilationUnitSyntax.DescendantNodes().OfType<MethodDeclarationSyntax>()
                where methodDeclaration.Identifier.ValueText == "PublicMethodTest"
                select methodDeclaration;

            IEnumerable<MemberAccessExpressionSyntax> asserts =
                from assertDeclaration in method.FirstOrDefault().Body.DescendantNodes().OfType<MemberAccessExpressionSyntax>()
                where assertDeclaration.Expression.ToString() == "Assert"
                select assertDeclaration;

            Assert.IsNotNull(asserts.FirstOrDefault());
        }

        [TestMethod]
        public void ClassesCountTest()
        {
            SetUp("EmptyClass");
            IEnumerable<ClassDeclarationSyntax> classes =
                from classDeclaration in compilationUnitSyntax.DescendantNodes().OfType<ClassDeclarationSyntax>()
                where classDeclaration.Identifier.ValueText == "EmptyClassTests"
                select classDeclaration;

            Assert.IsNotNull(classes.FirstOrDefault());
        }
    }
}
