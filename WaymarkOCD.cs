using Dalamud.Data;
using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.JobGauge;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.ClientState.Party;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.Game.Network;
using Dalamud.IoC;
using Dalamud.Plugin;
using System;

namespace WaymarkOCDPlugin
{
    public class WaymarkOCD : IDalamudPlugin
    {
        public string Name => "Waymark OCD Plugin";

        [PluginService][RequiredVersion("1.0")] public static CommandManager commandManager { get; private set; } = null!;
        [PluginService][RequiredVersion("1.0")] public static ChatGui chatGui { get; private set; } = null!;
        [PluginService][RequiredVersion("1.0")] public static ClientState clientState { get; private set; } = null!;
        [PluginService][RequiredVersion("1.0")] public static JobGauges jobGauges { get; private set; } = null!;
        [PluginService][RequiredVersion("1.0")] public static Condition condition { get; private set; } = null!;
        [PluginService][RequiredVersion("1.0")] public static SigScanner sigScanner { get; private set; } = null!;
        [PluginService][RequiredVersion("1.0")] public static PartyList partyList { get; private set; } = null!;
        [PluginService][RequiredVersion("1.0")] public static TargetManager targetManager { get; private set; } = null!;
        [PluginService][RequiredVersion("1.0")] public static ObjectTable objectTable { get; private set; } = null!;
        [PluginService][RequiredVersion("1.0")] public static DalamudPluginInterface pluginInterface { get; private set; } = null!;
        [PluginService][RequiredVersion("1.0")] public static GameNetwork gameNetwork { get; private set; } = null!;
        [PluginService][RequiredVersion("1.0")] public static DataManager dataManager { get; private set; } = null!;

        public WaymarkOCD()
        {
            MemoryHandler.Init();
            ZoneInfoHandler.Init();

            commandManager.AddHandler("/alignx", new(OnAlignXCommand));
            commandManager.AddHandler("/aligny", new(OnAlignYCommand));
        }

        private unsafe void OnAlignXCommand(string command, string args)
        {
            string[] arguments = args.Split(" ");
            if (arguments.Length != 2)
            {
                chatGui.Print("Usage: /alignx a 1");
                chatGui.Print("In this example, the A marker will be aligned to the 1 marker along the X axis");
                return;
            }

            string markerToBeAligned = arguments[0];
            string markerToAlignTo = arguments[1];

            GamePreset? preset = MemoryHandler.GetCurrentWaymarksAsPresetData();

            if (preset == null)
            {
                chatGui.Print("No waymarks detected");
                return;
            }

            var (pointToBeAligned, pointToAlignTo) = GetMarkers(markerToBeAligned, markerToAlignTo);
            if (pointToBeAligned == null)
            {
                chatGui.Print("Invalid waymark name: " + pointToBeAligned);
                chatGui.Print("Valid waymark names: a, b, c, d, 1, 2, 3, 4");
                return;
            }
            if (pointToAlignTo == null)
            {
                chatGui.Print("Invalid waymark name: " + markerToBeAligned);
                chatGui.Print("Valid waymark names: a, b, c, d, 1, 2, 3, 4");
                return;
            }

            GamePreset realPreset = preset.Value;
            realPreset.A.X = pointToAlignTo.Value.X;

            MemoryHandler.DirectPlacePreset(realPreset);
        }

        private static (GamePresetPoint?, GamePresetPoint?) GetMarkers(String marker1, String marker2)
        {
            return (GetMarker(marker1), GetMarker(marker2));
        }

        private static GamePresetPoint? GetMarker(String marker)
        {
            GamePreset? preset = MemoryHandler.GetCurrentWaymarksAsPresetData();
            if (preset == null)
            {
                return null;
            }

            GamePreset gamePreset = preset.Value;

            return marker switch
            {
                "a" or "A" => gamePreset.A,
                "b" or "B" => gamePreset.B,
                "c" or "C" => gamePreset.C,
                "d" or "D" => gamePreset.D,
                "1" => gamePreset.One,
                "2" => gamePreset.Two,
                "3" => gamePreset.Three,
                "4" => gamePreset.Four,
                _ => null,
            };
        }

        private unsafe void OnAlignYCommand(string command, string args)
        {
        }

        //	Cleanup
        public void Dispose()
        {
            commandManager.RemoveHandler("/waymark");
            MemoryHandler.Uninit();
        }
    }
}
