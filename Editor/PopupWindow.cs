using System.Collections.Generic;
using System.Linq;
using Devi.Framework.Editor.Popup.Search;
using UnityEditor;
using UnityEngine;

namespace Devi.Framework.Editor.Popup
{
    internal class PopupWindow : EditorWindow
    {
        private Popup mPopup;
        public IPopupConfig config;

        private Vector2 mUnconvertedPosition; 
        private Vector2 mScreenPosition;
        private Vector2 mAutoSize;
        private Vector2 mScreenSize;
        
        private string mSearchText = "";
        
        private bool mNeedUpdate = true;
        
        private bool mScrollToSelected;
        
        private readonly List<BaseGroupElement> mContexts = new List<BaseGroupElement>();
        
        //Animation
        private float mCurrentLastContextOffset;
        private int mFinishLastContextOffset;
        
        private readonly TimeDelta mTimeDelta = new TimeDelta(false);
        
        private bool HasSearch
        {
            get { return config.UseSearch && !string.IsNullOrEmpty(mSearchText); }
        }

        private BaseGroupElement ActiveParent
        {
            get { return mContexts[mContexts.Count - 2 + mFinishLastContextOffset]; }
        }

        private BasePopupElement ActiveElement
        {
            get { return ActiveParent?.Children[ActiveParent.SelectedIndex]; }
        }

        /// <summary> Создание окна </summary>
        public static PopupWindow Create(Vector2 pos, bool gui, Popup popup, IPopupConfig config)
        {
            var window = CreateInstance<PopupWindow>();
            window.mPopup = popup;
            window.config = config;
            window.mUnconvertedPosition = pos;
            window.mScreenPosition = gui ? GUIUtility.GUIToScreenPoint(pos) : pos;
            return window;
        }

        public new void Show()
        {
            mCurrentLastContextOffset = 1f;
            mFinishLastContextOffset = 1; 
            
            mPopup.Root.CallBeforeContentNeeded(mPopup);
            mPopup.Search.CallBeforeContentNeeded(mPopup);

            CalcAutoSize();
            InitializeContext();
            
            mNeedUpdate = false;
            RebuildSearch();

            mScreenSize = new Vector2
            {
                x = !config.HasFixedWidth ? Mathf.Min(mAutoSize.x, config.MaxWidth) : config.MaxWidth,
                y = !config.HasFixedHeight ? Mathf.Min(mAutoSize.y, config.MaxHeight) : config.MaxHeight
            };

            ShowAsDropDown(new Rect(mScreenPosition, Vector2.one), mScreenSize);
            Focus();
            wantsMouseMove = true;
        }

        private void CalcAutoSize()
        {
            var headerHeight = (config.UseSearch ? 30f : 0f) + 26f;
            var width = 0f;
            var height = 0f;
            CalcSizeRecursion(mPopup.Root, ref width, ref height);
            mAutoSize.x = width;
            mAutoSize.y = headerHeight + height;
        }

        private void CalcSizeRecursion(BaseGroupElement groupElement, 
            ref float width,
            ref float height)
        {
            float sum = 0f;
            for (int i = 0; i < groupElement.Children.Count; i++)
            {
                var child = groupElement.Children[i];
                if (child.IsVisible)
                {
                    sum += child.GetHeight();

                    var w = child.GetWidth();
                    if (width < w)
                        width = w;
                }
            }
            if (height < sum)
                height = sum;
            for (int i = 0; i < groupElement.Children.Count; i++)
            {
                var child = groupElement.Children[i] as BaseGroupElement;
                if (child != null && child.IsVisible)
                    CalcSizeRecursion(child, ref width, ref height);
            }
        }

        private void InitializeContext()
        {
            if (mContexts.Count == 0)
            {
                mContexts.Add(mPopup.Root);
            }
        }
        
