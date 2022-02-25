using UnityEngine;
using ColossalFramework.UI;

namespace PublicTransportInfo
{
    public class UITitleBar : UIPanel
    {
        const int iTITLE_HEIGHT = 25;

        private UISprite m_icon;
        private UILabel m_title;
        private UIButton m_close;
        private UIDragHandle m_drag;

        public bool isModal = false;

        public UIButton closeButton
        {
            get { return m_close; }
        }

        public string title
        {
            get { return m_title.text; }
            set
            {
                if (m_title == null)
                {
                    SetupControls(value);
                } 
                m_title.text = value; 
            }
        }

        private void SetupControls(string sTitle)
        {
            width = parent.width;
            height = iTITLE_HEIGHT;
            isVisible = true;
            canFocus = true;
            isInteractive = true;
            relativePosition = Vector3.zero;

            m_title = AddUIComponent<UILabel>();
            m_title.text = sTitle;
            m_title.textAlignment = UIHorizontalAlignment.Center;
            m_title.position = new Vector3(this.width / 2f - m_title.width / 2f, -20f + m_title.height / 2f);

            m_close = AddUIComponent<UIButton>();
            m_close.relativePosition = new Vector3(width - 35, 2);
            m_close.normalBgSprite = "buttonclose";
            m_close.hoveredBgSprite = "buttonclosehover";
            m_close.pressedBgSprite = "buttonclosepressed";
            m_close.eventClick += (component, param) =>
            {
                /*
                if (isModal)
                    UIView.PopModal();
                parent.Hide();
                */
                PublicTransportInstance.HidePanel();
            };

            m_drag = AddUIComponent<UIDragHandle>();
            m_drag.width = width - 50;
            m_drag.height = height;
            m_drag.relativePosition = Vector3.zero;
            m_drag.target = parent;
        }
    }
}