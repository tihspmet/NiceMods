using Ensage.SDK.Service;
using Ensage.SDK.Service.Metadata;
using System.ComponentModel.Composition;

namespace NiceOverlay
{
    [ExportPlugin("NiceOverlay", StartupMode.Auto, "tihspmet", "1.0")]
    public class NiceOverlayPlugin : Plugin
    {
        private readonly IServiceContext context;
        private readonly HeroOverlay heroOverlay;

        [ImportingConstructor]
        public NiceOverlayPlugin(IServiceContext context)
        {
            this.context = context;

            heroOverlay = new HeroOverlay(context);
        }

        protected override void OnActivate()
        {
            heroOverlay.OnActivate();
        }

        protected override void OnDeactivate()
        {
            heroOverlay.OnDeactivate();
        }
    }
}
