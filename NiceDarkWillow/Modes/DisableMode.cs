using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Ensage;
using Ensage.Common.Menu;
using Ensage.SDK.Abilities.Items;
using Ensage.SDK.Abilities.npc_dota_hero_dark_willow;
using Ensage.SDK.Extensions;
using Ensage.SDK.Inventory.Metadata;
using Ensage.SDK.Menu;
using Ensage.SDK.Orbwalker.Modes;
using Ensage.SDK.Service;
using Ensage.SDK.TargetSelector;
using log4net;
using PlaySharp.Toolkit.Logging;


namespace NiceDarkWillow.Modes
{
    internal class DisableMode : KeyPressOrbwalkingModeAsync
    {
        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static string SheepstickModifier = "modifier_sheepstick_debuff";
        private static string EulsModifier = "modifier_eul_cyclone";

        private readonly ITargetSelectorManager targetSelector;
        private readonly dark_willow_cursed_crown cursedCrown;

        [ItemBinding]
        private item_cyclone Euls { get; set; }
        [ItemBinding]
        private item_sheepstick Sheepstick { get; set; }
        [ItemBinding]
        private item_orchid Orchid { get; set; }
        [ItemBinding]
        private item_bloodthorn Bloodthorn { get; set; }
        [ItemBinding]
        private item_nullifier Nullifier { get; set; }

        public DisableMode(IServiceContext context, MenuItem<KeyBind> keyBind)
            : base(context, keyBind)
        {
            targetSelector = context.TargetSelector;

            cursedCrown = Context.AbilityFactory.GetAbility<dark_willow_cursed_crown>();
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            Context.Inventory.Attach(this);
        }

        protected override void OnDeactivate()
        {
            Context.Inventory.Detach(this);

            base.OnDeactivate();
        }

        public override async Task ExecuteAsync(CancellationToken token)
        {
            try
            {
                if (!Owner.IsAlive)
                {
                    return;
                }

                var target = targetSelector.Active.GetTargets().FirstOrDefault(x => Owner.Distance2D(x) < 1200);
                if (target == null || target.IsInvulnerable() || target.IsMagicImmune())
                {
                    Orbwalker.OrbwalkTo(null);
                    return;
                }

                if (cursedCrown.CanBeCasted
                    && cursedCrown.CanHit(target))
                {
                    cursedCrown.UseAbility(target);
                    await Task.Delay(cursedCrown.GetCastDelay(target) + GetLagDelay(), token);
                }

                if (Sheepstick?.CanBeCasted == true
                    && Sheepstick.CanHit(target)
                    && !target.IsHexed())
                {
                    Sheepstick.UseAbility(target);
                    await Task.Delay(Sheepstick.GetCastDelay(target) + GetLagDelay(), token);
                }

                if (Euls?.CanBeCasted == true
                    && Euls.CanHit(target)
                    && !target.IsSilenced()
                    && !target.IsMuted()
                    && !target.IsStunned()
                    && !target.IsRooted()
                    // only on cursed crown with at least 100ms time after euls
                    && (target.HasModifier(cursedCrown.TargetModifierName)
                        && target.GetModifierByName(cursedCrown.TargetModifierName).RemainingTime * 1000 > 2600 + Euls.GetCastDelay(target)))
                {
                    Euls.UseAbility(target);
                    await Task.Delay(Euls.GetCastDelay(target) + GetLagDelay(), token);
                }

                if (Bloodthorn?.CanBeCasted == true
                    && Bloodthorn.CanHit(target)
                    && !target.IsSilenced())
                {
                    Bloodthorn.UseAbility(target);
                    await Task.Delay(Bloodthorn.GetCastDelay(target) + GetLagDelay(), token);
                }

                if (Orchid?.CanBeCasted == true
                    && Orchid.CanHit(target)
                    && !target.IsSilenced())
                {
                    Orchid.UseAbility(target);
                    await Task.Delay(Orchid.GetCastDelay(target) + GetLagDelay(), token);
                }

                if (Nullifier?.CanBeCasted == true
                    && Nullifier.CanHit(target)
                    && !target.HasModifier(EulsModifier) 
                    // stack nullifier and sheepstick 100ms
                    && (!target.HasModifier(SheepstickModifier)
                        || target.GetModifierByName(SheepstickModifier).RemainingTime * 1000 < 100 + Nullifier.GetHitTime(target)))
                {
                    Nullifier.UseAbility(target);
                    await Task.Delay(Nullifier.GetCastDelay(target) + GetLagDelay(), token);
                }

                Orbwalker.OrbwalkTo(null);
            }
            catch (TaskCanceledException)
            {

            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        private int GetLagDelay()
        {
            return (int) Math.Max(50 - Game.Ping, 0);
        }
    }
}
