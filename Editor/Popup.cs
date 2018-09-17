using System;
using UnityEngine;

namespace Devi.Framework.Editor.Popup
{
    public class Popup
    {
        /// <summary> Окно, которое связано с попапом </summary>
        internal PopupWindow window;

        private IPopupConfig config;
        public IPopupConfig Config
        {
            get { return config; }
        }

        public GroupElement Root { get; private set; } 
        public GroupElement Search { get; private set; }
        
        /// <summary> Создание окна </summary>
        public Popup(IPopupConfig config)
        {
            this.config = config;
            Root = new GroupElement(config.RootName, null);
            Search = new GroupElement("Search", null);
        }
        
        public bool ShowOnScreen(Vector2 screenPoint)
        {
            if (Root.Children.Count > 0)
            {
                window = PopupWindow.Create(screenPoint, false, this, config);
                window.Show();
                return true;
            }
            return false;
        }

        public bool ShowOnGUI(Vector2 guiPoint)
        {
            if (Root.Children.Count > 0)
            {
                window = PopupWindow.Create(guiPoint, true, this, config);
                window.Show();
                return true;
            }
            return false;
        }

        public void Repaint()
        {
            if (window)
                window.Repaint();
        }

        public void Close()
        {
            if (window)
            {
                window.Close();
                window = null;
            }
        }

        public BaseGroupElement GetOrCreateGroup(string path)
        {
            if (path == "")
                return Root;
            
            s_Separators[0] = config.Separator;
            var parts = path.Split(s_Separators, StringSplitOptions.None);
            BaseGroupElement groupElement = Root;
            
            string currentPath = "";
            for (int i = 0; i < parts.Length; i++)
            {
                var title = parts[i];
                currentPath += title;

                groupElement.OnBeforeContentNeeded(this);
                bool any = false;
                for (int j = 0; j < groupElement.Children.Count; j++)
                {
                    var child = groupElement.Children[j];
                    if (child.Title == title && child is BaseGroupElement)
                    {
                        groupElement = (BaseGroupElement) child;
                        any = true;
                        break;
                    }
                }

                if (!any)
                {
                    var g = new GroupElement(title, null);
                    groupElement.Children.Add(g);
                    groupElement = g;
                }
                
                currentPath += config.Separator;
            }

            return groupElement;
        }

        private static readonly string[] s_Separators = new string[1];
        public bool Goto(string path)
        {
            s_Separators[0] = config.Separator;
            var parts = path.Split(s_Separators, StringSplitOptions.None);

            BaseGroupElement groupElement = Root;
            if (groupElement == null)
            {
                Debug.LogError("Not found root");
                return false;
            }
            
            string currentPath = "";
            for (int i = 0; i < parts.Length - 1; i++)
            {
                var p = parts[i];
                currentPath += p;

                groupElement.OnBeforeContentNeeded(this);
                bool any = false;
                for (int j = 0; j < groupElement.Children.Count; j++)
                {
                    var child = groupElement.Children[j];
                    if (child.Title == p && child is BaseGroupElement)
                    {
                        groupElement = (BaseGroupElement) child;
                        any = true;
                        break;
                    }
                }

                if (!any)
                {
                    Debug.LogError("Not found element in path: " + currentPath);
                    return false;
                }
                
                currentPath += config.Separator;
            }

//            currentPath += parts[parts.Length - 1];

            bool anyFound = false;
            BaseGroupElement gotoElement = groupElement;
            for (int i = 0; i < groupElement.Children.Count; i++)
            {
                var child = groupElement.Children[i];
                if (child.Title == parts[parts.Length-1])
                {
                    anyFound = true;
                    if (child is BaseGroupElement)
                    {
                        gotoElement = (BaseGroupElement) child;
                    }
                    else
                    {
                        gotoElement.SelectedIndex = i;
                    }
                    break;
                }
            }

            if (anyFound)
                Goto(gotoElement, false);
            
            return anyFound;
        }

        public void Goto(BaseGroupElement groupElement, bool clearOldContext)
        {
            if (window)
                window.SetContext(groupElement, clearOldContext);
        }
    }
}