using System;

namespace Devi.Graph.Search
{
    public interface IPopupSearchController
    {
        void OnBegin(string searchPattern);
        void OnEnd();

        bool CanShow(IPopupConfig config, BaseCallElement item, out int priority);
    }

    public class DefaultPopupSearchController : IPopupSearchController
    {
        public static IPopupSearchController Instance { get; } = new DefaultPopupSearchController();
        
        private string[] searchLowerWords;
        
        public void OnBegin(string searchPattern)
        {
            var separatorSearch = new[] {' '};
            searchLowerWords = searchPattern.ToLower().Split(separatorSearch);
        }

        public bool CanShow(IPopupConfig config, BaseCallElement item, out int priority)
        {
            var itemNameStartIndex = item.Title.LastIndexOf(config.Separator, StringComparison.Ordinal);
            var itemName = itemNameStartIndex == -1 ? item.Title : item.Title.Substring(itemNameStartIndex + 1);
            
            var itemNameShortLower = itemName.ToLower().Replace(" ", string.Empty);
            bool all = true;
            bool any = false;
            bool startWith = false;
            for (int i = 0; i < searchLowerWords.Length; i++)
            {
                var searchLowerWord = searchLowerWords[i];
                if (!itemNameShortLower.Contains(searchLowerWord))
                {
                    all = false;
                    continue;
                }
                else
                {
                    any = true;
                    if (i == 0 && itemNameShortLower.StartsWith(searchLowerWord))
                        startWith = true;
                }
            }

            if (!any)
            {
                priority = 0;
                return false;
            }

            if (all && startWith)
                priority = 3;
            else if (all || startWith)
                priority = 2;
            else
                priority = 1;
            
            return true;
        }

        public void OnEnd()
        {
        }
    }
}