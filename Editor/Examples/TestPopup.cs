using UnityEditor;
using UnityEngine;

namespace Devi.Framework.Editor.Popup.Examples
{
    public static class TestPopup
    {
        [MenuItem("Test/Assembly Popup")]
        public static void ShowAssemblyPopup()
        {
            AssemblyPopupManager.Instance.popup.ShowOnScreen(new Vector2(0, 200));
        }
        
        [MenuItem("Test/Popup")]
        public static void ShowPopup()
        {
            var popup = new Popup(new BasicPopupConfig
            {
                Id = 1,
                Separator = "/",
                HasFixedHeight = false,
                HasFixedWidth = false,
                RootName = "Root title",
                UseFavorites = true,
                UseSearch = true
            });
            popup.Root.Children.Add(new CallElement("Item1", mp => {}));
            popup.Root.Children.Add(new CallElement("Item2", mp => {}));
            popup.Root.Children.Add(new CallElement("Item3", mp => {}));
            popup.Root.Children.Add(new GroupElement("Item4")
            {
                Children = { new CallElement("Itettette", mp => {})}
            });
            popup.GetOrCreateGroup("Item5/321312").Children.Add(new CallElement("Iteter", null, null));
            popup.GetOrCreateGroup("Item5/321312").Children.Add(new CallElement("Iteter", null, null));
            popup.GetOrCreateGroup("Item5/321312").Children.Add(new CallElement("Disabled Element", null, null)
            {
                IsEnabledFunc = () => false
            });
            popup.Root.Children.Add(new DownloadGroupElement("Download Group", null, s =>
            {
                Debug.Log("Load");
                s.Children.Add(new GroupElement("Group")
                {
                    Children = {new CallElement("Call Element", null, mp => Debug.Log("Hello"))}
                });
            }));
            popup.GetOrCreateGroup("Handle Parent/And again").Children.Add(new HandleGroupElement("Handle Group", "Item5/321312"));
            popup.ShowOnScreen(new Vector2(0, 200));
        }
    }
}