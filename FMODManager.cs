using System;
using System.Runtime.InteropServices;

public class FMODManager  
{
    static FmodInitMode mode;
    public static bool usesStudio => mode == FmodInitMode.CoreAndStudio;
    internal static bool initialized {get; private set;} = false;

    public static void Init
    (
        INativeFMODLibrary nativeFMODLibrary, 
        FmodInitMode _mode, 
        string rootDir, 
        int maxChannels = 256, 
        uint dspBufferLength = 4, 
        int dspBufferCount = 32,
        FMOD.INITFLAGS coreInitFlags = FMOD.INITFLAGS.CHANNEL_LOWPASS | FMOD.INITFLAGS.CHANNEL_DISTANCEFILTER,
        FMOD.Studio.INITFLAGS studioInitFlags = FMOD.Studio.INITFLAGS.NORMAL,
        bool logging = true,
        Action preInitAction = null
    )
    {
        if(initialized) throw new Exception("Stinky ass manager is already initialized fmod momenet");
        initialized = true;
        mode = _mode;
        nativeFMODLibrary.Init(mode, logging);
        if(usesStudio) 
        {
            FMOD.Memory.GetStats(out int currentAllocated, out int maxAllocated);
            FMOD.Studio.System.create(out Sound.fmodStudioSys);
            Sound.fmodStudioSys.getCoreSystem(out Sound.fmodCoreSys);
            preInitAction?.Invoke();
            Sound.fmodStudioSys.initialize(maxChannels, studioInitFlags, coreInitFlags, (IntPtr)0);
            // I'm jaking it i'm jaking it
        }
        else
        {
            FMOD.Factory.System_Create(out Sound.fmodCoreSys);
            preInitAction?.Invoke();
            Sound.fmodCoreSys.init(maxChannels, coreInitFlags, (IntPtr)0);
        }

        Sound.fmodCoreSys.setDSPBufferSize(dspBufferLength, dspBufferCount);
    }

    public static void Update()
    {
        CheckInitialized();
        if(usesStudio) Sound.fmodStudioSys.update();
        else Sound.fmodCoreSys.update();
    }

    public static void CheckInitialized()
    {
        if(!initialized) throw new Exception("The fucking FMODManager isn't inititialized you stupid peice of shit.");
    }
}