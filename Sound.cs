using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using FMOD;
using FMOD.Studio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;


public class Sound
{
    public static FMOD.Studio.System fmodStudioSys;
    public static FMOD.System fmodCoreSys;
    static List<EventInstance> eventInstances = new List<EventInstance>();
    //List<StudioEventEmitter> eventEmitters = new List<StudioEventEmitter>();
    

    static Dictionary<string, EventDescription> eventDictionary = new Dictionary<string, EventDescription>();


    public static void Initialize()
    {
        FMODManager.Init(new NativeFmodLibrary(), FmodInitMode.CoreAndStudio, "Content");
        //FMOD.Debug.Initialize(DEBUG_FLAGS.LOG, DEBUG_MODE.TTY, filename: "Goblin");
    }

    public static void Load()
    {
        ImportEvents();
    }

    public static void Update()
    {
        FMODManager.Update();
    }

    public static void UnLoad()
    {
        fmodStudioSys.release();
        fmodCoreSys.release();
    }
    
    //Adds all audio Events in the FMOD project to our eventDictionary so they can be refrenced later 
    static void ImportEvents()
    {
        //if (bankPath == null || bankPath == "") return;

        fmodStudioSys.loadBankFile("Content/Master.bank", LOAD_BANK_FLAGS.DECOMPRESS_SAMPLES, out Bank bank);
        fmodStudioSys.loadBankFile("Content/Master.strings.bank", LOAD_BANK_FLAGS.DECOMPRESS_SAMPLES, out Bank stringsBank);
        bank.loadSampleData();
        stringsBank.loadSampleData();
        

        EventDescription[] eventDescs = new EventDescription[0];
        
        bank.getEventList(out eventDescs); //Get all the events inside of the audio bank
        
        
        foreach(EventDescription eventDesc in eventDescs) 
        {
            string eventPath = "";
            eventDesc.getPath(out eventPath);
            if(eventPath == "") return;
            System.Diagnostics.Debug.WriteLine("Event file path: " + eventPath);

            string eventName = eventPath.Substring(eventPath.LastIndexOf('/') + 1).ToLower(); //This is the name that we will reference in other scripts, makes things easier on us
            
            eventDictionary.Add(eventName, eventDesc);
        }
    }
    
    //Searches our eventDictionary for audioReferences with a matching name and returns the first one found.
    public static bool TryGetEventDesc(string name, out EventDescription eventDescription)
    {
        if (eventDictionary.TryGetValue(name.ToLower(), out EventDescription v))
        {
            eventDescription = v;
            return true;
        }
        eventDescription = v;
        
        
        System.Diagnostics.Debug.WriteLine($"Event Reference: {name} could not be found.");
        return false;
    }

    public static void Play(string sound, Vector2 pos)
    {
        if(TryGetEventDesc(sound, out EventDescription eventDescription))
        {
            eventDescription.createInstance(out EventInstance eventInstance);


            ATTRIBUTES_3D attributes = new ATTRIBUTES_3D();
            VECTOR _pos = new VECTOR();
            _pos.x = pos.X;
            _pos.y = pos.Y;

            attributes.position = _pos;

            eventInstance.set3DAttributes(attributes);
            eventInstance.start();
            eventInstance.release();
        }
    }
}