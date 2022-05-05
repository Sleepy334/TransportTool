using System.Collections.Generic;

namespace PublicTransportInfo
{
    public class LineIssueManager
    {
        public Dictionary<ushort, LineIssueDetector> m_LineDetectors;
        private List<LineIssue> m_lineIssues;
        private LineIssue.IssueLevel m_issueLevel;

        public LineIssueManager()
        {
            m_LineDetectors = new Dictionary<ushort, LineIssueDetector>();
            m_lineIssues = new List<LineIssue>();
            m_issueLevel = LineIssue.IssueLevel.ISSUE_NONE;
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


        public void UpdateVehicleDetectors()
        {
            ModSettings oSettings = PublicTransportInstance.GetSettings();
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

            bool bWarnDespawned = PublicTransportInstance.GetSettings().WarnVehicleDespawed;
            bool bWarnVehicleMovesSlowly = PublicTransportInstance.GetSettings().WarnVehicleMovesSlowly;
            bool bWarnVehicleStopsMoving = PublicTransportInstance.GetSettings().WarnVehicleStopsMoving;

            if (bWarnDespawned || bWarnVehicleMovesSlowly || bWarnVehicleStopsMoving)
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
                    
                    if (PublicTransportInstance.s_mainPanel != null && PublicTransportInstance.GetSettings().PlaySoundForWarnings)
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
            Vehicle oVehicle = VehicleManager.instance.m_vehicles.m_buffer[usVehicle];
            if (oVehicle.m_transportLine != 0)
            {
                if (m_LineDetectors != null && m_LineDetectors.ContainsKey(oVehicle.m_transportLine))
                {
                    LineIssueDetector oLine = m_LineDetectors[oVehicle.m_transportLine];
                    if (oLine != null)
                    {
                        oLine.DespawnVehicle(usVehicle, oVehicle);
                        UpdateWarningIcons();
                    }
                } 
                else
                {
                    Debug.Log("Unable to locate VehicleProgressLine: " + oVehicle.m_transportLine + "\r\n");
                }
            }
        }

        public List<LineIssue> GetVisibleLineIssues()
        {
            List<LineIssue> list = new List<LineIssue>();

            foreach (LineIssue lineIssue in m_lineIssues)
            {
                if (lineIssue != null && !lineIssue.IsHidden())
                {
                    list.Add(lineIssue);
                }
            }

            return list;
        }

        public int FindIssueForVehicle(LineIssue issue)
        {
            if (issue.GetVehcileId() != 0)
            {
                for (int i = 0; i < m_lineIssues.Count; ++i)
                {
                    LineIssue lineIssue = m_lineIssues[i];
                    if (lineIssue.GetVehcileId() != 0 && lineIssue.GetVehcileId() == issue.GetVehcileId())
                    {
                        return i;
                    }
                }
            }
            
            return -1;
        }

        public void AddLineIssue(LineIssue oIssue, bool bUpdate)
        {
            bool bAdded = false;

            // We only want 1 issue for each vehicle
            int iIndex = FindIssueForVehicle(oIssue);
            if (iIndex >= 0)
            {
                // Choose which one to keep
                if (oIssue.GetIssueType() == LineIssue.IssueType.ISSUE_TYPE_DESPAWNED)
                {
                    int iActiveIssueId = 0;
                    if (PublicTransportInstance.s_LineIssuePanel != null && PublicTransportInstance.s_LineIssuePanel.isVisible)
                    {
                        iActiveIssueId = PublicTransportInstance.s_LineIssuePanel.GetActiveIssueId();
                    }
                    bool bUpdateActiveIssue = m_lineIssues[iIndex].GetIssueId() == iActiveIssueId;
                    m_lineIssues[iIndex] = oIssue;
                    bAdded = true;

                    if (bUpdateActiveIssue && PublicTransportInstance.s_LineIssuePanel != null && PublicTransportInstance.s_LineIssuePanel.isVisible)
                    {
                        PublicTransportInstance.s_LineIssuePanel.UpdateActiveIssue(oIssue);
                    }
                }
            }
            else if (!m_lineIssues.Contains(oIssue))
            {
                // Check if vehicle has despawned
                m_lineIssues.Add(oIssue);
                bAdded = true;
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
            foreach (LineIssue oIssue in m_lineIssues)
            {
                if (oIssue != null && !oIssue.IsHidden())
                {
                    oIssue.Update();
                }
            }

            RemoveHiddenResolvedIssues();

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

        public void RemoveHiddenResolvedIssues()
        {
            double dTimeDelayBeforeDeleting = 5.0; // seconds
            
            int iActiveIssueId = 0;
            if (PublicTransportInstance.s_LineIssuePanel != null && PublicTransportInstance.s_LineIssuePanel.isVisible)
            {
                iActiveIssueId = PublicTransportInstance.s_LineIssuePanel.GetActiveIssueId();
            }

            // Remove hidden issues that are now resolved
            bool bDeleteResolved = PublicTransportInstance.GetSettings().DeleteResolvedIssuesAutomatically;
            List<LineIssue> oKeepIssues = new List<LineIssue>();
            foreach (LineIssue oIssue in m_lineIssues)
            {
                // Delete hidden issues if able to
                bool bRemoveIssue = oIssue.IsHidden() && oIssue.CanDelete();

                // Delete resolved issues automatically if desired
                if (bDeleteResolved)
                {
                    bRemoveIssue = bRemoveIssue || (oIssue.GetIssueId() != iActiveIssueId && oIssue.GetLevel() == LineIssue.IssueLevel.ISSUE_NONE && oIssue.GetLineIssueCreationTimeSeconds() > dTimeDelayBeforeDeleting);
                }

                if (!bRemoveIssue)
                {
                    oKeepIssues.Add(oIssue);
                }
            }

            m_lineIssues = oKeepIssues;

            /*
            string sMessage = "";
            foreach (LineIssue oIssue in m_lineIssues)
            {
                sMessage += "\r\n" + oIssue.GetIssueId() + ": " + oIssue.GetIssueLocation() + " " + oIssue.GetIssueDescription();
            }
            Debug.Log(sMessage);
            */
        }

        public bool HasVisibleLineIssues()
        {
            UpdateLineIssues();
            return GetVisibleLineIssues().Count > 0;
        }

        public void UpdateLineIssuePanel()
        {
            if (PublicTransportInstance.s_LineIssuePanel != null && PublicTransportInstance.s_LineIssuePanel.isVisible)
            {
                PublicTransportInstance.s_LineIssuePanel.UpdatePanel();
            }
        }

        public void ClearIssuesWhenClosingPanel()
        {
            if (PublicTransportInstance.GetSettings().DeleteLineIssuesOnClosing)
            {
                List<LineIssue> oKeepIssues = new List<LineIssue>();

                foreach (LineIssue oIssue in m_lineIssues)
                {
                    if (!oIssue.CanDelete())
                    {
                        oIssue.SetHidden(true);
                        oKeepIssues.Add(oIssue);
                    }
                }

                m_lineIssues = oKeepIssues;
                UpdateLineIssuePanel();
                UpdateWarningIcons();
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
