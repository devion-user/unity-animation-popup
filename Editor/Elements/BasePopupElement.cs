using UnityEngine;

namespace Devi.Framework.Editor.Popup
{
    public abstract class BasePopupElement
    {
        private readonly string title;
        public string Title
        {
            get { return title; }
        }
        
        
        public abstract bool IsVisible { get; }

        public abstract float GetHeight();
        public abstract float GetWidth();
        public abstract void Draw(Rect rect, Popup popup, BaseGroupElement parent, int index);

        protected BasePopupElement(string title)
        {
            this.title = title;
        }

        protected internal virtual void OnBeforeContentNeeded(Popup popup)
        {
            
        }
    }
}