using ColossalFramework.UI;
using System.Collections.Generic;
using UnityEngine;

namespace SleepyCommon
{
    public class NewListViewHeader : UIPanel
    {
        protected List<NewListViewHeaderColumnBase> m_columns;
        protected NewListViewRowComparer.Columns m_eSortColumn = NewListViewRowComparer.Columns.COLUMN_TIME;
        public bool m_bSortDesc = false;
        private ListViewColumnClickEvent? m_eventOnListViewColumnClick = null;

        public delegate void ListViewColumnClickEvent(NewListViewRowComparer.Columns eColumn, bool bSortDescending);

        public NewListViewHeader()
        {
            m_columns = new List<NewListViewHeaderColumnBase>();
        }

        public static NewListViewHeader? Create(UIComponent parent, float iWidth, ListViewColumnClickEvent? m_eventOnListViewColumnClick)
        {
            NewListViewHeader header = parent.AddUIComponent<NewListViewHeader>();
            if (header != null)
            {
                header.Setup(iWidth, m_eventOnListViewColumnClick);
            }
            return header;
        }

        public virtual void Setup(float fWidth, ListViewColumnClickEvent? eventOnListViewColumnClick)
        {
            m_eventOnListViewColumnClick = eventOnListViewColumnClick;
            width = fWidth;
            height = 20;
            backgroundSprite = "ListItemHighlight";
            autoLayoutDirection = LayoutDirection.Horizontal;
            autoLayoutStart = LayoutStart.TopLeft;
            autoLayoutPadding = new RectOffset(2, 2, 2, 2);
            autoLayout = true;
        }

        public void AddColumn(NewListViewRowComparer.Columns eColumn, string sText, string sTooltip, float fTextScale, int iWidth, int iHeight, UIHorizontalAlignment oTextAlignment, UIAlignAnchor oAncor, NewListViewHeaderColumnBase.OnListViewColumnClick eventClickCallback)
        {
            if (m_columns != null)
            {
                m_columns.Add(new NewListViewHeaderColumnLabel(eColumn, this, sText, sTooltip, fTextScale, iWidth, iHeight, oTextAlignment, oAncor, eventClickCallback));
            }
        }

        public void OnListViewColumnClick(NewListViewHeaderColumnBase oColumn)
        {
            if (oColumn != null)
            {
                NewListViewRowComparer.Columns eColumn = oColumn.GetColumn();
                HandleSort(eColumn);
            }
        }

        public void HandleSort(NewListViewRowComparer.Columns eColumn)
        {
            if (m_eSortColumn == eColumn)
            {
                m_bSortDesc = !m_bSortDesc;
            }
            else
            {
                m_eSortColumn = eColumn;
            }

            // Update columns
            foreach (NewListViewHeaderColumnBase column in m_columns)
            {
                column.Sort(m_eSortColumn, m_bSortDesc);
            }

            // Notify parent
            if (m_eventOnListViewColumnClick != null)
            {
                m_eventOnListViewColumnClick(m_eSortColumn, m_bSortDesc);
            }
        }

        public NewListViewRowComparer.Columns GetSortColumn()
        {
            return m_eSortColumn;
        }

        public bool GetSortDirection()
        {
            return m_bSortDesc;
        }

        public void Destroy()
        {
            foreach (NewListViewHeaderColumnBase column in m_columns)
            {
                column.Destroy();
            }
        }
    }
}
