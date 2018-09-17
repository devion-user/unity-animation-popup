using System.Collections.Generic;
using UnityEngine;

namespace Devi.Graph
{
    public class GroupElement : BaseGroupElement
    {
        private readonly List<BasePopupElement> children = new List<BasePopupElement>();
        public override List<BasePopupElement> Children
        {
            get { return children; }
        }

        public GroupElement(string title, Texture2D icon) : base(title, icon)
        {
        }

        public GroupElement(string title) : base(title, null)
        {
        }

        public override void OnClick(Popup popup, Vector2 mousePosition)
        {
            popup.Goto(this, false);
        }
    }
}