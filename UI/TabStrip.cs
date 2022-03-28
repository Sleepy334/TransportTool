using ColossalFramework;
using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PublicTransportInfo
{
    public class TabStrip : UIPanel
    {
        public const float fBUITTON_WIDTH = 32.0f;
        const float fBUITTON_HEIGHT = 22.0f;

        public class TabStripSelectionChangedArgs : EventArgs
        {
            public PublicTransportType m_eType { get; set; }
        }

        public event EventHandler? SelectionChangedEventHandler = null;

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
            foreach (UIComponent componentTab in components)
            {
                if (componentTab != null && componentTab is UIButton)
                {
                    UIButton btnTab = (UIButton)componentTab;
                    btnTab.isEnabled = (btnTab.name != eType.ToString());
                }
            }
        }

        public void AddTab(PublicTransportType eTransportType)
        {
            UIButton btnToolbar = AddUIComponent<UIButton>();
            btnToolbar.name = eTransportType.ToString();
            btnToolbar.tooltip = eTransportType.ToString();
            btnToolbar.autoSize = false;
            btnToolbar.width = fBUITTON_WIDTH + 10;
            btnToolbar.height = fBUITTON_HEIGHT + 10;
            btnToolbar.normalBgSprite = "InfoviewPanel";
            btnToolbar.hoveredBgSprite = "ButtonMenuHovered";
            btnToolbar.disabledBgSprite = "InfoviewPanel";
            btnToolbar.pressedBgSprite = "InfoviewPanel";
            btnToolbar.color = PublicTransportTypeUtils.GetDefaultLineColor(eTransportType);
            btnToolbar.disabledColor = Color.gray;
            btnToolbar.pressedColor = Color.gray;

            void mouseEventHandler(UIComponent c, UIMouseEventParameter p)
            {
                TabStripSelectionChangedArgs eArgs = new TabStripSelectionChangedArgs();
                eArgs.m_eType = eTransportType;
                OnSelectionChangedEventHandler(eArgs);
            }
            btnToolbar.eventMouseDown += mouseEventHandler;

            UISprite spriteToolbar = btnToolbar.AddUIComponent<UISprite>();
            if (spriteToolbar != null)
            {
                spriteToolbar.name = "sprite" + eTransportType.ToString();
                spriteToolbar.autoSize = false;
                spriteToolbar.width = fBUITTON_WIDTH;
                spriteToolbar.height = fBUITTON_HEIGHT;
                string vehicleTypeIcon = "";
                List<TransportInfo.TransportType> oList = PublicTransportTypeUtils.Convert(eTransportType);
                if (oList.Count > 0)
                {
                    vehicleTypeIcon = PublicTransportTypeUtils.GetVehicleTypeIcon(oList[0]);
                }
                spriteToolbar.spriteName = vehicleTypeIcon;
                spriteToolbar.CenterToParent();
            }
        }

        protected virtual void OnSelectionChangedEventHandler(TabStripSelectionChangedArgs e)
        {
            EventHandler? handler = SelectionChangedEventHandler;
            if (handler != null)
            {
                handler.Invoke(this, e);
            }
            
        }
    }
}
