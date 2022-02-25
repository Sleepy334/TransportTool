using ColossalFramework;
using ColossalFramework.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PublicTransportInfo
{
    public class ListView : UIPanel
    {
        public const int iSCROLL_BAR_WIDTH = 20;

        public ListViewMainPanel m_listPanel;
        public UIPanel m_scrollbarPanel;
        public UIScrollbar m_scrollbar;
        public UIPanel m_headingPanel;

        public ListView() : base() {
            m_scrollbar = null;
            m_listPanel = null;
            m_scrollbarPanel = null;
        }

        public static ListView Create(UIComponent oParent)
        {
            try
            {
                Debug.Log("ListView::Create");
                ListView listView = oParent.AddUIComponent<ListView>();
                if (listView != null)
                {
                    listView.SetupListView();

                    listView.backgroundSprite = "GenericPanelWhite";
                    listView.autoLayoutDirection = LayoutDirection.Horizontal;
                    listView.autoLayoutStart = LayoutStart.TopLeft;
                    listView.autoLayout = true;
                }
                return listView;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                if (ex.InnerException != null)
                {
                    Debug.LogException(ex.InnerException);
                }
            }

            return null;
        }

        public void SetupListView()
        {
            m_listPanel = this.AddUIComponent<ListViewMainPanel>();
            if (m_listPanel != null)
            {
                m_listPanel.Start();
            } else
            { 
                Debug.Log("m_listPanel is null");
                return;
            }

            m_scrollbarPanel = this.AddUIComponent<UIPanel>();
            if (m_scrollbarPanel != null)
            {
                m_scrollbarPanel.width = iSCROLL_BAR_WIDTH;
                m_scrollbarPanel.height = m_listPanel.height;
                m_scrollbarPanel.relativePosition = new Vector3(width - iSCROLL_BAR_WIDTH, 0.0f);
                m_scrollbar = SetUpScrollbar();
            }
            else
            {
                Debug.Log("m_scrollbarPanel is null");
                return;
            }

            m_listPanel.verticalScrollbar = m_scrollbar;
            m_listPanel.eventMouseWheel += (MouseEventHandler)((component, param) => this.m_listPanel.scrollPosition += new Vector2(0.0f, Mathf.Sign(param.wheelDelta) * -1f * m_scrollbar.incrementAmount));
        }

        public UIScrollbar SetUpScrollbar()
        {
            m_scrollbar = m_scrollbarPanel.AddUIComponent<UIScrollbar>();
            m_scrollbar.name = "Scrollbar";
            m_scrollbar.width = iSCROLL_BAR_WIDTH;
            m_scrollbar.height = 495; // m_scrollbarPanel.height - 5; // Seem to have to hard code this, not sure why yet...
            m_scrollbar.orientation = UIOrientation.Vertical;
            m_scrollbar.pivot = UIPivotPoint.BottomLeft;
            m_scrollbar.relativePosition = Vector2.zero;
            m_scrollbar.minValue = 0;
            m_scrollbar.value = 0;
            m_scrollbar.incrementAmount = 50;

            UISlicedSprite tracSprite = m_scrollbar.AddUIComponent<UISlicedSprite>();
            tracSprite.relativePosition = Vector2.zero;
            tracSprite.autoSize = true;
            tracSprite.size = tracSprite.parent.size;
            tracSprite.fillDirection = UIFillDirection.Vertical;
            tracSprite.spriteName = "ScrollbarTrack";
            tracSprite.name = "Track";
            m_scrollbar.trackObject = tracSprite;
            m_scrollbar.trackObject.height = m_scrollbar.height;

            UISlicedSprite thumbSprite = tracSprite.AddUIComponent<UISlicedSprite>();
            thumbSprite.relativePosition = Vector2.zero;
            thumbSprite.fillDirection = UIFillDirection.Vertical;
            thumbSprite.autoSize = true;
            thumbSprite.width = thumbSprite.parent.width - 8;
            thumbSprite.spriteName = "ScrollbarThumb";
            thumbSprite.name = "Thumb";

            m_scrollbar.thumbObject = thumbSprite;
            m_scrollbar.isVisible = true;
            m_scrollbar.isEnabled = true;
            return m_scrollbar;
        }

        public ListViewRow AddItem(LineInfo oLineInfo)
        {
            ListViewRow poRow = ListViewRow.Create(this, oLineInfo);
            return poRow;
        }

        public void Clear()
        {
            if (m_listPanel == null)
            {
                return;
            }
            m_listPanel.Clear();
        }

        protected override void OnSizeChanged()
        {
            Debug.Log("ListView::OnSizeChanged - Width" + width + " height: " + height); 
            base.OnSizeChanged();

            if (m_listPanel != null)
            {
                m_listPanel.width = width - iSCROLL_BAR_WIDTH;
                m_listPanel.height = height;
            }
            
            if (m_scrollbarPanel != null)
            {
                m_scrollbarPanel.height = m_scrollbarPanel.parent.height;
            }

            if (m_scrollbar != null)
            {
                m_scrollbar.height = m_scrollbar.parent.height;
            }
        }

        public void UpdateLineData(ListViewRowComparer.Columns eSortColumn, bool bDesc)
        {
            foreach (ListViewRow oRow in m_listPanel.components)
            {
                oRow.UpdateLineData();
            }

            Sort(eSortColumn, bDesc);
        }

        public void Sort(ListViewRowComparer.Columns eSortColumn, bool bDesc)
        {
            if (m_listPanel != null)
            {
                m_listPanel.Sort(eSortColumn, bDesc);
                m_listPanel.Invalidate();
            }
        }

        public override void OnDestroy()
        {
            if (m_listPanel != null)
            {
                UnityEngine.Object.Destroy(m_listPanel.gameObject);
            }
            if (m_scrollbarPanel != null)
            {
                UnityEngine.Object.Destroy(m_scrollbarPanel.gameObject);
            }
            if (m_scrollbar != null)
            {
                UnityEngine.Object.Destroy(m_scrollbar.gameObject);
            }
            base.OnDestroy();
        }
    }
}
