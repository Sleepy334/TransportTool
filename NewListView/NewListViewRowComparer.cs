using ColossalFramework.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SleepyCommon
{
	public class NewListViewRowComparer : IComparer<UIComponent>
	{
		public enum Columns
        {
			COLUMN_TIME,
			COLUMN_TYPE,
			COLUMN_SOURCE,
			COLUMN_LOCATION,
			COLUMN_DESCRIPTION,
		}
        
        public Columns m_eSortColumn;
		public bool m_bSortDesc;

		public NewListViewRowComparer(Columns eSortColumn, bool bSortDesc)
        {
            m_eSortColumn = eSortColumn;
			m_bSortDesc = bSortDesc;
		}

		public int Compare(UIComponent o1, UIComponent o2)
		{
			NewListViewRow? oRow1 = o1 as NewListViewRow;
			NewListViewRow? oRow2 = o2 as NewListViewRow;

			int iResult = 1;
			if (oRow1 != null && oRow2 != null)
            {
                iResult = oRow1.CompareTo(oRow2);
				if (m_bSortDesc)
                {
					iResult = -iResult;
				}
			}
			return iResult;
		}
	}
}
