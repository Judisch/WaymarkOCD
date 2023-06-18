using System;

using Newtonsoft.Json;

namespace WaymarkOCDPlugin
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]  //	Shouldn't have any null waymarks, but just in case...
    public class WaymarkPreset
    {
        public WaymarkPreset()
        {
            Name = DefaultPresetName;
            A.ID = 0;
            B.ID = 1;
            C.ID = 2;
            D.ID = 3;
            One.ID = 4;
            Two.ID = 5;
            Three.ID = 6;
            Four.ID = 7;
        }

        public WaymarkPreset(WaymarkPreset objToCopy)
        {
            if (objToCopy != null)
            {
                Name = objToCopy.Name;
                MapID = objToCopy.MapID;
                Time = objToCopy.Time;
                A = new Waymark(objToCopy.A);
                B = new Waymark(objToCopy.B);
                C = new Waymark(objToCopy.C);
                D = new Waymark(objToCopy.D);
                One = new Waymark(objToCopy.One);
                Two = new Waymark(objToCopy.Two);
                Three = new Waymark(objToCopy.Three);
                Four = new Waymark(objToCopy.Four);
            }
        }

        public static WaymarkPreset Parse(GamePreset gamePreset)
        {
            WaymarkPreset newPreset = new();

            newPreset.A.X = gamePreset.A.X / 1000.0f;
            newPreset.A.Y = gamePreset.A.Y / 1000.0f;
            newPreset.A.Z = gamePreset.A.Z / 1000.0f;
            newPreset.A.Active = gamePreset.A.Active;
            newPreset.A.ID = 0;

            newPreset.B.X = gamePreset.B.X / 1000.0f;
            newPreset.B.Y = gamePreset.B.Y / 1000.0f;
            newPreset.B.Z = gamePreset.B.Z / 1000.0f;
            newPreset.B.Active = gamePreset.B.Active;
            newPreset.B.ID = 1;

            newPreset.C.X = gamePreset.C.X / 1000.0f;
            newPreset.C.Y = gamePreset.C.Y / 1000.0f;
            newPreset.C.Z = gamePreset.C.Z / 1000.0f;
            newPreset.C.Active = gamePreset.C.Active;
            newPreset.C.ID = 2;

            newPreset.D.X = gamePreset.D.X / 1000.0f;
            newPreset.D.Y = gamePreset.D.Y / 1000.0f;
            newPreset.D.Z = gamePreset.D.Z / 1000.0f;
            newPreset.D.Active = gamePreset.D.Active;
            newPreset.D.ID = 3;

            newPreset.One.X = gamePreset.One.X / 1000.0f;
            newPreset.One.Y = gamePreset.One.Y / 1000.0f;
            newPreset.One.Z = gamePreset.One.Z / 1000.0f;
            newPreset.One.Active = gamePreset.One.Active;
            newPreset.One.ID = 4;

            newPreset.Two.X = gamePreset.Two.X / 1000.0f;
            newPreset.Two.Y = gamePreset.Two.Y / 1000.0f;
            newPreset.Two.Z = gamePreset.Two.Z / 1000.0f;
            newPreset.Two.Active = gamePreset.Two.Active;
            newPreset.Two.ID = 5;

            newPreset.Three.X = gamePreset.Three.X / 1000.0f;
            newPreset.Three.Y = gamePreset.Three.Y / 1000.0f;
            newPreset.Three.Z = gamePreset.Three.Z / 1000.0f;
            newPreset.Three.Active = gamePreset.Three.Active;
            newPreset.Three.ID = 6;

            newPreset.Four.X = gamePreset.Four.X / 1000.0f;
            newPreset.Four.Y = gamePreset.Four.Y / 1000.0f;
            newPreset.Four.Z = gamePreset.Four.Z / 1000.0f;
            newPreset.Four.Active = gamePreset.Four.Active;
            newPreset.Four.ID = 7;

            newPreset.MapID = gamePreset.ContentFinderConditionID;
            newPreset.Time = DateTimeOffset.FromUnixTimeSeconds(gamePreset.UnixTime);
            newPreset.Name = DefaultPresetName;

            return newPreset;
        }

        public GamePreset GetAsGamePreset()
        {
            GamePreset preset = new();

            preset.A.Active = A.Active;
            preset.A.X = A.Active ? (int)(A.X * 1000.0) : 0;
            preset.A.Y = A.Active ? (int)(A.Y * 1000.0) : 0;
            preset.A.Z = A.Active ? (int)(A.Z * 1000.0) : 0;

            preset.B.Active = B.Active;
            preset.B.X = B.Active ? (int)(B.X * 1000.0) : 0;
            preset.B.Y = B.Active ? (int)(B.Y * 1000.0) : 0;
            preset.B.Z = B.Active ? (int)(B.Z * 1000.0) : 0;

            preset.C.Active = C.Active;
            preset.C.X = C.Active ? (int)(C.X * 1000.0) : 0;
            preset.C.Y = C.Active ? (int)(C.Y * 1000.0) : 0;
            preset.C.Z = C.Active ? (int)(C.Z * 1000.0) : 0;

            preset.D.Active = D.Active;
            preset.D.X = D.Active ? (int)(D.X * 1000.0) : 0;
            preset.D.Y = D.Active ? (int)(D.Y * 1000.0) : 0;
            preset.D.Z = D.Active ? (int)(D.Z * 1000.0) : 0;

            preset.One.Active = One.Active;
            preset.One.X = One.Active ? (int)(One.X * 1000.0) : 0;
            preset.One.Y = One.Active ? (int)(One.Y * 1000.0) : 0;
            preset.One.Z = One.Active ? (int)(One.Z * 1000.0) : 0;

            preset.Two.Active = Two.Active;
            preset.Two.X = Two.Active ? (int)(Two.X * 1000.0) : 0;
            preset.Two.Y = Two.Active ? (int)(Two.Y * 1000.0) : 0;
            preset.Two.Z = Two.Active ? (int)(Two.Z * 1000.0) : 0;

            preset.Three.Active = Three.Active;
            preset.Three.X = Three.Active ? (int)(Three.X * 1000.0) : 0;
            preset.Three.Y = Three.Active ? (int)(Three.Y * 1000.0) : 0;
            preset.Three.Z = Three.Active ? (int)(Three.Z * 1000.0) : 0;

            preset.Four.Active = Four.Active;
            preset.Four.X = Four.Active ? (int)(Four.X * 1000.0) : 0;
            preset.Four.Y = Four.Active ? (int)(Four.Y * 1000.0) : 0;
            preset.Four.Z = Four.Active ? (int)(Four.Z * 1000.0) : 0;

            preset.ContentFinderConditionID = MapID;
            preset.UnixTime = (int)Time.ToUnixTimeSeconds();

            return preset;
        }

        public string GetPresetDataString(GetZoneNameDelegate dGetZoneName, bool showIDToo = false)
        {
            //	Try to get the zone name from the function passed to us if we can.
            string zoneName = "";
            if (dGetZoneName != null)
            {
                try
                {
                    zoneName = dGetZoneName(MapID, showIDToo);
                }
                catch
                {
                    zoneName = "Error retrieving zone name!";
                }
            }

            //	Construct the string.
            string str = "";
            str += "A: " + A.GetWaymarkDataString() + "\r\n";
            str += "B: " + B.GetWaymarkDataString() + "\r\n";
            str += "C: " + C.GetWaymarkDataString() + "\r\n";
            str += "D: " + D.GetWaymarkDataString() + "\r\n";
            str += "1: " + One.GetWaymarkDataString() + "\r\n";
            str += "2: " + Two.GetWaymarkDataString() + "\r\n";
            str += "3: " + Three.GetWaymarkDataString() + "\r\n";
            str += "4: " + Four.GetWaymarkDataString() + "\r\n";
            str += "Zone: " + zoneName + "\r\n";
            str += "Last Modified: " + Time.LocalDateTime.ToString();
            return str;
        }

        public delegate string GetZoneNameDelegate(UInt16 zoneID, bool showID);

        //	This looks gross, but it's easier to be compatible with PP presets if we have each waymark be a named member instead of in a collection :(
        public string Name { get; set; } = "Unknown";

        protected static string DefaultPresetName => "New Preset";

        //	Don't serialize in order to read older configs properly.
        [NonSerialized] protected UInt16 mMapID;

        //	PP sometimes gives bogus MapIDs that are outside the UInt16, so use a converter to handle those.
        [JsonConverter(typeof(MapIDJsonConverter))]
        public UInt16 MapID
        {
            get
            {
                return mMapID;
            }
            set
            {
                mMapID = value;
            }
        }

        public DateTimeOffset Time { get; set; } = new DateTimeOffset(DateTimeOffset.Now.UtcDateTime);
        public Waymark A { get; set; } = new Waymark();
        public Waymark B { get; set; } = new Waymark();
        public Waymark C { get; set; } = new Waymark();
        public Waymark D { get; set; } = new Waymark();
        public Waymark One { get; set; } = new Waymark();
        public Waymark Two { get; set; } = new Waymark();
        public Waymark Three { get; set; } = new Waymark();
        public Waymark Four { get; set; } = new Waymark();

        public Waymark this[int i]
        {
            get => i switch
            {
                0 => A,
                1 => B,
                2 => C,
                3 => D,
                4 => One,
                5 => Two,
                6 => Three,
                7 => Four,
                _ => throw new ArgumentOutOfRangeException($"Error in WaymarkPreset indexer: Invalid index \"{i}\""),
            };
        }

        public static string GetNameForWaymarkIndex(int i, bool getLongName = false)
        {
            return i switch
            {
                0 => "A",
                1 => "B",
                2 => "C",
                3 => "D",
                4 => getLongName ? "One" : "1",
                5 => getLongName ? "Two" : "2",
                6 => getLongName ? "Three" : "3",
                7 => getLongName ? "Four" : "4",
                _ => throw new ArgumentOutOfRangeException($"Error in WaymarkPreset.GetNameForWaymarkIndex(): Invalid index \"{i}\""),
            };
        }

        public virtual bool ShouldSerializeTime()   //More JSON bullshit because it has to be polymorphic for the serializer to check it in a derived class apparently.
        {
            return true;
        }
    }

    //	We may be getting the MapID as something that won't fit in UInt16, so this class helps us handle that.
    public class MapIDJsonConverter : JsonConverter<UInt16>
    {
        public override void WriteJson(JsonWriter writer, UInt16 value, JsonSerializer serializer)
        {
            writer.WriteValue(value);
        }
        public override UInt16 ReadJson(JsonReader reader, Type objectType, UInt16 existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            object? value = reader.Value;
            if (value == null)
            {
                return 0;
            }

            long val = (long)value;
            if (val > UInt16.MaxValue || val < 0)
            {
                return 0;
            }
            else
            {
                return (UInt16)val;
            }
        }
    }
}