        private void RebuildSearch()
        {
            if (!HasSearch)
            {
                //exit from search
                var lastContext = mContexts[mContexts.Count - 1];
                if (lastContext == mPopup.Search)
                {
//                    mTimeDelta.Reset();
                    mContexts.Clear();
                    mContexts.Add(mPopup.Root);
                }

                mFinishLastContextOffset = 1;
                return;
            }

            var searchController = DefaultPopupSearchController.Instance;
            searchController.OnBegin(mSearchText);

            var elementsDict = new Dictionary<int, List<BaseCallElement>>();
            DoSearchRecursion(searchController, mPopup.Root, elementsDict);
            
            searchController.OnEnd();

            mPopup.Search.Children.Clear();

            var priors = elementsDict.Keys.OrderByDescending(x => x).ToArray();

            for (int i = 0; i < priors.Length; i++)
            {
                var list = elementsDict[priors[i]];
                mPopup.Search.Children.AddRange(list);
            }
            
            mContexts.Clear();
            mContexts.Add(mPopup.Search);
            
            //todo activeParent?
            if (ActiveParent.Children.Count >= 1)
                ActiveParent.SelectedIndex = 0;
            else
                ActiveParent.SelectedIndex = -1;
        }

        private void DoSearchRecursion(IPopupSearchController searchController, GroupElement parent, Dictionary<int, List<BaseCallElement>> elementsDict)
        {
            for (int i = 0; i < parent.Children.Count; i++)
            {
                var callElement = parent.Children[i] as BaseCallElement;
                if (callElement != null)
                {
                    int priority;
                    if (searchController.CanShow(config, callElement, out priority))
                    {
                        //add to search
                        List<BaseCallElement> elements;
                        if (!elementsDict.TryGetValue(priority, out elements))
                        {
                            elements = new List<BaseCallElement>();
                            elementsDict.Add(priority, elements);
                        }
                        elements.Add(callElement);
                    }
                    continue;
                }

                var groupElement = parent.Children[i] as GroupElement;
                if (groupElement != null)
                {
                    DoSearchRecursion(searchController, groupElement, elementsDict);
                }
            }
        }
        
        public void OnGUI()
        {
            if (config == null)
            {
                Close();
                return;
            }
            
            HandleKeyboard();
            
            GUI.Label(new Rect(0, 0, position.width, position.height), GUIContent.none, Styles.Background);

            //Поиск
            if (config.UseSearch)
            {
                GUILayout.Space(7f);
                var rectSearch = GUILayoutUtility.GetRect(10f, 20f);
                rectSearch.x += 8f;
                rectSearch.width -= 16f;
                EditorGUI.FocusTextInControl("SearchField");
                GUI.SetNextControlName("SearchField");
                if (SearchField(rectSearch, ref mSearchText))
                    RebuildSearch();
            }
            
            //Элементы
            SectionGUI(mCurrentLastContextOffset, GetElementRelative(0), GetElementRelative(-1));
            if (mCurrentLastContextOffset < 1f && mContexts.Count > 1)
                SectionGUI(mCurrentLastContextOffset + 1f, GetElementRelative(-1), GetElementRelative(-2));
        }

        private void Update()
        {
            var timeDelta = mTimeDelta.UpdateDelta(false);
            if (mCurrentLastContextOffset != mFinishLastContextOffset)
            {
                mCurrentLastContextOffset = Mathf.MoveTowards(mCurrentLastContextOffset, mFinishLastContextOffset, timeDelta*4f);
                if (mFinishLastContextOffset == 0 && mCurrentLastContextOffset == 0f)
                {
                    mCurrentLastContextOffset = 1f;
                    mFinishLastContextOffset = 1;
                    mContexts.RemoveAt(mContexts.Count - 1);
                }

                Repaint();
            }
        }

