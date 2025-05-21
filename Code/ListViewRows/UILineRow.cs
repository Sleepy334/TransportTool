using ColossalFramework.UI;
using SleepyCommon;
using System;
using UnityEngine;

namespace PublicTransportInfo
{
    public class UILineRow : UIPanel, IUIFastListRow
    {
        public const float fROW_HEIGHT = 26;

        private UIPanel? m_pnlColor = null;
        private UILabelLiveTooltip? m_lblName = null;
        private UILabelLiveTooltip? m_lblStops = null;
        private UILabelLiveTooltip? m_lblVehicles = null;
        private UILabelLiveTooltip? m_lblPassengers = null;
        private UILabelLiveTooltip? m_lblVehicleUsage = null;
        private UILabelLiveTooltip? m_lblWaiting = null;
        private UILabelLiveTooltip? m_lblBusiest = null;
        private UILabelLiveTooltip? m_lblBored = null;
        private UISprite? m_spriteToolbar = null;
        private LineInfoBase? m_data = null;

        private static UITextureAtlas? m_atlas = null;
        private static ushort m_usHighlightLine = 0;

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

            UIPanel oIconContainer = AddUIComponent<UIPanel>();
            if (oIconContainer != null)
            {
                oIconContainer.name = "oIconContainer";
                oIconContainer.width = MainPanel.iCOLUMN_WIDTH_COLOR;
                oIconContainer.height = fROW_HEIGHT - 4;

                m_pnlColor = oIconContainer.AddUIComponent<UIPanel>();
                if (m_pnlColor != null)
                {
                    m_pnlColor.name = "m_lblColor";
                    m_pnlColor.backgroundSprite = "InfoviewPanel";
                    m_pnlColor.height = MainPanel.iCOLUMN_WIDTH_COLOR;
                    m_pnlColor.width = MainPanel.iCOLUMN_WIDTH_COLOR;
                    m_pnlColor.CenterToParent();
                    m_pnlColor.eventClick += new MouseEventHandler(OnItemClicked);
                }
            }

            m_lblName = AddUIComponent<UILabelLiveTooltip>();
            if (m_lblName != null)
            {
                m_lblName.name = "m_lblName";
                m_lblName.text = "";
                m_lblName.textScale = MainPanel.fTEXT_SCALE;
                m_lblName.tooltip = "";
                m_lblName.textAlignment = UIHorizontalAlignment.Left;
                m_lblName.verticalAlignment = UIVerticalAlignment.Middle;
                m_lblName.autoSize = false;
                m_lblName.height = height;
                m_lblName.width = MainPanel.iCOLUMN_WIDTH_NAME;
                m_lblName.eventClicked += new MouseEventHandler(OnItemClicked);
                m_lblName.eventTooltipEnter += new MouseEventHandler(OnTooltipEnter);
                m_lblName.eventMouseEnter += new MouseEventHandler(OnMouseEnter);
                m_lblName.eventMouseLeave += new MouseEventHandler(OnMouseLeave);
            }

            m_lblStops = AddUIComponent<UILabelLiveTooltip>();
            if (m_lblStops != null)
            {
                m_lblStops.name = "m_lblStops";
                m_lblStops.text = "";
                m_lblStops.textScale = MainPanel.fTEXT_SCALE;
                m_lblStops.tooltip = "";
                m_lblStops.textAlignment = UIHorizontalAlignment.Center;
                m_lblStops.verticalAlignment = UIVerticalAlignment.Middle;
                m_lblStops.autoSize = false;
                m_lblStops.height = height;
                m_lblStops.width = MainPanel.iCOLUMN_WIDTH_STOPS;
                m_lblStops.eventClicked += new MouseEventHandler(OnItemClicked);
                m_lblStops.eventTooltipEnter += new MouseEventHandler(OnTooltipEnter);
                m_lblStops.eventMouseEnter += new MouseEventHandler(OnMouseEnter);
                m_lblStops.eventMouseLeave += new MouseEventHandler(OnMouseLeave);
            }

