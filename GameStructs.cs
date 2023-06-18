﻿using System;
using System.Runtime.InteropServices;

namespace WaymarkOCDPlugin
{
    //	Ideally we'd be setting up these structures as a pseudo-union with both named fields and as arrays, but marshalling for
    //	arrays is absolutely fucked, and you can't really do indexers for a lot of the nested structures here, so it seems that
    //	we have to just do almost everything as only an explicit field I guess.

    //	The representation of each actual waymark in-memory.  The floats are what is used to render the marker, and adjusting
    //	them or the flag immediately changes the waymark.  The parallel int (x1000) values are used for saving a preset and
    //	probably also for transmission to server when placing a waymark or preset.
    [StructLayout(LayoutKind.Explicit, Size = 0x20)]
    public struct GameWaymark
    {
        [FieldOffset(0x00)] public float X_Float;
        [FieldOffset(0x04)] public float Y_Float;
        [FieldOffset(0x08)] public float Z_Float;
        //	Four bytes of padding here.

        [FieldOffset(0x10)] public int X_Int;
        [FieldOffset(0x14)] public int Y_Int;
        [FieldOffset(0x18)] public int Z_Int;

        [MarshalAs(UnmanagedType.Bool)]
        [FieldOffset(0x1C)] public bool Active;

        public GameWaymark(GamePresetPoint presetPoint, bool ignoreInts = false)
        {
            X_Float = (float)presetPoint.X / 1000.0f;
            Y_Float = (float)presetPoint.Y / 1000.0f;
            Z_Float = (float)presetPoint.Z / 1000.0f;

            X_Int = ignoreInts ? 0 : presetPoint.X;
            Y_Int = ignoreInts ? 0 : presetPoint.Y;
            Z_Int = ignoreInts ? 0 : presetPoint.Z;

            Active = presetPoint.Active;
        }

        public override string ToString()
        {
            return $" {Active} | {X_Float}, {Y_Float}, {Z_Float} | {X_Int}, {Y_Int}, {Z_Int}";
        }
    }

    //	The representation of all eight waymarks in memory.  Just a sequential array of the above struct.  This is only
    //	a subset of the entire Field Markers object (See WaymarkInfo.txt for more information on object layout and behavior).
    [StructLayout(LayoutKind.Sequential, Pack = 0, Size = 0x100)]
    public struct GameWaymarks
    {
        public GameWaymark A;
        public GameWaymark B;
        public GameWaymark C;
        public GameWaymark D;
        public GameWaymark One;
        public GameWaymark Two;
        public GameWaymark Three;
        public GameWaymark Four;

        public GameWaymarks(GamePreset preset)
        {
            A = new GameWaymark(preset.A);
            B = new GameWaymark(preset.B);
            C = new GameWaymark(preset.C);
            D = new GameWaymark(preset.D);
            One = new GameWaymark(preset.One);
            Two = new GameWaymark(preset.Two);
            Three = new GameWaymark(preset.Three);
            Four = new GameWaymark(preset.Four);
        }

        public override string ToString()
        {
            return $"A: {A}\r\n" +
                    $"B: {B}\r\n" +
                    $"C: {C}\r\n" +
                    $"D: {D}\r\n" +
                    $"1: {One}\r\n" +
                    $"2: {Two}\r\n" +
                    $"3: {Three}\r\n" +
                    $"4: {Four}";
        }
    }

    //	Helper structure used to manage individual waymark coordinates as they appear in a preset stored by the game.
    [StructLayout(LayoutKind.Sequential, Pack = 0, Size = 12)]
    public struct GamePresetPoint
    {
        public int X;
        public int Y;
        public int Z;
        public bool Active;

        public GamePresetPoint(GameWaymark waymark, bool useClientVals = true)
        {
            if (useClientVals)
            {
                X = (int)(waymark.X_Float * 1000.0f);
                Y = (int)(waymark.Y_Float * 1000.0f);
                Z = (int)(waymark.Z_Float * 1000.0f);
                Active = waymark.Active;
            }
            else
            {
                X = waymark.X_Int;
                Y = waymark.Y_Int;
                Z = waymark.Z_Int;
                Active = waymark.Active;
            }
        }

        public override string ToString()
        {
            if (!Active)
            {
                return "Not active";
            }
            return $"{X}, {Z}";
        }
    }

    //	The representation of a waymark preset in the game memory and on-disk.
    [StructLayout(LayoutKind.Sequential, Pack = 0, Size = 104)]
    public struct GamePreset
    {
        public GamePresetPoint A;
        public GamePresetPoint B;
        public GamePresetPoint C;
        public GamePresetPoint D;
        public GamePresetPoint One;
        public GamePresetPoint Two;
        public GamePresetPoint Three;
        public GamePresetPoint Four;
        private readonly byte _reserved;
        public UInt16 ContentFinderConditionID;
        public Int32 UnixTime;

