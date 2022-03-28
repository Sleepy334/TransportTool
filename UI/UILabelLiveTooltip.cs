using ColossalFramework.UI;

namespace PublicTransportInfo
{
    public class UILabelLiveTooltip : UILabel
    {
        private bool m_bUpdatingTooltip = false;

        public void SetTooltip(string sTooltip)
        {
            try
            {
                m_bUpdatingTooltip = true;

                if (m_TooltipShowing)
                {
                    m_TooltipShowing = false;
                    UILabel uILabel = (UILabel)tooltipBox;
                    if (uILabel != null)
                    {
                        uILabel.text = sTooltip;
                    }
                    m_TooltipShowing = true;
                }
                else
                {
                    tooltip = sTooltip;
                }
            }
            finally
            {
                m_bUpdatingTooltip = false;
            }
        }

        protected override void OnTooltipHover(UIMouseEventParameter p)
        {
            if (m_bUpdatingTooltip)
            {
                // Do nothing
            }
            else
            {
                base.OnTooltipHover(p);
            }
        }
    }
}
