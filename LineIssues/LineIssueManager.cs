using System.Collections.Generic;

namespace PublicTransportInfo
{
    public class LineIssueManager
    {
        public static LineIssueManager? Instance = null;
        public Dictionary<ushort, LineIssueDetector> m_LineDetectors;
        private List<LineIssue> m_lineIssues;
        private LineIssue.IssueLevel m_issueLevel;
        static readonly object s_IssueLock = new object();

        public LineIssueManager()
        {
            m_LineDetectors = new Dictionary<ushort, LineIssueDetector>();
            m_lineIssues = new List<LineIssue>();
            m_issueLevel = LineIssue.IssueLevel.ISSUE_NONE;
        }

        public static void Init()
        {
            if (Instance == null)
            {
                Instance = new LineIssueManager();
            }
        }

        public LineIssueDetector? GetLineIssueDetector(ushort usLineId)
        {
            if (m_LineDetectors != null && m_LineDetectors.ContainsKey(usLineId))
            {
                return m_LineDetectors[usLineId];
            } else
            {
                return null;
            }
        }

        public bool ContainsLine(ushort iLineId)
        {
            return m_LineDetectors.ContainsKey(iLineId);
        }

        private void RemoveDeletedLines(List<ushort> aTransportLines)
        {
            // Check if any lines have been deleted
            List<ushort> aVehicleProgressKeysToDelete = new List<ushort>();
            foreach (KeyValuePair<ushort, LineIssueDetector> oTransportLine in m_LineDetectors)
            {
                if (!aTransportLines.Contains(oTransportLine.Key))
                {
                    // Line has been deleted
                    aVehicleProgressKeysToDelete.Add(oTransportLine.Key);
                }
            }

            // Delete extra VehicleProgressLine objects
            foreach (ushort oKey in aVehicleProgressKeysToDelete)
            {
                m_LineDetectors.Remove(oKey);
            }
        }


        public void UpdateData()
        {
            ModSettings oSettings = ModSettings.GetSettings();
            if (oSettings.TrackVehicles())
            {
                List<ushort> aTransportLines = TransportManagerUtils.GetCompleteTransportLines();

                // Clear out deleted transport lines
                RemoveDeletedLines(aTransportLines);

                // Now add new LineIssueDetector objects
                foreach (ushort iLineId in aTransportLines)
                {
                    if (!m_LineDetectors.ContainsKey(iLineId))
                    {
                        LineIssueDetector oLineDetector = new LineIssueDetector(iLineId);
                        m_LineDetectors.Add(iLineId, oLineDetector);
                    }
                }

                UpdateLineIssues();
                UpdateWarningIcons();
            } 
            else
            {
                m_issueLevel = LineIssue.IssueLevel.ISSUE_NONE;

                if (m_LineDetectors != null)
                {
                    m_LineDetectors.Clear();
                }
                if (PublicTransportInstance.s_ToolbarButton != null)
                {
                    PublicTransportInstance.s_ToolbarButton.ShowWarningLevel(m_issueLevel);
                }
                if (UnifiedUITool.Instance != null)
                {
                    UnifiedUITool.Instance.ShowWarningLevel(m_issueLevel);
                }
            }
        }

        public ushort GetHighestLineWarningIcons(out LineIssue.IssueLevel eLevel)
        {
            ushort usLineId = 0;
            eLevel = LineIssue.IssueLevel.ISSUE_NONE;

            bool bWarnDespawned = ModSettings.GetSettings().WarnVehicleDespawed;
            bool bWarnVehicleMovesSlowly = ModSettings.GetSettings().WarnVehicleMovesSlowly;
            bool bWarnVehicleStopsMoving = ModSettings.GetSettings().WarnVehicleStopsMoving;
            bool bWarnBoredCount = ModSettings.GetSettings().WarnBoredCountExceedsThreshold;

            if (bWarnDespawned || bWarnVehicleMovesSlowly || bWarnVehicleStopsMoving || bWarnBoredCount)
            {
                UpdateLineIssues();
                foreach (LineIssue oIssue in GetVisibleLineIssues())
                {
                    if (oIssue.GetLevel() > eLevel)
                    {
                        eLevel = oIssue.GetLevel();
                        usLineId = oIssue.m_iLineId;
                    }
                }
            }

            return usLineId;
        }

        public LineIssue.IssueLevel GetLineWarningLevel(ushort usLineId, out string sTooltip)
        {
            LineIssue.IssueLevel eLevel = LineIssue.IssueLevel.ISSUE_NONE;
            sTooltip = "";

            UpdateLineIssues();
            foreach (LineIssue oIssue in GetVisibleLineIssues())
            {
                if (oIssue.m_iLineId == usLineId)
                {
                    LineIssue.IssueLevel eThisIssueLevel = oIssue.GetLevel();
                    if (eThisIssueLevel != LineIssue.IssueLevel.ISSUE_NONE)
                    {
                        sTooltip += oIssue.GetIssueTooltip() + "\r\n";
                        if (eThisIssueLevel > eLevel)
                        {
                            eLevel = eThisIssueLevel;
                        }
                    }
                }
            }

            return eLevel;
        }

