using System;

namespace Devi.Framework.Editor.Popup.Examples
{
    internal class AssemblyPopupManager
    {
        public static readonly AssemblyPopupManager Instance = new AssemblyPopupManager();

        public Popup popup;
        
        private AssemblyPopupManager()
        {
            popup = new Popup(new BasicPopupConfig
            {
                Id = "Assembly".GetHashCode(),
                UseFavorites = true,
                UseSearch = true
            });
            var commonElement = new GroupElement("~");
            popup.Root.Children.Add(commonElement);
            
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (string.IsNullOrEmpty(type.Namespace))
                    {
                        commonElement.Children.Add(new TypeElement(type));
                    }
                    else
                    {
                        popup.GetOrCreateGroup(type.Namespace.Replace(".", "/")).Children.Add(new TypeElement(type));
                    }
                }
            }
        }
    }
}