using System;
using System.Diagnostics;
using ScripterLang;

public static class PerfTest
{
    public static void Run()
    {
        const int iterations = 100;
        const string code = @"
export function run() {
    var x1 = 0;
    function test(x2) {
        for(var i = 0; i < 100; i++) {
            x2++;
        }
        return x2;
    }
    {
        for(var j = 0; j < 100; j++) {
            x1 = test(x1);
        }
    }
}
";
        var program = new Program();
        var module = program.Add("index.js", code);
        var ns = module.Import();
        var run = ns.Exports["run"].AsFunction;
        var args = new Value[0];
        run(null, args);
        var sw = new Stopwatch();
        sw.Start();
        for (var i = 0; i < iterations; i++)
        {
            run(null, args);
        }
        sw.Stop();
        SuperController.LogMessage($"Scripter: Ran {iterations} iterations in {sw.Elapsed.TotalSeconds:0.0000}ms");
    }
}
