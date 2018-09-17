using System.Collections.Generic;
using UnityEngine;

namespace Devi.Framework.Editor.Popup
{
    public class HandleGroupElement : BaseGroupElement
    {
        private readonly List<BaseElement> children = new List<BaseElement>();
        public override List<BaseElement> Children
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