using ColossalFramework;
using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PublicTransportInfo
{
    public class ListViewRow : UIPanel
    {
        public const int iROW_HEIGHT = 30;
        public const int iROW_MARGIN = 10;
        public LineInfoBase? m_oLineInfo = null;
        private UIPanel? m_lblColor = null;
        private UISprite? m_spriteToolbar = null;
        private List<ListViewRowColumn> m_columns;
        private static ushort m_HighlightedLineId = 0;
        private static UITextureAtlas? m_atlas = null;

        public ListViewRow() : base()
        {
            m_columns = new List<ListViewRowColumn>();
        }

        public static ListViewRow? Create(ListView oParent, LineInfoBase oLineInfo)
        {
            if (oParent != null && oParent.m_listPanel != null)
            {
                ListViewRow oRow = oParent.m_listPanel.AddUIComponent<ListViewRow>();
                oRow.m_oLineInfo = oLineInfo;

                //oRow.color = oLineInfo.m_color;
                oRow.backgroundSprite = "GenericPanel";
                oRow.position = new Vector3(iROW_MARGIN, -iROW_MARGIN);
                oRow.width = oParent.width - (iROW_MARGIN * 2);
                oRow.height = iROW_HEIGHT;
                oRow.autoLayoutDirection = LayoutDirection.Horizontal;
                oRow.autoLayoutStart = LayoutStart.TopLeft;
                oRow.autoLayoutPadding = new RectOffset(2, 2, 2, 2);
                oRow.autoLayout = true;
                oRow.clipChildren = true;
                oRow.eventMouseEnter += new MouseEventHandler(oRow.OnItemEnter);
                oRow.eventMouseLeave += new MouseEventHandler(oRow.OnItemLeave);
                oRow.Setup(oLineInfo);

                return oRow;
            }
            else
            {
                return null;
            }
        }

        public void Setup(LineInfoBase oLineInfo)
        {
            UIPanel oIconContainer = AddUIComponent<UIPanel>();
            if (oIconContainer != null)
            {
                oIconContainer.name = "oIconContainer";
                oIconContainer.width = iROW_HEIGHT - 4;
                oIconContainer.height = iROW_HEIGHT - 4;

                m_lblColor = oIconContainer.AddUIComponent<UIPanel>();
                if (m_lblColor != null)
                {
                    m_lblColor.name = "m_lblColor";
                    m_lblColor.backgroundSprite = "InfoviewPanel";
                    m_lblColor.color = oLineInfo.m_color;
                    m_lblColor.height = PublicTransportInfoPanel.iCOLUMN_WIDTH_COLOR;
                    m_lblColor.width = PublicTransportInfoPanel.iCOLUMN_WIDTH_COLOR;
                    m_lblColor.CenterToParent();
                    m_lblColor.eventClick += new MouseEventHandler(OnItemClicked);
                }
            }
            
            m_columns.Add(ListViewRowColumn.Create(ListViewRowComparer.Columns.COLUMN_NAME, this, oLineInfo.GetLineName(), "", PublicTransportInfoPanel.iCOLUMN_WIDTH_NAME, iROW_HEIGHT, UIHorizontalAlignment.Left, UIAlignAnchor.TopLeft, OnItemClicked, OnGetColumnTooltip));
            m_columns.Add(ListViewRowColumn.Create(ListViewRowComparer.Columns.COLUMN_STOPS, this, oLineInfo.GetStopCount().ToString(), "", PublicTransportInfoPanel.iCOLUMN_WIDTH_STOPS, iROW_HEIGHT, UIHorizontalAlignment.Center, UIAlignAnchor.TopRight, OnItemClicked, OnGetColumnTooltip));
            m_columns.Add(ListViewRowColumn.Create(ListViewRowComparer.Columns.COLUMN_VEHICLES, this, oLineInfo.m_iVehicleCount.ToString(), "", PublicTransportInfoPanel.iCOLUMN_WIDTH_VEHICLES, iROW_HEIGHT, UIHorizontalAlignment.Center, UIAlignAnchor.TopRight, OnItemClicked, OnGetColumnTooltip));
            m_columns.Add(ListViewRowColumn.Create(ListViewRowComparer.Columns.COLUMN_PASSENGERS, this, oLineInfo.GetPassengersDescription(), "", PublicTransportInfoPanel.iCOLUMN_WIDTH_PASSENGER, iROW_HEIGHT, UIHorizontalAlignment.Center, UIAlignAnchor.TopRight, OnItemClicked, OnGetColumnTooltip));
            m_columns.Add(ListViewRowColumn.Create(ListViewRowComparer.Columns.COLUMN_VEHICLE_USAGE, this, oLineInfo.GetUsage().ToString(), "", PublicTransportInfoPanel.iCOLUMN_WIDTH_STOPS, iROW_HEIGHT, UIHorizontalAlignment.Center, UIAlignAnchor.TopRight, OnItemClicked, OnGetColumnTooltip));
            m_columns.Add(ListViewRowColumn.Create(ListViewRowComparer.Columns.COLUMN_WAITING, this, oLineInfo.m_iWaiting.ToString(), "", PublicTransportInfoPanel.iCOLUMN_WIDTH_WAITING, iROW_HEIGHT, UIHorizontalAlignment.Center, UIAlignAnchor.TopRight, OnItemClicked, OnGetColumnTooltip));
            m_columns.Add(ListViewRowColumn.Create(ListViewRowComparer.Columns.COLUMN_BUSIEST, this, oLineInfo.m_iBusiest.ToString(), "", PublicTransportInfoPanel.iCOLUMN_WIDTH_BUSIEST, iROW_HEIGHT, UIHorizontalAlignment.Center, UIAlignAnchor.TopRight, OnItemClicked, OnGetColumnTooltip));
            m_columns.Add(ListViewRowColumn.Create(ListViewRowComparer.Columns.COLUMN_BORED, this, oLineInfo.m_iBored.ToString(), "", PublicTransportInfoPanel.iCOLUMN_WIDTH_BORED, iROW_HEIGHT, UIHorizontalAlignment.Center, UIAlignAnchor.TopRight, OnItemClicked, OnGetColumnTooltip));

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
                UpdateWarningIconSprite();
            }

            UpdateLineData();
        }

        private void UpdateWarningIconSprite()
        {
            string sTooltip = "";
            LineIssue.IssueLevel eLevel = LineIssue.IssueLevel.ISSUE_NONE;

            if (m_oLineInfo != null && LineIssueManager.Instance != null)
            {
                eLevel = LineIssueManager.Instance.GetLineWarningLevel((ushort)m_oLineInfo.m_iLineId, out sTooltip);
            }

            if (m_spriteToolbar != null)
            {
                if (eLevel == LineIssue.IssueLevel.ISSUE_WARNING)
                {
                    m_spriteToolbar.spriteName = "Warning_icon48x48";
                }
                else if (eLevel == LineIssue.IssueLevel.ISSUE_INFORMATION)
                {
                    m_spriteToolbar.spriteName = "Information";
                }
                else
                {
                    m_spriteToolbar.spriteName = "";
                }

                if (m_spriteToolbar.tooltipBox != null && m_spriteToolbar.tooltipBox.isVisible && m_spriteToolbar.tooltip != sTooltip)
                {
                    m_spriteToolbar.tooltipBox.isVisible = false;
                }
                m_spriteToolbar.tooltip = sTooltip;
            }
        }

        private void OnItemClicked(UIComponent component, UIMouseEventParameter eventParam)
        {
            if (DependencyUtilities.IsCommuterDestinationsRunning() && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
            {
                ShowWorldInfoPanelCommuterDestinations(); 
            }
            else
            {
                ShowWorldInfoPanel();
            }
        }

        public string OnGetColumnTooltip(ListViewRowColumn oColumn) 
        {
            string sTooltip = GetColumnTooltip(oColumn.GetColumn());
            return sTooltip;
        }

        private void OnWarningItemClicked(UIComponent component, UIMouseEventParameter eventParam)
        {
            if (m_oLineInfo != null && m_oLineInfo.GetLineIssueDetector() != null && LineIssueManager.Instance != null)
            {
                LineIssueManager.Instance.UpdateLineIssues();
                if (LineIssueManager.Instance.HasVisibleLineIssues())
                {
                    PublicTransportInstance.HideMainPanel();
                    PublicTransportInstance.ShowLineIssuePanel(m_oLineInfo.m_iLineId);
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

        private void OnItemEnter(UIComponent component, UIMouseEventParameter eventParam)
        {        
            ListViewRow oRow = (ListViewRow) component;
            if (oRow != null)
            {
                if (m_HighlightedLineId != 0 && m_HighlightedLineId != (ushort)m_oLineInfo.m_iLineId)
                {
                    ClearHighlight();
                }

                m_HighlightedLineId = (ushort)m_oLineInfo.m_iLineId;
                backgroundSprite = "ListItemHighlight";
                TransportManagerUtils.HighlightLine(m_HighlightedLineId);
            }
        }

        private void OnItemLeave(UIComponent component, UIMouseEventParameter eventParam)
        {
            ClearHighlight();
            ClearRowHighlight();

            ListViewRow oRow = (ListViewRow) component;
            if (oRow != null && m_HighlightedLineId != 0)
            {
                TransportManagerUtils.ClearLineHighlight(m_HighlightedLineId);
                m_HighlightedLineId = 0;
            }
        }

        public void ClearRowHighlight()
        {
            backgroundSprite = "GenericPanel";
        }

        private void ClearHighlight()
        {
            if (m_HighlightedLineId != 0)
            {
                TransportManagerUtils.ClearLineHighlight(m_HighlightedLineId);
                m_HighlightedLineId = 0;
            }
        }
 

        public void ShowWorldInfoPanel()
        {
            try
            {
                // Hide the main panel before showing PTWI panel.
                PublicTransportInstance.HideMainPanel(false);

                // Shift camera to busiest stop
                if (m_oLineInfo != null)
                {
                    m_oLineInfo.ShowBusiestStop();
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
                PublicTransportInstance.HideMainPanel(false);

                // Shift camera to busiest stop
                if (m_oLineInfo != null)
                {
                    m_oLineInfo.ShowBusiestStopCommuterDestinations();
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
            }
        }
        

        private string GetColumnText(ListViewRowComparer.Columns eColumn)
        {
            if (m_oLineInfo != null)
            {
                switch (eColumn)
                {
                    case ListViewRowComparer.Columns.COLUMN_NAME: return m_oLineInfo.GetLineName();
                    case ListViewRowComparer.Columns.COLUMN_STOPS: return m_oLineInfo.GetStopCount().ToString();
                    case ListViewRowComparer.Columns.COLUMN_VEHICLES: return m_oLineInfo.m_iVehicleCount.ToString();
                    case ListViewRowComparer.Columns.COLUMN_PASSENGERS: return m_oLineInfo.GetPassengersDescription();
                    case ListViewRowComparer.Columns.COLUMN_VEHICLE_USAGE: return m_oLineInfo.GetUsage().ToString();
                    case ListViewRowComparer.Columns.COLUMN_WAITING: return m_oLineInfo.m_iWaiting.ToString();
                    case ListViewRowComparer.Columns.COLUMN_BUSIEST: return m_oLineInfo.m_iBusiest.ToString();
                    case ListViewRowComparer.Columns.COLUMN_BORED: return m_oLineInfo.m_iBored.ToString();
                }
            }
            
            return "";
        }

        private string GetColumnTooltip(ListViewRowComparer.Columns eColumn)
        {
            switch (eColumn)
            {
                case ListViewRowComparer.Columns.COLUMN_STOPS:
                    {
                        string sTooltip = "";
                        if (m_oLineInfo != null && m_columns != null && m_columns.Count > 0)
                        {
                            sTooltip = m_oLineInfo.GetStopsTooltip();
                        }
                        return sTooltip;
                    }
                case ListViewRowComparer.Columns.COLUMN_VEHICLES:
                    {
                        string sTooltip = "";
                        if (m_oLineInfo != null && m_columns != null && m_columns.Count > 0)
                        {
                            sTooltip = m_oLineInfo.GetVehicleTooltip();
                        }
                        return sTooltip;
                    }
                case ListViewRowComparer.Columns.COLUMN_PASSENGERS:
                    {
                        if (m_oLineInfo != null)
                        {
                            return m_oLineInfo.GetPassengersTooltip();
                        }
                        else
                        {
                            return "";
                        }
                    }
                case ListViewRowComparer.Columns.COLUMN_WAITING:
                    {
                        string sTooltip = "";
                        if (m_oLineInfo != null && m_columns != null && m_columns.Count > 0)
                        {
                            sTooltip = m_oLineInfo.GetWaitingTooltip();
                        }
                        return sTooltip;
                    }
                case ListViewRowComparer.Columns.COLUMN_BUSIEST:
                    {
                        if (m_oLineInfo != null)
                        {
                            return "Stop: " + m_oLineInfo.m_iBusiestStopNumber;
                        }
                        else
                        {
                            return "";
                        }
                    }
                case ListViewRowComparer.Columns.COLUMN_BORED:
                    {
                        string sTooltip = "";
                        if (m_oLineInfo != null && m_columns != null && m_columns.Count > 0)
                        {
                            sTooltip = m_oLineInfo.GetBoredTooltip();
                        }
                        return sTooltip;
                    }
            }
            return "";
        }

        public void UpdateLineData()
        {
            if (m_oLineInfo != null)
            {
                m_oLineInfo.UpdateInfo();

                if (m_oLineInfo.GetLineIssueDetector() != null)
                {
                    UpdateWarningIconSprite();
                }

                if (m_lblColor != null)
                {
                    m_lblColor.color = m_oLineInfo.m_color;
                }
            }

            if (m_columns != null)
            {
                foreach (ListViewRowColumn oColumn in m_columns)
                {
                    if (oColumn != null)
                    {
                        ListViewRowComparer.Columns eColumn = oColumn.GetColumn();
                        oColumn.SetText(GetColumnText(eColumn));

                        // Update tooltip, but only if visible as the OnEnter event handler will load the tooltip normally.
                        if (oColumn.IsTooltipVisible())
                        {
                            oColumn.SetTooltip(GetColumnTooltip(eColumn));
                        }
                    }
                }
            }
        }

        private void HideTooltipBox(UIComponent? uiComponent)
        {
            if (uiComponent != null && uiComponent.tooltipBox != null)
            {
                uiComponent.tooltipBox.isVisible = false;
            }
        }
        
        public override void OnDestroy()
        {
            ClearHighlight(); 
            
            foreach (ListViewRowColumn oColumn in m_columns)
            { 
                oColumn.HideTooltipBox();
                oColumn.Destroy();
            }
            m_columns.Clear();
            HideTooltipBox(m_spriteToolbar);  
            
            if (m_lblColor != null)
            {
                Destroy(m_lblColor.gameObject);
            }
            if (m_spriteToolbar != null)
            {
                Destroy(m_spriteToolbar.gameObject);
            }
            base.OnDestroy();
        }
    }
     
}
