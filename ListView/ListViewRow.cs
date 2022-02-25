using ColossalFramework;
using ColossalFramework.UI;
using System;
using UnityEngine;

namespace PublicTransportInfo
{
    public class ListViewRow : UIPanel
    {
        public const int iROW_HEIGHT = 30;
        public const int iROW_MARGIN = 10;
        public LineInfo m_oLineInfo;
        private UIPanel m_lblColor;
        private UILabel m_lblName;
        private UILabel m_lblStops;
        private UILabel m_lblPassengers;
        private UILabel m_lblWaiting;
        private UILabel m_lblBusiest;
        private static ushort m_HighlightedLineId = 0;

        public ListViewRow() : base()
        {
            m_oLineInfo = null;
            m_lblColor = null;
            m_lblName = null;
            m_lblName = null;
            m_lblPassengers = null;
            m_lblWaiting = null;
            m_lblBusiest = null;
        }

        public static ListViewRow Create(ListView oParent, LineInfo oLineInfo)
        {
            if (oParent != null && oParent.m_listPanel != null)
            {
                ListViewRow oRow = oParent.m_listPanel.AddUIComponent<ListViewRow>();
                oRow.m_oLineInfo = oLineInfo;

                //oRow.color = oLineInfo.m_color;
                oRow.backgroundSprite = "GenericPanel";
                oRow.position = new Vector3(iROW_MARGIN, -iROW_MARGIN);
                oRow.width = oParent.width - (iROW_MARGIN * 2);
                oRow.height = iROW_HEIGHT;
                oRow.autoLayoutDirection = LayoutDirection.Horizontal;
                oRow.autoLayoutStart = LayoutStart.TopLeft;
                oRow.autoLayoutPadding = new RectOffset(2, 2, 2, 2);
                oRow.autoLayout = true;
                oRow.eventClick += new MouseEventHandler(oRow.OnItemClicked);
                oRow.eventMouseEnter += new MouseEventHandler(oRow.OnItemEnter);
                oRow.eventMouseLeave += new MouseEventHandler(oRow.OnItemLeave);

                UIPanel oIconContainer = oRow.AddUIComponent<UIPanel>();
                oIconContainer.name = "oIconContainer";
                oIconContainer.width = iROW_HEIGHT - 4;
                oIconContainer.height = iROW_HEIGHT - 4;

                oRow.m_lblColor = oIconContainer.AddUIComponent<UIPanel>();
                oRow.m_lblColor.name = "m_lblColor";
                oRow.m_lblColor.backgroundSprite = "InfoviewPanel";
                oRow.m_lblColor.color = oLineInfo.m_color;
                oRow.m_lblColor.height = PublicTransportInfoPanel.iCOLUMN_WIDTH_COLOR;
                oRow.m_lblColor.width = PublicTransportInfoPanel.iCOLUMN_WIDTH_COLOR;
                oRow.m_lblColor.CenterToParent();

                oRow.m_lblName = oRow.AddUIComponent<UILabel>();
                oRow.m_lblName.name = "m_lblName";
                oRow.m_lblName.text = oLineInfo.m_sName;
                oRow.m_lblName.verticalAlignment = UIVerticalAlignment.Middle; 
                oRow.m_lblName.autoSize = false;
                oRow.m_lblName.height = iROW_HEIGHT;
                oRow.m_lblName.width = PublicTransportInfoPanel.iCOLUMN_WIDTH_NAME;

                oRow.m_lblStops = oRow.AddUIComponent<UILabel>();
                oRow.m_lblStops.name = "m_lblStops";
                oRow.m_lblStops.text = oLineInfo.m_iStops.ToString();
                oRow.m_lblStops.textAlignment = UIHorizontalAlignment.Center;
                oRow.m_lblStops.verticalAlignment = UIVerticalAlignment.Middle;
                oRow.m_lblStops.autoSize = false;
                oRow.m_lblStops.height = iROW_HEIGHT;
                oRow.m_lblStops.width = PublicTransportInfoPanel.iCOLUMN_WIDTH_STOPS;
                oRow.m_lblStops.AlignTo(oRow, UIAlignAnchor.TopRight);

                oRow.m_lblPassengers = oRow.AddUIComponent<UILabel>();
                oRow.m_lblPassengers.name = "m_lblPassengers";
                //m_lblPassengers.backgroundSprite = "MenuPanel2";
                oRow.m_lblPassengers.text = oLineInfo.m_iPassengers.ToString() + " / " + oLineInfo.m_iCapacity;
                oRow.m_lblPassengers.textAlignment = UIHorizontalAlignment.Center;
                oRow.m_lblPassengers.verticalAlignment = UIVerticalAlignment.Middle;
                oRow.m_lblPassengers.autoSize = false;
                oRow.m_lblPassengers.height = iROW_HEIGHT;
                oRow.m_lblPassengers.width = PublicTransportInfoPanel.iCOLUMN_WIDTH_PASSENGER;
                oRow.m_lblPassengers.AlignTo(oRow, UIAlignAnchor.TopRight);

                oRow.m_lblWaiting = oRow.AddUIComponent<UILabel>();
                oRow.m_lblWaiting.name = "m_lblWaiting";
                //lblWaiting.backgroundSprite = "MenuPanel2";
                oRow.m_lblWaiting.text = oLineInfo.m_iWaiting.ToString();
                oRow.m_lblWaiting.textAlignment = UIHorizontalAlignment.Center;
                oRow.m_lblWaiting.verticalAlignment = UIVerticalAlignment.Middle;
                oRow.m_lblWaiting.autoSize = false;
                oRow.m_lblWaiting.height = iROW_HEIGHT;
                oRow.m_lblWaiting.width = PublicTransportInfoPanel.iCOLUMN_WIDTH_WAITING;
                oRow.m_lblWaiting.AlignTo(oRow, UIAlignAnchor.TopRight);

                oRow.m_lblBusiest = oRow.AddUIComponent<UILabel>();
                oRow.m_lblBusiest.name = "m_lblBusiest";
                //lblBusiest.backgroundSprite = "MenuPanel2";
                oRow.m_lblBusiest.text = oLineInfo.m_iBusiest.ToString();
                oRow.m_lblBusiest.textAlignment = UIHorizontalAlignment.Center;
                oRow.m_lblBusiest.verticalAlignment = UIVerticalAlignment.Middle;
                oRow.m_lblBusiest.autoSize = false;
                oRow.m_lblBusiest.height = iROW_HEIGHT;
                oRow.m_lblBusiest.width = PublicTransportInfoPanel.iCOLUMN_WIDTH_BUSIEST;
                oRow.m_lblBusiest.AlignTo(oRow, UIAlignAnchor.TopRight);
            
                return oRow;
            } else
            {
               return null;
            }
        }

