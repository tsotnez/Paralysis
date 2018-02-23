using System;
using UnityEngine;

public class SectorLayerMaskManager
{
    public static LayerMask CreateLayerMaskWith(int NewLayer)
    {
        return AddLayerToMask(new LayerMask(), NewLayer);
    }

    public static LayerMask CreateLayerMaskWith(int[] NewLayer)
    {
        return AddLayerToMask(new LayerMask(), NewLayer);
    }

    public static LayerMask AddLayerToMask(LayerMask LayerMaskForAdd, int NewLayer)
    {
        LayerMaskForAdd |= (1 << NewLayer);
        return LayerMaskForAdd;
    }

    public static LayerMask AddLayerToMask(LayerMask LayerMaskForAdd, int[] NewLayers)
    {
        foreach (int Layer in NewLayers)
        {
            LayerMaskForAdd |= (1 << Layer);
        }      
        return LayerMaskForAdd;
    }

    public static LayerMask RemoveLayerFromMask(LayerMask LayerMaskForAdd, int RemoveLayer)
    {
        throw new NotImplementedException();
    }

    public static LayerMask RemoveLayerFromMask(LayerMask LayerMaskForAdd, int[] RemoveLayers)
    {
        throw new NotImplementedException();
    }
}
