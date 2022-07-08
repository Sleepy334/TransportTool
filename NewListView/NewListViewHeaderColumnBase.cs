using ColossalFramework.UI;
using System;
using UnityEngine;

namespace SleepyCommon
{
    public abstract class NewListViewHeaderColumnBase
    {
        public delegate void OnListViewColumnClick(NewListViewHeaderColumnBase eColumn); 
        
        protected OnListViewColumnClick? m_eventClickCallback = null;
        protected NewListViewRowComparer.Columns m_eColumn;
        protected string m_sText;

        public NewListViewHeaderColumnBase(NewListViewRowComparer.Columns eColumn, string sText, OnListViewColumnClick eventClickCallback)
        {
            m_eColumn = eColumn;
            m_sText = sText;
            m_eventClickCallback = eventClickCallback;
        }

        abstract public void Sort(NewListViewRowComparer.Columns eColumn, bool bDescending);

        virtual public void SetText(string sText)
        {
        }

        virtual public void SetTooltip(string sText)
        {
        }

        virtual public bool IsHit(UIComponent component)
        {
            return false;
        }

        abstract public void Destroy();

        public NewListViewRowComparer.Columns GetColumn()
        {
            return m_eColumn;
        }

        protected void OnItemClicked(UIComponent component, UIMouseEventParameter eventParam)
        {
            if (m_eventClickCallback != null)
            {
                m_eventClickCallback(this);
            }
        }
    }
}
