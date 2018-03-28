using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Ensage.Common.Extensions;
using Ensage.Common.Menu;
using Ensage.SDK.Abilities.npc_dota_hero_dark_willow;
using Ensage.SDK.Extensions;
using Ensage.SDK.Menu;
using Ensage.SDK.Orbwalker.Modes;
using Ensage.SDK.Service;
using Ensage.SDK.TargetSelector;
using log4net;
using PlaySharp.Toolkit.Logging;

namespace NiceDarkWillow.Modes
{
    internal class ShadowRealmMode : KeyPressOrbwalkingModeAsync
    {
        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ITargetSelectorManager targetSelector;
        private dark_willow_shadow_realm shadowRealm;

        public ShadowRealmMode(IServiceContext context, MenuItem<KeyBind> keyBind)
            : base(context, keyBind)
        {
            targetSelector = context.TargetSelector;

            shadowRealm = Context.AbilityFactory.GetAbility<dark_willow_shadow_realm>();
        }

        public override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (!Owner.IsAlive)
                {
                    return;
                }

                var snipeRange = Owner.GetAttackRange() + 600;

                var target = targetSelector.Active.GetTargets().FirstOrDefault(x => x.Distance2D(Owner) < snipeRange);
                var modifier = Owner.Modifiers.FirstOrDefault(x => x.Name == shadowRealm.ModifierName);

                if (modifier == null && shadowRealm.CanBeCasted)
                {
                    shadowRealm.UseAbility();
                    await Task.Delay(shadowRealm.GetCastDelay(), cancellationToken);
                }

                if (target != null
                    && modifier != null
                    && Owner.CanAttack(target)
                    && modifier.ElapsedTime + Owner.GetTurnTime(target) > 3.0f)
                {
                    Owner.Attack(target);
                    await Task.Delay(100, cancellationToken);
                }
                else
                {
                    Orbwalker.OrbwalkTo(null);
                }
            }
            catch (TaskCanceledException)
            {

            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}
