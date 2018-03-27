using System;
using Ensage.Common.Menu;
using Ensage.SDK.Menu;

namespace NiceDarkWillow
{
    internal class Settings : IDisposable
    {
        private readonly MenuFactory factory;

        public MenuItem<KeyBind> DisableKey { get; }
        public MenuItem<KeyBind> ShadowRealmKey { get; }
        public MenuItem<KeyBind> NoiseKey { get; }

        public Settings()
        {
            factory = MenuFactory.Create("Dark Willow");
            DisableKey = factory.Item("Disable Combo", new KeyBind('L'));
            ShadowRealmKey = factory.Item("Shadow Realm", new KeyBind('L'));
            NoiseKey = factory.Item("Noise", new KeyBind('L'));
        }

        public void Dispose()
        {
            factory.Dispose();
        }
    }
}
