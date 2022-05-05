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
        //private int m_iIssueIndex = 0;
        private LineIssue? m_activeLineIssue = null;
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
            eventPositionChanged += (sender, e) =>
            {
                ModSettings settings = PublicTransportInstance.GetSettings();
                settings.LineIssueLocationSaved = true;
                settings.LineIssueLocation = absolutePosition;
                settings.Save();
            };

            if (PublicTransportInstance.GetSettings().LineIssueLocationSaved)
            {
                absolutePosition = PublicTransportInstance.GetSettings().LineIssueLocation;
                FitToScreen();
            } 
            else
            {
                CenterToParent();
            }

            // Title Bar
            m_title = AddUIComponent<UITitleBar>();
            m_title.SetOnclickHandler(OnCloseClick);
            m_title.SetFollowHandler(OnFollowClick);
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
            m_btnDelete.tooltip = "Delete Issue";
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

        private void FitToScreen()
        {
            Vector2 oScreenVector = UIView.GetAView().GetScreenResolution();
            float fX = Math.Max(0.0f, Math.Min(absolutePosition.x, oScreenVector.x - width));
            float fY = Math.Max(0.0f, Math.Min(absolutePosition.y, oScreenVector.y - height));
            Vector3 oFitPosition = new Vector3(fX, fY, absolutePosition.z);
            absolutePosition = oFitPosition;
        }

        new public void Show()
        {
            UpdatePanel();
            base.Show();
            ShowIssue();
        }

        new public void Hide()
        {
            m_activeLineIssue = null;
            base.Hide();
        }

        public void OnCloseClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            PublicTransportInstance.HideLineIssuePanel();
        }

        public void OnFollowClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            ShowIssue();
        }

        public void SetInitialLineId(int iLineId)
        {
            List<LineIssue> oIssues = GetVisibleLineIssues();
            if (oIssues != null)
            {
                foreach (LineIssue oIssue in oIssues)
                {
                    if (oIssue.m_iLineId == iLineId)
                    {
                        m_activeLineIssue = oIssue;
                        break;
                    }
                }
                if (m_activeLineIssue == null && oIssues.Count > 0)
                {
                    m_activeLineIssue = oIssues[0];
                }
            }
        }

        public int GetActiveIndex()
        {
            int iIndex = 0;
            List<LineIssue> oIssues = GetVisibleLineIssues();
            if (oIssues != null && oIssues.Count > 0 && m_activeLineIssue != null)
            {
                iIndex = Math.Max(0, oIssues.IndexOf(m_activeLineIssue));
            }
            return iIndex;
        }

        public void OnPrevClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            List<LineIssue> oIssues = GetVisibleLineIssues();
            if (oIssues != null && oIssues.Count > 0)
            {
                int iIndex = Mathf.Max(GetActiveIndex() - 1, 0);
                m_activeLineIssue = oIssues[iIndex];
                ShowIssue();
            }
        }

        public void OnNextClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            List<LineIssue> oIssues = GetVisibleLineIssues();
            if (oIssues != null && oIssues.Count > 0)
            {
                int iCurrentIndex = GetActiveIndex();
                int iNewIndex = Mathf.Min(iCurrentIndex + 1, oIssues.Count - 1);
                Debug.Log("iCurrentIndex" + iCurrentIndex + "iNewIndex" + iNewIndex);
                m_activeLineIssue = oIssues[iNewIndex];
                ShowIssue();
            }
        }

        public void OnDeleteClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            if (m_activeLineIssue != null)
            {
                m_activeLineIssue.SetHidden(true);
                m_activeLineIssue = null;
            }
               
            // Update visible issue array
            List<LineIssue> oIssues = GetVisibleLineIssues();                
            if (oIssues.Count > 0)
            {
                ShowIssue();
            } 
            else
            {
                PublicTransportInstance.HideLineIssuePanel();
            }
        }

        private LineIssue? GetActiveLineIssue()
        {
            if (m_activeLineIssue == null)
            {
                List<LineIssue> oIssues = GetVisibleLineIssues();
                if (oIssues != null && oIssues.Count > 0)
                {
                    m_activeLineIssue = oIssues[0];
                }
            }

            return m_activeLineIssue;
        }

        public void ShowIssue()
        {
            LineIssue? m_activeLineIssue = GetActiveLineIssue();
            if (isVisible && m_activeLineIssue != null)
            {
                PlayClickSound(this);
                UpdatePanel();
                m_activeLineIssue.ShowIssue();
            }
        }

        public void UpdatePanel()
        {
            List<LineIssue> oIssues = GetVisibleLineIssues();
            int iIssueCount = oIssues.Count;
            if (iIssueCount > 0)
            {
                LineIssue? activeLineIssue = GetActiveLineIssue();
                if (activeLineIssue != null)
                {
                    activeLineIssue.Update();

                    if (m_lblLine != null)
                    {
                        TransportLine oLine = TransportManager.instance.m_lines.m_buffer[activeLineIssue.m_iLineId];
                        m_lblLine.text = oLine.Info.m_transportType.ToString() + " Line: " + TransportManagerUtils.GetSafeLineName(activeLineIssue.m_iLineId);
                    }
                    if (m_lblVehicle != null)
                    {
                        m_lblVehicle.text = activeLineIssue.GetIssueLocation();
                    }
                    if (m_lblIssue != null)
                    {
                        m_lblIssue.text = activeLineIssue.GetIssueDescription();
                    }
                    if (m_btnDelete != null)
                    {
                        m_btnDelete.isEnabled = true;
                    }
                } else {
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
                    if (m_btnDelete != null)
                    {
                        m_btnDelete.isEnabled = false;
                    }
                }

                int iIndex = GetActiveIndex();
                if (m_title != null)
                {
                    m_title.title = "Line Issues (" + (iIndex + 1) + "/" + iIssueCount + ")";
                }
                if (m_btnPrev != null)
                {
                    m_btnPrev.isEnabled = iIndex > 0;
                }
                if (m_btnNext != null)
                {
                    m_btnNext.isEnabled = iIndex < iIssueCount - 1;
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
                    m_lblLine.text = "No issues detected.";
                }
                if (m_lblVehicle != null)
                {
                    m_lblVehicle.text = "";
                }
                if (m_lblIssue != null)
                {
                    m_lblIssue.text = "";
                }
                if (m_btnDelete != null)
                {
                    m_btnDelete.isEnabled = false;
                }
                if (m_btnPrev != null)
                {
                    m_btnPrev.isEnabled = false;
                }
                if (m_btnNext != null)
                {
                    m_btnNext.isEnabled = false;
                }
            }
        }

        public int GetActiveIssueId()
        {
            if (m_activeLineIssue != null)
            {
                return m_activeLineIssue.GetIssueId();
            }
            else
            {
                return 0;
            }
        }

        public void UpdateActiveIssue(LineIssue oNewIssue)
        {
            m_activeLineIssue = oNewIssue;
            UpdatePanel();
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
