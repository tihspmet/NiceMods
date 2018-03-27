using Ensage;
using Ensage.Common;
using Ensage.SDK.Extensions;
using Ensage.SDK.Service;
using SharpDX;
using System;
using System.Linq;

namespace NiceOverlay
{
    class HeroOverlay
    {
        private string fontName = OverlayConfig.FontName;
        private Vector2 skillFontSize = OverlayConfig.SkillFontSize;
        private FontFlags skillFontFlags = OverlayConfig.SkillFontFlags;
        private float barBackgroundFade = OverlayConfig.BarBackgroundFade;

        private readonly Hero owner;

        public HeroOverlay(IServiceContext context)
        {
            owner = context.Owner as Hero;
        }

        public void OnActivate()
        {
            Drawing.OnDraw += OnDraw;
        }

        public void OnDeactivate()
        {
            Drawing.OnDraw -= OnDraw;
        }

        void OnDraw(EventArgs ea)
        {
            foreach (var hero in ObjectManager.GetEntities<Hero>())
            {
                // skip invalid heroes
                if (!hero.IsAlive || !hero.IsVisible || HUDInfo.GetHPbarPosition(hero).IsZero)
                {
                    continue;
                }

                // skip allied illusions
                if (hero.IsIllusion && hero.IsAlly(owner))
                {
                    continue;
                }

                // skip local hero
                if (hero.Equals(owner))
                {
                    continue;
                }

                var healthColor = GetHeroColor(hero);
                var healthPos = GetHpBarPosition(hero);
                var healthSize = GetHpBarSize(hero);
                var manaPos = healthPos + new Vector2(0, healthSize.Y);
                var manaSize = new Vector2(healthSize.X, healthSize.Y * 0.66f);

                DrawHealthBar(hero.Health, hero.MaximumHealth, healthPos, healthSize, healthColor);
                DrawManaBar(hero.Mana, hero.MaximumMana, manaPos, manaSize, OverlayConfig.Mana);

                if (!hero.IsIllusion)
                {
                    DrawSpells(hero, manaPos + new Vector2(0, manaSize.Y));
                }
            }
        }

        private static Vector2 GetHpBarPosition(Hero hero)
        {
            var pos = hero.Position + new Vector3(0, 0, hero.HealthBarOffset);

            if (!Drawing.WorldToScreen(pos, out var screenPos))
            {
                return Vector2.Zero;
            }

            var barOffset = new Vector2((float)-HUDInfo.HpBarX, -HUDInfo.HpBarY) * HUDInfo.Monitor;

            if (hero == ObjectManager.LocalHero)
            {
                // the magic numbers from SDK do not work for me.. so I'll write down my own
                barOffset *= new Vector2(1.07f, 1.20f);
            }

            return screenPos + barOffset - new Vector2(0, 1);
        }

        private static Vector2 GetHpBarSize(Hero hero)
        {
            return new Vector2(HUDInfo.GetHPBarSizeX(hero), HUDInfo.GetHpBarSizeY(hero));
        }

        private Color GetHeroColor(Hero hero)
        {
            if (!hero.IsEnemy(owner))
            {
                return OverlayConfig.Team;
            }
            else if (hero.IsIllusion)
            {
                return OverlayConfig.Illusion;
            }
            else
            {
                return OverlayConfig.Enemy;
            }
        }

        private void DrawManaBar(float mana, float maxMana, Vector2 pos, Vector2 size, Color color)
        {
            Drawing.DrawRect(pos, size, Color.Lerp(Color.Black, color, barBackgroundFade));

            var fillPos = pos + new Vector2(1, 1);
            var fillSize = size - 2 * new Vector2(1, 1);

            Drawing.DrawRect(fillPos, new Vector2(fillSize.X * mana / maxMana, fillSize.Y), color);
        }

        private void DrawHealthBar(float health, float maxHealth, Vector2 pos, Vector2 size, Color color)
        {
            Drawing.DrawRect(pos, size, Color.Lerp(Color.Black, color, barBackgroundFade));

            var fillPos = pos + new Vector2(1, 1);
            var fillSize = size - 2 * new Vector2(1, 1);

            Drawing.DrawRect(fillPos, new Vector2(fillSize.X * health / maxHealth, fillSize.Y), color);

            // draw ticks
            var tickWidth = fillSize.X / maxHealth * 250;
            for (int i = 1; i < health / 250; i++)
            {
                var start = new Vector2(fillPos.X + i * tickWidth, fillPos.Y);
                var end = new Vector2(start.X, start.Y + fillSize.Y);

                if (i % 4 == 0)
                {
                    // thick
                    Drawing.DrawLine(start, end, new Color(0, 0, 0, 255));
                    Drawing.DrawLine(start - new Vector2(1, 0), end - new Vector2(1, 0), new Color(0, 0, 0, 180));
                    Drawing.DrawLine(start + new Vector2(1, 0), end + new Vector2(1, 0), new Color(0, 0, 0, 180));
                }
                else
                {
                    // thin
                    Drawing.DrawLine(start, end, new Color(0, 0, 0, 180));
                }
            }
        }

        private void DrawSpells(Hero hero, Vector2 pos)
        {
            var spells = hero.Spellbook.Spells.Where(x =>
                x.IsValid && !x.IsHidden && x.Name != "generic_hidden"
                && (x.AbilityType == AbilityType.Basic || x.AbilityType == AbilityType.Ultimate));

            var padding = new Vector2(2, 0);
            var offset = new Vector2(0, 0);

            foreach (var spell in spells)
            {
                var text = String.Format("{0}", spell.Level);
                var textSize = Drawing.MeasureText(text, fontName, skillFontSize, skillFontFlags);
                var boxSize = new Vector2(textSize.Y, textSize.Y);

                var spellColor = (spell.Cooldown > 0) ? OverlayConfig.Cooldown : (hero.Mana < spell.ManaCost) ? OverlayConfig.NoMana : Color.Black;
                
                Drawing.DrawRect(pos + offset, boxSize, spellColor);
                Drawing.DrawRect(pos + offset, boxSize, Color.Black, true);
                Drawing.DrawText(text, fontName, pos + offset + new Vector2(boxSize.X/2 - textSize.X/2, 0), skillFontSize, Color.White, skillFontFlags);

                offset.X += boxSize.X;
            }
        }
    }
}
