using ColossalFramework.UI;
using UnityEngine;

namespace PublicTransportInfo.UI.ListViewRows
{
    public class UIIssueRow : UIListRow<LineIssue>
    {
        public const float fROW_HEIGHT = 26;

        private UILabel? m_lblTime = null;
        private UILabel? m_lblType = null;
        private UILabel? m_lblName = null;
        private UILabel? m_lblLocation = null;
        private UILabel? m_lblDescription = null;

        private LineIssue? m_data = null;

        public override void Start()
        {
            base.Start();

            isVisible = true;
            canFocus = true;
            isInteractive = true;
            width = parent.width;
            height = fROW_HEIGHT;
            autoLayoutDirection = LayoutDirection.Horizontal;
            autoLayoutStart = LayoutStart.TopLeft;
            autoLayoutPadding = new RectOffset(2, 2, 2, 2);
            autoLayout = true;
            clipChildren = true;
            fullRowSelect = true;

            m_lblTime = AddUIComponent<UILabel>();
            if (m_lblTime != null)
            {
                m_lblTime.name = "m_lblTime";
                m_lblTime.text = "";
                m_lblTime.textScale = LineIssuePanel.fTEXT_SCALE;
                m_lblTime.tooltip = "";
                m_lblTime.textAlignment = UIHorizontalAlignment.Left;
                m_lblTime.verticalAlignment = UIVerticalAlignment.Middle;
                m_lblTime.autoSize = false;
                m_lblTime.height = height;
                m_lblTime.width = LineIssuePanel.iCOLUMN_WIDTH_TIME;
                m_lblTime.eventMouseEnter += new MouseEventHandler(OnMouseEnter);
                m_lblTime.eventMouseLeave += new MouseEventHandler(OnMouseLeave);
            }

            m_lblType = AddUIComponent<UILabel>();
            if (m_lblType != null)
            {
                m_lblType.name = "m_lblType";
                m_lblType.text = "";
                m_lblType.textScale = LineIssuePanel.fTEXT_SCALE;
                m_lblType.tooltip = "";
                m_lblType.textAlignment = UIHorizontalAlignment.Left;
                m_lblType.verticalAlignment = UIVerticalAlignment.Middle;
                m_lblType.autoSize = false;
                m_lblType.height = height;
                m_lblType.width = LineIssuePanel.iCOLUMN_WIDTH_NORMAL;
                m_lblType.eventMouseEnter += new MouseEventHandler(OnMouseEnter);
                m_lblType.eventMouseLeave += new MouseEventHandler(OnMouseLeave);
            }

            m_lblName = AddUIComponent<UILabel>();
            if (m_lblName != null)
            {
                m_lblName.name = "m_lblName";
                m_lblName.text = "";
                m_lblName.textScale = LineIssuePanel.fTEXT_SCALE;
                m_lblName.tooltip = "";
                m_lblName.textAlignment = UIHorizontalAlignment.Left;
                m_lblName.verticalAlignment = UIVerticalAlignment.Middle;
                m_lblName.autoSize = false;
                m_lblName.height = height;
                m_lblName.width = LineIssuePanel.iCOLUMN_VEHICLE_WIDTH;
                m_lblName.eventMouseEnter += new MouseEventHandler(OnMouseEnter);
                m_lblName.eventMouseLeave += new MouseEventHandler(OnMouseLeave);
            }

            m_lblLocation = AddUIComponent<UILabel>();
            if (m_lblLocation != null)
            {
                m_lblLocation.name = "m_lblLocation";
                m_lblLocation.text = "";
                m_lblLocation.textScale = LineIssuePanel.fTEXT_SCALE;
                m_lblLocation.tooltip = "";
                m_lblLocation.textAlignment = UIHorizontalAlignment.Left;
                m_lblLocation.verticalAlignment = UIVerticalAlignment.Middle;
                m_lblLocation.autoSize = false;
                m_lblLocation.height = height;
                m_lblLocation.width = LineIssuePanel.iCOLUMN_VEHICLE_WIDTH;
                m_lblLocation.eventMouseEnter += new MouseEventHandler(OnMouseEnter);
                m_lblLocation.eventMouseLeave += new MouseEventHandler(OnMouseLeave);
            }

            m_lblDescription = AddUIComponent<UILabel>();
            if (m_lblDescription != null)
            {
                m_lblDescription.name = "m_lblDescription";
                m_lblDescription.text = "";
                m_lblDescription.textScale = LineIssuePanel.fTEXT_SCALE;
                m_lblDescription.tooltip = "";
                m_lblDescription.textAlignment = UIHorizontalAlignment.Left;
                m_lblDescription.verticalAlignment = UIVerticalAlignment.Middle;
                m_lblDescription.autoSize = false;
                m_lblDescription.height = height;
                m_lblDescription.width = LineIssuePanel.iCOLUMN_DESCRIPTION_WIDTH;
                m_lblDescription.eventMouseEnter += new MouseEventHandler(OnMouseEnter);
                m_lblDescription.eventMouseLeave += new MouseEventHandler(OnMouseLeave);
            }

            base.AfterStart();
        }

        protected override void Display()
        {
            if (data != null)
            {
                m_lblTime.text = data.GetCreationTime();
                m_lblType.text = data.GetTransportType();
                m_lblName.text = data.GetLineDescription().ToString();
                m_lblLocation.text = data.GetIssueLocation();
                m_lblDescription.text = data.GetIssueDescription();
            }
        }

        protected override void Clear()
        {
            m_lblTime.text = "";
            m_lblType.text = "";
            m_lblName.text = "";
            m_lblLocation.text = "";
            m_lblDescription.text = "";
        }

        protected override void ClearTooltips()
        {
            m_lblTime.tooltip = "";
            m_lblType.tooltip = "";
            m_lblName.tooltip = "";
            m_lblLocation.tooltip = "";
            m_lblDescription.tooltip = "";
        }

        protected override string GetTooltipText(UIComponent component)
        {
            return "";
        }

        protected override void OnClicked(UIComponent component)
        {
            data.ShowIssue();
        }
    }
}