using ColossalFramework;
using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace PublicTransportInfo
{
    public class PublicTransportInfoPanel : UIPanel
    {
        private const float PanelWidth = 950; 
        private const float PanelHeight = 600;
        private const float Margin = 10;
        private const int iHEADER_HEIGHT = 20;

        public const int iCOLUMN_WIDTH_COLOR = 20;
        public const int iCOLUMN_WIDTH_NAME = 380;
        public const int iCOLUMN_WIDTH_STOPS = 70;
        public const int iCOLUMN_WIDTH_PASSENGER = 150;
        public const int iCOLUMN_WIDTH_WAITING = 85;
        public const int iCOLUMN_WIDTH_BUSIEST = 80;
        public const int iCOLUMN_WIDTH_BORED = 80;

        private TabStrip? m_tabStrip;
        private static ListView? s_ListView = null;
        private List<PublicTransportType> m_TransportList;
        private PublicTransportType m_SelectedTransportType;
        private ListViewRowComparer.Columns m_eSortColumn = ListViewRowComparer.Columns.COLUMN_NAME;
        private bool m_bSortDesc = false;

        private static bool m_bLoadingLines = false;

        public PublicTransportInfoPanel() : base()
        {
            m_TransportList = new List<PublicTransportType>();
            m_SelectedTransportType = PublicTransportType.Bus;
            m_tabStrip = null;
        }

        public override void Start()
        {
            Debug.Log("PublicTransportInfoPanel::Start");
            //base.Start();

            UIView view = GetUIView();

            this.name = "PublicTransportInfoPanel";  
            //this.autoLayout = false;
            this.width = PanelWidth;
            this.height = PanelHeight;
            this.backgroundSprite = "UnlockingPanel2";
            this.canFocus = true;
            this.isInteractive = true;
            this.isVisible = false;
            playAudioEvents = true;
            //this.relativePosition = new Vector3(Mathf.Floor((view.fixedWidth - width) / 2), Mathf.Floor((view.fixedHeight - height) / 2));
            this.CenterToParent();

            // Title Bar
            UITitleBar m_title = AddUIComponent<UITitleBar>();
            //m_title.iconSprite = "InfoIconTrafficCongestion";
            m_title.title = ITransportInfoMain.Title;

            m_tabStrip = this.AddUIComponent<TabStrip>();
            if (m_tabStrip != null)
            {
                m_tabStrip.backgroundSprite = "GenericPanel";
                m_tabStrip.name = "tabStrip";
                //tabStrip.relativePosition = new Vector3(Margin, TitleHeight + 20);
                m_tabStrip.position = new Vector3(Margin, -m_title.height - Margin);
                m_tabStrip.size = new Vector2(this.width - (Margin * 2), 40);
                m_tabStrip.autoLayoutDirection = LayoutDirection.Horizontal;
                //tabStrip.autoFitChildrenHorizontally = true;
                m_tabStrip.autoLayoutStart = LayoutStart.TopLeft;
                m_tabStrip.autoLayoutPadding = new RectOffset(4, 4, 4, 4);
                m_tabStrip.autoLayout = true;
                m_tabStrip.SelectionChangedEventHandler += OnTabSelectionChanged;
            } else
            {
                Debug.Log("m_tabStrip is null");
                return;
            }

            Debug.Log("Create container panel");
            UIPanel tabPanel = this.AddUIComponent<UIPanel>();
            if (tabPanel != null)
            {
                tabPanel.autoLayoutDirection = LayoutDirection.Vertical;
                //tabPanel.backgroundSprite = "GenericPanel";
                tabPanel.backgroundSprite = "InfoviewPanel";
                tabPanel.position = new Vector3(Margin, -m_title.height - Margin - m_tabStrip.height);
                // Matching the size of the root panel so no scrolling happens accidentally
                tabPanel.size = new Vector2(this.width - (Margin * 2), this.height - m_title.height - m_tabStrip.height - (Margin * 2));
                tabPanel.autoLayout = true;

                UIPanel m_headingPanel = tabPanel.AddUIComponent<UIPanel>();
                if (m_headingPanel != null)
                {
                    void OnMouseHover(UIComponent component, UIMouseEventParameter eventParam)
                    {
                        UILabel txtLabel = component as UILabel;
                        if (txtLabel != null)
                        {
                            txtLabel.textColor = Color.yellow;
                        }
                    }

                    void OnMouseLeave(UIComponent component, UIMouseEventParameter eventParam)
                    {
                        UILabel txtLabel = component as UILabel;
                        if (txtLabel != null)
                        {
                            txtLabel.textColor = Color.white;
                        }
                    }

                    m_headingPanel.width = tabPanel.width;
                    m_headingPanel.height = iHEADER_HEIGHT;
                    m_headingPanel.backgroundSprite = "ListItemHighlight";
                    m_headingPanel.autoLayoutDirection = LayoutDirection.Horizontal;
                    m_headingPanel.autoLayoutStart = LayoutStart.TopLeft;
                    m_headingPanel.autoLayoutPadding = new RectOffset(2, 2, 2, 2);
                    m_headingPanel.autoLayout = true;

                    UILabel lblColor = m_headingPanel.AddUIComponent<UILabel>();
                    //lblName.backgroundSprite = "MenuPanel2";
                    lblColor.text = "";
                    lblColor.autoSize = false;
                    lblColor.height = iHEADER_HEIGHT;
                    lblColor.width = iCOLUMN_WIDTH_COLOR;

                    UILabel lblName = m_headingPanel.AddUIComponent<UILabel>();
                    //lblName.backgroundSprite = "MenuPanel2";
                    lblName.text = "Line Name";
                    lblName.autoSize = false;
                    lblName.height = iHEADER_HEIGHT;
                    lblName.width = iCOLUMN_WIDTH_NAME;
                    lblName.eventMouseEnter += OnMouseHover;
                    lblName.eventMouseLeave += OnMouseLeave;

                    UILabel lblStops = m_headingPanel.AddUIComponent<UILabel>();
                    //lblName.backgroundSprite = "MenuPanel2";
                    lblStops.text = "Stops";
                    lblStops.tooltip = "Total number of stops on this line";
                    lblStops.textAlignment = UIHorizontalAlignment.Center;
                    lblStops.autoSize = false;
                    lblStops.height = iHEADER_HEIGHT;
                    lblStops.width = iCOLUMN_WIDTH_STOPS;
                    lblStops.AlignTo(m_headingPanel, UIAlignAnchor.TopRight);
                    lblStops.eventMouseEnter += OnMouseHover;
                    lblStops.eventMouseLeave += OnMouseLeave;

                    UILabel lblPassengers = m_headingPanel.AddUIComponent<UILabel>();
                    //lblPassengers.backgroundSprite = "MenuPanel2";
                    lblPassengers.name = "lblPassengers";
                    lblPassengers.text = "Passengers";
                    lblPassengers.tooltip = "Current Passengers / Total Capacity";
                    lblPassengers.textAlignment = UIHorizontalAlignment.Center;
                    lblPassengers.autoSize = false;
                    lblPassengers.height = iHEADER_HEIGHT;
                    lblPassengers.width = iCOLUMN_WIDTH_PASSENGER;
                    lblPassengers.AlignTo(m_headingPanel, UIAlignAnchor.TopRight);
                    lblPassengers.eventMouseEnter += OnMouseHover;
                    lblPassengers.eventMouseLeave += OnMouseLeave;

                    UILabel lblWaiting = m_headingPanel.AddUIComponent<UILabel>();
                    //lblWaiting.backgroundSprite = "MenuPanel2";
                    lblWaiting.text = "Waiting";
                    lblWaiting.tooltip = "Total number of people waiting at all stops";
                    lblWaiting.textAlignment = UIHorizontalAlignment.Center;
                    lblWaiting.autoSize = false;
                    lblWaiting.height = iHEADER_HEIGHT;
                    lblWaiting.width = iCOLUMN_WIDTH_WAITING;
                    lblWaiting.AlignTo(m_headingPanel, UIAlignAnchor.TopRight);
                    lblWaiting.eventMouseEnter += OnMouseHover;
                    lblWaiting.eventMouseLeave += OnMouseLeave;

                    UILabel lblBusiest = m_headingPanel.AddUIComponent<UILabel>();
                    //lblBusiest.backgroundSprite = "MenuPanel2";
                    lblBusiest.text = "Busiest"; 
                    lblBusiest.tooltip = "Number of people waiting at busiest stop";
                    lblBusiest.textAlignment = UIHorizontalAlignment.Center;
                    //lblBusiest.position = new Vector3(640, -2);
                    lblBusiest.autoSize = false;
                    lblBusiest.height = iHEADER_HEIGHT;
                    lblBusiest.width = iCOLUMN_WIDTH_BUSIEST;
                    lblBusiest.AlignTo(m_headingPanel, UIAlignAnchor.TopRight);
                    lblBusiest.eventMouseEnter += OnMouseHover;
                    lblBusiest.eventMouseLeave += OnMouseLeave;

                    UILabel lblBored = m_headingPanel.AddUIComponent<UILabel>();
                    lblBored.text = "Bored";
                    lblBored.tooltip = "Number of people who are waiting too long";
                    lblBored.textAlignment = UIHorizontalAlignment.Center; 
                    lblBored.autoSize = false;
                    lblBored.height = iHEADER_HEIGHT;
                    lblBored.width = iCOLUMN_WIDTH_BUSIEST;
                    lblBored.AlignTo(m_headingPanel, UIAlignAnchor.TopRight);
                    lblBored.eventMouseEnter += OnMouseHover;
                    lblBored.eventMouseLeave += OnMouseLeave;

                    void OnMouseClick(UIComponent component, UIMouseEventParameter eventParam)
                    {
                        if (m_bLoadingLines)
                        {
                            return; // Don't try and reload lines til line loading is done.
                        }
                        
                        UILabel txtLabel = component as UILabel;
                        if (txtLabel != null)
                        {
                            UILabel txtSelectedColumn = lblName;
                            ListViewRowComparer.Columns eColumn = 0;
                            if (txtLabel == lblName) {
                                eColumn = ListViewRowComparer.Columns.COLUMN_NAME;
                                txtSelectedColumn = lblName;
                            }
                            else if (txtLabel == lblStops)
                            {
                                eColumn = ListViewRowComparer.Columns.COLUMN_STOPS;
                                txtSelectedColumn = lblStops;
                            }
                            else if (txtLabel == lblPassengers)
                            {
                                eColumn = ListViewRowComparer.Columns.COLUMN_PASSENGERS;
                                txtSelectedColumn = lblPassengers;
                            }
                            else if (txtLabel == lblWaiting)
                            {
                                eColumn = ListViewRowComparer.Columns.COLUMN_WAITING;
                                txtSelectedColumn = lblWaiting;
                            }
                            else if (txtLabel == lblBusiest)
                            {
                                eColumn = ListViewRowComparer.Columns.COLUMN_BUSIEST;
                                txtSelectedColumn = lblBusiest;
                            }
                            else if (txtLabel == lblBored)
                            {
                                eColumn = ListViewRowComparer.Columns.COLUMN_BORED;
                                txtSelectedColumn = lblBored;
                            }

                            if (m_eSortColumn == eColumn)
                            {
                                m_bSortDesc = !m_bSortDesc;
                            }
                            else
                            {
                                m_eSortColumn = eColumn;
                            }

                            AddSortCharacter(txtSelectedColumn);

                            // Redraw lines
                            SetSelectedTransportType(m_SelectedTransportType);
                        }
                    }

                    void AddSortCharacter(UILabel txtSelectedColumn)
                    {
                        lblName.text = "Line Name";
                        lblStops.text = "Stops";
                        lblPassengers.text = "Passengers";
                        lblWaiting.text = "Waiting";
                        lblBusiest.text = "Busiest";
                        lblBored.text = "Bored";

                        string sSortCharacter = Utils.GetSortCharacter(m_bSortDesc);
                        if (txtSelectedColumn.textAlignment == UIHorizontalAlignment.Left)
                        {
                            txtSelectedColumn.text += " " + sSortCharacter;
                        }
                        else
                        {
                            txtSelectedColumn.text = sSortCharacter + " " + txtSelectedColumn.text;
                        }
                    }

                    lblName.eventClick += OnMouseClick;
                    lblStops.eventClick += OnMouseClick;
                    lblPassengers.eventClick += OnMouseClick;
                    lblWaiting.eventClick += OnMouseClick;
                    lblBusiest.eventClick += OnMouseClick;
                    lblBored.eventClick += OnMouseClick;

                    AddSortCharacter(lblName);
                }

                s_ListView = ListView.Create(tabPanel);
                if (s_ListView != null)
                {
                    s_ListView.position = new Vector3(0f, 0f);
                    s_ListView.size = new Vector2(this.width - (Margin * 2), this.height - m_title.height - m_tabStrip.height - (Margin * 2) - iHEADER_HEIGHT);
                } else
                {
                    Debug.Log("listView is null");
                }
            }

            LoadTransportLineTabs();
        }

        public void LoadTransportLineTabs()
        {
            if (m_tabStrip != null)
            {
                List<PublicTransportType> oLineTypes = LineInfoLoader.GetLineTypes();
                if (m_TransportList != oLineTypes)
                {
                    m_tabStrip.Clear();
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
                }
            }
        }

        public void OnTabSelectionChanged(object sender, EventArgs e)
        {
            if (e != null)
            {
                TabStrip.TabStripSelectionChangedArgs eArgs = e as TabStrip.TabStripSelectionChangedArgs;
                if (eArgs != null)
                {
                    Debug.Log("Selection changed" + eArgs.m_eType.ToString());
                    SetSelectedTransportType(eArgs.m_eType);
                }
            }
            
        }

        public void ShowPanel()
        {
            Debug.Log("PublicTransportInfoPanel::ShowPanel");
            PlayClickSound(this);

            if (s_ListView != null)
            {
                // Turn on public transport mode so you can see the lines
                Singleton<InfoManager>.instance.SetCurrentMode(InfoManager.InfoMode.Transport, InfoManager.SubInfoMode.Default);
                UIView.library.Hide("PublicTransportInfoViewPanel");

                Debug.Log("SetSelectedTransportType");
                LoadTransportLineTabs();
                SetSelectedTransportType(m_SelectedTransportType);
            }

            Debug.Log("Show");
            Show();
        }

        public void HidePanel(bool bClearInfoMode = true)
        {
            Debug.Log("PublicTransportInfoPanel::HidePanel");
            PlayClickSound(this);
            if (bClearInfoMode)
            {
                Singleton<InfoManager>.instance.SetCurrentMode(InfoManager.InfoMode.None, InfoManager.SubInfoMode.Default);
            }
            Hide();
            s_ListView?.Clear();
        }

        public void SetSelectedTransportType(PublicTransportType eType)
        {
            if (m_bLoadingLines)
            {
                return;
            }
            m_bLoadingLines = true;

            m_SelectedTransportType = eType;

            try
            {
                // Select correct tab button
                m_tabStrip?.SelectTab(m_SelectedTransportType);

                if (s_ListView != null)
                {
                    LineInfoLoader oLoad = new LineInfoLoader();
                    List<LineInfo> oList = oLoad.GetLineList(m_SelectedTransportType);

                    s_ListView.Clear();

                    if (oList != null)
                    {
                        // Sort list before adding it so the items are in the correct order from the start
                        LineInfoLoader.SortList(oList, m_eSortColumn, m_bSortDesc);

                        foreach (LineInfo oInfo in oList)
                        {
                            s_ListView.AddItem(oInfo);
                        }
                    }
                }
                else
                {
                    Debug.Log("s_ListView is null");
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                if (ex.InnerException != null)
                {
                    Debug.LogException(ex.InnerException);
                }
            } finally { 
                m_bLoadingLines = false; 
            }
        }

        public void UpdateLineData() 
        {
            if (m_bLoadingLines)
            {
                return;
            }
            s_ListView?.UpdateLineData(m_eSortColumn, m_bSortDesc);
        }
    }
}
