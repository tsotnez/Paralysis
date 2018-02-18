using System.Collections;
using UnityEngine;
using UnityEngine.U2D;

[System.Serializable]
public class SectorAnimation
{
    // Animation Parameters
    public AnimationController.AnimationTypes AnimType { get; private set; }        // Type of Animation
    public AnimationController.AnimationPlayTypes AnimPlayType { get; private set; }// Is Animation Looping
    public float StartAnimDuration { get; private set; }                            // Duration of StartAnimation (Duration/SpriteCount)
    public float DefaultAnimDuration { get; private set; }                          // Duration of Animation (Duration/SpriteCount)
    public float EndAnimDuration { get; private set; }                              // Duration of EndAnimation (Duration/SpriteCount)

    // Animation available
    public bool StartAnimAvaiable { get; private set; }
    public bool DefaultAnimAvaiable { get; private set; }
    public bool EndAnimAvaiable { get; private set; }

    // Cache for Atlasses
    SpriteAtlas startAnimAtlas = null;
    SpriteAtlas defaultAnimAtlas = null;
    SpriteAtlas endAnimAtlas = null;

    // FS Properties
    string AtlasPath;
    string AtlasPathSuffix;

    // stuff
    private bool DebugLogging = false;

    #region Init

    public SectorAnimation(AnimationController AnimConReference, AnimationController.AnimationTypes AnimType, AnimationController.AnimationPlayTypes AnimPlayType,
        float StartAnimDuration, float DefaultAnimDuration, float EndAnimDuration)
    {
        // Apply Parameters
        this.AnimType = AnimType;
        this.AnimPlayType = AnimPlayType;
        this.StartAnimDuration = StartAnimDuration;
        this.DefaultAnimDuration = DefaultAnimDuration;
        this.EndAnimDuration = EndAnimDuration;

        // Stuff
        this.DebugLogging = AnimConReference.DebugLogging;

        // Build Path
        AtlasPath = "Animations\\" + AnimConReference.CharacterClass + "\\" + AnimConReference.CharacterSkin + "\\" + AnimType.ToString();
        AtlasPathSuffix = "Atlas";

        // Check for Atlasses
        StartAnimAvaiable = Resources.Load<SpriteAtlas>(AtlasPath + "Start" + AtlasPathSuffix) != null;
        DefaultAnimAvaiable = Resources.Load<SpriteAtlas>(AtlasPath + AtlasPathSuffix) != null;
        EndAnimAvaiable = Resources.Load<SpriteAtlas>(AtlasPath + "End" + AtlasPathSuffix) != null;
    }

    #endregion

    #region GET Atlas

    public SpriteAtlas StartAnimAtlas
    {
        get
        {
            if (StartAnimAvaiable && startAnimAtlas == null)
            {
                startAnimAtlas = Resources.Load<SpriteAtlas>(AtlasPath + "Start" + AtlasPathSuffix);
            }
            return startAnimAtlas;
        }
    }

    public SpriteAtlas DefaultAnimAtlas
    {
        get
        {
            if (DefaultAnimAvaiable && defaultAnimAtlas == null)
            {
                defaultAnimAtlas = Resources.Load<SpriteAtlas>(AtlasPath + AtlasPathSuffix);
            }
            return defaultAnimAtlas;
        }
    }

    public SpriteAtlas EndAnimAtlas
    {
        get
        {
            if (EndAnimAvaiable && endAnimAtlas == null)
            {
                endAnimAtlas = Resources.Load<SpriteAtlas>(AtlasPath + "End" + AtlasPathSuffix);
            }
            return endAnimAtlas;
        }
    }

    #endregion

    #region Destroy Atlas

    public void DestroySpriteAtlasses()
    {
        // Kill Start-Anim
        if (startAnimAtlas != null)
        {
            if (DebugLogging) Debug.LogWarning("Destruction of " + AnimType.ToString() + " - Start");
            Resources.UnloadAsset(startAnimAtlas);
            startAnimAtlas = null;
        }

        // Kill Default-Anim
        if (defaultAnimAtlas != null)
        {
            if (DebugLogging) Debug.LogWarning("Destruction of " + AnimType.ToString() + " - Default");
            Resources.UnloadAsset(defaultAnimAtlas);
            defaultAnimAtlas = null;
        }

        // Kill End-Anim
        if (endAnimAtlas != null)
        {
            if (DebugLogging) Debug.LogWarning("Destruction of " + AnimType.ToString() + " - End");
            Resources.UnloadAsset(endAnimAtlas);
            endAnimAtlas = null;
        }

        // Destroy all unused Objects
        Resources.UnloadUnusedAssets();
    }

    #endregion
}