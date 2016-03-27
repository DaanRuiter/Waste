
using UnityEngine;

namespace Mythic.Modules
{
    public abstract class ModuleBase
    {
        //DELEGATES
        public delegate void OnModuleEventHandler(ModuleBase module);
        public OnModuleEventHandler OnMoudleStartEvent;
        public OnModuleEventHandler OnModuleUpdateEvent;
        public OnModuleEventHandler OnModuleLoadEvent;

        //MEMBERS
        private string m_name = "undefined";
        private short m_ID = -1;

        private bool m_loadOnStart = true;
        private bool m_enabled = true;

        //COMPONENTS
        public readonly ScriptWrapper ScriptWrapper;

        //PROPERTIES
        public string Name
        {
            get
            {
                return m_name;
            }
        }
        public short ID
        {
            get
            {
                return m_ID;
            }
        }
        public bool LoadOnStart
        {
            get
            {
                return m_loadOnStart;
            }
        }
        public bool Enabled
        {
            get
            {
                return m_enabled;
            }
        }

        public ModuleBase()
        {
            ScriptWrapper = new ScriptWrapper();
        }

        protected void FINISH_INITIALISATION(string moduleName, bool loadOnStart = true)
        {
            m_name = moduleName;
            m_loadOnStart = loadOnStart;
            m_ID = this.GENERATE_MODULE_INDEX();
        }

        //ENGINE EVENTS
        internal virtual void Load() { }
        internal virtual void Start() { }
        internal virtual void Update() { }
        internal virtual void LateUpdate() { }
        internal virtual void FixedUpdate() { }

        //PROTECTED METHODS
        protected T GET_MODULE<T>(string nameSpecific = "undefined") where T : ModuleBase
        {
            return this.FIND_MODULE_OF_TYPE_AND_NAME<T>(nameSpecific);
        }

        //OPERATORS
        public static bool operator == (ModuleBase a, ModuleBase b)
        {
            if (ReferenceEquals(a, b))
                return true;
            
            if (((object)a == null) || ((object)b == null))
                return false;
            
            return a.ID == b.ID ? true : false;
        }
        public static bool operator != (ModuleBase a, ModuleBase b)
        {
            if (((object)a == null) && ((object)b == null))
                return false;

            if ((object)a != null && (object)b == null)
                return true;

            return !(a.ID == b.ID);
        }
        public override string ToString()
        {
            string status = "enabled";
            if (!m_enabled)
                status = "disabled";
            return string.Format("{0} - {1} | {2}", ID, Name, status);
        }
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}