        public void UpdateWarningIcons()
        {
            LineIssue.IssueLevel eLevel;
            ushort usLineId = GetHighestLineWarningIcons(out eLevel);

            // Update toolbar warning icons
            if (eLevel == LineIssue.IssueLevel.ISSUE_WARNING)
            {
                if (m_issueLevel != LineIssue.IssueLevel.ISSUE_WARNING)
                {
                    m_issueLevel = LineIssue.IssueLevel.ISSUE_WARNING; 
                    
                    if (PublicTransportInstance.s_mainPanel != null && ModSettings.GetSettings().PlaySoundForWarnings)
                    {
                        PublicTransportInstance.s_mainPanel.PlayWarningSound();
                    }
                }
            }
            else if (eLevel == LineIssue.IssueLevel.ISSUE_INFORMATION)
            {
                m_issueLevel = LineIssue.IssueLevel.ISSUE_INFORMATION; 
            }
            else
            {
                m_issueLevel = LineIssue.IssueLevel.ISSUE_NONE;                 
            }

            if (PublicTransportInstance.s_ToolbarButton != null)
            {
                PublicTransportInstance.s_ToolbarButton.ShowWarningLevel(m_issueLevel);
            }

            if (UnifiedUITool.Instance != null)
            {
                UnifiedUITool.Instance.ShowWarningLevel(m_issueLevel);
            }
        }

        public void DespawnVehicle(ushort usVehicle)
        {
            if (ModSettings.GetSettings().WarnVehicleDespawed)
            {
                Vehicle oVehicle = VehicleManager.instance.m_vehicles.m_buffer[usVehicle];
                if (oVehicle.m_transportLine != 0)
                {
                    TransportLine oLine = TransportManager.instance.m_lines.m_buffer[oVehicle.m_transportLine];
                    TransportInfo oInfo = oLine.Info;

                    // Add to despawned list
                    LineIssue oLineIssue = new LineIssueDespawned(oVehicle.m_transportLine, oInfo.m_transportType, usVehicle, oVehicle.GetLastFramePosition());
                    oLineIssue.m_iLineId = oVehicle.m_transportLine;
                    AddLineIssue(oLineIssue, false); // Don't update as we are in a separate thread
                }
            }
        }

        public List<LineIssue> GetVisibleLineIssues()
        {
            lock (s_IssueLock)
            {
                return new List<LineIssue>(m_lineIssues);
            }
        }

        public int FindIssueForVehicle(LineIssue issue)
        {
            if (issue.GetVehicleId() != 0)
            {
                lock (s_IssueLock)
                {
                    for (int i = 0; i < m_lineIssues.Count; ++i)
                    {
                        LineIssue lineIssue = m_lineIssues[i];
                        if (lineIssue.GetVehicleId() != 0 && lineIssue.GetVehicleId() == issue.GetVehicleId())
                        {
                            return i;
                        }
                    }
                }
            }
            
            return -1;
        }

        public void AddLineIssue(LineIssue oIssue, bool bUpdate)
        {
            bool bAdded = false;

            // We only want 1 issue for each vehicle
            lock (s_IssueLock)
            {
                int iIndex = FindIssueForVehicle(oIssue);
                if (iIndex >= 0)
                {
                    // Choose which one to keep
                    if (oIssue.GetIssueType() == LineIssue.IssueType.ISSUE_TYPE_DESPAWNED)
                    {
                        m_lineIssues[iIndex] = oIssue;
                        bAdded = true;
                    }
                }
                else if (!m_lineIssues.Contains(oIssue))
                {
                    // Check if vehicle has despawned
                    m_lineIssues.Add(oIssue);
                    bAdded = true;
                }
            }
            if (bAdded && bUpdate)
            {
                UpdateWarningIcons();
                UpdateLineIssuePanel();
            }
        }

        public void UpdateLineIssues()
        {
            // Update existing line issues
            lock (s_IssueLock)
            {
                foreach (LineIssue oIssue in m_lineIssues)
                {
                    if (oIssue != null)
                    {
                        oIssue.Update();
                    }
                }
            }
            

            RemoveResolvedIssues();

            // Get any new line issues
            foreach (KeyValuePair<ushort, LineIssueDetector> kvp in m_LineDetectors)
            {
                List<LineIssue> lineIssues = kvp.Value.GetLineIssues();
                foreach (LineIssue lineIssue in lineIssues)
                {
                    AddLineIssue(lineIssue, false);
                }
            }

            UpdateLineIssuePanel();
        }

        public void RemoveResolvedIssues()
        {
            // Remove issues that are now resolved
            List<LineIssue> oKeepIssues = new List<LineIssue>();
            lock (s_IssueLock)
            {
                foreach (LineIssue oIssue in m_lineIssues)
                {
                    // Delete resolved issues automatically
                    if (!oIssue.CanDelete())
                    {
                        oKeepIssues.Add(oIssue);
                    }
                }
                m_lineIssues = oKeepIssues;
            }
        }

        public bool HasVisibleLineIssues()
        {
            UpdateLineIssues();
            return GetVisibleLineIssues().Count > 0;
        }

        public void UpdateLineIssuePanel()
        {
            if (PublicTransportInstance.s_LineIssuePanel2 != null && PublicTransportInstance.s_LineIssuePanel2.isVisible)
            {
                PublicTransportInstance.UpdateLineIssuePanel();
            }
        }

        public string GetTooltip()
        {
            string sTooltip = "";
            List<LineIssue> oIssues = GetVisibleLineIssues();

            foreach (LineIssue oIssue in oIssues)
            {
                sTooltip += oIssue.GetIssueTooltip() + "\r\n";
            }

            return sTooltip;
        }
    }
}
