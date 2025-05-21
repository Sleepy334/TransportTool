using ColossalFramework.UI;
using System;
using System.Collections;
using UnityEngine;

namespace PublicTransportInfo
{
    internal class KeyboardShortcuts : MonoBehaviour
    {
        private Coroutine? m_shortcutCoroutine = null;

        public void Start()
        {
            try
            {
                if (m_shortcutCoroutine is null)
                {
                    m_shortcutCoroutine = StartCoroutine(WaitForShortcutHotkeyCoroutine());
                }
            }
            catch (Exception e)
            {
                Debug.Log("Exception: " + e.Message);
            }
        }

        public void OnDestroy()
        {
            try
            {
                if (m_shortcutCoroutine is not null)
                {
                    StopCoroutine(m_shortcutCoroutine);
                    m_shortcutCoroutine = null;
                }
            }
            catch (Exception e)
            {
                Debug.Log("Exception: " + e.Message);
            }
        }

        private bool IsShortcutPressed()
        {
            return ModSettings.LineIssueHotkey.IsPressed() ||
                    ModSettings.Hotkey.IsPressed();
        }

        private IEnumerator WaitForShortcutHotkeyCoroutine()
        {
            while (true)
            {
                // Wait for key to be released
                yield return new WaitUntil(() => !IsShortcutPressed());

                // Now wait for it to be pressed again
                yield return new WaitUntil(() => IsShortcutPressed());

                if (UIView.HasModalInput() || UIView.HasInputFocus())
                {
                    continue;
                }

                if (ModSettings.LineIssueHotkey.IsPressed())
                {
                    // Create panel if needed
                    LineIssuePanel.TogglePanel();
                }
                else if (ModSettings.Hotkey.IsPressed())
                {
                    // Create panel if needed
                    MainPanel.TogglePanel();
                }
            }
        }
    }
}
