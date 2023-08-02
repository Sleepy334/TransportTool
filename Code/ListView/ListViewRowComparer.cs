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
			COLUMN_COLOR,
			COLUMN_NAME,
            COLUMN_STOPS,
			COLUMN_VEHICLES,
            COLUMN_PASSENGERS,
			COLUMN_VEHICLE_USAGE,
			COLUMN_WAITING,
            COLUMN_BUSIEST,
			COLUMN_BORED,
			COLUMN_TIME,
			COLUMN_TYPE,
			COLUMN_SOURCE,
			COLUMN_LOCATION,
			COLUMN_DESCRIPTION,
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
			/*
			ListViewRow? oRow1 = o1 as ListViewRow;
			ListViewRow? oRow2 = o2 as ListViewRow;

			int iResult = 1;
			if (oRow1 != null && oRow2 != null)
            {
                iResult = LineInfoBase.CompareTo(m_eSortColumn, oRow1.m_oLineInfo, oRow2.m_oLineInfo);
				if (m_bSortDesc)
                {
					iResult = -iResult;
				}
			}
			return iResult;
			*/
			return 1;
		}
	}
}
