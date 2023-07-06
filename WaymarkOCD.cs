using Dalamud.Data;
using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
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
        [PluginService][RequiredVersion("1.0")] public static Condition condition { get; private set; } = null!;
        [PluginService][RequiredVersion("1.0")] public static SigScanner sigScanner { get; private set; } = null!;
        [PluginService][RequiredVersion("1.0")] public static DataManager dataManager { get; private set; } = null!;

        public WaymarkOCD()
        {
            MemoryHandler.Init();
            ZoneInfoHandler.Init();

            commandManager.AddHandler("/setwaymark", new(OnSetWaymarkCommand)
            {
                HelpMessage = "Sets a waymark's coordinates.\n"
                                + "/setwaymark a 0 10000\n"
                                + "/setwaymark 1 10000 10000\n ",

                ShowInHelp = true
            });
            commandManager.AddHandler("/getwaymark", new(OnGetWaymarkCommand)
            {
                HelpMessage = "Gets a waymark's coordinates.\n"
                                + "/getwaymark a\n"
                                + "/getwaymark 1\n ",

                ShowInHelp = true
            });
            commandManager.AddHandler("/setwaymarkpolar", new(OnSetWaymarkCommandPolar)
            {
                HelpMessage = "Sets a waymark's coordinates using a polar coordinate system.\n"
                                + "/setwaymarkpolar a 0 10000\n"
                                + "/setwaymarkpolar a 45 10000\n"
                                + "NOTE: You may need to place a waymark manually before using the /setwaymarkpolar command.\n ",

                ShowInHelp = true
            });
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
                chatGui.Print("Usage: /setwaymarkpolar a degrees distance");
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

            GamePreset? preset = MemoryHandler.GetCurrentWaymarksAsPresetData();
            if (preset == null)
            {
                chatGui.Print("No waymarks detected");
                return;
            }
            GamePreset realPreset = preset.Value;

            var (x, y) = PolarCoordinatesHelper.GetXYCoordinatesFromPolar(distance, degrees);

            if (Is100kArena(realPreset))
            {
                x += 100000;
                y += 100000;
            }

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

        // Pre-Stormblood, every arena was centered on 0,0. But halfway through Stormblood, they started using 100000,100000 as the center of the arena. 
        // This function checks to see if we are in a 100k,100k arena or not
        // We can only determine this by checking existing waymarks (the player coordinate system is completely different than the waymark coordinate system)
        private static bool Is100kArena(GamePreset preset)
        {
            return IsWaymarkFromA100kArena(preset.A)
                || IsWaymarkFromA100kArena(preset.B)
                || IsWaymarkFromA100kArena(preset.C)
                || IsWaymarkFromA100kArena(preset.D)
                || IsWaymarkFromA100kArena(preset.One)
                || IsWaymarkFromA100kArena(preset.Two)
                || IsWaymarkFromA100kArena(preset.Three)
                || IsWaymarkFromA100kArena(preset.Four);
        }

        private static bool IsWaymarkFromA100kArena(GamePresetPoint waymark)
        {
            return waymark.X > 80000 || waymark.Z > 80000;
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
