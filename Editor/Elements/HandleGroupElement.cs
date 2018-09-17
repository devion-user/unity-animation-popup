using System.Collections.Generic;
using UnityEngine;

namespace Devi.Graph
{
    public class HandleGroupElement : BaseGroupElement
    {
        private readonly List<BasePopupElement> children = new List<BasePopupElement>();
        public override List<BasePopupElement> Children
        {
            get { return children; }
        }
        
        public string HandlePath { get; set; }

        public HandleGroupElement(string title, Texture2D icon, string handlePath) : base(title, icon)
        {
            HandlePath = handlePath;
        }

        public HandleGroupElement(string title, string handlePath) : base(title, null)
        {
            HandlePath = handlePath;
        }

        public override void OnClick(Popup popup, Vector2 mousePosition)
        {
            popup.Goto(HandlePath);
        }
    }
}