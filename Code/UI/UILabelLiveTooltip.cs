using ColossalFramework.UI;
using UnityEngine;

namespace PublicTransportInfo
{
    public class UILabelLiveTooltip : UILabel
    {
        private UITooltip? m_tooltipWindow = null;
        private string m_tooltip = "";
        private TextAnchor m_tooltipTextAnchor = TextAnchor.MiddleCenter;

        public UILabelLiveTooltip()
        {
        }

        public bool IsTooltipVisible()
        {
            return m_tooltipWindow != null && m_tooltipWindow.Visible;
        }

        public void SetTooltip(string sTooltip)
        {
            m_tooltip = sTooltip;

            if (m_tooltipWindow != null)
            {
                m_tooltipWindow.SetTooltip(m_tooltip);
            }
        }
        public void SetTooltipTextAnchor(TextAnchor textAnchor)
        {
            m_tooltipTextAnchor = textAnchor;
        }

        protected override void OnTooltipHover(UIMouseEventParameter p)
        {
            if (m_tooltipWindow != null && !IsTooltipVisible())
            {
                if (Time.realtimeSinceStartup - m_HoveringStartTime > 0.4f)
                { 
                    m_tooltipWindow.SetTooltip(m_tooltip);
                    m_tooltipWindow.Show();
                }
            }
        }

        protected override void OnTooltipEnter(UIMouseEventParameter p)
        {
            base.OnTooltipEnter(p);
            if (m_tooltip != "")
            {
                if (m_tooltipWindow is null)
                {
                    m_tooltipWindow = UITooltip.Create(this, m_tooltip, m_tooltipTextAnchor);
                }
            }
        }

        protected override void OnTooltipLeave(UIMouseEventParameter p)
        {
            if (m_tooltipWindow != null) 
            {
                m_tooltipWindow.Visible = false;
                Destroy(m_tooltipWindow);
                m_tooltipWindow = null;
            }

            base.OnTooltipLeave(p);
        }
    }
}
