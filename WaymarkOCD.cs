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

            commandManager.AddHandler("/setwaymark", new(OnSetWaymarkCommand));
            commandManager.AddHandler("/getwaymark", new(OnGetWaymarkCommand));
            commandManager.AddHandler("/setwaymarkpolar", new(OnSetWaymarkCommandPolar));
        }

        private static GamePresetPoint? GetMarker(GamePreset gamePreset, String marker)
        {
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

        private unsafe void OnSetWaymarkCommand(string command, string args)
        {
            string[] arguments = args.Split(" ");
            if (arguments.Length != 3)
            {
                chatGui.Print("Usage: /setwaymark a xCoordinate yCoordinate");
                chatGui.Print("Valid waymark names: a, b, c, d, 1, 2, 3, 4");
                return;
            }

            string marker = arguments[0];
            string xString = arguments[1];
            string yString = arguments[2];

            if (!int.TryParse(xString, out int x))
            {
                chatGui.Print("x coordinate was not a number: " + xString);
                return;
            }

            if (!int.TryParse(yString, out int y))
            {
                chatGui.Print("y coordinate was not a number: " + yString);
                return;
            }

            GamePreset? preset = MemoryHandler.GetCurrentWaymarksAsPresetData();

            if (preset == null)
            {
                chatGui.Print("No waymarks detected");
                return;
            }

            GamePreset realPreset = preset.Value;

            switch (marker)
            {
                case "a":
                case "A":
                    realPreset.A.X = x;
                    realPreset.A.Z = y;
                    realPreset.A.Active = true;
                    break;
                case "b":
                case "B":
                    realPreset.B.X = x;
                    realPreset.B.Z = y;
                    realPreset.B.Active = true;
                    break;
                case "c":
                case "C":
                    realPreset.C.X = x;
                    realPreset.C.Z = y;
                    realPreset.C.Active = true;
                    break;
                case "d":
                case "D":
                    realPreset.D.X = x;
                    realPreset.D.Z = y;
                    realPreset.D.Active = true;
                    break;
                case "1":
                    realPreset.One.X = x;
                    realPreset.One.Z = y;
                    realPreset.One.Active = true;
                    break;
                case "2":
                    realPreset.Two.X = x;
                    realPreset.Two.Z = y;
                    realPreset.Two.Active = true;
                    break;
                case "3":
                    realPreset.Three.X = x;
                    realPreset.Three.Z = y;
                    realPreset.Three.Active = true;
                    break;
                case "4":
                    realPreset.Four.X = x;
                    realPreset.Four.Z = y;
                    realPreset.Four.Active = true;
                    break;
                default:
                    chatGui.Print("Invalid waymark name: " + marker);
                    chatGui.Print("Valid waymark names: a, b, c, d, 1, 2, 3, 4");
                    break;
            }

            MemoryHandler.DirectPlacePreset(realPreset);
        }

        private unsafe void OnGetWaymarkCommand(string command, string args)
        {
            GamePreset? preset = MemoryHandler.GetCurrentWaymarksAsPresetData();

            if (preset == null)
            {
                chatGui.Print("No waymarks detected");
                return;
            }

            GamePresetPoint? point = GetMarker(preset.Value, args);
            if (point == null)
            {
                chatGui.Print("Invalid waymark name: " + point);
                chatGui.Print("Valid waymark names: a, b, c, d, 1, 2, 3, 4");
                return;
            }

            chatGui.Print("Marker " + args + ": " + point.Value);
        }

        private unsafe void OnSetWaymarkCommandPolar(string command, string args)
        {
            string[] arguments = args.Split(" ");
            if (arguments.Length != 3)
            {
                chatGui.Print("Usage: /setwaymark a degrees distance");
                chatGui.Print("Valid waymark names: a, b, c, d, 1, 2, 3, 4");
                return;
            }

            string marker = arguments[0];
            string degreesString = arguments[1];
            string distanceString = arguments[2];

            if (!int.TryParse(degreesString, out int degrees))
            {
                chatGui.Print("degrees value was not a number: " + degrees);
                return;
            }

            if (!int.TryParse(distanceString, out int distance))
            {
                chatGui.Print("distance was not a number: " + distance);
                return;
            }

            var (x, y) = PolarCoordinatesHelper.GetXYCoordinatesFromPolar(distance, degrees);

            GamePreset? preset = MemoryHandler.GetCurrentWaymarksAsPresetData();

            if (preset == null)
            {
                chatGui.Print("No waymarks detected");
                return;
            }

            GamePreset realPreset = preset.Value;

            switch (marker)
            {
                case "a":
                case "A":
                    realPreset.A.X = x;
                    realPreset.A.Z = y;
                    realPreset.A.Active = true;
                    break;
                case "b":
                case "B":
                    realPreset.B.X = x;
                    realPreset.B.Z = y;
                    realPreset.B.Active = true;
                    break;
                case "c":
                case "C":
                    realPreset.C.X = x;
                    realPreset.C.Z = y;
                    realPreset.C.Active = true;
                    break;
                case "d":
                case "D":
                    realPreset.D.X = x;
                    realPreset.D.Z = y;
                    realPreset.D.Active = true;
                    break;
                case "1":
                    realPreset.One.X = x;
                    realPreset.One.Z = y;
                    realPreset.One.Active = true;
                    break;
                case "2":
                    realPreset.Two.X = x;
                    realPreset.Two.Z = y;
                    realPreset.Two.Active = true;
                    break;
                case "3":
                    realPreset.Three.X = x;
                    realPreset.Three.Z = y;
                    realPreset.Three.Active = true;
                    break;
                case "4":
                    realPreset.Four.X = x;
                    realPreset.Four.Z = y;
                    realPreset.Four.Active = true;
                    break;
                default:
                    chatGui.Print("Invalid waymark name: " + marker);
                    chatGui.Print("Valid waymark names: a, b, c, d, 1, 2, 3, 4");
                    break;
            }

            MemoryHandler.DirectPlacePreset(realPreset);
        }

        //	Cleanup
        public void Dispose()
        {
            commandManager.RemoveHandler("/setwaymark");
            commandManager.RemoveHandler("/setwaymarkpolar");
            commandManager.RemoveHandler("/getwaymark");
            MemoryHandler.Uninit();

            GC.SuppressFinalize(this);
        }
    }
}
