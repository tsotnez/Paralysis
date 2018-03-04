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
    Sprite[] startAnimAtlas = null;
    Sprite[] defaultAnimAtlas = null;
    Sprite[] endAnimAtlas = null;

    // FS Properties
    string AtlasPath;

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
        AtlasPath = "Animations/Champions/" + AnimConReference.CharacterClass + "/" + AnimConReference.CharacterSkin + "/" + AnimType.ToString();

        // Check for Sprites
        StartAnimAvaiable = Resources.LoadAll<Sprite>(AtlasPath + "Start").Length > 0;
        DefaultAnimAvaiable = Resources.LoadAll<Sprite>(AtlasPath).Length > 0;
        EndAnimAvaiable = Resources.LoadAll<Sprite>(AtlasPath + "End").Length > 0;
    }

    #endregion

    #region GET Atlas

    public Sprite[] StartAnimAtlas
    {
        get
        {
            if (StartAnimAvaiable && startAnimAtlas == null)
            {
                startAnimAtlas = Resources.LoadAll<Sprite>(AtlasPath + "Start");
            }
            return startAnimAtlas;
        }
    }

    public Sprite[] DefaultAnimAtlas
    {
        get
        {
            if (DefaultAnimAvaiable && defaultAnimAtlas == null)
            {
                defaultAnimAtlas = Resources.LoadAll<Sprite>(AtlasPath);
            }
            return defaultAnimAtlas;
        }
    }

    public Sprite[] EndAnimAtlas
    {
        get
        {
            if (EndAnimAvaiable && endAnimAtlas == null)
            {
                endAnimAtlas = Resources.LoadAll<Sprite>(AtlasPath + "End");
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
            foreach (Sprite sp in startAnimAtlas)
            {
                Resources.UnloadAsset(sp);
            }
            startAnimAtlas = null;
        }

        // Kill Default-Anim
        if (defaultAnimAtlas != null)
        {
            if (DebugLogging) Debug.LogWarning("Destruction of " + AnimType.ToString() + " - Default");
            foreach (Sprite sp in defaultAnimAtlas)
            {
                Resources.UnloadAsset(sp);
            }
            defaultAnimAtlas = null;
        }

        // Kill End-Anim
        if (endAnimAtlas != null)
        {
            if (DebugLogging) Debug.LogWarning("Destruction of " + AnimType.ToString() + " - End");
            foreach (Sprite sp in endAnimAtlas)
            {
                Resources.UnloadAsset(sp);
            }
            endAnimAtlas = null;
        }

        // Destroy all unused Objects
        Resources.UnloadUnusedAssets();
    }

    #endregion
}