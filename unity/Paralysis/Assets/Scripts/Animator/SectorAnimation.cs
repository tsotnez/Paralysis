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

    // Stuff
    AnimationController animCon;
    Coroutine destroyRoutine = null;

    #region Init

    public SectorAnimation(AnimationController AnimConReference, AnimationController.AnimationTypes AnimType, AnimationController.AnimationPlayTypes AnimPlayType,
        float StartAnimDuration, float DefaultAnimDuration, float EndAnimDuration)
    {
        // Apply Parameters
        this.animCon = AnimConReference;
        this.AnimType = AnimType;
        this.AnimPlayType = AnimPlayType;
        this.StartAnimDuration = StartAnimDuration;
        this.DefaultAnimDuration = DefaultAnimDuration;
        this.EndAnimDuration = EndAnimDuration;

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
                LoadSpriteAtlasses();
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
                LoadSpriteAtlasses();
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
                LoadSpriteAtlasses();
            }
            return endAnimAtlas;
        }
    }

    #endregion

    #region Load/Destroy Atlas

    private void LoadSpriteAtlasses()
    {
        if (DefaultAnimAvaiable)
        {
            // Load Sprite from Resources
            if (StartAnimAvaiable)
            {
                startAnimAtlas = Resources.Load<SpriteAtlas>(AtlasPath + "Start" + AtlasPathSuffix);
            }
            defaultAnimAtlas = Resources.Load<SpriteAtlas>(AtlasPath + AtlasPathSuffix);
            if (EndAnimAvaiable)
            {
                endAnimAtlas = Resources.Load<SpriteAtlas>(AtlasPath + "End" + AtlasPathSuffix);
            }
        }
    }

    public IEnumerator DestroySpriteAtlasses()
    {
        // Wait till Animation is not played anymore
        yield return new WaitUntil(() => animCon.CurrentAnimation != this.AnimType);

        // Kill Start-Anim
        if (startAnimAtlas != null)
        {
            Resources.UnloadAsset(startAnimAtlas);
            Object.Destroy(startAnimAtlas);
            startAnimAtlas = null;
        }

        // Kill Default-Anim
        if (defaultAnimAtlas != null)
        {
            Resources.UnloadAsset(defaultAnimAtlas);
            Object.Destroy(defaultAnimAtlas);
            defaultAnimAtlas = null;
        }

        // Kill End-Anim
        if (endAnimAtlas != null)
        {
            Resources.UnloadAsset(endAnimAtlas);
            Object.Destroy(endAnimAtlas);
            endAnimAtlas = null;
        }

        // Destroy all unused Objects
        Resources.UnloadUnusedAssets();
    }

    #endregion
}