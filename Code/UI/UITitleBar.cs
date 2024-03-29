﻿using UnityEngine;
using ColossalFramework.UI;

namespace PublicTransportInfo
{
    public class UITitleBar : UIPanel
    {
        const int iTITLE_HEIGHT = 35;
        const int iBUTTON_MARGIN = 35;

        private UISprite? m_icon = null;
        private UILabel? m_title = null;
        private UIButton? m_close = null;
        private UIDragHandle? m_drag = null;
        private MouseEventHandler? m_onClick = null;

        private UIButton? m_btnFollow = null;
        private MouseEventHandler? m_onFollowClick = null;

        private UIButton? m_btnIssues = null;
        private MouseEventHandler? m_onIssueClick = null;

        public bool isModal = false;

        public UITitleBar()
        {
        }

        public UIButton? closeButton
        {
            get { return m_close; }
        }

        public string title
        {
            get { return m_title?.text ?? ""; }
            set
            {
                if (m_title == null)
                {
                    SetupControls(value);
                } 
                if (m_title != null)
                {
                    m_title.text = value;
                }
            }
        }

        public void SetOnclickHandler(MouseEventHandler handler)
        {
            m_onClick = handler;
        }

        public void SetFollowHandler(MouseEventHandler handler)
        {
            m_onFollowClick = handler;
        }

        public void SetIssuesHandler(MouseEventHandler handler)
        {
            m_onIssueClick = handler;
        }

        private void SetupControls(string sTitle)
        {
            width = parent.width - 8;
            height = iTITLE_HEIGHT;
            isVisible = true;
            canFocus = true;
            isInteractive = true;
            relativePosition = Vector3.zero;

            m_title = AddUIComponent<UILabel>();
            m_title.text = sTitle;
            m_title.textAlignment = UIHorizontalAlignment.Center;
            m_title.position = new Vector3(this.width / 2f - m_title.width / 2f, -20f + m_title.height / 2f);

            float fOffset = iBUTTON_MARGIN;
            m_close = AddUIComponent<UIButton>();
            if (m_close != null)
            {
                m_close.relativePosition = new Vector3(width - fOffset, 2);
                m_close.width = height;
                m_close.height = height;
                m_close.normalBgSprite = "buttonclose";
                m_close.hoveredBgSprite = "buttonclosehover";
                m_close.pressedBgSprite = "buttonclosepressed";
                m_close.eventClick += (component, param) =>
                {
                    if (m_onClick != null)
                    {
                        m_onClick(component, param);
                    }
                };
                fOffset += m_close.width;
            }

            if (m_onFollowClick != null)
            {
                m_btnFollow = AddUIComponent<UIButton>();
                m_btnFollow.name = "m_btnFollow";
                m_btnFollow.tooltip = "Show";
                m_btnFollow.relativePosition = new Vector3(width - fOffset, 2);
                m_btnFollow.width = height;
                m_btnFollow.height = height;
                m_btnFollow.normalBgSprite = "LocationMarkerActiveNormal";
                m_btnFollow.hoveredBgSprite = "LocationMarkerActiveHovered";
                m_btnFollow.focusedBgSprite = "LocationMarkerActiveFocused";
                m_btnFollow.disabledBgSprite = "LocationMarkerActiveDisabled";
                m_btnFollow.pressedBgSprite = "LocationMarkerActivePressed";
                m_btnFollow.eventClick += (component, param) =>
                {
                    if (m_onFollowClick != null)
                    {
                        m_onFollowClick(component, param);
                    }
                };
                fOffset += m_btnFollow.width;
            }

            if (m_onIssueClick != null)
            {
                m_btnIssues = AddUIComponent<UIButton>();
                m_btnIssues.name = "m_btnIssues";
                m_btnIssues.tooltip = "Show Issues Panel";
                m_btnIssues.width = height;
                m_btnIssues.height = height;
                m_btnIssues.relativePosition = new Vector3(width - fOffset, 2);
                m_btnIssues.normalBgSprite = "IconWarning";
                m_btnIssues.color = Color.white;
                m_btnIssues.eventClick += (component, param) =>
                {
                    if (m_onIssueClick != null)
                    {
                        m_onIssueClick(component, param);
                    }
                };
                fOffset += m_btnIssues.width;
            }

            m_drag = AddUIComponent<UIDragHandle>();
            m_drag.width = width - fOffset;
            m_drag.height = height;
            m_drag.relativePosition = Vector3.zero;
            m_drag.target = parent;
        }

        public override void OnDestroy()
        {
            if (m_icon != null)
            {
                Destroy(m_icon.gameObject);
            }
            if (m_title != null)
            {
                Destroy(m_title.gameObject);
            }
            if (m_close != null)
            {
                Destroy(m_close.gameObject);
            }
            if (m_drag != null)
            {
                Destroy(m_drag.gameObject);
            }

            base.OnDestroy();
        }
    }
}