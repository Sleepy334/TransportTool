using ColossalFramework.UI;
using SleepyCommon;
using UnityEngine;

namespace PublicTransportInfo
{
    public class UIIssueRow : UIPanel, IUIFastListRow
    {
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
            height = ListView.iROW_HEIGHT;
            autoLayoutDirection = LayoutDirection.Horizontal;
            autoLayoutStart = LayoutStart.TopLeft;
            autoLayoutPadding = new RectOffset(2, 2, 2, 2);
            autoLayout = true;
            clipChildren = true;

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
                m_lblTime.eventClicked += new MouseEventHandler(OnItemClicked);
                m_lblTime.eventTooltipEnter += new MouseEventHandler(OnTooltipEnter);
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
                m_lblType.eventClicked += new MouseEventHandler(OnItemClicked);
                m_lblType.eventTooltipEnter += new MouseEventHandler(OnTooltipEnter);
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
                m_lblName.eventClicked += new MouseEventHandler(OnItemClicked);
                m_lblName.eventTooltipEnter += new MouseEventHandler(OnTooltipEnter);
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
                m_lblLocation.eventClicked += new MouseEventHandler(OnItemClicked);
                m_lblLocation.eventTooltipEnter += new MouseEventHandler(OnTooltipEnter);
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
                m_lblDescription.eventClicked += new MouseEventHandler(OnItemClicked);
                m_lblDescription.eventTooltipEnter += new MouseEventHandler(OnTooltipEnter);
                m_lblDescription.eventMouseEnter += new MouseEventHandler(OnMouseEnter);
                m_lblDescription.eventMouseLeave += new MouseEventHandler(OnMouseLeave);
            }

            if (m_data != null)
            {
                Display(m_data, false);
            }
        }

        public void Display(object data, bool isRowOdd)
        {
            LineIssue? rowData = (LineIssue?)data;
            if (rowData != null)
            {
                m_data = rowData;
                if (m_lblTime != null)
                {
                    m_lblTime.text = rowData.GetCreationTime();
                }
                if (m_lblType != null)
                {
                    m_lblType.text = rowData.GetTransportType();
                }
                if (m_lblName != null)
                {
                    m_lblName.text = rowData.GetLineDescription().ToString();
                }
                if (m_lblLocation != null)
                {
                    m_lblLocation.text = rowData.GetIssueLocation();
                }
                if (m_lblDescription != null)
                {
                    m_lblDescription.text = rowData.GetIssueDescription();
                }
            }
            else
            {
                m_data = null;
            }
        }

        public void Enabled(object data)
        {
        }

        public void Disabled()
        {
            m_data = null;
        }

        public void Select(bool isRowOdd)
        {
        }

        public void Deselect(bool isRowOdd)
        {
        }

        private void OnItemClicked(UIComponent component, UIMouseEventParameter eventParam)
        {
            if (m_data != null)
            {
                m_data.ShowIssue();
            }
        }

        private void OnTooltipEnter(UIComponent component, UIMouseEventParameter eventParam)
        {
        }

        protected void OnMouseEnter(UIComponent component, UIMouseEventParameter eventParam)
        {
            backgroundSprite = "ListItemHighlight";
        }

        protected void OnMouseLeave(UIComponent component, UIMouseEventParameter eventParam)
        {
            backgroundSprite = "InfoviewPanel";
            color = new Color32(81, 87, 89, 225);
        }
    }
}