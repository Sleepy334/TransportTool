using System.Collections.Generic;

namespace PublicTransportInfo
{
    public class LineIssueManager
    {
        public Dictionary<ushort, VehicleProgressLine> m_LineVehicleProgress;
        private List<LineIssue> m_lineIssues;
        private LineIssue.IssueLevel m_issueLevel;

        public LineIssueManager()
        {
            m_LineVehicleProgress = new Dictionary<ushort, VehicleProgressLine>();
            m_lineIssues = new List<LineIssue>();
            m_issueLevel = LineIssue.IssueLevel.ISSUE_NONE;
        }

        public VehicleProgressLine? GetVehicleProgressForLine(ushort usLineId)
        {
            if (m_LineVehicleProgress != null && m_LineVehicleProgress.ContainsKey(usLineId))
            {
                return m_LineVehicleProgress[usLineId];
            } else
            {
                return null;
            }
            
        }

        public void AddVehicleProgressForLine(ushort usLineId, VehicleProgressLine oProgress)
        {
            if (m_LineVehicleProgress.ContainsKey(usLineId))
            {
                m_LineVehicleProgress[usLineId] = oProgress;
            }
        }

        public bool ContainsLine(ushort iLineId)
        {
            return m_LineVehicleProgress.ContainsKey(iLineId);
        }

        private void RemoveDeletedLines(List<ushort> aTransportLines)
        {
            // Check if any lines have been deleted
            List<ushort> aVehicleProgressKeysToDelete = new List<ushort>();
            foreach (KeyValuePair<ushort, VehicleProgressLine> oTransportLine in m_LineVehicleProgress)
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
                m_LineVehicleProgress.Remove(oKey);
            }
        }


        public void UpdateVehicleProgress()
        {
            ModSettings oSettings = PublicTransportInstance.GetSettings();
            if (oSettings.TrackVehicles())
            {
                List<ushort> aTransportLines = TransportManagerUtils.GetCompleteTransportLines();

                // Clear out deleted transport lines
                RemoveDeletedLines(aTransportLines);
                
                // Now update remaining VehicleProgressLine objects
                foreach (ushort iLineId in aTransportLines)
                {
                    if (m_LineVehicleProgress.ContainsKey(iLineId))
                    {
                        VehicleProgressLine oLineProgress = m_LineVehicleProgress[iLineId];
                        oLineProgress.LoadVehicleProgress();
                        m_LineVehicleProgress[iLineId] = oLineProgress;
                    }
                    else
                    {
                        VehicleProgressLine oLineProgress = new VehicleProgressLine(iLineId);
                        oLineProgress.LoadVehicleProgress();
                        m_LineVehicleProgress.Add(iLineId, oLineProgress);
                    }
                }

                UpdateWarningIcons();
            } 
            else
            {
                m_issueLevel = LineIssue.IssueLevel.ISSUE_NONE;

                if (m_LineVehicleProgress != null)
                {
                    m_LineVehicleProgress.Clear();
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
            else if (eLevel == LineIssue.IssueLevel.ISSUE_INFORMATION)
            {
                if (m_issueLevel != LineIssue.IssueLevel.ISSUE_INFORMATION)
                {
                    m_issueLevel = LineIssue.IssueLevel.ISSUE_INFORMATION; 
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
            else
            {
                m_issueLevel = LineIssue.IssueLevel.ISSUE_NONE; 
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

        public void DespawnVehicle(ushort usVehicle)
        {
            Vehicle oVehicle = VehicleManager.instance.m_vehicles.m_buffer[usVehicle];
            if (oVehicle.m_transportLine != 0)
            {
                if (m_LineVehicleProgress != null && m_LineVehicleProgress.ContainsKey(oVehicle.m_transportLine))
                { 
                    VehicleProgressLine oLine = m_LineVehicleProgress[oVehicle.m_transportLine];
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

        public void AddLineIssue(LineIssue oIssue)
        {
            if (!m_lineIssues.Contains(oIssue))
            {
                int iCount = m_lineIssues.Count;
                m_lineIssues.Add(oIssue);
                UpdateWarningIcons();
                UpdateLineIssuePanel();
            }
        }

        public void UpdateLineIssues()
        {
            // Get any new line issues
            foreach (KeyValuePair<ushort, VehicleProgressLine> kvp in m_LineVehicleProgress)
            {
                List<LineIssue> lineIssues = kvp.Value.GetLineIssues();
                foreach (LineIssue lineIssue in lineIssues)
                {
                    AddLineIssue(lineIssue);
                }
            }

            // Update existing line issues
            foreach (LineIssue oIssue in m_lineIssues)
            {
                if (oIssue != null && !oIssue.IsHidden())
                {
                    oIssue.Update();
                }
            }

            UpdateLineIssuePanel();
        }

        public bool HasVisibleLineIssues()
        {
            UpdateLineIssues();
            return GetVisibleLineIssues().Count > 0;
        }

        public void RemoveLineIssue(LineIssue oIssue)
        {
            int iIndex = m_lineIssues.IndexOf(oIssue);
            if (iIndex != -1)
            {
                m_lineIssues.RemoveAt(iIndex);
            } else
            {
                Debug.Log("Issue not found: " + oIssue.ToString());
            }

            UpdateLineIssuePanel();
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

                m_lineIssues.Clear();
                m_lineIssues.AddRange(oKeepIssues);
                UpdateLineIssuePanel();
                UpdateWarningIcons();
            }
            
        }
    }
}
