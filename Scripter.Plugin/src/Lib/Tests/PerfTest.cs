using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace ScripterLang
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class PerfTestDotNet
    {
        private int x1;

        public void Run()
        {
            for(var j = 0; j < 100; j++) {
                x1 = test(x1);
            }
        }

        private static int test(int x2)
        {
            for(var i = 0; i < 100; i++) {
                x2++;
            }
            return x2;
        }
    }

    public static class PerfTest
    {
        public static void Run()
        {
            const string code = @"
export function run() {
    var x1 = 0;

    {
        for(var j = 0; j < 100; j++) {
            var x2 = x1;
            for(var i = 0; i < 100; i++) {
                x2++;
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
            var run = ns.Exports["run"].AsFunction;
            var args = new Value[0];
            for (var i = 0; i < 100; i++)
            {
                run(null, args);
            }
            sw.Start();
            for (var i = 0; i < 100; i++)
            {
                run(null, args);
            }
            sw.Stop();
            var scripterTime = sw.Elapsed.TotalSeconds;
            SuperController.LogMessage($"Scripter: {scripterTime:0.0000}ms");

            var dotnet = new PerfTestDotNet();
            dotnet.Run();
            sw.Reset();
            sw.Start();
            for (var i = 0; i < 100; i++)
            {
                dotnet.Run();
            }
            sw.Stop();

            var nativeTime = sw.Elapsed.TotalSeconds;
            SuperController.LogMessage($"Native: {nativeTime:0.0000}ms");

            SuperController.LogMessage($"Ratio: {scripterTime / nativeTime:0.0} times slower ({nativeTime / scripterTime * 100.0:0.00}% of native speed)");
        }
    }
}

