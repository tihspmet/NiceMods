using Ensage;
using Ensage.SDK.Service;
using Ensage.SDK.Service.Metadata;
using System.ComponentModel.Composition;

namespace NiceDarkWillow
{
    [ExportPlugin("NiceDarkWillow", StartupMode.Auto, "tihspmet", "0.1", "A nice dark willow assembly.", 500, HeroId.npc_dota_hero_dark_willow)]
    internal class NiceDarkWillowPlugin : Plugin
    {
        private readonly IServiceContext context;

        private DisableMode disableMode;
        private ShadowRealmMode shadowRealmMode;
        private Settings settings;

        [ImportingConstructor]
        public NiceDarkWillowPlugin(IServiceContext context)
        {
            this.context = context;
        }

        protected override void OnActivate()
        {
            settings = new Settings();

            disableMode = new DisableMode(context, settings.DisableKey);
            shadowRealmMode = new ShadowRealmMode(context, settings.ShadowRealmKey);

            context.Orbwalker.RegisterMode(disableMode);
            context.Orbwalker.RegisterMode(shadowRealmMode);
        }

        protected override void OnDeactivate()
        {
            context.Orbwalker.UnregisterMode(disableMode);
            context.Orbwalker.UnregisterMode(shadowRealmMode);

            settings.Dispose();
        }
    }
}
