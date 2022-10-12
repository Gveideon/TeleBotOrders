using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;

namespace TeleBotOrders
{
    internal static class Parser
    {
        private static ScriptEngine _engine;
        private static ScriptScope _scope;
        private static string url = "https://dubna-china.ru";

        public static dynamic GetCafe()
        {
            _engine = Python.CreateEngine();
            _scope = _engine.CreateScope();
            var searchPaths = _engine.GetSearchPaths();
            //searchPaths.Add(@"..\..\..\app\Parser_for_CE.py");
            searchPaths.Add(@"D:\Work\Programms\CSharp\TelegramBot\TeleBotOrders\app");
            searchPaths.Add(@"D:\Work\Programms\CSharp\TelegramBot\TeleBotOrders\app\venv\Lib\site-packages\bs4\__init__.py");
            searchPaths.Add(@"..\..\..\app\bs4\");
            _engine.SetSearchPaths(searchPaths);
            var path = @"..\..\..\app\Parser.py";
            _engine.ExecuteFile(path, _scope);
            var typeParser = _scope.GetVariable("Parser");
            var parser = _engine.Operations.CreateInstance(typeParser, url);
            var cafe = parser.get_cafe();
            Cafe staticTypeCafe = DynamicToStatic(cafe);
            return cafe;
        }

        private static Cafe DynamicToStatic(dynamic @object)
        {
            Cafe cafe = JsonSerializer.Deserialize(JsonSerializer.Serialize(@object));
            return cafe;
        }
    }
}
