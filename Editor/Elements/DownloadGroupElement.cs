using System;
using System.Collections.Generic;
using UnityEngine;

namespace Devi.Graph
{
    public class DownloadGroupElement : BaseGroupElement
    {
        private readonly List<BasePopupElement> children = new List<BasePopupElement>();
        public override List<BasePopupElement> Children
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