using System;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;

using DG.Tweening;

using Mythic.Modules;
using Mythic.Engine;
using Mythic;

namespace Mythic.Engine
{
    public class Main : MonoBehaviour
    {
        internal static short MODULE_MAX_INDEX = -1;
        public readonly static List<ModuleBase> Modules = new List<ModuleBase>();

        //UNITY EVENTS
        //Start the engine
        private void Start()
        {
            //Setup Console
            CMD.Init(gameObject);
            CMD.enableKeyCode = KeyCode.Tab;

            //Find References to modules
            Type[] moduleTypes = FindModules();

            //Setup modules
            ModuleBase[] modules = SetupModules(moduleTypes);

            //Load Base Modules
            for (int i = 0; i < modules.Length; i++)
                if (modules[i].LoadOnStart)
                    LoadModule(modules[i]);

            //Start modules
            for (int i = 0; i < Modules.Count; i++)
            {
                Modules[i].Start();
                Modules[i].ScriptWrapper.StartScripts();
            }

            string moduleNames = "Loaded Modules:\n";
            for (int i = 0; i < Modules.Count; i++)
            {
                moduleNames += Modules[i].Name + "\n";
            }
            Debug.Log(moduleNames);

            DOTween.Init();
        }

        //Update the engine
        private void Update()
        {
            if (Modules == null)
                return;

            for (int i = 0; i < Modules.Count; i++)
            {
                Modules[i].Update();
            }

            //If playing
            for (int i = 0; i < Unity.UNITY_SCRIPTS.Count; i++)
            {
                Unity.UNITY_SCRIPTS[i].OnTick();
            }

            Timer.UPDATE_TIMERS();
        }

        private void LateUpdate()
        {
            if (Modules == null)
                return;

            for (int i = 0; i < Modules.Count; i++)
            {
                Modules[i].LateUpdate();
            }
        }
        private void FixedUpdate()
        {
            if (Modules == null)
                return;

            for (int i = 0; i < Modules.Count; i++)
            {
                Modules[i].FixedUpdate();
            }
        }

        //INTERNAL EVENTS
        private Type[] FindModules()
        {
            Assembly assembly = Assembly.GetAssembly(typeof(Main));
            Type[] allTypes = assembly.GetTypes();
            List<Type> modules = new List<Type>();
            for (int i = 0; i < allTypes.Length; i++)
                if (allTypes[i].BaseType == typeof(ModuleBase))
                    modules.Add(allTypes[i]);

            return modules.ToArray();
        }
        private ModuleBase[] SetupModules(Type[] moduleTypes)
        {
            ModuleBase[] modules = new ModuleBase[moduleTypes.Length];
            for (int i = 0; i < moduleTypes.Length; i++)
                modules[i] = Activator.CreateInstance(moduleTypes[i]) as ModuleBase;
            return modules;
        }

        //PUBLIC METHODS
        public ModuleBase SetupModule(Type moduleType)
        {
            if(moduleType == typeof(ModuleBase))
                return Activator.CreateInstance(moduleType) as ModuleBase;

            CMD.Error("Module type is not type of ModuleBase");
            return null;
        }
        public void LoadModule(ModuleBase module)
        {
            if (module != null)
            {
                module.Load();
                Modules.Add(module);
            }
        }
        public static T FindModule<T>(string nameSpecific = "undefined") where T : ModuleBase
        {
            for (int i = 0; i < Modules.Count; i++)
            {
                if (Modules[i].GetType() == typeof(T))
                {
                    if (nameSpecific != "undefined")
                    {
                        if (Modules[i].Name == nameSpecific)
                            return Modules[i] as T;
                    }
                    else
                        return Modules[i] as T;
                }
            }
            return default(T);
        }
    }
}

namespace UnityEngine
{
    //UNITY CODE
    public interface IUnityScript
    {
        void AddComponents();
        void OnStart();
        void OnTick();
        string ObjectName { get; }
    }

    public class ScriptWrapper
    {
        private List<IUnityScript> m_scripts = new List<IUnityScript>();

        public MonoBehaviour AddScript(IUnityScript script)
        {
            m_scripts.Add(script);
            return script as MonoBehaviour;
        }

        public T GetScript<T>() where T : MonoBehaviour, IUnityScript
        {
            for (int i = 0; i < m_scripts.Count; i++)
            {
                if (m_scripts[i].GetType() == typeof(T))
                    return m_scripts[i] as T;
            }
            return null;
        }
        public IUnityScript GetScript(int index)
        {
            return m_scripts[index];
        }

        public void StartScripts()
        {
            for (int i = 0; i < m_scripts.Count; i++)
            {
                m_scripts[i].OnStart();
            }
        }

        public int Count
        {
            get
            {
                return m_scripts.Count;
            }
        }
    }

    public static class Unity
    {
        public static List<IUnityScript> UNITY_SCRIPTS = new List<IUnityScript>();

        public static Script Initialize<Script>(GameObject gameObject = null) where Script : MonoBehaviour, IUnityScript
        {
            GameObject result = gameObject;
            if (result == null)
                result = new GameObject();

            Script script = result.AddComponent<Script>();
            script.AddComponents();
            
            UNITY_SCRIPTS.Add(script);

            if(gameObject == null)
                result.name = script.ObjectName;

            return script;
        }
    }
}

namespace Mythic.Modules
{
    public static class ModuleExtensions
    {
        internal static short GENERATE_MODULE_INDEX(this ModuleBase module)
        {
            Engine.Main.MODULE_MAX_INDEX++;
            return Engine.Main.MODULE_MAX_INDEX;
        }
        internal static T FIND_MODULE_OF_TYPE_AND_NAME<T>(this ModuleBase module, string nameSpecific = "undefined") where T : ModuleBase
        {
            return Main.FindModule<T>(nameSpecific);
        }
    }
}