using ColossalFramework.UI;
using PublicTransportInfo;
using System.Collections.Generic;
using UnityEngine;

namespace SleepyCommon
{
    public class ListViewHeader : UIPanel
    {
        protected List<ListViewHeaderColumnBase> m_columns;
        public delegate void ListViewColumnClickEvent(ListViewRowComparer.Columns eColumn, bool bSortDescending);

        public ListViewHeader()
        {
            m_columns = new List<ListViewHeaderColumnBase>();
        }

        public static ListViewHeader? Create(UIComponent parent, float iWidth, ListViewColumnClickEvent? m_eventOnListViewColumnClick)
        {
            ListViewHeader header = parent.AddUIComponent<ListViewHeader>();
            if (header != null)
            {
                header.Setup(iWidth, m_eventOnListViewColumnClick);
            }
            return header;
        }

        public virtual void Setup(float fWidth, ListViewColumnClickEvent? eventOnListViewColumnClick)
        {
            width = fWidth;
            height = MainPanel.iHEADER_HEIGHT;
            backgroundSprite = "ListItemHighlight";
            autoLayoutDirection = LayoutDirection.Horizontal;
            autoLayoutStart = LayoutStart.TopLeft;
            autoLayoutPadding = new RectOffset(2, 2, 2, 2);
            autoLayout = true;
        }

        public void AddColumn(ListViewRowComparer.Columns eColumn, string sText, string sTooltip, float fTextScale, int iWidth, int iHeight, UIHorizontalAlignment oTextAlignment, UIAlignAnchor oAncor, ListViewHeaderColumnBase.OnListViewColumnClick eventClickCallback)
        {
            if (m_columns != null)
            {
                m_columns.Add(new ListViewHeaderColumnLabel(eColumn, this, sText, sTooltip, fTextScale, iWidth, iHeight, oTextAlignment, oAncor, eventClickCallback));
            }
        }

        public void AddIconColumn(ListViewRowComparer.Columns eColumn, string sIconName, string sTooltip, int iWidth, int iHeight, UIHorizontalAlignment oTextAlignment, UIAlignAnchor oAncor, ListViewHeaderColumnBase.OnListViewColumnClick eventClickCallback)
        {
            if (m_columns != null)
            {
                m_columns.Add(new ListViewHeaderColumnIcon(eColumn, this, sIconName, sTooltip, iWidth, iHeight, oTextAlignment, oAncor, eventClickCallback));
            }
        }

        public void Destroy()
        {
            foreach (ListViewHeaderColumnBase column in m_columns)
            {
                column.Destroy();
            }
        }
    }
}