            m_lblVehicles = AddUIComponent<UILabelLiveTooltip>();
            if (m_lblVehicles != null)
            {
                m_lblVehicles.name = "m_lblVehicles";
                m_lblVehicles.text = "";
                m_lblVehicles.textScale = MainPanel.fTEXT_SCALE;
                m_lblVehicles.tooltip = "";
                m_lblVehicles.textAlignment = UIHorizontalAlignment.Center;
                m_lblVehicles.verticalAlignment = UIVerticalAlignment.Middle;
                m_lblVehicles.autoSize = false;
                m_lblVehicles.height = height;
                m_lblVehicles.width = MainPanel.iCOLUMN_WIDTH_VEHICLES;
                m_lblVehicles.eventClicked += new MouseEventHandler(OnItemClicked);
                m_lblVehicles.eventTooltipEnter += new MouseEventHandler(OnTooltipEnter);
                m_lblVehicles.eventMouseEnter += new MouseEventHandler(OnMouseEnter);
                m_lblVehicles.eventMouseLeave += new MouseEventHandler(OnMouseLeave);
            }

            m_lblPassengers = AddUIComponent<UILabelLiveTooltip>();
            if (m_lblPassengers != null)
            {
                m_lblPassengers.name = "m_lblPassengers";
                m_lblPassengers.text = "";
                m_lblPassengers.textScale = MainPanel.fTEXT_SCALE;
                m_lblPassengers.tooltip = "";
                m_lblPassengers.textAlignment = UIHorizontalAlignment.Center;
                m_lblPassengers.verticalAlignment = UIVerticalAlignment.Middle;
                m_lblPassengers.autoSize = false;
                m_lblPassengers.height = height;
                m_lblPassengers.width = MainPanel.iCOLUMN_WIDTH_PASSENGER;
                m_lblPassengers.eventClicked += new MouseEventHandler(OnItemClicked);
                m_lblPassengers.eventTooltipEnter += new MouseEventHandler(OnTooltipEnter);
                m_lblPassengers.eventMouseEnter += new MouseEventHandler(OnMouseEnter);
                m_lblPassengers.eventMouseLeave += new MouseEventHandler(OnMouseLeave);
            }

            m_lblVehicleUsage = AddUIComponent<UILabelLiveTooltip>();
            if (m_lblVehicleUsage != null)
            {
                m_lblVehicleUsage.name = "m_lblVehicleUsage";
                m_lblVehicleUsage.text = "";
                m_lblVehicleUsage.textScale = MainPanel.fTEXT_SCALE;
                m_lblVehicleUsage.tooltip = "";
                m_lblVehicleUsage.textAlignment = UIHorizontalAlignment.Center;
                m_lblVehicleUsage.verticalAlignment = UIVerticalAlignment.Middle;
                m_lblVehicleUsage.autoSize = false;
                m_lblVehicleUsage.height = height;
                m_lblVehicleUsage.width = MainPanel.iCOLUMN_WIDTH_VEHICLE_USAGE;
                m_lblVehicleUsage.eventClicked += new MouseEventHandler(OnItemClicked);
                m_lblVehicleUsage.eventTooltipEnter += new MouseEventHandler(OnTooltipEnter);
                m_lblVehicleUsage.eventMouseEnter += new MouseEventHandler(OnMouseEnter);
                m_lblVehicleUsage.eventMouseLeave += new MouseEventHandler(OnMouseLeave);
            }
            
            m_lblWaiting = AddUIComponent<UILabelLiveTooltip>();
            if (m_lblWaiting != null)
            {
                m_lblWaiting.name = "m_lblWaiting";
                m_lblWaiting.text = "";
                m_lblWaiting.textScale = MainPanel.fTEXT_SCALE;
                m_lblWaiting.tooltip = "";
                m_lblWaiting.textAlignment = UIHorizontalAlignment.Center;
                m_lblWaiting.verticalAlignment = UIVerticalAlignment.Middle;
                m_lblWaiting.autoSize = false;
                m_lblWaiting.height = height;
                m_lblWaiting.width = MainPanel.iCOLUMN_WIDTH_WAITING;
                m_lblWaiting.eventClicked += new MouseEventHandler(OnItemClicked);
                m_lblWaiting.eventTooltipEnter += new MouseEventHandler(OnTooltipEnter);
                m_lblWaiting.eventMouseEnter += new MouseEventHandler(OnMouseEnter);
                m_lblWaiting.eventMouseLeave += new MouseEventHandler(OnMouseLeave);
            }