        private void HandleKeyboard()
        {
            Event evt = Event.current;
            if (evt.type == EventType.KeyDown)
            {
                if (evt.keyCode == KeyCode.DownArrow)
                {
                    ActiveParent.SelectedIndex++;
                    ActiveParent.SelectedIndex = Mathf.Min(ActiveParent.SelectedIndex,
                        ActiveParent.Children.Count - 1);
                    mScrollToSelected = true;
                    evt.Use();
                }

                if (evt.keyCode == KeyCode.UpArrow)
                {
                    ActiveParent.SelectedIndex--;
                    ActiveParent.SelectedIndex = Mathf.Max(ActiveParent.SelectedIndex, 0);
                    mScrollToSelected = true;
                    evt.Use();
                }

                if (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter)
                {
                    GoToChild(ActiveElement, false);
                    evt.Use();
                }

                if (!HasSearch)
                {
                    if (evt.keyCode == KeyCode.LeftArrow || evt.keyCode == KeyCode.Backspace)
                    {
                        GoToParent();
                        evt.Use();
                    }

                    if (evt.keyCode == KeyCode.RightArrow)
                    {
                        GoToChild(ActiveElement, true);
                        evt.Use();
                    }

                    if (evt.keyCode == KeyCode.Escape)
                    {
                        Close();
                        evt.Use();
                    }
                }
            }
        }

        private static bool SearchField(Rect position, ref string text)
        {
            var rectField = position;
            rectField.width -= 15f;
            var startText = text;
            text = GUI.TextField(rectField, startText ?? "", Styles.SearchTextField);

            var rectCancel = position;
            rectCancel.x += position.width - 15f;
            rectCancel.width = 15f;
            var styleCancel = text == "" ? Styles.SearchCancelButtonEmpty : Styles.SearchCancelButton;
            if (GUI.Button(rectCancel, GUIContent.none, styleCancel) && text != "")
            {
                text = "";
                GUIUtility.keyboardControl = 0;
            }

            return startText != text;
        }

        private void SectionGUI(float anim, BaseGroupElement parent, BaseGroupElement grandParent)
        {
            anim = Mathf.Floor(anim) + Mathf.SmoothStep(0f, 1f, Mathf.Repeat(anim, 1f));
            Rect rectArea = position;
            rectArea.x = position.width * (1f - anim) + 1f;
            rectArea.y = config.UseSearch ? 30f : 0;
            rectArea.height -= config.UseSearch ? 30f : 0;
            rectArea.width -= 2f;
            GUILayout.BeginArea(rectArea);
            {
                var rectHeader = GUILayoutUtility.GetRect(10f, 25f);
                var nameHeader = parent.Title;
                GUI.Label(rectHeader, nameHeader, Styles.Header);
                if (grandParent != null)
                {
                    var evt = Event.current;
                    var rectHeaderBackArrow = new Rect(rectHeader.x + 4f, rectHeader.y + 7f, 13f, 13f);
                    if (evt.type == EventType.Repaint)
                        Styles.LeftArrow.Draw(rectHeaderBackArrow, false, false, false, false);
                    if (evt.type == EventType.MouseDown && rectHeader.Contains(Event.current.mousePosition))
                    {
                        GoToParent();
                        evt.Use();
                    }
                }

                ListGUI(new Rect(0, 0, rectArea.width, rectArea.height), parent);
            }
            GUILayout.EndArea();
        }

        private void ListGUI(Rect listRect, BaseGroupElement parent)
        {
            parent.Scroll = GUILayout.BeginScrollView(parent.Scroll);
            EditorGUIUtility.SetIconSize(new Vector2(16f, 16f));
            var children = parent.Children;
            var rect = new Rect();
            for (int i = 0; i < children.Count; i++)
            {
                var element = children[i];
                if (element.IsVisible)
                {
                    var options = new[] {GUILayout.ExpandWidth(true)};
                    var rectElement = GUILayoutUtility.GetRect(16f, element.GetHeight(), options);
                    if ((Event.current.type == EventType.MouseMove || Event.current.type == EventType.MouseDown)
                        && parent.SelectedIndex != i && rectElement.Contains(Event.current.mousePosition))
                    {
                        parent.SelectedIndex = i;
                        Repaint();
                    }

                    if (i == parent.SelectedIndex)
                    {
                        rect = rectElement;
                    }

                    if (Event.current.type == EventType.Repaint && listRect.Overlaps(rectElement))
                    {
                        element.Draw(rectElement, mPopup, parent, i);
                    }

                    if (Event.current.type == EventType.MouseDown && rectElement.Contains(Event.current.mousePosition))
                    {
                        Event.current.Use();
                        parent.SelectedIndex = i;
                        GoToChild(element, false);
                    }
                }
            }

            EditorGUIUtility.SetIconSize(Vector2.zero);
            GUILayout.EndScrollView();
            if (mScrollToSelected && Event.current.type == EventType.Repaint)
            {
                mScrollToSelected = false;
                var lastRect = GUILayoutUtility.GetLastRect();
                
                var parentScroll = parent.Scroll;
                if ((rect.yMax - lastRect.height) > parentScroll.y)
                {
                    
                    parentScroll.y = rect.yMax - lastRect.height;
                    Repaint();
                }

                if (rect.y < parentScroll.y)
                {
                    parentScroll.y = rect.y;
                    Repaint();
                }
                parent.Scroll = parentScroll;
            }
        }

