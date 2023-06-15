using System;
using System.Runtime.InteropServices;

using Dalamud.Game.ClientState.Conditions;
using Dalamud.Logging;

namespace WaymarkOCDPlugin
{
    public static class MemoryHandler
    {
        private static IntPtr mpWaymarksObj;

        private delegate IntPtr GetConfigSectionDelegate(IntPtr pConfigFile, byte sectionIndex);
        private delegate IntPtr GetPresetAddressForSlotDelegate(IntPtr pMarkerDataStart, uint slotNum);
        private delegate byte GetCurrentContentFinderLinkTypeDelegate();
        private delegate void DirectPlacePresetDelegate(IntPtr pObj, IntPtr pData);
        private delegate void GetCurrentWaymarkDataDelegate(IntPtr pObj, IntPtr pData);

        private static GetCurrentContentFinderLinkTypeDelegate? mdGetCurrentContentFinderLinkType;
        private static DirectPlacePresetDelegate? mdDirectPlacePreset;
        private static GetCurrentWaymarkDataDelegate? mdGetCurrentWaymarkData;

        public static void Init()
        {
            try
            {
                IntPtr fpGetCurrentContentFinderLinkType = WaymarkOCD.sigScanner.ScanText("48 83 ?? ?? 48 8B ?? ?? ?? ?? ?? 48 85 ?? 0F ?? ?? ?? ?? ?? ?? B8 ?? ?? ?? ?? ?? 0F ?? ?? ?? ?? ?? ?? 89");
                if (fpGetCurrentContentFinderLinkType == IntPtr.Zero)
                {
                    WaymarkOCD.chatGui.Print("Failed to get fpGetCurrentContentFinderLinkType");
                    return;
                }
                mdGetCurrentContentFinderLinkType = Marshal.GetDelegateForFunctionPointer<GetCurrentContentFinderLinkTypeDelegate>(fpGetCurrentContentFinderLinkType);
                if (mdGetCurrentContentFinderLinkType == null)
                {
                    WaymarkOCD.chatGui.Print("Failed to get mdGetCurrentContentFinderLinkType");
                    return;
                }

                IntPtr fpDirectPlacePreset = WaymarkOCD.sigScanner.ScanText("E8 ?? ?? ?? ?? 84 C0 0F 94 C0 EB 19");
                if (fpDirectPlacePreset == IntPtr.Zero)
                {
                    WaymarkOCD.chatGui.Print("Failed to get fpDirectPlacePreset");
                    return;
                }
                mdDirectPlacePreset = Marshal.GetDelegateForFunctionPointer<DirectPlacePresetDelegate>(fpDirectPlacePreset);
                if (mdDirectPlacePreset == null)
                {
                    WaymarkOCD.chatGui.Print("Failed to get mdDirectPlacePreset");
                    return;
                }

                IntPtr fpGetCurrentWaymarkData = WaymarkOCD.sigScanner.ScanText("48 89 ?? ?? ?? 57 48 83 ?? ?? 48 8B ?? 48 8B ?? 33 D2 48 8B");
                if (fpGetCurrentWaymarkData == IntPtr.Zero)
                {
                    WaymarkOCD.chatGui.Print("Failed to get fpGetCurrentWaymarkData");
                    return;
                }
                mdGetCurrentWaymarkData = Marshal.GetDelegateForFunctionPointer<GetCurrentWaymarkDataDelegate>(fpGetCurrentWaymarkData);
                if (mdGetCurrentWaymarkData == null)
                {
                    WaymarkOCD.chatGui.Print("Failed to get mdGetCurrentWaymarkData");
                    return;
                }

                mpWaymarksObj = WaymarkOCD.sigScanner.GetStaticAddressFromSig("41 80 F9 08 7C BB 48 8D ?? ?? ?? 48 8D ?? ?? ?? ?? ?? E8 ?? ?? ?? ?? 84 C0 0F 94 C0 EB 19", 11);
                if (mpWaymarksObj == IntPtr.Zero)
                {
                    WaymarkOCD.chatGui.Print("Failed to get mpWaymarksObj");
                    return;
                }
            }
            catch (Exception e)
            {
                WaymarkOCD.chatGui.Print($"Error in \"MemoryHandler.Init()\" while searching for \"optional\" function signatures;  this probably means that the plugin needs to be updated due to changes in FFXIV.  Raw exception as follows:\r\n{e}");
            }
        }

        public static void Uninit()
        {
            mpWaymarksObj = IntPtr.Zero;
            mdGetCurrentContentFinderLinkType = null;
            mdDirectPlacePreset = null;
            mdGetCurrentWaymarkData = null;
        }

        private static bool IsSafeToDirectPlacePreset()
        {
            //	Basically impose all of the same conditions that the game does, but without checking the preset's zone ID.
            byte currentContentLinkType = mdGetCurrentContentFinderLinkType!.Invoke();
            return WaymarkOCD.condition != null &&
                    WaymarkOCD.clientState != null &&
                    WaymarkOCD.clientState.LocalPlayer != null &&
                    !WaymarkOCD.condition[ConditionFlag.InCombat] &&
                    currentContentLinkType > 0 && currentContentLinkType < 4;
        }

        public static void DirectPlacePreset(GamePreset preset)
        {
            if (!IsSafeToDirectPlacePreset())
            {
                WaymarkOCD.chatGui.Print("Unable to directly place preset");
                return;
            }

            GamePreset_Placement placementStruct = new(preset);
            PluginLog.LogDebug($"Attempting to place waymark preset with data:\r\n{placementStruct}");
            unsafe
            {
                mdDirectPlacePreset!.Invoke(mpWaymarksObj, new IntPtr(&placementStruct));
            }
        }

        public static GamePreset? GetCurrentWaymarksAsPresetData()
        {
            byte currentContentLinkType = mdGetCurrentContentFinderLinkType!.Invoke();
            if (currentContentLinkType >= 0 && currentContentLinkType < 4)  //	Same as the game check, but let it do overworld maps too.
            {
                GamePreset_Placement rawWaymarkData = new();
                unsafe
                {
                    mdGetCurrentWaymarkData!.Invoke(mpWaymarksObj, new IntPtr(&rawWaymarkData));
                }

                GamePreset rPresetData = new(rawWaymarkData)
                {
                    ContentFinderConditionID = ZoneInfoHandler.GetContentFinderIDFromTerritoryTypeID(WaymarkOCD.clientState.TerritoryType),   //*****TODO: How do we get this as a territory type for non-instanced zones? The return type might need to be changed, or pass in another ref paramter or something. *****
                    UnixTime = (Int32)DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                };

                PluginLog.Debug($"Obtained current waymarks with the following data:\r\n" +
                                    $"Territory: {WaymarkOCD.clientState.TerritoryType}\r\n" +
                                    $"ContentFinderCondition: {rPresetData.ContentFinderConditionID}\r\n" +
                                    $"Waymark Struct:\r\n{rawWaymarkData}");
                return rPresetData;
            }
            else
            {
                WaymarkOCD.chatGui.Print($"Error in MemoryHandler.GetCurrentWaymarksAsPresetData: Disallowed ContentLinkType: {currentContentLinkType}");
                return null;
            }
        }
    }
}