            m_lblBusiest = AddUIComponent<UILabelLiveTooltip>();
            if (m_lblBusiest != null)
            {
                m_lblBusiest.name = "m_lblBusiest";
                m_lblBusiest.text = "";
                m_lblBusiest.textScale = MainPanel.fTEXT_SCALE;
                m_lblBusiest.tooltip = "";
                m_lblBusiest.textAlignment = UIHorizontalAlignment.Center;
                m_lblBusiest.verticalAlignment = UIVerticalAlignment.Middle;
                m_lblBusiest.autoSize = false;
                m_lblBusiest.height = height;
                m_lblBusiest.width = MainPanel.iCOLUMN_WIDTH_BUSIEST;
                m_lblBusiest.eventClicked += new MouseEventHandler(OnItemClicked);
                m_lblBusiest.eventTooltipEnter += new MouseEventHandler(OnTooltipEnter);
                m_lblBusiest.eventMouseEnter += new MouseEventHandler(OnMouseEnter);
                m_lblBusiest.eventMouseLeave += new MouseEventHandler(OnMouseLeave);
            }

            m_lblBored = AddUIComponent<UILabelLiveTooltip>();
            if (m_lblBored != null)
            {
                m_lblBored.name = "m_lblBored";
                m_lblBored.text = "";
                m_lblBored.textScale = MainPanel.fTEXT_SCALE;
                m_lblBored.tooltip = "";
                m_lblBored.textAlignment = UIHorizontalAlignment.Center;
                m_lblBored.verticalAlignment = UIVerticalAlignment.Middle;
                m_lblBored.autoSize = false;
                m_lblBored.height = height;
                m_lblBored.width = MainPanel.iCOLUMN_WIDTH_BORED;
                m_lblBored.eventClicked += new MouseEventHandler(OnItemClicked);
                m_lblBored.eventTooltipEnter += new MouseEventHandler(OnTooltipEnter);
                m_lblBored.eventMouseEnter += new MouseEventHandler(OnMouseEnter);
                m_lblBored.eventMouseLeave += new MouseEventHandler(OnMouseLeave);
            }

            m_spriteToolbar = AddUIComponent<UISprite>();
            if (m_spriteToolbar != null)
            {
                if (m_atlas == null)
                {
                    m_atlas = PublicTransportInstance.LoadResources();
                }
                m_spriteToolbar.name = "spriteToolbar";
                m_spriteToolbar.autoSize = false;
                m_spriteToolbar.width = 24;
                m_spriteToolbar.height = 24;
                m_spriteToolbar.atlas = m_atlas;
                m_spriteToolbar.AlignTo(this, UIAlignAnchor.TopRight);
                m_spriteToolbar.eventClick += new MouseEventHandler(OnWarningItemClicked);
            }

            if (m_data != null)
            {
                Display(m_data, false);
            }
        }