        private void GoToParent()
        {
            if (mContexts.Count <= 1)
                return;
            
            mFinishLastContextOffset = 0;
        }
        
        private void GoToChild(BasePopupElement e, bool right)
        {
            var element = e as BaseCallElement;
            if (element != null)
            {
                if (right)
                    return;
                if (element.IsEnabled)
                {
                    element.OnClick(mPopup, mUnconvertedPosition);
                    Close();
                }
                return;
            }

            var selectableElement = e as BaseSelectableElement;
            if (selectableElement != null)
            {
                var group = e as BaseGroupElement;
                group?.CallBeforeContentNeeded(mPopup);
                    
                selectableElement.OnClick(mPopup, mUnconvertedPosition);
                
//                if (mFinishLastContextOffset == 0)
//                    mFinishLastContextOffset = 1;
//                if (mCurrentLastContextOffset == 1f)
//                {
//                    mCurrentLastContextOffset = 0f;
//
//                    
////                    var eGroup = e as BaseGroupElement;
////                    if (eGroup != null)
////                    {
////                        mContexts.Add(eGroup);
////                        eGroup.CallBeforeContentNeeded(mPopup);
////                    }
//                }
//                mFinishLastContextOffset = 1;
            }
        }

        public void SetContext(BaseGroupElement groupElement, bool clearOldContext)
        {
            if (mPopup.Search == groupElement)
            {
                mContexts.Clear();
                mContexts.Add(groupElement);
                return;
            }
            
            if (HasSearch)
            {
                mSearchText = "";
                
                //exit from search
                var lastContext = mContexts[mContexts.Count - 1];
                if (lastContext == mPopup.Search)
                {
                    mContexts.Clear();
                    mContexts.Add(mPopup.Root);
                }
                mFinishLastContextOffset = 1;
                mContexts.Add(groupElement);
                return;
            }
            
            if (mFinishLastContextOffset == 0)
                mFinishLastContextOffset = 1;

            if (mCurrentLastContextOffset == 1f)
                mCurrentLastContextOffset = 0f;
            
            if (clearOldContext)
            {
                mContexts.Clear();
                mContexts.Add(mPopup.Root);
            }
            
            mContexts.Add(groupElement);
        }
        
        private BasePopupElement GetChild(IList<BasePopupElement> elements, GroupElement parent, string path)
        {
            for (int i = 0; i < elements.Count; i++)
            {
                var element = elements[i] as BaseSelectableElement;
                if (element != null && parent.Children.Contains(element) && element.Title == path)
                {
                    return element;
                }
            }
            return null;
        }

        private BaseGroupElement GetElementRelative(int rel)
        {
            int num = mContexts.Count - 1 + rel;
            return num < 0 ? null : mContexts[num];
        }

        private static class Styles
        {
            public static readonly GUIStyle SearchTextField = "SearchTextField";
            public static readonly GUIStyle SearchCancelButton = "SearchCancelButton";
            public static readonly GUIStyle SearchCancelButtonEmpty = "SearchCancelButtonEmpty";

            public static readonly GUIStyle Background = "grey_border";
            public static readonly GUIStyle Header = new GUIStyle("In BigTitle");
            public static readonly GUIStyle LeftArrow = "AC LeftArrow";

            static Styles()
            {
                Header.font = EditorStyles.boldLabel.font;
            }
        }
    }
}