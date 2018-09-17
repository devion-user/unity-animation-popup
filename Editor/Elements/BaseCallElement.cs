using UnityEngine;

namespace Devi.Graph
{
    public abstract class BaseCallElement : BaseSelectableElement
    {
        private static readonly GUIContent s_Content = new GUIContent();
        private const float c_ElementHeight = 20f;
        
        public abstract bool IsEnabled { get; }
        
        protected BaseCallElement(string title, Texture2D icon) : base(title, icon)
        {
        }

        public override float GetWidth()
        {
            s_Content.text = Title;
            s_Content.image = Icon;
            float min, max;
            Styles.CallItem.CalcMinMaxWidth(s_Content, out min, out max);
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
                
                
                Styles.CallItem.normal.textColor = IsEnabled ? new Color(0, 0, 0) : new Color(0.41f, 0.41f, 0.41f);
                Styles.CallItem.onNormal.textColor = IsEnabled ? new Color(1, 1, 1) : new Color(0.8f, 0.8f, 0.8f);
                
                Styles.CallItem.Draw(rect, s_Content, false, false, isSelected, isSelected && IsEnabled);
            }
        }

        private static class Styles
        {
            public static readonly GUIStyle CallItem;

            static Styles()
            {
                CallItem = new GUIStyle("PR Label");
                CallItem.padding.left -= 15;
                CallItem.alignment = TextAnchor.MiddleLeft;
                CallItem.fixedHeight = 20f;
                CallItem.padding.left += 0x11;
                
//                CallItem = new GUIStyle("PR Label");
//                CallItem.padding.left -= 15;
//                CallItem.alignment = TextAnchor.MiddleLeft;
//                CallItem.fixedHeight = 20f;
            }
        }
    }
}