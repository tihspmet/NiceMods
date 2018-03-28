using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Ensage;
using Ensage.Common.Menu;
using Ensage.SDK.Abilities.npc_dota_hero_dark_willow;
using Ensage.SDK.Menu;
using Ensage.SDK.Orbwalker.Modes;
using Ensage.SDK.Service;
using Ensage.SDK.TargetSelector;
using log4net;
using PlaySharp.Toolkit.Logging;

namespace NiceDarkWillow.Modes
{
    internal class NoiseMode : KeyPressOrbwalkingModeAsync
    {
        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ITargetSelectorManager targetSelector;
        private dark_willow_terrorize terrorize;

        public NoiseMode(IServiceContext context, MenuItem<KeyBind> keyBind)
            : base(context, keyBind)
        {
            targetSelector = context.TargetSelector;

            terrorize = Context.AbilityFactory.GetAbility<dark_willow_terrorize>();
        }

        public override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (!Owner.IsAlive)
                {
                    return;
                }

                if (terrorize.CanBeCasted)
                {
                    terrorize.UseAbility(Game.MousePosition);
                    await Task.Delay(50);
                    Owner.Stop();
                    await Task.Delay(50, cancellationToken);
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
