using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public enum Cardinals : byte
{
    NORTH = 0x00000011,
    SOUTH = 0x00000000,
    WEST =  0x00000010,
    EAST =  0x00000001,

    COUNT = 4
};

static public class UtilsConverter
{
    public static Cardinals OrientToCard(Utils.ORIENTATION orient)
    {
        switch (orient)
        {
            case Utils.ORIENTATION.NORTH: return Cardinals.NORTH;
            case Utils.ORIENTATION.SOUTH: return Cardinals.SOUTH;
            case Utils.ORIENTATION.WEST: return Cardinals.WEST;
            case Utils.ORIENTATION.EAST: return Cardinals.EAST;
            default: Debug.LogError("Orientation to Cardinals convert error!"); return Cardinals.NORTH;
        }
    }

    public static Utils.ORIENTATION CardToOrient(Cardinals cardinal)
    {
        switch (cardinal)
        {
            case Cardinals.NORTH: return Utils.ORIENTATION.NORTH;
            case Cardinals.SOUTH: return Utils.ORIENTATION.SOUTH;
            case Cardinals.WEST: return Utils.ORIENTATION.WEST;
            case Cardinals.EAST: return Utils.ORIENTATION.EAST;
            default: Debug.LogError("Cardinals to Orientation convert error!"); return Utils.ORIENTATION.NORTH;
        }
    }
}