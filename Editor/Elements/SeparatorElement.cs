using UnityEngine;

namespace Devi.Framework.Editor.Popup
{
    public class SeparatorElement : BaseElement
    {
        private const float c_ElementHeight = 8f;

        public override bool IsVisible
        {
            get { return true; }
        }

        public override float GetHeight()
        {
            return c_ElementHeight;
        }

        public override float GetWidth()
        {
            return 10f;
        }

        public override void Draw(Rect rect, Popup popup, BaseGroupElement parent, int index)
        {
            
        }

        public SeparatorElement(string title) : base(title)
        {
        }
    }
}