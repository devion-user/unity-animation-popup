using System;
using UnityEngine;

namespace Devi.Framework.Editor.Popup
{
    public class CallElement : BaseCallElement
    {
        public Func<bool> IsVisibleFunc { get; set; } 
        public Func<bool> IsEnabledFunc { get; set; } 
        
        public override bool IsVisible
        {
            get { return IsVisibleFunc == null || IsVisibleFunc.Invoke(); }
        }

        public override bool IsEnabled
        {
            get { return IsEnabledFunc == null || IsEnabledFunc.Invoke(); }
        }

        public CallElement(string title, Texture2D icon, Action<Vector2> callback) : base(title, icon)
        {
            OnUsing = callback;
        }

        public CallElement(string title, Action<Vector2> callback) : base(title, null)
        {
            OnUsing = callback;
        }

        public event Action<Vector2> OnUsing;
        
        public override void OnClick(Popup popup, Vector2 mousePosition)
        {
            if (IsEnabled)
                OnUsing?.Invoke(mousePosition);
        }
    }
}