## cs-dllproxy

Create C++ dll proxy template using PE file import table
<br><br>
Uses LoadLibraryExA and GetProcAddress to redirect function calls to sys32 dll at runtime.<br>
Generated function calls do not have their corresponding arguments and return types set per default. They need to be set manually by looking up msdn documentation. Replace /\*args\*/ and function return types
<br><br>
*barely tested*

## Usage
```
cs-dllproxy.exe <filePath>
```
## Example Output
> cpp file
```C++
#include <windows.h>

//LOOK UP CALLING CONVENTIONS FOR FUNCTIONS MANUALLY
typedef VOID(__stdcall* f_SetupDiEnumDeviceInfo(/*args*/);
VOID __stdcall fSetupDiEnumDeviceInfo(/*args*/) {f_SetupDiEnumDeviceInfo func = (f_SetupDiEnumDeviceInfo)GetProcAddress(LoadLibraryExA("SETUPAPI.dll", NULL, LOAD_LIBRARY_SEARCH_SYSTEM32), "SetupDiEnumDeviceInfo"); return func(/*args*/);}

typedef VOID(__stdcall* f_SetupDiDestroyDeviceInfoList)(/*args*/);
VOID __stdcall fSetupDiDestroyDeviceInfoList(/*args*/) {f_SetupDiDestroyDeviceInfoList func = (f_SetupDiDestroyDeviceInfoList)GetProcAddress(LoadLibraryExA("SETUPAPI.dll", NULL, LOAD_LIBRARY_SEARCH_SYSTEM32), "SetupDiDestroyDeviceInfoList"); return func(/*args*/);}

typedef VOID(__stdcall* f_SetupDiGetDeviceRegistryPropertyA)(/*args*/);
VOID __stdcall fSetupDiGetDeviceRegistryPropertyA(/*args*/) {f_SetupDiGetDeviceRegistryPropertyA func = (f_SetupDiGetDeviceRegistryPropertyA)GetProcAddress(LoadLibraryExA("SETUPAPI.dll", NULL, LOAD_LIBRARY_SEARCH_SYSTEM32), "SetupDiGetDeviceRegistryPropertyA"); return func(/*args*/);}

typedef VOID(__stdcall* f_SetupDiGetClassDevsW)(/*args*/);
VOID __stdcall fSetupDiGetClassDevsW(/*args*/) {f_SetupDiGetClassDevsW func = (f_SetupDiGetClassDevsW)GetProcAddress(LoadLibraryExA("SETUPAPI.dll", NULL, LOAD_LIBRARY_SEARCH_SYSTEM32), "SetupDiGetClassDevsW"); return func(/*args*/);}


void doStuff()
{
    return;
}

BOOL APIENTRY DllMain(HMODULE hModule, DWORD ul_reason_for_call, LPVOID lpReserved)
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
        CreateThread(NULL, 0, (LPTHREAD_START_ROUTINE)doStuff, NULL, 0, NULL);
        break;
    case DLL_THREAD_ATTACH:
        break;
    case DLL_THREAD_DETACH:
        break;
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}
```

> def file
```
LIBRARY
EXPORTS
SetupDiEnumDeviceInfo=fSetupDiEnumDeviceInfo
SetupDiDestroyDeviceInfoList=fSetupDiDestroyDeviceInfoList
SetupDiGetDeviceRegistryPropertyA=fSetupDiGetDeviceRegistryPropertyA
SetupDiGetClassDevsW=fSetupDiGetClassDevsW
```
