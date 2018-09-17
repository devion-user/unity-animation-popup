using System;
using UnityEngine;

namespace Devi.Graph
{
    public abstract class BaseSelectableElement : BasePopupElement
    {
        private Texture2D icon;
        public Texture2D Icon
        {
            get { return icon; }
        }

        protected BaseSelectableElement(string title, Texture2D icon) : base(title)
        {
            this.icon = icon;
        }

        protected internal override void OnBeforeContentNeeded(Popup popup)
        {
            base.OnBeforeContentNeeded(popup);
        }

        public abstract void OnClick(Popup popup, Vector2 mousePosition);
    }
}