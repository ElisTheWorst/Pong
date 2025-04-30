using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;


/// Windows and Linux-specific part of an audio manager.
public class NativeFmodLibrary : INativeFMODLibrary
{
    private bool _loggingEnabled;

    public void Init(FmodInitMode mode, bool loggingEnabled = false)
    {
        _loggingEnabled = loggingEnabled;
        NativeLibrary.SetDllImportResolver(
            mode.GetType().Assembly, // Scoping the dllimport switcher for the crossplatform fmod lib.
            (libraryName, assembly, dllImportSearchPath) =>
            {
                libraryName = Path.GetFileNameWithoutExtension(libraryName);
                if (dllImportSearchPath == null)
                {
                    dllImportSearchPath = DllImportSearchPath.AssemblyDirectory;
                }
                Debug.WriteLine(dllImportSearchPath);
                return NativeLibrary.Load(SelectDefaultLibraryName(libraryName, _loggingEnabled), assembly, dllImportSearchPath);
            }
        );
    }


    private string SelectDefaultLibraryName(string libName, bool loggingEnabled = false)
    {
        string name;

        if (OperatingSystem.IsWindows())
        {
            name = loggingEnabled ? $"{libName}L.dll" : $"{libName}.dll";
        }
        else if (OperatingSystem.IsLinux() || OperatingSystem.IsAndroid())
        {
            name = loggingEnabled ? $"lib{libName}L.so" : $"lib{libName}.so";
        }
        else
        {
            throw new PlatformNotSupportedException();
        }

        return name;
    }
}

public enum FmodInitMode
{
    /// <summary>
    /// Initializes only the core system.
    /// </summary>
    Core,

    /// <summary>
    /// Initializes the core and the studio system.
    /// </summary>
    CoreAndStudio
}
