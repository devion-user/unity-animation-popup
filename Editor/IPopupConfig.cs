namespace Devi.Framework.Editor.Popup
{
    public interface IPopupConfig
    {
        int Id { get; }
        bool UseSearch { get; }
        bool UseFavorites { get; }
        string Separator { get; }
        
        float MaxWidth { get; }
        float MaxHeight { get; }
        
        bool HasFixedWidth { get; }
        bool HasFixedHeight { get; }

        string RootName { get; }
    }
}