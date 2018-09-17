namespace Devi.Framework.Editor.Popup
{
    public class BasicPopupConfig : IPopupConfig
    {
        public int Id { get; set; }
        public bool UseSearch { get; set; } = true;
        public bool UseFavorites { get; set; }
        public string Separator { get; set; } = "/";
        public float MaxWidth { get; set; } = 150f;
        public float MaxHeight { get; set; } = 600f;
        public bool HasFixedWidth { get; set; } = true;
        public bool HasFixedHeight { get; set; } = true;
        public string RootName { get; set; } = "";
    }
}