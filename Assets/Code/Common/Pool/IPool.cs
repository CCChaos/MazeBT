using System;
using System.Collections.Generic;

namespace CCCommon
{
    public interface IWaterdropable
    {
        // 获取可循环对象
        void OnWaterdropOutPool();
        // 回收可循环对象
        void OnWaterdropBackPool();
    }

    public abstract class CWaterdrop : IWaterdropable
    {
        #region Members
        private Int32 m_nIndex;
        private Int32 m_nNextIndex;
        #endregion
        // 当前索引
        public Int32 Index { get { return m_nIndex; } set { m_nIndex = value; } }
        // 下个可用元素索引
        public Int32 NextIndex { get { return m_nNextIndex; } set { m_nNextIndex = value; } }

        public abstract void OnWaterdropOutPool();
        public abstract void OnWaterdropBackPool();
    }

    public class CCPool<T> where T : CWaterdrop, new()
    {
        #region PreDefine
        private const Int32 c_nInvalidIndex = -1;
        private Int32 m_nInitLength = 8;
        private Int32 m_nExtendStep = 16;
        #endregion
        #region Memebrs
        private T[] m_tWaterdrops;
        private Int32 m_nCurIndex;
        #endregion
        #region Methods
        // Constructor
        public CCPool()
        {
            Int32 nLength = Extend(m_nInitLength);
            if (nLength <= 0)
            {
                m_nCurIndex = c_nInvalidIndex;
            }
            else 
            {
                m_nCurIndex = 0;
            }
            
        }
        // Constructor with initial length
        public CCPool(Int32 nInitLength)
        {
            m_nInitLength = nInitLength;
            Int32 nLength = Extend(m_nInitLength);
            if (nLength <= 0)
            {
                m_nCurIndex = c_nInvalidIndex;
            }
            else
            {
                m_nCurIndex = 0;
            }
        }
        // Constructor where initial length and extend step
        public CCPool(Int32 nInitLength, Int32 nExtenStep)
        {
            m_nInitLength = nInitLength;
            m_nExtendStep = nExtenStep;
            Int32 nLength = Extend(m_nInitLength);
            if (nLength <= 0)
            {
                m_nCurIndex = c_nInvalidIndex;
            }
            else
            {
                m_nCurIndex = 0;
            }
        }
        // Extend Container Length
        private Int32 Extend(Int32 nAppendLength)
        {
            if (nAppendLength <= 0)
            {
                return -1; 
            }

            Int32 nOriginalLength = 0;
            Int32 nTotalLength = 0;
            Int32 ni = 0;
            Int32 nNexti = 0;
            T tWaterdrop = null;

            T[] tOldWaterdrops = m_tWaterdrops;
            if (tOldWaterdrops != null)
            {
                nOriginalLength = tOldWaterdrops.Length;
            }
            nTotalLength = nOriginalLength + nAppendLength;
            T[] tNewWaterdrops = new T[nTotalLength];
            
            for (ni = 0; ni < nTotalLength; ++ni )
            { 
                if (ni < nOriginalLength)
                {
                    tWaterdrop = tOldWaterdrops[ni];
                    if (tWaterdrop.NextIndex == c_nInvalidIndex)
                    {
                        tWaterdrop.NextIndex = ni + 1;
                    }
                    tNewWaterdrops[ni] = tOldWaterdrops[ni];
                    continue;
                }

                nNexti = ni + 1;
                if (nNexti >= nTotalLength)
                {
                    nNexti = c_nInvalidIndex;
                }
                tWaterdrop = new T();
                tWaterdrop.Index = ni;
                tWaterdrop.NextIndex = nNexti;
                tNewWaterdrops[ni] = tWaterdrop;
            }

            if (m_nCurIndex == c_nInvalidIndex)
            {
                m_nCurIndex = m_nCurIndex + 1;
            }

            m_tWaterdrops = tNewWaterdrops;

            return nTotalLength;
        }
        // 获取水滴
        public T BorrowWaterdrop()
        {
            T tWaterdrop = null;
            if (m_nCurIndex != c_nInvalidIndex)
            {
                tWaterdrop = m_tWaterdrops[m_nCurIndex];
                m_nCurIndex = tWaterdrop.NextIndex;
                return tWaterdrop;    
            }
            Int32 nTotalLength = Extend(m_nExtendStep);
            if (nTotalLength <= 0 || m_nCurIndex == c_nInvalidIndex)
            {
                return null;
            }
            tWaterdrop = m_tWaterdrops[m_nCurIndex];
            m_nCurIndex = tWaterdrop.NextIndex;
            return tWaterdrop;
        }
        // 归还水滴
        public void BackWaterdrop(T tWaterdrop)
        {
            if (tWaterdrop == null)
            {
                return;
            }
            Int32 nIndex = tWaterdrop.Index;
            if (m_tWaterdrops == null || m_tWaterdrops.Length <= nIndex)
            {
                return;
            }
            if (m_tWaterdrops[nIndex] != tWaterdrop)
            {
                return; 
            }

            tWaterdrop.NextIndex = m_nCurIndex;
            m_nCurIndex = nIndex;
        }
        #endregion
    }
}