        private void OnItemClicked(UIComponent component, UIMouseEventParameter eventParam)
        {
            Debug.Log("ListViewRow.OnItemClicked."); 
            ShowWorldInfoPanel();
        }

        private void OnItemEnter(UIComponent component, UIMouseEventParameter eventParam)
        {        
            ListViewRow oRow = component as ListViewRow;
            if (oRow != null)
            {
                if (m_HighlightedLineId != 0 && m_HighlightedLineId != (ushort)m_oLineInfo.m_iLineId)
                {
                    ClearHighlight();
                }

                m_HighlightedLineId = (ushort)m_oLineInfo.m_iLineId;
                backgroundSprite = "ListItemHighlight";
                TransportManagerUtils.HighlightLine(m_HighlightedLineId);
            }

        }

        private void OnItemLeave(UIComponent component, UIMouseEventParameter eventParam)
        {
            ClearHighlight();
            ClearRowHighlight();

            ListViewRow oRow = component as ListViewRow;
            if (oRow != null && m_HighlightedLineId != 0)
            {
                TransportManagerUtils.ClearLineHighlight(m_HighlightedLineId);
                m_HighlightedLineId = 0;
            }
        }

        public void ClearRowHighlight()
        {
            backgroundSprite = "GenericPanel";
        }

        private void ClearHighlight()
        {
            if (m_HighlightedLineId != 0)
            {
                TransportManagerUtils.ClearLineHighlight(m_HighlightedLineId);
                m_HighlightedLineId = 0;
            }
        }

        public void ShowWorldInfoPanel()
        {
            try
            {
                // Hide the main panel before showing PTWI panel.
                PublicTransportInstance.HidePanel(false);

                // Turn on public transport mode so you can see the lines
                Singleton<InfoManager>.instance.SetCurrentMode(InfoManager.InfoMode.Transport, InfoManager.SubInfoMode.Default);
                UIView.library.Hide("PublicTransportInfoViewPanel");

                // Shift camera to busiest stop
                InstanceID oInstanceId = new InstanceID { TransportLine = (ushort)m_oLineInfo.m_iLineId };
                Vector3 oStopPosition = Singleton<NetManager>.instance.m_nodes.m_buffer[m_oLineInfo.m_usBusiestStopId].m_position;
                PublicTransportVehicleButton.cameraController.SetTarget(oInstanceId, oStopPosition, true);

                // Open transport line panel
                WorldInfoPanel.Show<PublicTransportWorldInfoPanel>(oStopPosition, oInstanceId);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                if (ex.InnerException != null)
                {
                    Debug.LogException(ex.InnerException); 
                }
            }
        }

        public void UpdateLineData()
        {
            m_oLineInfo = LineInfoLoader.GetLineInfo(m_oLineInfo.m_iLineId);
            m_lblColor.color = m_oLineInfo.m_color;
            m_lblName.text = m_oLineInfo.m_sName;
            m_lblStops.text = m_oLineInfo.m_iStops.ToString();
            m_lblPassengers.text = m_oLineInfo.m_iPassengers.ToString() + " / " + m_oLineInfo.m_iCapacity;
            m_lblWaiting.text = m_oLineInfo.m_iWaiting.ToString();
            m_lblBusiest.text = m_oLineInfo.m_iBusiest.ToString();
        }

        public override void OnDestroy()
        {
            ClearHighlight();
            if (m_lblColor != null)
            {
                UnityEngine.Object.Destroy(m_lblColor.gameObject);
            }
            if (m_lblName != null)
            {
                UnityEngine.Object.Destroy(m_lblName.gameObject);
            }
            if (m_lblStops != null)
            {
                UnityEngine.Object.Destroy(m_lblStops.gameObject);
            }
            if (m_lblPassengers != null)
            {
                UnityEngine.Object.Destroy(m_lblPassengers.gameObject);
            }
            if (m_lblWaiting != null)
            {
                UnityEngine.Object.Destroy(m_lblWaiting.gameObject);
            }
            if (m_lblBusiest != null)
            {
                UnityEngine.Object.Destroy(m_lblBusiest.gameObject);
            }
            base.OnDestroy();
        }
    }
     
}
