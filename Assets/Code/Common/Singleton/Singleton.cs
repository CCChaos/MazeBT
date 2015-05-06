using System;

namespace CCCommon
{
    // Singleton
    public abstract class CSingleton<T> : ISingleton
        where T : class , ISingleton, new()
    {
        #region Members
        private static T s_tInstance;
        #endregion
        #region Methods
        // Constructor
        public CSingleton()
        {
        }
        // Get Instance
        public static T GetInstance()
        {
            if (s_tInstance == null)
            {
                s_tInstance = new T();
                CSingletonRegister.RegisterSingletion(s_tInstance);
            }
            return s_tInstance;
        }
        //init
        public abstract void InitSingleton();
        // clear
        public abstract void ReleaseSingleton();
        // update
        public abstract void OnUpdate();
        #endregion
    }
}
