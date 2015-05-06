using System;
using System.Collections.Generic;


namespace CCCommon
{
    public class CCEventParam
    {
        // members
        private List<System.Object> mParamList;
        // constractor
        public CCEventParam()
        { }
        // get param
        public virtual System.Object GetParam(Int32 nIndex)
        {
            if (mParamList == null)
            {
                return null;
            }
            if (nIndex < 0 || nIndex >= mParamList.Count)
            {
                return null;
            }
            return mParamList[nIndex];
        }
        // get length
        public Int32 GetParamCount()
        {
            if (mParamList == null)
            {
                return 0;
            }
            return mParamList.Count;
        }
        // get
        public virtual void SetParam(List<System.Object> lsParamList)
        {
            mParamList = lsParamList;
        }
    }
}

