using ColossalFramework.UI;
using Epic.OnlineServices.Presence;
using SleepyCommon;
using System;
using System.ComponentModel.Design;
using System.Xml;
using UnityEngine;
using static PublicTransportInfo.MainPanel;

namespace PublicTransportInfo.UI.ListViewRows
{
    public class UILineRow : UIListRow<LineInfoBase>
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

        private static UITextureAtlas? m_atlas = null;
        private static ushort s_usHighlightLine = 0;

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

            base.AfterStart();
        }

        protected override void Display()
        {
            if (data != null)
            {
                m_pnlColor.color = data.m_color;
                m_lblName.text = data.GetLineName();
                m_lblStops.text = data.GetStopCount().ToString();
                m_lblVehicles.text = data.m_iVehicleCount.ToString();
                m_lblPassengers.text = data.m_iPassengers.ToString();
                m_lblVehicleUsage.text = data.GetUsage().ToString();
                m_lblWaiting.text = data.m_iWaiting.ToString();
                m_lblBusiest.text = data.m_iBusiest.ToString();
                m_lblBored.text = data.m_iBored.ToString();

                if (m_spriteToolbar != null)
                {
                    switch (data.m_eLevel)
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

                    m_spriteToolbar.tooltip = data.m_lineIssueTooltip;
                }
            }
        }

        protected override void Clear()
        {
            m_lblName.text = "";
            m_lblStops.text = "";
            m_lblVehicles.text = "";
            m_lblPassengers.text = "";
            m_lblWaiting.text = "";
            m_lblBusiest.text = "";
            m_lblBored.text = "";
        }

        protected override string GetTooltipText(UIComponent component)
        {
            string sTooltip = "";
            if (component == m_lblStops)
            {
                sTooltip = data.GetStopsTooltip();
            }
            else if (component == m_lblVehicles)
            {
                sTooltip = data.GetVehicleTooltip();
            }
            else if (component == m_lblPassengers)
            {
                sTooltip = data.GetPassengersTooltip();
            }
            else if (component == m_lblWaiting)
            {
                sTooltip = data.GetWaitingTooltip();
            }
            else if (component == m_lblBusiest)
            {
                sTooltip = $"Stop:{data.m_iBusiestStopNumber}";
            }
            else if (component == m_lblBored)
            {
                sTooltip = data.GetBoredTooltip();
            }

            return sTooltip;
        }

        protected override void ClearTooltips()
        {
            m_lblName.SetTooltip("");
            m_lblStops.SetTooltip("");
            m_lblVehicles.SetTooltip("");
            m_lblPassengers.SetTooltip("");
            m_lblWaiting.SetTooltip("");
            m_lblBusiest.SetTooltip("");
            m_lblBored.SetTooltip("");
        }

        protected override void OnMouseEnter(UIComponent component, UIMouseEventParameter eventParam)
        {
            base.OnMouseEnter(component, eventParam);

            if (data is not null)
            {
                HighlightLine((ushort)data.GetLineId());
            }
        }

        protected override void OnMouseLeave(UIComponent component, UIMouseEventParameter eventParam)
        {
            base.OnMouseLeave(component, eventParam);
            ClearHighlightLine();
        } 

        private StopInfo GetStopInfo()
        {
            StopInfo info = MainPanel.Instance.GetStopInfo();
            
            if (data is not null)
            {
                // Update stop info
                if (info.m_transportType == data.GetTransportType() &&
                    info.m_currentLineId == data.GetLineId() &&
                    info.m_currentStopId != 0)
                {
                    StopInfo newInfo = new StopInfo();

                    newInfo.m_transportType = data.GetTransportType();
                    newInfo.m_currentLineId = data.GetLineId();

                    // Cycle through lines stops.
                    ushort nextStopId = data.GetNextStop(info.m_currentStopId, out int stopNumber);
                    if (nextStopId != 0)
                    {
                        newInfo.m_currentStopId = nextStopId;
                        newInfo.m_stopNumber = stopNumber;
                    }

                    return newInfo;
                }
                else
                {
                    // Get lines busiest stop
                    return data.GetStopInfo();
                }
            }

            return info;
        }

        protected override void OnClicked(UIComponent component)
        {
            if (data is not null) 
            {
                StopInfo info = GetStopInfo();

                if (info.m_currentStopId != 0)
                {
                    // Move to location
                    InstanceHelper.ShowInstance(new InstanceID { NetNode = info.m_currentStopId });

                    bool bCtrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
                    bool bShift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

                    bool bHideInfo = false;
                    if (bCtrl || bShift || data.IsWorldInfoPanelVisible())
                    {
                        // Hide main panel
                        if (bCtrl)
                        {
                            MainPanel.Instance.Hide();
                        }

                        data.ShowStopWorldInfoPanel(info.m_currentStopId);
                        bHideInfo = true;
                    }

                    // Update current stop info
                    MainPanel.Instance.SetCurrentStop(info);

                    if (bHideInfo)
                    {
                        MainPanel.Instance.HideInfo();
                    }
                }
            }
        }

        private void OnWarningItemClicked(UIComponent component, UIMouseEventParameter eventParam)
        {
            if (data != null && data.GetLineIssueDetector() != null && LineIssueManager.Instance != null)
            {
                if (LineIssueManager.Instance.HasVisibleLineIssues())
                {
                    MainPanel.Instance.Hide();
                    LineIssuePanel.Instance.Show();
                }
            }
            else
            {
                OnClicked(component); // Fall back to default handler
            }

            if (LineIssueManager.Instance != null)
            {
                LineIssueManager.Instance.UpdateWarningIcons();
            }
        }

        private void HighlightLine(ushort lineId)
        {
            ClearHighlightLine();

            s_usHighlightLine = lineId;

            TransportManagerUtils.HighlightLine(s_usHighlightLine);
        }

        private void ClearHighlightLine()
        {
            if (s_usHighlightLine != 0)
            {
                TransportManagerUtils.ClearLineHighlight(s_usHighlightLine);
                s_usHighlightLine = 0;
            }
        }

        protected override Color GetTextColor(UIComponent component, bool highlightRow)
        {
            if (highlightRow)
            {
                if (fullRowSelect)
                {
                    return Color.yellow;
                }
                else if (component == m_MouseEnterComponent)
                {
                    return Color.yellow;
                }
            }

            StopInfo info = MainPanel.Instance.GetStopInfo();
            if (data is not null &&
                data.GetTransportType() == info.m_transportType &&
                data.GetLineId() == info.m_currentLineId)
            {
                return Color.cyan;
            }
            else
            {
                return Color.white;
            }
        }
    }
}