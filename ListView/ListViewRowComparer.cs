using ColossalFramework.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PublicTransportInfo
{
	public class ListViewRowComparer : IComparer<UIComponent>
	{
		public enum Columns
        {
            COLUMN_NAME,
            COLUMN_STOPS,
            COLUMN_PASSENGERS,
            COLUMN_WAITING,
            COLUMN_BUSIEST,
			COLUMN_BORED,
        }
        
        public Columns m_eSortColumn;
		public bool m_bSortDesc;

		public ListViewRowComparer(Columns eSortColumn, bool bSortDesc)
        {
            m_eSortColumn = eSortColumn;
			m_bSortDesc = bSortDesc;
		}

		public int Compare(UIComponent o1, UIComponent o2)
		{
			ListViewRow oRow1 = o1 as ListViewRow;
			ListViewRow oRow2 = o2 as ListViewRow;

			int iResult = 1;
			if (oRow1 != null && oRow2 != null)
            {
                iResult = LineInfo.CompareTo(m_eSortColumn, oRow1.m_oLineInfo, oRow2.m_oLineInfo);
				if (m_bSortDesc)
                {
					iResult = -iResult;
				}
			}
			return iResult;
		}
	}
}
