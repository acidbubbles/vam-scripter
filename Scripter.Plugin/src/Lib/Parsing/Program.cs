using System;
using System.Collections.Generic;

namespace ScripterLang
{
    public class Program
    {
        public const string IndexModuleName = "./index.js";

        public readonly GlobalLexicalContext GlobalContext = new GlobalLexicalContext();

        private IModule _index;

        public IModule Add(string moduleName, string source)
        {
            var tokens = new List<Token>(Tokenizer.Tokenize(source));
            var module = new Parser(tokens).Parse(GlobalContext, "./" + moduleName);
            Add(module);
            return module;
        }

        public void Add(IModule module)
        {
            GlobalContext.DeclareModule(module.ModuleName, module);
            if (module.ModuleName == IndexModuleName)
                _index = module;
            GlobalContext.InvalidateModules();
        }

        public void Remove(string moduleName)
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
