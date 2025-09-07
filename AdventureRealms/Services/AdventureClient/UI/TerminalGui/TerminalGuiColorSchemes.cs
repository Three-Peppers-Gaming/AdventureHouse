using Terminal.Gui;

namespace AdventureRealms.Services.AdventureClient.UI.TerminalGui
{
    /// <summary>
    /// Centralized color schemes for Terminal.Gui Adventure Client
    /// </summary>
    public static class TerminalGuiColorSchemes
    {
        /// <summary>
        /// Blue scheme with green borders for frame views
        /// </summary>
        public static readonly ColorScheme BlueScheme = new ColorScheme
        {
            Normal = new Terminal.Gui.Attribute(Color.Green, Color.Black),     // Green borders
            Focus = new Terminal.Gui.Attribute(Color.Red, Color.Black),        // Red when selected
            HotNormal = new Terminal.Gui.Attribute(Color.Green, Color.Black),  // Green hotkeys (P, V, H, G)
            HotFocus = new Terminal.Gui.Attribute(Color.Red, Color.Black),     // Red hotkeys when selected
            Disabled = new Terminal.Gui.Attribute(Color.Gray, Color.Black)
        };
        
        /// <summary>
        /// Menu scheme for proper menu colors and readability
        /// </summary>
        public static readonly ColorScheme MenuScheme = new ColorScheme
        {
            Normal = new Terminal.Gui.Attribute(Color.White, Color.Black),     // White menu text (lay, elp, ame)
            Focus = new Terminal.Gui.Attribute(Color.White, Color.Black),      // White when selected
            HotNormal = new Terminal.Gui.Attribute(Color.Green, Color.Black),  // Green hotkeys (P, V, H, G)
            HotFocus = new Terminal.Gui.Attribute(Color.Red, Color.Black),     // Red hotkeys when selected
            Disabled = new Terminal.Gui.Attribute(Color.Gray, Color.Black)
        };
        
        /// <summary>
        /// Input field color scheme
        /// </summary>
        public static readonly ColorScheme InputScheme = new ColorScheme
        {
            Normal = new Terminal.Gui.Attribute(Color.White, Color.Black),
            Focus = new Terminal.Gui.Attribute(Color.White, Color.Black),
            HotNormal = new Terminal.Gui.Attribute(Color.White, Color.Black),
            HotFocus = new Terminal.Gui.Attribute(Color.White, Color.Black),
            Disabled = new Terminal.Gui.Attribute(Color.Gray, Color.Black)
        };
        
        /// <summary>
        /// Game text color scheme
        /// </summary>
        public static readonly ColorScheme GameScheme = new ColorScheme
        {
            Normal = new Terminal.Gui.Attribute(Color.White, Color.Black),
            Focus = new Terminal.Gui.Attribute(Color.White, Color.Black),
            HotNormal = new Terminal.Gui.Attribute(Color.Cyan, Color.Black),
            HotFocus = new Terminal.Gui.Attribute(Color.Cyan, Color.Black),
            Disabled = new Terminal.Gui.Attribute(Color.Gray, Color.Black)
        };
        
        /// <summary>
        /// Map display color scheme
        /// </summary>
        public static readonly ColorScheme MapScheme = new ColorScheme
        {
            Normal = new Terminal.Gui.Attribute(Color.Magenta, Color.Black),
            Focus = new Terminal.Gui.Attribute(Color.Magenta, Color.Black),
            HotNormal = new Terminal.Gui.Attribute(Color.Cyan, Color.Black),
            HotFocus = new Terminal.Gui.Attribute(Color.Cyan, Color.Black),
            Disabled = new Terminal.Gui.Attribute(Color.Gray, Color.Black)
        };
        
        /// <summary>
        /// Game info box color scheme - green text on black background
        /// </summary>
        public static readonly ColorScheme GameInfoScheme = new ColorScheme
        {
            Normal = new Terminal.Gui.Attribute(Color.Green, Color.Black),
            Focus = new Terminal.Gui.Attribute(Color.Green, Color.Black),
            HotNormal = new Terminal.Gui.Attribute(Color.Green, Color.Black),
            HotFocus = new Terminal.Gui.Attribute(Color.Green, Color.Black),
            Disabled = new Terminal.Gui.Attribute(Color.Gray, Color.Black)
        };
        
        /// <summary>
        /// Player command text color scheme - cyan
        /// </summary>
        public static readonly ColorScheme PlayerCommandScheme = new ColorScheme
        {
            Normal = new Terminal.Gui.Attribute(Color.Cyan, Color.Black),
            Focus = new Terminal.Gui.Attribute(Color.Cyan, Color.Black),
            HotNormal = new Terminal.Gui.Attribute(Color.Cyan, Color.Black),
            HotFocus = new Terminal.Gui.Attribute(Color.Cyan, Color.Black),
            Disabled = new Terminal.Gui.Attribute(Color.Gray, Color.Black)
        };
        
        /// <summary>
        /// Location text color scheme - bright yellow
        /// </summary>
        public static readonly ColorScheme LocationScheme = new ColorScheme
        {
            Normal = new Terminal.Gui.Attribute(Color.BrightYellow, Color.Black),
            Focus = new Terminal.Gui.Attribute(Color.BrightYellow, Color.Black),
            HotNormal = new Terminal.Gui.Attribute(Color.BrightYellow, Color.Black),
            HotFocus = new Terminal.Gui.Attribute(Color.BrightYellow, Color.Black),
            Disabled = new Terminal.Gui.Attribute(Color.Gray, Color.Black)
        };
        
        /// <summary>
        /// Response text color scheme - red
        /// </summary>
        public static readonly ColorScheme ResponseScheme = new ColorScheme
        {
            Normal = new Terminal.Gui.Attribute(Color.Red, Color.Black),
            Focus = new Terminal.Gui.Attribute(Color.Red, Color.Black),
            HotNormal = new Terminal.Gui.Attribute(Color.Red, Color.Black),
            HotFocus = new Terminal.Gui.Attribute(Color.Red, Color.Black),
            Disabled = new Terminal.Gui.Attribute(Color.Gray, Color.Black)
        };

        /// <summary>
        /// Items display color scheme - cyan text with white highlights
        /// </summary>
        public static readonly ColorScheme ItemsScheme = new ColorScheme
        {
            Normal = new Terminal.Gui.Attribute(Color.Cyan, Color.Black),
            Focus = new Terminal.Gui.Attribute(Color.Cyan, Color.Black),
            HotNormal = new Terminal.Gui.Attribute(Color.White, Color.Black),
            HotFocus = new Terminal.Gui.Attribute(Color.White, Color.Black),
            Disabled = new Terminal.Gui.Attribute(Color.Gray, Color.Black)
        };
    }
}