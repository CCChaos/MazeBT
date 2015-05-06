using System;
using System.Collections.Generic;


namespace CCCommon
{
    public class CSingletonRegister
    {
        #region Members
        private static List<ISingleton> m_lRegisterSingle;
        #endregion
        #region Methods
        public CSingletonRegister()
        {
            m_lRegisterSingle = new List<ISingleton>();
        }
        public static TMSGCODE RegisterSingletion(ISingleton cSingleton)
        {
            if (m_lRegisterSingle == null)
            {
                return TMSGCODE.emSys_Null;
            }
            if (m_lRegisterSingle.Contains(cSingleton) == true)
            {
                return TMSGCODE.emSys_InsertDup;
            }
            m_lRegisterSingle.Add(cSingleton);

            return TMSGCODE.emSUCCESS;

        }
        public static void OnUpdate()
        {
            if (m_lRegisterSingle == null)
            {
                return;
            }
            Int32 nSize = m_lRegisterSingle.Count;
            Int32 nIndex = 0;
            for(nIndex = 0; nIndex < nSize; ++nIndex)
            {
                ISingleton iSingleton = m_lRegisterSingle[nIndex];
                if (iSingleton == null)
                {
                    continue;
                }
                iSingleton.OnUpdate();
            }
        }
        #endregion

    }
}
