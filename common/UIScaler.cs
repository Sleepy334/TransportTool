using System;
using UnityEngine;
using ColossalFramework.UI;

namespace PublicTransportInfo
{
    public static class UIScaler {
        public static Matrix4x4 Mat0;

        static bool TryGetScreenResolution(out Vector2 resolution) {
            UIView uIView = UIView.GetAView();
            if (uIView) {
                resolution = uIView.GetScreenResolution();
                return true;
            } else {
                resolution = default;
                return false;
            }
        }

        public static float BaseResolutionX {
            get {
                if (TryGetScreenResolution(out Vector2 resolution))
                    return resolution.x;  // 1920f if aspect ratio is 16:9;
                else
                    return 1080f * AspectRatio;
            }
        }

        public static float BaseResolutionY {
            get {
                if (TryGetScreenResolution(out Vector2 resolution))
                    return resolution.y; // always 1080f. But we keep this code for the sake of future proofing
                else
                    return 1080f;
            }
        }

        public static float AspectRatio => Screen.width / (float)Screen.height;

        public static float MaxWidth
        {
            get
            {
                return Screen.width;
            }
        }

        public static float MaxHeight
        {
            get
            {
                return Screen.height;
            }
        }

        public static float UIScale
        {
            get
            {
                var w = Screen.width / MaxWidth;
                var h = Screen.height / MaxHeight;
                return Mathf.Min(w, h);
            }
        }

        public static Matrix4x4 ScaleMatrix => Matrix4x4.Scale(Vector3.one * UIScaler.UIScale);

        public static void ScaleGUIMatrix() {
            Mat0 = GUI.matrix;
            GUI.matrix = ScaleMatrix;
        }

        public static void RestoreGUIMatrix() => GUI.matrix = Mat0;

        public static Vector2 MousePosition
        {
            get
            {
                var mouse = Input.mousePosition;
                mouse.y = Screen.height - mouse.y;
                return mouse / UIScaler.UIScale;
            }
        }
    }
}
