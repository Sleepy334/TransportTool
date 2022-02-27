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
        private UILabel m_lblBored;
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
            m_lblBored = null;
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
                oRow.Setup(oLineInfo);

                return oRow;
            }
            else
            {
                return null;
            }
        }

        public void Setup(LineInfo oLineInfo)
        {
            UIPanel oIconContainer = AddUIComponent<UIPanel>();
            oIconContainer.name = "oIconContainer";
            oIconContainer.width = iROW_HEIGHT - 4;
            oIconContainer.height = iROW_HEIGHT - 4;

            m_lblColor = oIconContainer.AddUIComponent<UIPanel>();
            m_lblColor.name = "m_lblColor";
            m_lblColor.backgroundSprite = "InfoviewPanel";
            m_lblColor.color = oLineInfo.m_color;
            m_lblColor.height = PublicTransportInfoPanel.iCOLUMN_WIDTH_COLOR;
            m_lblColor.width = PublicTransportInfoPanel.iCOLUMN_WIDTH_COLOR;
            m_lblColor.CenterToParent();

            m_lblName = AddUIComponent<UILabel>();
            m_lblName.name = "m_lblName";
            m_lblName.text = oLineInfo.m_sName;
            m_lblName.verticalAlignment = UIVerticalAlignment.Middle; 
            m_lblName.autoSize = false;
            m_lblName.height = iROW_HEIGHT;
            m_lblName.width = PublicTransportInfoPanel.iCOLUMN_WIDTH_NAME;

            m_lblStops = AddUIComponent<UILabel>();
            m_lblStops.name = "m_lblStops";
            m_lblStops.text = oLineInfo.m_iStops.ToString();
            m_lblStops.textAlignment = UIHorizontalAlignment.Center;
            m_lblStops.verticalAlignment = UIVerticalAlignment.Middle;
            m_lblStops.autoSize = false;
            m_lblStops.height = iROW_HEIGHT;
            m_lblStops.width = PublicTransportInfoPanel.iCOLUMN_WIDTH_STOPS;
            m_lblStops.AlignTo(this, UIAlignAnchor.TopRight);

            m_lblPassengers = AddUIComponent<UILabel>();
            m_lblPassengers.name = "m_lblPassengers";
            m_lblPassengers.text = oLineInfo.m_iPassengers.ToString() + " / " + oLineInfo.m_iCapacity;
            m_lblPassengers.tooltip = "Vehicles: " + m_oLineInfo.m_iVehicleCount;
            m_lblPassengers.textAlignment = UIHorizontalAlignment.Center;
            m_lblPassengers.verticalAlignment = UIVerticalAlignment.Middle;
            m_lblPassengers.autoSize = false;
            m_lblPassengers.height = iROW_HEIGHT;
            m_lblPassengers.width = PublicTransportInfoPanel.iCOLUMN_WIDTH_PASSENGER;
            m_lblPassengers.AlignTo(this, UIAlignAnchor.TopRight);

            m_lblWaiting = AddUIComponent<UILabel>();
            m_lblWaiting.name = "m_lblWaiting";
            m_lblWaiting.text = oLineInfo.m_iWaiting.ToString();
            m_lblWaiting.textAlignment = UIHorizontalAlignment.Center;
            m_lblWaiting.verticalAlignment = UIVerticalAlignment.Middle;
            m_lblWaiting.autoSize = false;
            m_lblWaiting.height = iROW_HEIGHT;
            m_lblWaiting.width = PublicTransportInfoPanel.iCOLUMN_WIDTH_WAITING;
            m_lblWaiting.AlignTo(this, UIAlignAnchor.TopRight);

            m_lblBusiest = AddUIComponent<UILabel>();
            m_lblBusiest.name = "m_lblBusiest";
            m_lblBusiest.text = oLineInfo.m_iBusiest.ToString();
            m_lblBusiest.tooltip = "Stop: " + oLineInfo.m_iBusiestStopNumber;
            m_lblBusiest.textAlignment = UIHorizontalAlignment.Center;
            m_lblBusiest.verticalAlignment = UIVerticalAlignment.Middle;
            m_lblBusiest.autoSize = false;
            m_lblBusiest.height = iROW_HEIGHT;
            m_lblBusiest.width = PublicTransportInfoPanel.iCOLUMN_WIDTH_BUSIEST;
            m_lblBusiest.AlignTo(this, UIAlignAnchor.TopRight);

            m_lblBored = AddUIComponent<UILabel>();
            m_lblBored.name = "m_lblBored";
            m_lblBored.text = oLineInfo.m_iBored.ToString();
            m_lblBored.textAlignment = UIHorizontalAlignment.Center;
            m_lblBored.verticalAlignment = UIVerticalAlignment.Middle;
            m_lblBored.autoSize = false;
            m_lblBored.height = iROW_HEIGHT;
            m_lblBored.width = PublicTransportInfoPanel.iCOLUMN_WIDTH_BORED;
            m_lblBored.AlignTo(this, UIAlignAnchor.TopRight);
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
            m_lblBored.text = m_oLineInfo.m_iBored.ToString();
        }

        public override void OnDestroy()
        {
            if (m_lblBusiest != null && m_lblBusiest.tooltipBox != null)
            {
                m_lblBusiest.tooltipBox.isVisible = false;
            }
            if (m_lblPassengers != null && m_lblPassengers.tooltipBox != null)
            {
                m_lblPassengers.tooltipBox.isVisible = false;
            }
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
            if (m_lblBored != null)
            {
                UnityEngine.Object.Destroy(m_lblBored.gameObject);
            }
            base.OnDestroy();
        }
    }
     
}
