using System;
using System.Collections.Generic;
using CCCommon;

/// <summary>
/// Event Manager
/// </summary>
public class EventManager : CSingleton<EventManager>
{
    #region Members
    public delegate void HandleEvent(CCEventParam cParam);
    private class CRegisterEventInfo
    {
        #region Members
        private UInt32 m_uEventId;
        private List<HandleEvent> m_hHandleList;
        #endregion
        #region Methods
        // constructor
        public CRegisterEventInfo(UInt32 uEventId)
        {
            m_uEventId = uEventId;
            m_hHandleList = null;
        }
        // get event ID
        public UInt32 GetEventId()
        {
            return m_uEventId;
        }
        // remove handle
        public TMSGCODE RemoveHandle(HandleEvent hHandle)
        {
            if (hHandle == null)
            {
                return TMSGCODE.emSys_Null;
            }
            if (m_hHandleList == null)
            {
                return TMSGCODE.emSys_DeleteEmpty;
            }
            Int32 nSize = m_hHandleList.Count;
            Int32 nIndex = 0;
            for (nIndex = 0; nIndex < nSize; ++nIndex)
            {
                if (m_hHandleList[nIndex] == hHandle)
                {
                    m_hHandleList.RemoveAt(nIndex);
                    return TMSGCODE.emSUCCESS;
                }
            }
            return TMSGCODE.emSys_DeleteEmpty;
        }
        // add handle
        public TMSGCODE AddHandle(HandleEvent hHandle)
        {
            if (hHandle == null)
            {
                return TMSGCODE.emSys_Null;
            }
            if (m_hHandleList == null)
            {
                m_hHandleList = new List<HandleEvent>();
            }
            Int32 nSize = m_hHandleList.Count;
            Int32 nIndex = 0;
            for(nIndex = 0; nIndex < nSize; ++nIndex)
            {
                if(m_hHandleList[nIndex] == hHandle)
                {
                    return TMSGCODE.emSys_InsertDup;
                }
            }
            m_hHandleList.Add(hHandle);
            return TMSGCODE.emSUCCESS;
        }
        // hit event
        public void OnEvent(CCEventParam cParam)
        {
            if (m_hHandleList == null)
            {
                return;
            }
            Int32 nSize = m_hHandleList.Count;
            Int32 nIndex = 0;
            for(nIndex = 0; nIndex < nSize; ++nIndex)
            {
                HandleEvent hHandle = m_hHandleList[nIndex];
                if (hHandle == null)
                {
                    continue;
                }
                hHandle(cParam);
            }
        }
        #endregion

    }
    private class CEventInfo : CWaterdrop
    {
        #region Members
        private UInt32 m_uEventId;
        private CCEventParam m_cEventParam;
        #endregion
        #region Methods
        public CEventInfo()
        {
            m_uEventId = 0xFFFFFFFF;
            m_cEventParam = null;
        }
        public UInt32 GetEventId()
        {
            return m_uEventId;
        }
        public CCEventParam GetEventParam()
        {
            return m_cEventParam;
        }
        public override void OnWaterdropBackPool()
        {
            m_uEventId = 0xFFFFFFFF;
            m_cEventParam = null;
        }
        public override void OnWaterdropOutPool()
        {
            m_uEventId = 0xFFFFFFFF;
            m_cEventParam = null;
        }
        public void SetEvent(UInt32 uEventId, CCEventParam cEventParam)
        {
            m_uEventId = uEventId;
            m_cEventParam = cEventParam;
        }
        #endregion
    }
    private List<CRegisterEventInfo> mEventHandleList;
    private List<CEventInfo> mEventList;
    private CCPool<CEventInfo> mEventPool;
    #endregion
    #region Methods
    /// <summary>
    /// Constroctor
    /// </summary>
    public EventManager()
    {
        mEventHandleList = new List<CRegisterEventInfo>();
        mEventList = new List<CEventInfo>();
        mEventPool = new CCPool<CEventInfo>();
    }
    // Unregister Handle
    public TMSGCODE UnRegisterEvent(UInt32 uEventId, HandleEvent hHandle)
    {
        if (uEventId == 0xFFFFFFFF)
        {
            return TMSGCODE.emSys_Invalid;
        }
        if (hHandle == null)
        {
            return TMSGCODE.emSys_Null;
        }
        CRegisterEventInfo cInfo = GetEventInfo(uEventId, false);
        if (cInfo == null)
        {
            return TMSGCODE.emSys_UpdateEmpty;
        }
        TMSGCODE tCode = cInfo.RemoveHandle(hHandle);
        return tCode;
    }
    // Register Handle
    public TMSGCODE RegisterEvent(UInt32 uEventId, HandleEvent hHandle)
    {
        if (uEventId == 0xFFFFFFFF)
        {
            return TMSGCODE.emSys_Invalid;
        }
        if (hHandle == null)
        {
            return TMSGCODE.emSys_Null;
        }
        CRegisterEventInfo cInfo = GetEventInfo(uEventId, true);
        if (cInfo == null)
        {
            return TMSGCODE.emSys_UpdateEmpty;
        }
        TMSGCODE tCode = cInfo.AddHandle(hHandle);
        return tCode;
    }
    // Push Event
    public void PushEvent(UInt32 uEventId, CCEventParam cEventParam)
    {
        if (mEventPool == null)
        { 
            return; 
        }
        CEventInfo cEventInfo = mEventPool.BorrowWaterdrop();
        if (cEventInfo == null)
        {
            return;
        }
        cEventInfo.SetEvent(uEventId, cEventParam);
        if (mEventList == null)
        {
            mEventList = new List<CEventInfo>();
        }
        mEventList.Add(cEventInfo);
    }
    // Call Event Handle
    private void OnEvent(CEventInfo cEventInfo)
    {
        if (cEventInfo == null)
        {
            return;
        }
        UInt32 uEventId = cEventInfo.GetEventId();
        CCEventParam cEventParam = cEventInfo.GetEventParam();
        CRegisterEventInfo cRegEventInfo = GetEventInfo(uEventId, false);
        if (cEventInfo == null)
        {
            return;
        }
        cRegEventInfo.OnEvent(cEventParam);
    }
    // Get Event Info
    private CRegisterEventInfo GetEventInfo(UInt32 uEventId, bool bCreateIfNotExist)
    {
        if (mEventHandleList == null)
        {
            mEventHandleList = new List<CRegisterEventInfo>();
        }
        Int32 nSize = mEventHandleList.Count;
        Int32 nIndex = 0;
        CRegisterEventInfo cNewInfo = null;
        for(nIndex = 0; nIndex < nSize; ++nIndex)
        {
            CRegisterEventInfo info = mEventHandleList[nIndex];
            if (info == null)
            {
                continue;
            }
            UInt32 uCurEventId = info.GetEventId();
            if (uCurEventId == uEventId)
            {
                return info;
            }
            if (bCreateIfNotExist == true && uCurEventId > uEventId)
            {
                cNewInfo = new CRegisterEventInfo(uEventId);
                mEventHandleList.Insert(nIndex, cNewInfo);
                return cNewInfo;
            }
        }
        if (bCreateIfNotExist == true)
        {
            cNewInfo = new CRegisterEventInfo(uEventId);
            mEventHandleList.Add(cNewInfo);
            return cNewInfo;
        }
        return null;
    }

    // Init
    public override void InitSingleton()
    {
        
    }
    // Release
    public override void ReleaseSingleton()
    {
        
    }
    // Update
    public override void OnUpdate()
    {
        if (mEventList == null)
		{
			return;
		}
		Int32 nSize = mEventList.Count;
		for(Int32 i = 0; i < nSize; ++i)
		{
			CEventInfo cInfo = mEventList[i];
			if (cInfo == null)
			{
				continue;
			}
			OnEvent(cInfo);
			mEventPool.BackWaterdrop(cInfo);
		}
		mEventList.Clear();
    }

    #endregion


}
