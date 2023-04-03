using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ScripterLang
{
    public class Program
    {
        private const string _indexModuleName = "./index.js";

        public readonly GlobalLexicalContext globalContext = new GlobalLexicalContext();

        private IModule _index;

        public IModule RegisterFile(string fileName, string source)
        {
            var localModuleName = "./" + fileName;
            globalContext.RemoveModule(localModuleName);
            var tokens = new List<Token>(Tokenizer.Tokenize(source));
            var module = new Parser(tokens).Parse(globalContext, localModuleName);
            Register(module);
            return module;
        }

        private void Register(IModule module)
        {
            globalContext.DeclareModule(module);
            if (module.ModuleName == _indexModuleName)
                _index = module;
            globalContext.InvalidateModules();
        }

        public void Unregister(string moduleName)
        {
            globalContext.RemoveModule(moduleName);
            globalContext.InvalidateModules();
        }

        [SuppressMessage("ReSharper", "UnusedMethodReturnValue.Global")]
        public Value Run()
        {
            if (_index == null) throw new NullReferenceException("There was no index.js script registered in the program");
            return _index.Import().returned;
        }

        public bool CanRun()
        {
            return _index != null;
        }
    }
}
