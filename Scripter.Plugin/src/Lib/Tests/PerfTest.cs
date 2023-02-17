using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace ScripterLang
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class PerfTestDotNet
    {
        private int x1;

        public void Run(PerfObject obj)
        {
            for(var j = 0; j < 100; j++)
            {
                var x2 = x1 + obj.Value;
                x2 = test(x2);
                x1 = x2;
            }
        }

        private static int test(int x2)
        {
            for(var i = 0; i < 100; i++) {
                x2++;
            }
            return x2;
        }

        public class PerfObject
        {
            public int Value => 1;
        }
    }

    public class PerfObject : ObjectReference
    {
        public override Value GetProperty(string name)
        {
            switch (name)
            {
                case "value":
                    return 1;
                default:
                    return base.GetProperty(name);
            }
        }
    }

    public static class PerfTest
    {
        public static void Run()
        {
            const string code = @"
export function run(obj) {
    var x1 = 0;

    {
        for(var j = 0; j < 100; j++) {
            var x2 = x1 + obj.value;
            for(var i = 0; i < 100; i++) {
                x2++;
                x2 = x2;
            }
            x1 = x2;
        }
    }
}
";
            var sw = new Stopwatch();
            SuperController.singleton.ClearMessages();

            var program = new Program();
            var module = program.Register("index.js", code);
            var ns = module.Import();
            var run = ns.exports["run"].AsFunction;
            var jsObj = new PerfObject();
            var jsArgs = new Value[] { jsObj };
            for (var i = 0; i < 100; i++)
            {
                run(null, jsArgs);
            }
            sw.Start();
            for (var i = 0; i < 100; i++)
            {
                run(null, jsArgs);
            }
            sw.Stop();
            var scripterTime = sw.Elapsed.TotalSeconds;
            SuperController.LogMessage($"Scripter: {scripterTime:0.0000}ms");

            var dotnet = new PerfTestDotNet();
            var dnObj = new PerfTestDotNet.PerfObject();
            dotnet.Run(dnObj);
            sw.Reset();
            sw.Start();
            for (var i = 0; i < 100; i++)
            {
                dotnet.Run(dnObj);
            }
            sw.Stop();

            var nativeTime = sw.Elapsed.TotalSeconds;
            SuperController.LogMessage($"Native: {nativeTime:0.0000}ms");

            SuperController.LogMessage($"Ratio: {scripterTime / nativeTime:0.0} times slower ({nativeTime / scripterTime * 100.0:0.00}% of native speed)");
        }
    }
}

