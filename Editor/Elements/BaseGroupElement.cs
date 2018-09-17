using System.Collections.Generic;
using UnityEngine;

namespace Devi.Framework.Editor.Popup
{
    public abstract class BaseGroupElement : BaseSelectableElement
    {
        private static readonly GUIContent s_Content = new GUIContent();
        private const float c_ElementHeight = 20f;
        
        public abstract List<BasePopupElement> Children { get; }

        public override bool IsVisible
        {
            get { return true; }
        }

        public int SelectedIndex { get; set; } //set by window
        public Vector2 Scroll { get; set; } //set by window
        
        protected BaseGroupElement(string title, Texture2D icon) : base(title, icon)
        {
        }
        
        public override float GetWidth()
        {
            s_Content.text = Title;
            s_Content.image = Icon;
            float min, max;
            Styles.GroupItem.CalcMinMaxWidth(s_Content, out min, out max);
            return max + 50;
        }

        public override float GetHeight()
        {
            return c_ElementHeight;
        }

        public override void Draw(Rect rect, Popup popup, BaseGroupElement parent, int index)
        {
            if (Event.current.type == EventType.Repaint)
            {
                var isSelected = parent.SelectedIndex == index;
                
                s_Content.text = Title;
                s_Content.image = Icon;
                Styles.GroupItem.Draw(rect, s_Content, false, false, isSelected, isSelected);
                
                var rectElementForwardArrow = new Rect(rect.xMax - 13f, rect.center.y - 7f, 13f, 13f);
                Styles.RightArrow.Draw(rectElementForwardArrow, false, false, false, false);
            }
        }

        public void CallBeforeContentNeeded(Popup popup)
        {
            OnBeforeContentNeeded(popup);
            for (int i = 0; i < Children.Count; i++)
                OnBeforeContentNeeded(popup);
        }

        private static class Styles
        {
            public static readonly GUIStyle GroupItem;
            public static readonly GUIStyle RightArrow = "AC RightArrow";

            static Styles()
            {
                GroupItem = new GUIStyle("PR Label");
                GroupItem.padding.left -= 15;
                GroupItem.alignment = TextAnchor.MiddleLeft;
                GroupItem.fixedHeight = 20f;
                GroupItem.padding.left += 0x11;
            }
        }
    }
}