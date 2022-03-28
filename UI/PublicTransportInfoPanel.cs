using ColossalFramework;
using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PublicTransportInfo
{
    public class PublicTransportInfoPanel : UIPanel
    {
        public const float PanelWidth = 1000;
        public const float PanelHeight = 600;
        public const int Margin = 10;
        public const int iHEADER_HEIGHT = 20;

        public const int iCOLUMN_WIDTH_COLOR = 20;
        public const int iCOLUMN_WIDTH_NAME = 370;
        public const int iCOLUMN_WIDTH_STOPS = 70;
        public const int iCOLUMN_WIDTH_VEHICLES = 40;
        public const int iCOLUMN_WIDTH_PASSENGER = 150;
        public const int iCOLUMN_WIDTH_WAITING = 85;
        public const int iCOLUMN_WIDTH_BUSIEST = 80;
        public const int iCOLUMN_WIDTH_BORED = 70;
        
        private TabStrip? m_tabStrip;
        private ListView? m_ListView = null;
        private ListViewHeader? m_headingPanel = null;
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
            this.name = "PublicTransportInfoPanel";
            this.width = PanelWidth;
            this.height = PanelHeight;
            this.backgroundSprite = "UnlockingPanel2";
            this.canFocus = true;
            this.isInteractive = true;
            this.isVisible = false;
            playAudioEvents = true;
            autoLayout = true;
            autoLayoutDirection = LayoutDirection.Vertical;
            autoLayoutPadding = new RectOffset(Margin, Margin, 0, 0);
            this.CenterToParent();

            // Title Bar
            m_title = AddUIComponent<UITitleBar>();
            m_title.SetOnclickHandler(OnCloseClick);
            m_title.title = ITransportInfoMain.Title;

            m_tabStrip = this.AddUIComponent<TabStrip>();
            if (m_tabStrip != null)
            {
                m_tabStrip.backgroundSprite = "GenericPanel";
                m_tabStrip.name = "tabStrip";
                //m_tabStrip.position = new Vector3(Margin, -m_title.height - Margin);
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

            float fHeadingPanelHeight = 0;
            m_headingPanel = AddUIComponent<ListViewHeader>();
            if (m_headingPanel != null)
            {
                m_headingPanel.Setup(m_tabStrip.width, ListViewColumnClickEvent);
                fHeadingPanelHeight = m_headingPanel.height;
            }
            else
            {
                Debug.LogError("ListViewHeader is null");
            }

            UIPanel tabPanel = this.AddUIComponent<UIPanel>();
            if (tabPanel != null)
            {
                tabPanel.autoLayoutDirection = LayoutDirection.Vertical;
                tabPanel.backgroundSprite = "InfoviewPanel";
                tabPanel.width = m_tabStrip.width;
                tabPanel.height = height - m_title.height - m_tabStrip.height - fHeadingPanelHeight - Margin;
                tabPanel.autoLayout = true;

                m_ListView = ListView.Create(tabPanel);
                if (m_ListView != null)
                {
                    m_ListView.width = tabPanel.width;
                    m_ListView.height = tabPanel.height;
                }
                else
                {
                    Debug.LogError("listView is null");
                }
            }

            LoadTransportLineTabs();
        }

        public void ListViewColumnClickEvent(ListViewRowComparer.Columns eColumn, bool bSortDescending)
        {
            if (m_bLoadingLines)
            {
                return; // Don't try and reload lines til line loading is done.
            }

            // Redraw lines
            SetSelectedTransportType(m_SelectedTransportType);
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
                TabStrip.TabStripSelectionChangedArgs eArgs = (TabStrip.TabStripSelectionChangedArgs) e;
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

                    Debug.Log("SetSelectedTransportType" + m_SelectedTransportType.ToString());
                    LoadTransportLineTabs();

                    LineIssue.IssueLevel eLevel;
                    ushort usLineId = PublicTransportInstance.GetLineIssueManager().GetHighestLineWarningIcons(out eLevel);
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

                if (m_title != null)
                {
                    m_title.title = "[" + m_SelectedTransportType.ToString() + "] " + ITransportInfoMain.Title;
                }

                // Select correct tab button
                m_tabStrip?.SelectTab(m_SelectedTransportType);

                if (m_ListView != null)
                {
                    LineInfoLoader oLoad = new LineInfoLoader();
                    List<LineInfoBase> oList = oLoad.GetLineList(m_SelectedTransportType);

                    m_ListView.Clear();

                    if (oList != null)
                    {
                        // Sort list before adding it so the items are in the correct order from the start
                        if (m_headingPanel != null)
                        {
                            LineInfoLoader.SortList(oList, m_headingPanel.GetSortColumn(), m_headingPanel.GetSortDirection());
                        }

                        foreach (LineInfoBase oInfo in oList)
                        {
                            m_ListView.AddItem(oInfo);
                        }
                    }
                }
                else
                {
                    Debug.Log("m_ListView is null");
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
            if (m_headingPanel != null)
            {
                m_ListView?.UpdateLineData(m_headingPanel.GetSortColumn(), m_headingPanel.GetSortDirection());
            }

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
                    m_lblOverview.text = "Overview";
                    m_lblOverview.tooltip = "Overview for all lines.";
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
                m_lblOverview.text = "Passengers: " + iPassengerCount + " | Waiting: " + iWaiting + " | Bored: " + iBored;
            }
            else
            {
                Debug.LogError("m_lblOverview is null"); 
            }
        }

        public override void OnDestroy()
        {
            if (m_headingPanel != null)
            {
                m_headingPanel.Destroy();
            }
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
