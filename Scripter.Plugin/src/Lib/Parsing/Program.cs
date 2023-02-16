using System;
using System.Collections.Generic;

namespace ScripterLang
{
    public class Program
    {
        private const string _indexModuleName = "./index.js";

        public readonly GlobalLexicalContext GlobalContext = new GlobalLexicalContext();

        private IModule _index;

        public IModule Register(string moduleName, string source)
        {
            var tokens = new List<Token>(Tokenizer.Tokenize(source));
            var module = new Parser(tokens).Parse(GlobalContext, "./" + moduleName);
            Register(module);
            return module;
        }

        public void Register(IModule module)
        {
            GlobalContext.RemoveModule(module.ModuleName);
            GlobalContext.DeclareModule(module);
            if (module.ModuleName == _indexModuleName)
                _index = module;
            GlobalContext.InvalidateModules();
        }

        public void Unregister(string moduleName)
        {
            GlobalContext.RemoveModule(moduleName);
            GlobalContext.InvalidateModules();
        }

        public Value Run()
        {
            if (_index == null) throw new NullReferenceException("There was no index.js script registered in the program");
            return _index.Import().Returned;
        }
    }
}
