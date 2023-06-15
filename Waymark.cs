﻿using System.Numerics;

namespace WaymarkOCDPlugin
{
    public class Waymark
    {
        public float X { get; set; } = 0.0f;
        public float Y { get; set; } = 0.0f;
        public float Z { get; set; } = 0.0f;
        public int ID { get; set; } = 0;    //This is kind of a BS field, but keep it for import/export interop with PP.
        public bool Active { get; set; } = false;

        public Waymark()
        {
        }

        public Waymark(Waymark objToCopy)
        {
            if (objToCopy != null)
            {
                X = objToCopy.X;
                Y = objToCopy.Y;
                Z = objToCopy.Z;
                ID = objToCopy.ID;
                Active = objToCopy.Active;
            }
        }

        public string GetWaymarkDataString()
        {
            return Active ? (X.ToString("0.00").PadLeft(7) + ", " + Y.ToString("0.00").PadLeft(7) + ", " + Z.ToString("0.00").PadLeft(7)) : "Unused";
        }

        public void SetCoords(Vector3 pos)
        {
            X = pos.X;
            Y = pos.Y;
            Z = pos.Z;
        }
    }
}
