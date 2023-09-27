using Dalamud.Configuration;
using Dalamud.Plugin;
using System;

namespace BigDamagePlugin
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 0;

        public bool Enabled = true;
        public bool Self = true;
        public bool Party = false;
        public bool Other = false;
        public float Multiplier = 1.0f;

        [NonSerialized]
        private DalamudPluginInterface? pluginInterface;

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            this.pluginInterface = pluginInterface;
        }

        public void Save()
        {
            this.pluginInterface!.SavePluginConfig(this);
        }
    }
}