        public void Display(object data, bool isRowOdd)
        {
            LineInfoBase? rowData = (LineInfoBase?)data;
            if (rowData != null)
            {
                m_data = rowData;

                if (m_pnlColor != null)
                {
                    m_pnlColor.color = rowData.m_color;
                }
                if (m_lblName != null)
                {
                    m_lblName.text = rowData.GetLineName();
                }
                if (m_lblStops != null)
                {
                    m_lblStops.text = rowData.GetStopCount().ToString();
                }
                if (m_lblVehicles != null)
                {
                    m_lblVehicles.text = rowData.m_iVehicleCount.ToString();
                }
                if (m_lblPassengers != null)
                {
                    m_lblPassengers.text = rowData.m_iPassengers.ToString();
                }
                if (m_lblVehicleUsage != null)
                {
                    m_lblVehicleUsage.text = rowData.GetUsage().ToString();
                }
                if (m_lblWaiting != null)
                {
                    m_lblWaiting.text = rowData.m_iWaiting.ToString();
                }
                if (m_lblBusiest != null)
                {
                    m_lblBusiest.text = rowData.m_iBusiest.ToString();
                }
                if (m_lblBored != null)
                {
                    m_lblBored.text = rowData.m_iBored.ToString();
                }
                if (m_spriteToolbar != null)
                {
                    switch (rowData.m_eLevel)
                    {
                        case LineIssue.IssueLevel.ISSUE_WARNING:
                            {
                                m_spriteToolbar.spriteName = "Warning_icon48x48";
                                break;
                            }
                        case LineIssue.IssueLevel.ISSUE_INFORMATION:
                            {
                                m_spriteToolbar.spriteName = "Information";
                                break;
                            }
                        case LineIssue.IssueLevel.ISSUE_NONE:
                            {
                                m_spriteToolbar.spriteName = "";
                                break;
                            }
                    }

                    m_spriteToolbar.tooltip = rowData.m_lineIssueTooltip;
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

            m_lblName.SetTooltip("");
            m_lblStops.SetTooltip("");
            m_lblVehicles.SetTooltip("");
            m_lblPassengers.SetTooltip("");
            m_lblWaiting.SetTooltip("");
            m_lblBusiest.SetTooltip("");
            m_lblBored.SetTooltip("");
        }

        private void OnTooltipEnter(UIComponent component, UIMouseEventParameter eventParam)
        {
            UILabelLiveTooltip? lblColumn = (UILabelLiveTooltip)component;
            if (lblColumn != null)
            {
                string sStopsTooltip = "";
                string sVehicleTooltip = "";
                string sPassengerTooltip = "";
                string sWaitingTooltip = "";
                string sBusiestStopTooltip = "";
                string sBoredTooltip = "";

                if (m_data != null)
                {
                    sStopsTooltip = m_data.GetStopsTooltip();
                    sVehicleTooltip = m_data.GetVehicleTooltip();
                    sPassengerTooltip = m_data.GetPassengersTooltip();
                    sWaitingTooltip = m_data.GetWaitingTooltip();
                    sBusiestStopTooltip = $"Stop:{m_data.m_iBusiestStopNumber}";
                    sBoredTooltip = m_data.GetBoredTooltip();
                }

                if (lblColumn == m_lblStops)
                {
                    lblColumn.SetTooltip(sStopsTooltip);
                }
                else if (lblColumn == m_lblVehicles)
                {
                    lblColumn.SetTooltip(sVehicleTooltip);
                }
                else if (lblColumn == m_lblPassengers)
                {
                    lblColumn.SetTooltip(sPassengerTooltip);
                }
                else if (lblColumn == m_lblWaiting)
                {
                    lblColumn.SetTooltip(sWaitingTooltip);
                }
                else if (lblColumn == m_lblBusiest)
                {
                    lblColumn.SetTooltip(sBusiestStopTooltip);
                }
                else if (lblColumn == m_lblBored)
                {
                    lblColumn.SetTooltip(sBoredTooltip);
                }
            }
        }

        public void Select(bool isRowOdd)
        {
        }

        public void Deselect(bool isRowOdd)
        {
        }

        private void OnItemClicked(UIComponent component, UIMouseEventParameter eventParam)
        {
            if (DependencyUtils.IsCommuterDestinationsRunning() && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
            {
                ShowWorldInfoPanelCommuterDestinations();
            }
            else
            {
                ShowWorldInfoPanel();
            }
        }

        public void ShowWorldInfoPanel()
        {
            try
            {
                // Hide the main panel before showing PTWI panel.
                MainPanel.Instance.Hide();

                // Shift camera to busiest stop
                if (m_data != null)
                {
                    m_data.ShowBusiestStop();
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
            }
        }

        public void ShowWorldInfoPanelCommuterDestinations()
        {
            try
            {
                // Hide the main panel before showing PTWI panel.
                MainPanel.Instance.Hide();

                // Shift camera to busiest stop
                if (m_data != null)
                {
                    m_data.ShowBusiestStopCommuterDestinations();
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
            }
        }

        protected void OnMouseEnter(UIComponent component, UIMouseEventParameter eventParam)
        {
            foreach (UIComponent c in components)
            {
                if (c is UILabel label)
                {
                    label.textColor = Color.yellow;
                }
            }

            if (m_data != null)
            {
                HighlightLine((ushort)m_data.m_iLineId);
            }
        }

        protected void OnMouseLeave(UIComponent component, UIMouseEventParameter eventParam)
        {
            foreach (UIComponent c in components)
            {
                if (c is UILabel label)
                {
                    label.textColor = Color.white;
                }
            }

            ClearHighlightLine();
        }

        private void OnWarningItemClicked(UIComponent component, UIMouseEventParameter eventParam)
        {
            if (m_data != null && m_data.GetLineIssueDetector() != null && LineIssueManager.Instance != null)
            {
                if (LineIssueManager.Instance.HasVisibleLineIssues())
                {
                    MainPanel.Instance.Hide();
                    LineIssuePanel.Instance.Show();
                }
            }
            else
            {
                ShowWorldInfoPanel();
            }

            if (LineIssueManager.Instance != null)
            {
                LineIssueManager.Instance.UpdateWarningIcons();
            }
        }

        private void HighlightLine(ushort lineId)
        {
            ClearHighlightLine();

            m_usHighlightLine = lineId;

            TransportManagerUtils.HighlightLine(m_usHighlightLine);
        }

        private void ClearHighlightLine()
        {
            if (m_usHighlightLine != 0)
            {
                TransportManagerUtils.ClearLineHighlight(m_usHighlightLine);
                m_usHighlightLine = 0;
            }
        }
    }
}