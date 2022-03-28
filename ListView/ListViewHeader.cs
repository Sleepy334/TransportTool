using ColossalFramework.UI;
using System.Collections.Generic;
using UnityEngine;

namespace PublicTransportInfo
{
    public class ListViewHeader : UIPanel
    {
        private UILabel? m_lblColor = null;
        private List<ListViewHeaderColumnBase> m_columns;
        private ListViewRowComparer.Columns m_eSortColumn = ListViewRowComparer.Columns.COLUMN_NAME;
        public bool m_bSortDesc = false;
        private ListViewColumnClickEvent? m_eventOnListViewColumnClick = null;

        public delegate void ListViewColumnClickEvent(ListViewRowComparer.Columns eColumn, bool bSortDescending);

        public ListViewHeader()
        {
            m_columns = new List<ListViewHeaderColumnBase>();
        }

        public void Setup(float fWidth, ListViewColumnClickEvent eventOnListViewColumnClick)
        {
            m_eventOnListViewColumnClick = eventOnListViewColumnClick;
            width = fWidth;
            height = PublicTransportInfoPanel.iHEADER_HEIGHT;
            backgroundSprite = "ListItemHighlight";
            autoLayoutDirection = LayoutDirection.Horizontal;
            autoLayoutStart = LayoutStart.TopLeft;
            autoLayoutPadding = new RectOffset(2, 2, 2, 2);
            autoLayout = true;

            // Add a spacer for the color label
            m_lblColor = AddUIComponent<UILabel>();
            m_lblColor.text = "";
            m_lblColor.autoSize = false;
            m_lblColor.height = PublicTransportInfoPanel.iHEADER_HEIGHT;
            m_lblColor.width = PublicTransportInfoPanel.iCOLUMN_WIDTH_COLOR + 10;

            m_columns.Add(new ListViewHeaderColumnLabel(ListViewRowComparer.Columns.COLUMN_NAME, this, "Line Name", "Name of line.", PublicTransportInfoPanel.iCOLUMN_WIDTH_NAME, PublicTransportInfoPanel.iHEADER_HEIGHT, UIHorizontalAlignment.Left, UIAlignAnchor.TopLeft, OnListViewColumnClick));
            m_columns.Add(new ListViewHeaderColumnLabel(ListViewRowComparer.Columns.COLUMN_STOPS, this, "Stops", "Total number of stops on this line.", PublicTransportInfoPanel.iCOLUMN_WIDTH_STOPS, PublicTransportInfoPanel.iHEADER_HEIGHT, UIHorizontalAlignment.Center, UIAlignAnchor.TopRight, OnListViewColumnClick));
            m_columns.Add(new ListViewHeaderColumnIcon(ListViewRowComparer.Columns.COLUMN_VEHICLES, this, "InfoIconPublicTransport", "Total number of vehicles on this line", PublicTransportInfoPanel.iCOLUMN_WIDTH_VEHICLES, PublicTransportInfoPanel.iHEADER_HEIGHT, UIHorizontalAlignment.Center, UIAlignAnchor.TopRight, OnListViewColumnClick));
            m_columns.Add(new ListViewHeaderColumnLabel(ListViewRowComparer.Columns.COLUMN_PASSENGERS, this, "Passengers", "Current Passengers / Total Capacity", PublicTransportInfoPanel.iCOLUMN_WIDTH_PASSENGER, PublicTransportInfoPanel.iHEADER_HEIGHT, UIHorizontalAlignment.Center, UIAlignAnchor.TopRight, OnListViewColumnClick));
            m_columns.Add(new ListViewHeaderColumnLabel(ListViewRowComparer.Columns.COLUMN_WAITING, this, "Waiting", "Total number of people waiting at all stops", PublicTransportInfoPanel.iCOLUMN_WIDTH_WAITING, PublicTransportInfoPanel.iHEADER_HEIGHT, UIHorizontalAlignment.Center, UIAlignAnchor.TopRight, OnListViewColumnClick));
            m_columns.Add(new ListViewHeaderColumnLabel(ListViewRowComparer.Columns.COLUMN_BUSIEST, this, "Busiest", "Number of people waiting at busiest stop", PublicTransportInfoPanel.iCOLUMN_WIDTH_BUSIEST, PublicTransportInfoPanel.iHEADER_HEIGHT, UIHorizontalAlignment.Center, UIAlignAnchor.TopRight, OnListViewColumnClick));
            m_columns.Add(new ListViewHeaderColumnLabel(ListViewRowComparer.Columns.COLUMN_BORED, this, "Bored", "Number of people who are waiting too long", PublicTransportInfoPanel.iCOLUMN_WIDTH_BORED, PublicTransportInfoPanel.iHEADER_HEIGHT, UIHorizontalAlignment.Center, UIAlignAnchor.TopRight, OnListViewColumnClick));

            HandleSort(ListViewRowComparer.Columns.COLUMN_NAME);
        }

        public void OnListViewColumnClick(ListViewHeaderColumnBase oColumn)
        {
            if (oColumn != null)
            {
                ListViewRowComparer.Columns eColumn = oColumn.GetColumn();
                HandleSort(eColumn);
            }
        }

        public void HandleSort(ListViewRowComparer.Columns eColumn)
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
            foreach (ListViewHeaderColumnBase column in m_columns)
            {
                column.Sort(m_eSortColumn, m_bSortDesc);
            }

            // Notify parent
            if (m_eventOnListViewColumnClick != null)
            {
                m_eventOnListViewColumnClick(m_eSortColumn, m_bSortDesc);
            }
        }

        public ListViewRowComparer.Columns GetSortColumn()
        {
            return m_eSortColumn;
        }

        public bool GetSortDirection()
        {
            return m_bSortDesc;
        }

        public void Destroy()
        {
            foreach (ListViewHeaderColumnBase column in m_columns)
            {
                column.Destroy();
            }
            if (m_lblColor != null)
            {
                Destroy(m_lblColor.gameObject);
            }
        }
    }
}