        public GamePreset(GameWaymarks waymarks)
        {
            A = new GamePresetPoint(waymarks.A);
            B = new GamePresetPoint(waymarks.B);
            C = new GamePresetPoint(waymarks.C);
            D = new GamePresetPoint(waymarks.D);
            One = new GamePresetPoint(waymarks.One);
            Two = new GamePresetPoint(waymarks.Two);
            Three = new GamePresetPoint(waymarks.Three);
            Four = new GamePresetPoint(waymarks.Four);

            _reserved = 0;
            ContentFinderConditionID = 0;
            UnixTime = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }
        public GamePreset(GamePreset_Placement placementPreset)
        {
            A = new GamePresetPoint();
            B = new GamePresetPoint();
            C = new GamePresetPoint();
            D = new GamePresetPoint();
            One = new GamePresetPoint();
            Two = new GamePresetPoint();
            Three = new GamePresetPoint();
            Four = new GamePresetPoint();

            A.X = placementPreset.X.A;
            A.Y = placementPreset.Y.A;
            A.Z = placementPreset.Z.A;
            A.Active = placementPreset.Active.A;

            B.X = placementPreset.X.B;
            B.Y = placementPreset.Y.B;
            B.Z = placementPreset.Z.B;
            B.Active = placementPreset.Active.B;

            C.X = placementPreset.X.C;
            C.Y = placementPreset.Y.C;
            C.Z = placementPreset.Z.C;
            C.Active = placementPreset.Active.C;

            D.X = placementPreset.X.D;
            D.Y = placementPreset.Y.D;
            D.Z = placementPreset.Z.D;
            D.Active = placementPreset.Active.D;

            One.X = placementPreset.X.One;
            One.Y = placementPreset.Y.One;
            One.Z = placementPreset.Z.One;
            One.Active = placementPreset.Active.One;

            Two.X = placementPreset.X.Two;
            Two.Y = placementPreset.Y.Two;
            Two.Z = placementPreset.Z.Two;
            Two.Active = placementPreset.Active.Two;

            Three.X = placementPreset.X.Three;
            Three.Y = placementPreset.Y.Three;
            Three.Z = placementPreset.Z.Three;
            Three.Active = placementPreset.Active.Three;

            Four.X = placementPreset.X.Four;
            Four.Y = placementPreset.Y.Four;
            Four.Z = placementPreset.Z.Four;
            Four.Active = placementPreset.Active.Four;

            _reserved = 0;
            ContentFinderConditionID = 0;
            UnixTime = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        public override string ToString()
        {
            return $"A: {A}\r\n" +
                    $"B: {B}\r\n" +
                    $"C: {C}\r\n" +
                    $"D: {D}\r\n" +
                    $"1: {One}\r\n" +
                    $"2: {Two}\r\n" +
                    $"3: {Three}\r\n" +
                    $"4: {Four}\r\n" +
                    $"Reserved: 0x{_reserved:X}\r\n" +
                    $"ContentFinderCondition: {ContentFinderConditionID}\r\n" +
                    $"Timestamp: {UnixTime}";
        }
    }

    //	The layout of the active flags in memory as used when the game is placing a waymark preset.  Just an eight (C++) bool array.
    [StructLayout(LayoutKind.Sequential, Pack = 0, Size = 8)]
    public struct GamePreset_Placement_AxisActive
    {
        public FakeBool A;
        public FakeBool B;
        public FakeBool C;
        public FakeBool D;
        public FakeBool One;
        public FakeBool Two;
        public FakeBool Three;
        public FakeBool Four;

        public GamePreset_Placement_AxisActive(BitField8 activeMarkers)
        {
            A = new FakeBool(activeMarkers[0]);
            B = new FakeBool(activeMarkers[1]);
            C = new FakeBool(activeMarkers[2]);
            D = new FakeBool(activeMarkers[3]);
            One = new FakeBool(activeMarkers[4]);
            Two = new FakeBool(activeMarkers[5]);
            Three = new FakeBool(activeMarkers[6]);
            Four = new FakeBool(activeMarkers[7]);
        }

        public override string ToString()
        {
            return $"{A}, {B}, {C}, {D}, {One}, {Two}, {Three}, {Four}";
        }
    }

    //	The layout of waymark coordinates per-axis in memory as used when the game is placing a waymark preset.  Just an array of eight ints.
    [StructLayout(LayoutKind.Sequential, Pack = 0, Size = 32)]
    public struct GamePreset_Placement_AxisCoords
    {
        public int A;
        public int B;
        public int C;
        public int D;
        public int One;
        public int Two;
        public int Three;
        public int Four;

