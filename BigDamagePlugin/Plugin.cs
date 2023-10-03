using BigDamagePlugin.Windows;
using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using ECommons;

namespace BigDamagePlugin
{
    public sealed class Plugin : IDalamudPlugin
    {
        internal static Plugin? p;
        public string Name => "BigDamage Plugin";
        private const string CommandName = "/big";
        private DalamudPluginInterface PluginInterface { get; init; }
        private ICommandManager CommandManager { get; init; }
        public Configuration Configuration { get; init; }
        public WindowSystem WindowSystem = new("BigDamagePlugin");
        private MainWindow MainWindow { get; init; }
        internal static FlyTextHandler FlyTextHandler { get; set; } = null!;
        internal static Configuration c => p.Configuration;
        public Plugin(
            [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
            [RequiredVersion("1.0")] ICommandManager commandManager)
        {
            this.PluginInterface = pluginInterface;
            this.CommandManager = commandManager;

            this.Configuration = this.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            this.Configuration.Initialize(this.PluginInterface);
            p = this;
            MainWindow = new MainWindow();
            WindowSystem.AddWindow(MainWindow);
            ECommonsMain.Init(pluginInterface, this);
            FlyTextHandler = new FlyTextHandler();
            FlyTextHandler.SwitchHook();
            this.CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
            {
                HelpMessage = "Show Main UI"
            });

            this.PluginInterface.UiBuilder.Draw += DrawUI;
            this.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
        }

        public void Dispose()
        {
            FlyTextHandler.Dispose();
            this.WindowSystem.RemoveAllWindows();
            ECommonsMain.Dispose();
            MainWindow.Dispose();
            
            this.CommandManager.RemoveHandler(CommandName);
        }

        private void OnCommand(string command, string args)
        {
            MainWindow.Toggle();
        }

        private void DrawUI()
        {
            this.WindowSystem.Draw();
        }

        public void DrawConfigUI()
        {
            MainWindow.Toggle();
        }
    }
}
