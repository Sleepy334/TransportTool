using ColossalFramework;
using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PublicTransportInfo
{
    public class TabStrip : UIPanel
    {
        public class TabStripSelectionChangedArgs : EventArgs
        {
            public PublicTransportType m_eType { get; set; }
        }

        public event EventHandler SelectionChangedEventHandler;

        public TabStrip() : base()
        {
        }

        public void Clear()
        {
            if (components == null)
            {
                return;
            }

            for (int index = 0; index < components.Count; ++index)
            {
                UnityEngine.Object.Destroy((UnityEngine.Object)components[index].gameObject);
            }

            m_ChildComponents = PoolList<UIComponent>.Obtain();
        }

        public void SelectTab(PublicTransportType eType)
        {
            // Select correct tab button
            foreach (UIPanel btnTab in components)
            {
                if (btnTab != null)
                {
                    if (btnTab.name == eType.ToString())
                    {
                        btnTab.backgroundSprite = "ListItemHover";
                    }
                    else
                    {
                        btnTab.backgroundSprite = "InfoviewPanel";
                    }
                }
            }
        }

        public void AddTab(PublicTransportType eTransportType)
        {
            const float fBUITTON_WIDTH = 32.0f;
            const float fBUITTON_HEIGHT = 22.0f;

            // Add Tab button
            UIPanel oButtonPanel = AddUIComponent<UIPanel>();
            oButtonPanel.width = fBUITTON_WIDTH + 10;
            oButtonPanel.height = fBUITTON_HEIGHT + 10;
            oButtonPanel.backgroundSprite = "InfoviewPanel";
            oButtonPanel.autoLayoutStart = LayoutStart.TopLeft;
            oButtonPanel.autoLayoutPadding = new RectOffset(4, 4, 4, 4);
            oButtonPanel.autoLayout = true;
            oButtonPanel.name = eTransportType.ToString();

            UIButton btnImage = oButtonPanel.AddUIComponent<UIButton>();
            btnImage.text = "";
            btnImage.tooltip = eTransportType.ToString();
            List<TransportInfo.TransportType> oList = PublicTransportTypeUtils.Convert(eTransportType);
            string vehicleTypeIcon = "";
            if (oList.Count > 0)
            {
                vehicleTypeIcon = PublicTransportWorldInfoPanel.GetVehicleTypeIcon(oList[0]);
            }
            btnImage.width = fBUITTON_WIDTH;
            btnImage.height = fBUITTON_HEIGHT;
            btnImage.normalBgSprite = vehicleTypeIcon;
            btnImage.hoveredBgSprite = vehicleTypeIcon + "Focused";
            btnImage.verticalAlignment = UIVerticalAlignment.Middle;

            void mouseEventHandler(UIComponent c, UIMouseEventParameter p)
            {
                TabStripSelectionChangedArgs eArgs = new TabStripSelectionChangedArgs();
                eArgs.m_eType = eTransportType;
                OnSelectionChangedEventHandler(eArgs);
            }
            btnImage.eventClick += mouseEventHandler;
        }

        protected virtual void OnSelectionChangedEventHandler(TabStripSelectionChangedArgs e)
        {
            EventHandler handler = SelectionChangedEventHandler;
            if (handler != null)
            {
                handler.Invoke(this, e);
            }
            
        }
    }
}
