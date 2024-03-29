﻿using ColossalFramework.UI;
using UnityEngine;

namespace PublicTransportInfo
{
    public class SeparatorComponents
    {
        public SeparatorComponents(GameObject mainToolbarSeparatorTemplate, GameObject emptyContainer, UIComponent separatorTab)
        {
            MainToolbarSeparatorTemplate = mainToolbarSeparatorTemplate;
            EmptyContainer = emptyContainer;
            SeparatorTab = separatorTab;
        }

        public GameObject MainToolbarSeparatorTemplate { get; }
        public GameObject EmptyContainer { get; }
        public UIComponent SeparatorTab { get; }
    }
}
