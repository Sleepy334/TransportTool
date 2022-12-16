using ColossalFramework;
using ColossalFramework.UI;
using PublicTransportInfo.Util;
using SleepyCommon;
using System;
using System.Collections.Generic;
using UnityEngine;
using static PublicTransportInfo.ListViewRowComparer;

namespace PublicTransportInfo
{
    public class PublicTransportInfoPanel : UIPanel
    {
        public const float fTEXT_SCALE = 1.0f;

        public const float PanelWidth = 1000;
        public const float PanelHeight = 600;
        public const int Margin = 10;
        public const int iHEADER_HEIGHT = 20;

        public const int iCOLUMN_WIDTH_COLOR = 20;
        public const int iCOLUMN_WIDTH_NAME = 320;
        public const int iCOLUMN_WIDTH_STOPS = 70;
        public const int iCOLUMN_WIDTH_VEHICLES = 40;
        public const int iCOLUMN_WIDTH_PASSENGER = 130;
        public const int iCOLUMN_WIDTH_VEHICLE_USAGE = 70;
        public const int iCOLUMN_WIDTH_WAITING = 85;
        public const int iCOLUMN_WIDTH_BUSIEST = 85;
        public const int iCOLUMN_WIDTH_BORED = 70;

        private TabStrip? m_tabStrip;
        private ListView? m_ListView = null;
        private UILabel? m_lblOverview = null;
        private UITitleBar? m_title = null;

        private List<PublicTransportType> m_TransportList;
        private PublicTransportType m_SelectedTransportType;

        public static bool m_bLoadingLines = false;

        public PublicTransportInfoPanel() : base()
        {
            m_TransportList = new List<PublicTransportType>();
            m_SelectedTransportType = PublicTransportType.Bus;
            m_tabStrip = null;
        }

        public override void Start()
        {
            //base.Start();
            name = "PublicTransportInfoPanel";
            width = PanelWidth;
            height = PanelHeight;
            backgroundSprite = "UnlockingPanel2";

            if (!ModSettings.GetSettings().DisableTransparency)
            {
                opacity = 0.95f;
            }
            
            canFocus = true;
            isInteractive = true;
            isVisible = false;
            playAudioEvents = true;
            clipChildren = true;
            CenterToParent();

            // Title Bar
            m_title = AddUIComponent<UITitleBar>();
            if (m_title != null)
            {
                m_title.SetOnclickHandler(OnCloseClick);
                m_title.SetIssuesHandler(OnIssuesClick);
                m_title.title = ITransportInfoMain.Title;
            }

            UIPanel mainPanel = AddUIComponent<UIPanel>();
            if (mainPanel != null)
            {
                mainPanel.width = width;
                mainPanel.height = height - m_title.height;
                mainPanel.padding = new RectOffset(Margin, Margin, 0, Margin);
                mainPanel.relativePosition = new Vector3(0f, m_title.height);
                mainPanel.autoLayout = true;
                mainPanel.autoLayoutDirection = LayoutDirection.Vertical;
            }

            m_tabStrip = mainPanel.AddUIComponent<TabStrip>();
            if (m_tabStrip != null)
            {
                m_tabStrip.backgroundSprite = "GenericPanel";
                m_tabStrip.name = "tabStrip";
                m_tabStrip.width = width - (Margin * 2);
                m_tabStrip.height = 40;
                m_tabStrip.autoLayoutDirection = LayoutDirection.Horizontal;
                m_tabStrip.autoLayoutStart = LayoutStart.TopLeft;
                m_tabStrip.autoLayoutPadding = new RectOffset(4, 0, 4, 4);
                m_tabStrip.autoLayout = true;
                m_tabStrip.SelectionChangedEventHandler += OnTabSelectionChanged;
                AddOverviewLabelToTabStrip();
            }
            else
            {
                Debug.Log("m_tabStrip is null");
                return;
            }

            UIPanel tabPanel = mainPanel.AddUIComponent<UIPanel>();
            if (tabPanel != null)
            {
                tabPanel.autoLayoutDirection = LayoutDirection.Vertical;
                tabPanel.width = m_tabStrip.width;
                tabPanel.height = height - m_title.height - m_tabStrip.height - Margin;
                tabPanel.autoLayout = true;

                m_ListView = ListView.Create<UILineRow>(tabPanel, Color.white, fTEXT_SCALE, UILineRow.fROW_HEIGHT, tabPanel.width, tabPanel.height);
                if (m_ListView != null)
                {
                    m_ListView.width = tabPanel.width;
                    m_ListView.height = tabPanel.height;
                    m_ListView.AddColumn(Columns.COLUMN_COLOR, "", "", iCOLUMN_WIDTH_COLOR, iHEADER_HEIGHT, UIHorizontalAlignment.Left, UIAlignAnchor.TopLeft);
                    m_ListView.AddColumn(Columns.COLUMN_NAME, Localization.Get("headerLineName"), Localization.Get("headerStopsTooltip"), iCOLUMN_WIDTH_NAME, iHEADER_HEIGHT, UIHorizontalAlignment.Left, UIAlignAnchor.TopLeft);
                    m_ListView.AddColumn(Columns.COLUMN_STOPS, Localization.Get("headerStops"), Localization.Get("headerVehicleTooltip"), iCOLUMN_WIDTH_STOPS, iHEADER_HEIGHT, UIHorizontalAlignment.Center, UIAlignAnchor.TopLeft);
                    m_ListView.AddIconColumn(Columns.COLUMN_VEHICLES, "InfoIconPublicTransport", Localization.Get("headerPassengersTooltip"), iCOLUMN_WIDTH_VEHICLES, iHEADER_HEIGHT, UIHorizontalAlignment.Center, UIAlignAnchor.TopLeft);
                    m_ListView.AddColumn(Columns.COLUMN_PASSENGERS, Localization.Get("OverviewPassengers"), Localization.Get("headerUsageTooltip"), iCOLUMN_WIDTH_PASSENGER, iHEADER_HEIGHT, UIHorizontalAlignment.Center, UIAlignAnchor.TopLeft);
                    m_ListView.AddColumn(Columns.COLUMN_VEHICLE_USAGE, Localization.Get("VehicleUsage"), Localization.Get("headerUsageTooltip"), iCOLUMN_WIDTH_VEHICLE_USAGE, iHEADER_HEIGHT, UIHorizontalAlignment.Center, UIAlignAnchor.TopLeft);
                    m_ListView.AddColumn(Columns.COLUMN_WAITING, Localization.Get("OverviewWaiting"), Localization.Get("headerWaitingTooltip"), iCOLUMN_WIDTH_WAITING, iHEADER_HEIGHT, UIHorizontalAlignment.Center, UIAlignAnchor.TopLeft);
                    m_ListView.AddColumn(Columns.COLUMN_BUSIEST, Localization.Get("headerBusiest"), Localization.Get("headerBusiestTooltip"), iCOLUMN_WIDTH_BUSIEST, iHEADER_HEIGHT, UIHorizontalAlignment.Center, UIAlignAnchor.TopLeft);
                    m_ListView.AddColumn(Columns.COLUMN_BORED, Localization.Get("OverviewBored"), Localization.Get("headerBoredTooltip"), iCOLUMN_WIDTH_BORED, iHEADER_HEIGHT, UIHorizontalAlignment.Center, UIAlignAnchor.TopLeft);
                    m_ListView.m_eventOnListViewColumnClick = ListViewColumnClickEvent;
                }
                else
                {
                    Debug.LogError("listView is null");
                }
            }

            LoadTransportLineTabs();
        }

