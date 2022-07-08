using ColossalFramework.UI;
using System;
using System.Collections.Generic;

namespace SleepyCommon
{
	public abstract class ListData : IComparable
	{
		public abstract string GetText(NewListViewRowComparer.Columns eColumn);
		public abstract int CompareTo(object second);
		public abstract void CreateColumns(NewListViewRow oRow, List<NewListViewRowColumn> m_columns);
		public virtual void OnClick(NewListViewRowColumn column)
		{

		}
		public virtual string OnTooltip(NewListViewRowColumn column)
		{
			return "";
		}
		public virtual void Update()
        {

        }
	}
}