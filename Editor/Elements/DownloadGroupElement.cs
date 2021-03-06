using System;
using System.Collections.Generic;
using UnityEngine;

namespace Devi.Framework.Editor.Popup
{
    public class DownloadGroupElement : BaseGroupElement
    {
        private readonly List<BaseElement> children = new List<BaseElement>();
        public override List<BaseElement> Children
        {
            get { return children; }
        }

        public event Action<DownloadGroupElement> OnDownload;
        private bool needDownload = true;

        public DownloadGroupElement(string title, Texture2D icon, Action<DownloadGroupElement> action) : base(title, icon)
        {
            OnDownload = action;
        }
        
        public DownloadGroupElement(string title, Action<DownloadGroupElement> action) : base(title, null)
        {
            OnDownload = action;
        }

        public override void OnClick(Popup popup, Vector2 mousePosition)
        {
            if (needDownload)
            {
                needDownload = false;
                OnDownload?.Invoke(this);
            }
            popup.Goto(this, false);
        }
    }
}