        public void ListViewColumnClickEvent()
        {
            Debug.Log("ListViewColumnClickEvent");
            if (m_bLoadingLines)
            {
                return; // Don't try and reload lines til line loading is done.
            }

            // Redraw lines
            SetSelectedTransportType(m_SelectedTransportType);
        }


        public void OnIssuesClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            PublicTransportInstance.ToggleLineIssuePanel();
        }

        public void OnCloseClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            PublicTransportInstance.HideMainPanel();
        }

        public void LoadTransportLineTabs()
        {
            if (m_tabStrip != null)
            {
                List<PublicTransportType> oLineTypes = LineInfoLoader.GetLineTypes();
                if (m_TransportList != oLineTypes)
                {
                    // Clearing tab strip also destroys m_lblOverview;
                    m_tabStrip.Clear();
                    m_lblOverview = null;

                    m_TransportList = oLineTypes;
                    foreach (PublicTransportType transportType in m_TransportList)
                    {
                        m_tabStrip.AddTab(transportType);
                    }

                    // Select first transport type if current selection is invalid 
                    if (m_TransportList != null && m_TransportList.Count > 0)
                    {
                        if (!m_TransportList.Contains(m_SelectedTransportType))
                        {
                            m_SelectedTransportType = m_TransportList[0];
                        }

                        // Select correct tab button
                        m_tabStrip.SelectTab(m_SelectedTransportType);
                    }

                    AddOverviewLabelToTabStrip();

                    m_tabStrip.ResetLayout();
                }
            }
        }



        public void OnTabSelectionChanged(object sender, EventArgs e)
        {
            if (e != null)
            {
                TabStrip.TabStripSelectionChangedArgs eArgs = (TabStrip.TabStripSelectionChangedArgs)e;
                if (eArgs != null)
                {
                    SetSelectedTransportType(eArgs.m_eType);
                }
            }

        }

        public void ShowPanel()
        {
            if (!isVisible)
            {
                PlayClickSound(this);

                if (m_ListView != null)
                {
                    // Turn on public transport mode so you can see the lines
                    Singleton<InfoManager>.instance.SetCurrentMode(InfoManager.InfoMode.Transport, InfoManager.SubInfoMode.Default);
                    UIView.library.Hide("PublicTransportInfoViewPanel");

                    LoadTransportLineTabs();

                    LineIssue.IssueLevel eLevel;
                    if (LineIssueManager.Instance != null)
                    {
                        ushort usLineId = LineIssueManager.Instance.GetHighestLineWarningIcons(out eLevel);
                        if (usLineId != 0 && eLevel != LineIssue.IssueLevel.ISSUE_NONE)
                        {
                            // Open tab for this line
                            TransportLine oLine = TransportManager.instance.m_lines.m_buffer[usLineId];
                            TransportInfo.TransportType eType = oLine.Info.m_transportType;
                            PublicTransportType eTransportType = PublicTransportTypeUtils.Convert(eType);
                            SetSelectedTransportType(eTransportType);
                        }
                        else
                        {
                            SetSelectedTransportType(m_SelectedTransportType);
                        }
                    }

                }

                Show();
            }
        }



        public void PlayWarningSound()
        {
            //PlayDisabledClickSound(this);
            PlayClickSound(this);
        }

        public void HidePanel(bool bClearInfoMode = true)
        {
            PlayClickSound(this);
            if (bClearInfoMode)
            {
                Singleton<InfoManager>.instance.SetCurrentMode(InfoManager.InfoMode.None, InfoManager.SubInfoMode.Default);
            }
            Hide();
            m_ListView?.Clear();
        }

        public void SetSelectedTransportType(PublicTransportType eType)
        {
            if (m_bLoadingLines)
            {
                return;
            }
            m_bLoadingLines = true;

            try
            {
                m_SelectedTransportType = eType;

                // Select correct tab button
                m_tabStrip?.SelectTab(m_SelectedTransportType);

                int iLineCount = 0;
                if (m_ListView != null)
                {
                    LineInfoLoader oLoad = new LineInfoLoader();
                    List<LineInfoBase> oList = oLoad.GetLineList(m_SelectedTransportType);
                    if (oList != null)
                    {
                        iLineCount = oList.Count;

                        // Sort list before adding it
                        oList.Sort(LineInfoBase.GetComparator(m_ListView.m_eSortColumn));

                        // Reverse if necessary
                        if (m_ListView.m_bSortDescending)
                        {
                            oList.Reverse();
                        }

                        m_ListView.GetList().rowsData = new FastList<object>
                        {
                            m_buffer = oList.ToArray(),
                            m_size = oList.Count,
                        };
                    }
                }
                else
                {
                    Debug.Log("m_ListView is null");
                }

                if (m_title != null)
                {
                    m_title.title = "[" + m_SelectedTransportType.ToString() + ":" + iLineCount + "] " + ITransportInfoMain.Title;
                }

                UpdateOverviewData();
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
            }
            finally
            {
                m_bLoadingLines = false;
            }
        }

        public void UpdateLineData()
        {
            if (m_bLoadingLines)
            {
                return;
            }

            // Update list view
            SetSelectedTransportType(m_SelectedTransportType);

            // Update overview label
            UpdateOverviewData();
        }

        public void AddOverviewLabelToTabStrip()
        {
            if (m_tabStrip != null && m_lblOverview == null)
            {
                int iButtonCount = m_tabStrip.childCount; 
                m_lblOverview = m_tabStrip.AddUIComponent<UILabel>();
                if (m_lblOverview != null)
                {
                    m_lblOverview.BringToFront(); // Bring to the right of other elements
                    m_lblOverview.name = "m_lblOverview";
                    m_lblOverview.text = Localization.Get("lblOverview");// "Overview";
                    m_lblOverview.tooltip = Localization.Get("lblOverviewTooltip");
                    m_lblOverview.verticalAlignment = UIVerticalAlignment.Middle;
                    m_lblOverview.textAlignment = UIHorizontalAlignment.Right;
                    m_lblOverview.autoSize = false;
                    m_lblOverview.height = m_tabStrip.height - 8; // 4 padding top and bottom
                    int iButtonWidth = (iButtonCount * 46 + 4 + 10);
                    m_lblOverview.width = m_tabStrip.width - iButtonWidth;

                    // DEBUG HELP
                    //Debug.Log("Overview width: " + m_lblOverview.width + "iButtonWidth: " + iButtonWidth);
                    //m_lblOverview.backgroundSprite = "InfoviewPanel";
                    //m_lblOverview.color = Color.red;
                }
                else
                {
                    Debug.LogError("m_lblOverview is null");
                }
            }
        }

        public void UpdateOverviewData()
        {
            // Update overview label
            if (m_lblOverview != null)
            {
                LineInfoLoader oLoader = new LineInfoLoader();
                List<LineInfoBase> oLines = oLoader.GetAllLinesList();

                int iPassengerCount = 0;
                int iWaiting = 0;
                int iBored = 0;
                foreach (LineInfoBase oInfo in oLines)
                {
                    iPassengerCount += oInfo.m_iPassengers; 
                    iWaiting += oInfo.m_iWaiting;
                    iBored += oInfo.m_iBored;
                }
                m_lblOverview.text = Localization.Get("OverviewPassengers") + ": " + iPassengerCount + " | " + Localization.Get("OverviewWaiting") + ": " + iWaiting + " | " + Localization.Get("OverviewBored") + ": " + iBored;
            }
            else
            {
                Debug.LogError("m_lblOverview is null"); 
            }
        }

        public override void OnDestroy()
        {
            if (m_ListView != null)
            {
                Destroy(m_ListView.gameObject);
            }
            if (m_title != null)
            {
                Destroy(m_title.gameObject);
            }
        }
    }
}