        public override string ToString()
        {
            return $"{A}, {B}, {C}, {D}, {One}, {Two}, {Three}, {Four}";
        }
    }

    //	The actual structure used by the game when calling the function to place a waymark preset.
    [StructLayout(LayoutKind.Sequential, Pack = 0, Size = 104)]
    public struct GamePreset_Placement
    {
        public GamePreset_Placement_AxisActive Active;
        public GamePreset_Placement_AxisCoords X;
        public GamePreset_Placement_AxisCoords Y;
        public GamePreset_Placement_AxisCoords Z;

        public GamePreset_Placement(GamePreset preset)
        {
            BitField8 ActiveMarkers = new();
            ActiveMarkers[0] = preset.A.Active;
            ActiveMarkers[1] = preset.B.Active;
            ActiveMarkers[2] = preset.C.Active;
            ActiveMarkers[3] = preset.D.Active;
            ActiveMarkers[4] = preset.One.Active;
            ActiveMarkers[5] = preset.Two.Active;
            ActiveMarkers[6] = preset.Three.Active;
            ActiveMarkers[7] = preset.Four.Active;

            Active = new GamePreset_Placement_AxisActive(ActiveMarkers);

            X = new GamePreset_Placement_AxisCoords();
            Y = new GamePreset_Placement_AxisCoords();
            Z = new GamePreset_Placement_AxisCoords();

            X.A = preset.A.X;
            Y.A = preset.A.Y;
            Z.A = preset.A.Z;

            X.B = preset.B.X;
            Y.B = preset.B.Y;
            Z.B = preset.B.Z;

            X.C = preset.C.X;
            Y.C = preset.C.Y;
            Z.C = preset.C.Z;

            X.D = preset.D.X;
            Y.D = preset.D.Y;
            Z.D = preset.D.Z;

            X.One = preset.One.X;
            Y.One = preset.One.Y;
            Z.One = preset.One.Z;

            X.Two = preset.Two.X;
            Y.Two = preset.Two.Y;
            Z.Two = preset.Two.Z;

            X.Three = preset.Three.X;
            Y.Three = preset.Three.Y;
            Z.Three = preset.Three.Z;

            X.Four = preset.Four.X;
            Y.Four = preset.Four.Y;
            Z.Four = preset.Four.Z;
        }

        public override string ToString()
        {
            return $"Active Flags: {Active}\r\n" +
                    $"X Coords: {X}\r\n" +
                    $"Y Coords: {Y}\r\n" +
                    $"Z Coords: {Z}";
        }
    }

    //	A helper struct to facilitate structure marshalling when C++ bools are involved.
    [StructLayout(LayoutKind.Sequential, Pack = 0, Size = 1)]
    public readonly struct FakeBool
    {
        private readonly byte BackingVal;

        public FakeBool(bool b)
        {
            BackingVal = b ? (byte)1 : (byte)0;
        }
        public static implicit operator bool(FakeBool b) { return b.BackingVal > 0; }
        public static implicit operator FakeBool(bool b) { return new FakeBool(b); }

        public override string ToString()
        {
            return $"{BackingVal}";
        }
    }

    //	A helper struct to make working with the active flags in a waymark preset structure easier.
    [StructLayout(LayoutKind.Sequential, Pack = 0, Size = 1)]
    public struct BitField8
    {
        public byte Data;

        public bool this[int i]
        {
            get
            {
                return i switch
                {
                    0 => (Data & 1) > 0,
                    1 => (Data & 2) > 0,
                    2 => (Data & 4) > 0,
                    3 => (Data & 8) > 0,
                    4 => (Data & 16) > 0,
                    5 => (Data & 32) > 0,
                    6 => (Data & 64) > 0,
                    7 => (Data & 128) > 0,
                    _ => throw new ArgumentOutOfRangeException(nameof(i), "Array index out of bounds."),
                };
            }
            set
            {
                Data = i switch
                {
                    0 => (byte)((value ? 1 : 0) | Data),
                    1 => (byte)((value ? 2 : 0) | Data),
                    2 => (byte)((value ? 4 : 0) | Data),
                    3 => (byte)((value ? 8 : 0) | Data),
                    4 => (byte)((value ? 16 : 0) | Data),
                    5 => (byte)((value ? 32 : 0) | Data),
                    6 => (byte)((value ? 64 : 0) | Data),
                    7 => (byte)((value ? 128 : 0) | Data),
                    _ => throw new ArgumentOutOfRangeException(nameof(i), "Array index out of bounds."),
                };
            }
        }

        public override string ToString()
        {
            return $"0x{Data:X}";
        }
    }
}
