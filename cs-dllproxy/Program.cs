using Spectre.Console;

namespace cs_dllproxy
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1) { Console.WriteLine("Usage: cs-dllproxy.exe [path]"); return; }
            string fileIn = args[0];

            if (!File.Exists(fileIn)) { Console.WriteLine("File not Found"); return; }

            PeNet.PeFile file = new(fileIn);

            Dictionary<string, List<string>> dict = new();
            
            foreach (var entry in file.ImportedFunctions)
            {
                if (entry.Name != "") 
                {
                    if (!dict.ContainsKey(entry.DLL)) { dict[entry.DLL] = new List<string>(); };
                    dict[entry.DLL].Add(entry.Name);
                }
            }

            var selection = AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Select dll to proxy (Must be system library)").AddChoices(dict.Keys));

            string template = "";
            string def = "LIBRARY\nEXPORTS";

            template += "#include <windows.h>\n\n//LOOK UP CALLING CONVENTIONS FOR FUNCTIONS MANUALLY\n";

            foreach (var value in dict[selection])
            {
                template += $"typedef VOID(__stdcall* f_{value})(/*args*/);\n";
                template += $"VOID __stdcall* f{value})(/*args*/) {{f_{value} func = (f_{value})GetProcAddress(LoadLibraryExA(\"{selection}\", NULL, LOAD_LIBRARY_SEARCH_SYSTEM32), \"{value}\"); return func(/*args*/);}}\n\n";
                def += $"\n{value}=f{value}";
            }

            template += "\n";

            template += "void doStuff()\n{\n    return;\n}\n\n";

            template += "BOOL APIENTRY DllMain(HMODULE hModule, DWORD ul_reason_for_call, LPVOID lpReserved)\r\n{\r\n    switch (ul_reason_for_call)\r\n    {\r\n    case DLL_PROCESS_ATTACH:\r\n        CreateThread(NULL, 0, (LPTHREAD_START_ROUTINE)doStuff, NULL, 0, NULL);\r\n        break;\r\n    case DLL_THREAD_ATTACH:\r\n        break;\r\n    case DLL_THREAD_DETACH:\r\n        break;\r\n    case DLL_PROCESS_DETACH:\r\n        break;\r\n    }\r\n    return TRUE;\r\n}";

            File.WriteAllText("proxy.cpp", template);
            File.WriteAllText("proxy.def", def);
        }
    }
}
