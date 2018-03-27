using Ensage;
using SharpDX;

namespace NiceOverlay
{
    class OverlayConfig
    {
        public static Color Team = new Color(100, 255, 50, 255);
        public static Color Enemy = new Color(255, 50, 50, 255);
        public static Color Mana = new Color(80, 50, 255, 255);
        public static Color Illusion = Mana;
        public static Color NoMana = Mana;
        public static Color Cooldown = new Color(200, 0, 0, 255);

        public static string FontName = "Arial";
        public static Vector2 SkillFontSize = new Vector2(16, 0);
        public static FontFlags SkillFontFlags = FontFlags.AntiAlias | FontFlags.StrikeOut;
        public static float BarBackgroundFade = 0.20f;
    }
}
