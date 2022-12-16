using ColossalFramework;
using ColossalFramework.UI;
using PublicTransportInfo.Util;
using SleepyCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PublicTransportInfo
{
    public class LineIssuePanel : UIPanel
    {
        public const float fTEXT_SCALE = 0.8f;

        const int iMARGIN = 8;
        public const int iCOLUMN_WIDTH_TIME = 60;
        public const int iCOLUMN_WIDTH_NORMAL = 80;
        public const int iCOLUMN_VEHICLE_WIDTH = 180;
        public const int iCOLUMN_DESCRIPTION_WIDTH = 300;

        private UITitleBar? m_title = null;
        private ListView? m_listIssues = null;

        public LineIssuePanel() : base()
        {
        }

        public override void Start()
        {
            base.Start();
            name = "LineIssuePanel";
            width = 740;
            height = 500;
            if (!ModSettings.GetSettings().DisableTransparency)
            {
                opacity = 0.95f;
            }
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
                ModSettings settings = ModSettings.GetSettings();
                settings.LineIssueLocationSaved = true;
                settings.LineIssueLocation = absolutePosition;
                settings.Save();
            };

            if (ModSettings.GetSettings().LineIssueLocationSaved)
            {
                absolutePosition = ModSettings.GetSettings().LineIssueLocation;
                FitToScreen();
            } 
            else
            {
                CenterToParent();
            }

            // Title Bar
            m_title = AddUIComponent<UITitleBar>();
            m_title.SetOnclickHandler(OnCloseClick);
            m_title.title = Localization.Get("titleLineIssuesPanel");

            // Issue list
            m_listIssues = ListView.Create<UIIssueRow>(this, new Color32(81, 87, 89, 225), 0.8f, ListView.iROW_HEIGHT, width - 2*iMARGIN, height - m_title.height - 10);
            if (m_listIssues != null)
            {
                m_listIssues.AddColumn(ListViewRowComparer.Columns.COLUMN_TIME, Localization.Get("listIssuesColumn1"), Localization.Get("listIssuesColumn1Tooltip"), iCOLUMN_WIDTH_TIME, 20, UIHorizontalAlignment.Left, UIAlignAnchor.TopLeft);
                m_listIssues.AddColumn(ListViewRowComparer.Columns.COLUMN_TYPE, Localization.Get("listIssuesColumnType"), Localization.Get("listIssuesColumnTypeTooltip"), iCOLUMN_WIDTH_NORMAL, 20, UIHorizontalAlignment.Left, UIAlignAnchor.TopLeft);
                m_listIssues.AddColumn(ListViewRowComparer.Columns.COLUMN_SOURCE, Localization.Get("listIssuesColumn2"), Localization.Get("listIssuesColumn2Tooltip"), iCOLUMN_VEHICLE_WIDTH, 20, UIHorizontalAlignment.Left, UIAlignAnchor.TopLeft);
                m_listIssues.AddColumn(ListViewRowComparer.Columns.COLUMN_LOCATION, Localization.Get("listIssuesColumn3"), Localization.Get("listIssuesColumn3Tooltip"), iCOLUMN_VEHICLE_WIDTH, 20, UIHorizontalAlignment.Left, UIAlignAnchor.TopLeft);
                m_listIssues.AddColumn(ListViewRowComparer.Columns.COLUMN_DESCRIPTION, Localization.Get("listIssuesColumn4"), Localization.Get("listIssuesColumn4Tooltip"), iCOLUMN_DESCRIPTION_WIDTH, 20, UIHorizontalAlignment.Left, UIAlignAnchor.TopLeft);
            }

            isVisible = true;
            UpdatePanel();
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
            base.Show();
            UpdatePanel();
        }

        new public void Hide()
        {
            if (m_listIssues != null) 
            {
                m_listIssues.Clear();
            }
            base.Hide();
        }

        public void OnCloseClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            Hide();
        }

        public List<LineIssue> GetLineIssues()
        {
            if (LineIssueManager.Instance != null)
            {
                return LineIssueManager.Instance.GetVisibleLineIssues();
            }
            else
            {
                return new List<LineIssue>();
            }
        }

        public void UpdatePanel()
        {
            if (m_listIssues != null && m_listIssues.GetList() != null) {
                List<LineIssue> list = GetLineIssues();
                if (list != null)
                {
                    list.Sort();

                    m_listIssues.GetList().rowsData = new FastList<object>
                    {
                        m_buffer = list.ToArray(),
                        m_size = list.Count,
                    };
                }
            }
        }

        public override void OnDestroy()
        {
            if (m_listIssues != null)
            {
                Destroy(m_listIssues.gameObject);
            }
            if (m_title != null)
            {
                Destroy(m_title.gameObject);
            }
        }
    }
}
