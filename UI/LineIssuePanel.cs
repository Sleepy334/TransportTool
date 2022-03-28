using ColossalFramework;
using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PublicTransportInfo
{
    public class LineIssuePanel : UIPanel
    {
        const int iMARGIN = 8;

        private UITitleBar? m_title = null;
        private UILabel? m_lblLine = null;
        private UILabel? m_lblVehicle = null;
        private UILabel? m_lblIssue = null;
        private int m_iIssueIndex = 0;
        private UIButton? m_btnPrev = null;
        private UIButton? m_btnNext = null;
        private UIButton? m_btnDelete = null;
        private UISprite? m_spriteDelete = null;

        public LineIssuePanel() : base()
        {
        }

        private List<LineIssue> GetVisibleLineIssues()
        {
            return PublicTransportInstance.GetLineIssueManager().GetVisibleLineIssues();
        }

        public override void Start()
        {
            //base.Start();
            name = "PublicTransportInfoPanel";
            width = 400;
            height = 210;
            padding = new RectOffset(iMARGIN, iMARGIN, 4, 4);
            autoLayout = true;
            autoLayoutDirection = LayoutDirection.Vertical;
            backgroundSprite = "UnlockingPanel2";
            canFocus = true;
            isInteractive = true;
            isVisible = false;
            playAudioEvents = true;
            m_ClipChildren = true;
            CenterToParent();

            // Title Bar
            m_title = AddUIComponent<UITitleBar>();
            m_title.SetOnclickHandler(OnCloseClick);
            m_title.title = "Line Issues";

            // Line label
            m_lblLine = AddUIComponent<UILabel>();
            m_lblLine.width = width - (iMARGIN * 2);
            m_lblLine.height = 25;
            m_lblLine.padding = new RectOffset(4, 4, 4, 4);

            // Vehicle label
            m_lblVehicle = AddUIComponent<UILabel>();
            m_lblVehicle.width = width - (iMARGIN * 2);
            m_lblVehicle.height = 25;
            m_lblVehicle.padding = new RectOffset(4, 4, 4, 4);

            m_lblIssue = AddUIComponent<UILabel>();
            m_lblIssue.width = width - (iMARGIN * 2);
            m_lblIssue.height = 25;
            m_lblIssue.padding = new RectOffset(4, 4, 4, 4);

            UIPanel panel = AddUIComponent<UIPanel>();
            panel.width = 200;
            panel.height = 40;
            panel.width = width;
            panel.height = 40;
            panel.padding = new RectOffset(8, 8, 4, 8);
            //panel.backgroundSprite = "InfoviewPanel";
            //panel.color = Color.red;

            m_btnPrev = panel.AddUIComponent<UIButton>();
            m_btnPrev.text = "Prev";
            m_btnPrev.width = 80;
            m_btnPrev.height = 30;
            m_btnPrev.eventClick += OnPrevClick;
            m_btnPrev.normalBgSprite = "ButtonMenu";
            m_btnPrev.hoveredBgSprite = "ButtonMenuHovered";
            m_btnPrev.disabledBgSprite = "ButtonMenuDisabled";
            m_btnPrev.pressedBgSprite = "ButtonMenuPressed";

            m_btnDelete = panel.AddUIComponent<UIButton>();
            m_btnDelete.tooltip = "Delete";
            m_btnDelete.width = 70;
            m_btnDelete.height = 30;
            m_btnDelete.CenterToParent();
            m_btnDelete.eventClick += OnDeleteClick;
            m_btnDelete.normalBgSprite = "ButtonMenu";
            m_btnDelete.hoveredBgSprite = "ButtonMenuHovered";
            m_btnDelete.disabledBgSprite = "ButtonMenuDisabled";
            m_btnDelete.pressedBgSprite = "ButtonMenuPressed";

            m_spriteDelete = m_btnDelete.AddUIComponent<UISprite>();
            m_spriteDelete.atlas = PublicTransportInstance.LoadResources();
            m_spriteDelete.width = m_btnDelete.height - 4;
            m_spriteDelete.height = m_btnDelete.height - 4;
            m_spriteDelete.spriteName = "clear";// "ButtonMenu"
            m_spriteDelete.CenterToParent();

            m_btnNext = panel.AddUIComponent<UIButton>();
            m_btnNext.text = "Next";
            m_btnNext.width = m_btnPrev.width;
            m_btnNext.height = m_btnPrev.height;
            m_btnNext.eventClick += OnNextClick;
            m_btnNext.normalBgSprite = "ButtonMenu";
            m_btnNext.hoveredBgSprite = "ButtonMenuHovered";
            m_btnNext.disabledBgSprite = "ButtonMenuDisabled";
            m_btnNext.pressedBgSprite = "ButtonMenuPressed";

            m_btnPrev.relativePosition = new Vector3(m_btnDelete.relativePosition.x - (m_btnDelete.width * 0.5f) - (m_btnPrev.width * 0.5f) - 10, (float)((panel.height - m_btnPrev.height) * 0.5));
            m_btnNext.relativePosition = new Vector3(m_btnDelete.relativePosition.x + (m_btnDelete.width * 0.5f) + (m_btnNext.width * 0.5f), (float)((panel.height - m_btnNext.height) * 0.5));

            UIPanel pnlTransportTool = AddUIComponent<UIPanel>();
            pnlTransportTool.width = width;
            pnlTransportTool.height = m_btnNext.height + 8;

            UIButton btnTranportTool = pnlTransportTool.AddUIComponent<UIButton>();
            btnTranportTool.text = "Transport Tool";
            btnTranportTool.width = 200;
            btnTranportTool.height = 30;
            btnTranportTool.CenterToParent();
            btnTranportTool.normalBgSprite = "ButtonMenu";
            btnTranportTool.hoveredBgSprite = "ButtonMenuHovered";
            btnTranportTool.disabledBgSprite = "ButtonMenuDisabled";
            btnTranportTool.pressedBgSprite = "ButtonMenuPressed";
            btnTranportTool.eventClick += (c, e) =>
            {
                PublicTransportInstance.HideLineIssuePanel();
                PublicTransportInstance.ShowMainPanel();
            };

            ShowIssue();
        }

        new public void Show()
        {
            UpdatePanel();
            base.Show();
            ShowIssue();
        }

        new public void Hide()
        {
            base.Hide();
        }

        public void OnCloseClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            PublicTransportInstance.HideLineIssuePanel();
        }

        public void SetInitialLineId(int iLineId)
        {
            m_iIssueIndex = 0;

            List<LineIssue> oIssues = GetVisibleLineIssues();
            if (oIssues != null)
            {
                foreach (LineIssue oIssue in oIssues)
                {
                    if (oIssue.m_iLineId == iLineId)
                    {
                        break;
                    }

                    m_iIssueIndex++;
                }
            }
        }

        public void OnPrevClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            List<LineIssue> oIssues = GetVisibleLineIssues();
            if (oIssues != null)
            {
                m_iIssueIndex = Mathf.Max(m_iIssueIndex - 1, 0);
                ShowIssue();
            }
        }

        public void OnNextClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            List<LineIssue> oIssues = GetVisibleLineIssues();
            if (oIssues != null)
            {
                m_iIssueIndex = Mathf.Min(m_iIssueIndex + 1, oIssues.Count - 1);
                ShowIssue();
            }
        }

        public void OnDeleteClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            List <LineIssue> oIssues = GetVisibleLineIssues();
            if (oIssues != null && m_iIssueIndex >= 0 && m_iIssueIndex < oIssues.Count)
            {
                LineIssue oIssue = oIssues[m_iIssueIndex];
                if (oIssue.CanDelete())
                {
                    PublicTransportInstance.GetLineIssueManager().RemoveLineIssue(oIssue);
                } 
                else
                {
                    oIssue.SetHidden(true);
                }
                UpdatePanel();

                // Update visible issue array
                oIssues = GetVisibleLineIssues();

                // Check index is still in range
                m_iIssueIndex = Math.Min(m_iIssueIndex, oIssues.Count - 1);
                m_iIssueIndex = Math.Max(0, m_iIssueIndex);
                
                if (oIssues.Count > 0)
                {
                    ShowIssue();
                } else
                {
                    PublicTransportInstance.HideLineIssuePanel();
                }
            }
        }

        public void ShowIssue()
        {
            if (isVisible)
            {
                List<LineIssue> oIssues = GetVisibleLineIssues();
                if (oIssues != null && oIssues.Count > 0 && m_iIssueIndex >= 0 && m_iIssueIndex < oIssues.Count)
                {
                    PlayClickSound(this);
                    UpdatePanel();

                    LineIssue oIssue = oIssues[m_iIssueIndex];
                    if (oIssue != null)
                    {
                        oIssue.ShowIssue();
                    }
                }
            }
        }

        public void UpdatePanel()
        {
            int iIssueCount = 0;

            List<LineIssue> oIssues = GetVisibleLineIssues();
            if (oIssues != null && oIssues.Count > 0)
            {
                iIssueCount = oIssues.Count;

                // MinMax it to ensure it is in range
                m_iIssueIndex = Mathf.Min(m_iIssueIndex, iIssueCount - 1);
                m_iIssueIndex = Mathf.Max(m_iIssueIndex, 0);

                if (m_title != null)
                {
                    m_title.title = "Line Issues (" + (m_iIssueIndex + 1) + "/" + iIssueCount + ")";
                }

                LineIssue oIssue = oIssues[m_iIssueIndex];
                if (oIssue != null)
                {
                    oIssue.Update();

                    if (m_lblLine != null)
                    {
                        TransportLine oLine = TransportManager.instance.m_lines.m_buffer[oIssue.m_iLineId];
                        m_lblLine.text = oLine.Info.m_transportType.ToString() + " Line: " + LineInfoBase.GetSafeLineName(oIssue.m_iLineId, oLine);
                    }
                    if (m_lblVehicle != null)
                    {
                        m_lblVehicle.text = oIssue.GetIssueLocation();
                    }
                    if (m_lblIssue != null)
                    {
                        m_lblIssue.text = oIssue.GetIssueDescription();
                    }
                }

                if (m_btnPrev != null)
                {
                    m_btnPrev.isEnabled = m_iIssueIndex > 0;
                }
                if (m_btnNext != null)
                {
                    m_btnNext.isEnabled = m_iIssueIndex < iIssueCount - 1;
                }
                if (m_btnDelete != null)
                {
                    m_btnDelete.isEnabled = true;
                }
            }
            else
            {
                if (m_title != null)
                {
                    m_title.title = "Line Issues";
                }
                if (m_lblLine != null)
                {
                    m_lblLine.text = "";
                }
                if (m_lblVehicle != null)
                {
                    m_lblVehicle.text = "";
                }
                if (m_lblIssue != null)
                {
                    m_lblIssue.text = "";
                }
                if (m_btnPrev != null)
                {
                    m_btnPrev.isEnabled = false;
                }
                if (m_btnNext != null)
                {
                    m_btnNext.isEnabled = false;
                }
                if (m_btnDelete != null)
                {
                    m_btnDelete.isEnabled = false;
                }
            }
        }

        public override void OnDestroy()
        {
            if (m_lblLine != null)
            {
                Destroy(m_lblLine.gameObject);
            }
            if (m_lblVehicle != null)
            {
                Destroy(m_lblVehicle.gameObject);
            }
            if (m_lblIssue != null)
            {
                Destroy(m_lblIssue.gameObject);
            }
            if (m_btnPrev != null)
            {
                Destroy(m_btnPrev.gameObject);
            }
            if (m_btnNext != null)
            {
                Destroy(m_btnNext.gameObject);
            }
            if (m_btnDelete != null)
            {
                Destroy(m_btnDelete.gameObject);
            }
            if (m_spriteDelete != null)
            {
                m_spriteDelete.atlas = null;
                Destroy(m_spriteDelete.gameObject);
            }
            if (m_title != null)
            {
                Destroy(m_title.gameObject);
            }
        }
    }
}
