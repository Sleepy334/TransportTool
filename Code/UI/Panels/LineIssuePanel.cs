﻿using ColossalFramework.UI;
using PublicTransportInfo.UI.ListView;
using PublicTransportInfo.UI.ListViewRows;
using SleepyCommon;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PublicTransportInfo
{
    public class LineIssuePanel : UIMainPanel<LineIssuePanel>
    {
        public const float fTEXT_SCALE = 0.8f;
        public const int Margin = 10;

        const int iMARGIN = 8;
        public const int iCOLUMN_WIDTH_TIME = 60;
        public const int iCOLUMN_WIDTH_NORMAL = 80;
        public const int iCOLUMN_VEHICLE_WIDTH = 180;
        public const int iCOLUMN_DESCRIPTION_WIDTH = 300;

        private UITitleBar? m_title = null;
        private UIListView? m_listIssues = null;
        private Coroutine? m_coroutine = null;

        public LineIssuePanel() : base()
        {
            m_coroutine = StartCoroutine(UpdatePanelCoroutine(4));
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
            backgroundSprite = "SubcategoriesPanel";
            canFocus = true;
            isInteractive = true;
            isVisible = false;
            playAudioEvents = true;
            clipChildren = true;
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
            m_title.Setup(Localization.Get("titleLineIssuesPanel"), PublicTransportInstance.LoadResources(), OnCloseClick);

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

            // Issue list
            m_listIssues = UIListView.Create<UIIssueRow>(mainPanel, new Color32(81, 87, 89, 225), 0.8f, UIListView.iROW_HEIGHT, width - 2*iMARGIN, height - m_title.height - 10);
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

        protected override void OnVisibilityChanged()
        {
            if (isVisible)
            {
                UpdatePanel();
            }
            else
            {
                m_listIssues?.Clear();
            }

            base.OnVisibilityChanged();
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
            CDebug.Log("Show");
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
                List<LineIssue> issues = LineIssueManager.Instance.GetVisibleLineIssues();
                issues.Sort();
                
                // We separate the resolved items from the current ones
                List<LineIssue> result = new List<LineIssue>();
                
                if (issues.Count > 0)
                {
                    // Add current issues
                    foreach (LineIssue issue in issues)
                    {
                        if (!issue.IsResolved())
                        {
                            result.Add(issue);
                        }
                    }

                    // Add a separator
                    if (result.Count > 0)
                    {
                        result.Add(new LineIssueSeparator());
                    }

                    // Add resolved issues
                    foreach (LineIssue issue in issues)
                    {
                        if (issue.IsResolved())
                        {
                            result.Add(issue);
                        }
                    }
                }

                return result;
            }
            else
            {
                return new List<LineIssue>();
            }
        }

        IEnumerator UpdatePanelCoroutine(int seconds)
        {
            while (true)
            {
                yield return new WaitForSeconds(seconds);

                if (!SimulationManager.instance.SimulationPaused)
                {
                    UpdatePanel();
                }
            }
        }

        public void UpdatePanel()
        {
            if (isVisible && m_listIssues != null && m_listIssues.GetList() != null) {
                List<LineIssue> list = GetLineIssues();
                if (list != null)
                {
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
