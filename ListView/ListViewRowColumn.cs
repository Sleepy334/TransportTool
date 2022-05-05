using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PublicTransportInfo
{
    public class ListViewRowColumn
    {
        private UILabelLiveTooltip? m_lblColumn = null;
        private MouseEventHandler? m_eventClickCallback = null;
        private OnGetColumnTooltip? m_GetColumnTooltip = null;
        private ListViewRowComparer.Columns m_eColumn;

        public delegate string OnGetColumnTooltip(ListViewRowColumn oColumn);

        public static ListViewRowColumn Create(ListViewRowComparer.Columns eColumn, UIComponent parent, string sText, string sTooltip, int iWidth, int iHeight, UIHorizontalAlignment oTextAlignment, UIAlignAnchor oAncor, MouseEventHandler eventClickCallback, OnGetColumnTooltip getColumnTooltip)
        {
            ListViewRowColumn oColumn = new ListViewRowColumn();
            oColumn.Setup(eColumn, parent, sText, sTooltip, iWidth, iHeight, oTextAlignment, oAncor, eventClickCallback, getColumnTooltip);
            return oColumn;
        }

        private void Setup(ListViewRowComparer.Columns eColumn, UIComponent parent, string sText, string sTooltip, int iWidth, int iHeight, UIHorizontalAlignment oTextAlignment, UIAlignAnchor oAncor, MouseEventHandler eventClickCallback, OnGetColumnTooltip getColumnTooltip)
        {
            m_eColumn = eColumn;
            m_eventClickCallback = eventClickCallback;
            m_GetColumnTooltip = getColumnTooltip;
            m_lblColumn = parent.AddUIComponent<UILabelLiveTooltip>();
            m_lblColumn.name = eColumn.ToString();
            m_lblColumn.text = sText;
            m_lblColumn.tooltip = sTooltip;
            m_lblColumn.textAlignment = oTextAlignment;// UIHorizontalAlignment.Center;
            m_lblColumn.verticalAlignment = UIVerticalAlignment.Middle;
            m_lblColumn.autoSize = false;
            m_lblColumn.height = iHeight;
            m_lblColumn.width = iWidth;
            m_lblColumn.AlignTo(parent, oAncor);
            m_lblColumn.eventClick += new MouseEventHandler(OnItemClicked);
            m_lblColumn.eventTooltipEnter += new MouseEventHandler(OnTooltipEnter);
        }

        public ListViewRowComparer.Columns GetColumn()
        {
            return m_eColumn;
        }

        public UILabel? GetLabel()
        {
            return m_lblColumn;
        }

        public void SetText(string sText)
        {
            if (m_lblColumn != null)
            {
                m_lblColumn.text = sText;
            }
        }

        public void SetTooltip(string sText)
        {
            if (m_lblColumn != null)
            {
                m_lblColumn.SetTooltip(sText);
            }
        }

        public bool IsTooltipVisible()
        {
            if (m_lblColumn != null)
            {
                return m_lblColumn.IsTooltipVisible();
            }
            return false;
        }

        public void HideTooltipBox()
        {
            if (m_lblColumn != null && m_lblColumn.tooltipBox != null)
            {
                m_lblColumn.tooltipBox.isVisible = false;
            }
        }

        private void OnItemClicked(UIComponent component, UIMouseEventParameter eventParam)
        {
            if (m_eventClickCallback != null)
            {
                m_eventClickCallback(component, eventParam);
            }
        }

        private void OnTooltipEnter(UIComponent component, UIMouseEventParameter eventParam)
        {
            if (m_lblColumn != null && m_GetColumnTooltip != null)
            {
                string sTooltip = m_GetColumnTooltip(this);
                m_lblColumn.SetTooltip(sTooltip);
            }
        }

        public void Destroy()
        {
            HideTooltipBox();
            if (m_lblColumn != null)
            {
                UnityEngine.Object.Destroy(m_lblColumn.gameObject);
            }
        }
    }
}
