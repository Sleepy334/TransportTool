using ColossalFramework.UI;
using UnityEngine;

namespace PublicTransportInfo
{
    internal class UITooltip : GUIWindow
    {
        private string m_tooltip = "";

        private UITooltip()
            : base("UITooltip", new Rect(16.0f, 16.0f, 100f, 100f), false, false)
        {
        }

        private static float GetFontWidthFactor()
        {
            return (float) (ModSettings.GetSettings().TooltipFontSize / 1.60f);
        }

        private static float GetFontHeightFactor()
        {
            return (float)(ModSettings.GetSettings().TooltipFontSize / 0.85f);
        }

        public static UITooltip Create(UIComponent parent, string tooltip, TextAnchor textAnchor)
        {
            var go = new GameObject("UITooltip");
            go.transform.parent = parent.transform;
            var viewer = go.AddComponent<UITooltip>();
            Vector2 mousePos = UIScaler.MousePosition;
            viewer.SetTooltipTextAnchor(textAnchor);
            viewer.MoveResize(new Rect(mousePos.x, mousePos.y, 400, 200));
            viewer.m_tooltip = tooltip;
            return viewer;
        }

        public void SetTooltip(string sTooltip)
        {
            m_tooltip = sTooltip;

            if (Visible)
            {
                Vector2 oSize = GetPreferredSize();
                Vector2 mousePos = UIScaler.MousePosition;
                Rect oTooltipPos = new Rect(mousePos.x + 20, mousePos.y - oSize.y - 20, oSize.x, oSize.y);
                MoveResize(oTooltipPos);
                Visible = true;
            }
        }

        private Vector2 GetPreferredSize()
        {
            TextGenerationSettings settings = new TextGenerationSettings();
            settings.generationExtents = Vector2.zero;
            settings.textAnchor = m_tooltipTextAnchor;
            settings.alignByGeometry = false;
            settings.scaleFactor = 1.0f;
            settings.color = Color.red;
            settings.font = PublicTransportInstance.GetConstantWidthFont();// m_skin.font;
            settings.fontSize = ModSettings.GetSettings().TooltipFontSize;
            settings.fontStyle = FontStyle.Bold;
            settings.pivot = Vector2.zero;
            settings.richText = false;
            settings.lineSpacing = 1.0f;
            settings.resizeTextForBestFit = false;
            settings.updateBounds = false;
            settings.horizontalOverflow = HorizontalWrapMode.Overflow;
            settings.verticalOverflow = VerticalWrapMode.Overflow;

            // Generate font to get font width and height
            TextGenerator generator = new TextGenerator();
            float fCharacterWidth = generator.GetPreferredWidth("M", settings);
            float fCharacterheight = generator.GetPreferredHeight("Mg", settings) * 1.3f; // leading scaling
            int iLines;
            string sLine = Utils.GetLongestLine(m_tooltip, out iLines);
            int iMaxLength = sLine.Length;
            float fWidth = iMaxLength * fCharacterWidth + 40;
            float fHeight = iLines * fCharacterheight + 40;

            return new Vector2(fWidth, fHeight);
        }

        public void Show()
        {
            if (!Visible)
            {
                Vector2 oSize = GetPreferredSize();
                Vector2 mousePos = UIScaler.MousePosition;
                Rect oTooltipPos = new Rect(mousePos.x + 20, mousePos.y - oSize.y - 20, oSize.x, oSize.y);
                MoveResize(oTooltipPos);
                Visible = true;
            }
        }

        protected override void DrawWindow()
        {
            GUILayout.BeginVertical();
            GUILayout.Label(m_tooltip, GUILayout.ExpandWidth(true));
            GUILayout.EndVertical();
        }
    }
}
