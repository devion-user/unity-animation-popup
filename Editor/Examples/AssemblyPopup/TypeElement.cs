using System;
using System.Collections.Generic;
using UnityEngine;

namespace Devi.Framework.Editor.Popup.Examples
{
    public class TypeElement : BaseGroupElement
    {
        private readonly List<BaseElement> children = new List<BaseElement>();
        public override List<BaseElement> Children
        {
            get { return children; }
        }

        public event Action<TypeElement> OnDownload;
        private bool needDownload = true;

        public TypeElement(Type type) : base(type.Name, null)
        {
           
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