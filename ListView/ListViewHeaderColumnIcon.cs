using ColossalFramework.UI;
using PublicTransportInfo;
using System;
using UnityEngine;

namespace SleepyCommon
{
    public class ListViewHeaderColumnIcon : ListViewHeaderColumnBase
    {
        private UIPanel? m_pnlIcon = null;
        private UILabel? m_lblIcon = null;
        private UILabel? m_lblIconSort = null;

        public ListViewHeaderColumnIcon(ListViewRowComparer.Columns eColumn, UIComponent parent, string sIconName, string sTooltip, int iWidth, int iHeight, UIHorizontalAlignment oTextAlignment, UIAlignAnchor oAncor, OnListViewColumnClick eventCallback) :
                base(eColumn, sIconName, eventCallback)
        {
            m_eColumn = eColumn;
            m_eventClickCallback = eventCallback;
            m_sText = sIconName;

            m_pnlIcon = parent.AddUIComponent<UIPanel>();
            m_pnlIcon.name = eColumn.ToString() + "Panel"; ;
            m_pnlIcon.autoSize = false;
            m_pnlIcon.tooltip = sTooltip;
            //m_pnlIcon.backgroundSprite = "InfoviewPanel";
            //m_pnlIcon.color = Color.red;
            m_pnlIcon.height = MainPanel.iHEADER_HEIGHT;
            m_pnlIcon.width = iWidth;
            //m_pnlIcon.autoLayoutDirection = LayoutDirection.Horizontal;
            //m_pnlIcon.autoLayout = true;
            m_pnlIcon.eventMouseEnter += OnMouseHover;
            m_pnlIcon.eventMouseLeave += OnMouseLeave;
            m_pnlIcon.eventClick += new MouseEventHandler(OnItemClicked);

            m_lblIcon = m_pnlIcon.AddUIComponent<UILabel>();
            m_lblIcon.name = eColumn.ToString() + "Icon";
            m_lblIcon.text = "";
            //m_lblVehicles.backgroundSprite = "InfoviewPanel";
            //m_lblVehicles.color = Color.blue;
            m_lblIcon.backgroundSprite = sIconName;
            m_lblIcon.textAlignment = UIHorizontalAlignment.Center;
            m_lblIcon.autoSize = false;
            m_lblIcon.height = MainPanel.iHEADER_HEIGHT;
            m_lblIcon.width = MainPanel.iHEADER_HEIGHT;
            m_lblIcon.AlignTo(m_pnlIcon, UIAlignAnchor.TopRight);
            m_lblIcon.eventMouseEnter += OnMouseHover;
            m_lblIcon.eventMouseLeave += OnMouseLeave;
            m_lblIcon.CenterToParent();
        }

        public override void Sort(ListViewRowComparer.Columns eColumn, bool bDescending)
        {
        }

        public override bool IsHit(UIComponent component)
        {
            return component == m_pnlIcon || component == m_lblIconSort || component == m_lblIcon;
        }

        protected void OnMouseHover(UIComponent component, UIMouseEventParameter eventParam)
        {
            if (m_lblIconSort != null)
            {
                m_lblIconSort.textColor = Color.yellow;
            }    
            if (m_lblIcon != null)
            {
                m_lblIcon.backgroundSprite = m_sText + "Hovered";
            }
        }

        protected void OnMouseLeave(UIComponent component, UIMouseEventParameter eventParam)
        {
            if (m_lblIconSort != null)
            {
                m_lblIconSort.textColor = Color.white;
            }
            if (m_lblIcon != null)
            {
                m_lblIcon.backgroundSprite = m_sText;
            }
        }

        new public void SetText(string sText)
        {
            // Do nothing we use an icon instead
            throw new Exception("No text to set for an icon column");
        }

        new public void SetTooltip(string sText)
        {
            if (m_pnlIcon != null)
            {
                m_pnlIcon.tooltip = sText;
            }
        }

        private void HideTooltipBox()
        {
            if (m_pnlIcon != null && m_pnlIcon.tooltipBox != null)
            {
                m_pnlIcon.tooltipBox.isVisible = false;
            }
        }

        public override void Destroy()
        {
            HideTooltipBox();
            if (m_lblIconSort != null)
            {
                UnityEngine.Object.Destroy(m_lblIconSort.gameObject);
            }
            if (m_lblIcon != null)
            {
                UnityEngine.Object.Destroy(m_lblIcon.gameObject);
            }
            if (m_pnlIcon != null)
            {
                UnityEngine.Object.Destroy(m_pnlIcon.gameObject);
            }
        }
    }
}
