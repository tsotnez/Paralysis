using System.Collections;
using UnityEngine;
using UnityEngine.U2D;

[System.Serializable]
public class SectorAnimation
{
    // Animation Parameters
    public AnimationController.AnimationTypes AnimType { get; private set; }        // Type of Animation
    public AnimationController.AnimationPlayTypes AnimPlayType { get; private set; }// Is Animation Looping
    public float StartAnimDuration { get; private set; }                              // Duration of StartAnimation (Duration/SpriteCount)
    public float DefaultAnimDuration { get; private set; }                            // Duration of Animation (Duration/SpriteCount)
    public float EndAnimDuration { get; private set; }                                // Duration of EndAnimation (Duration/SpriteCount)

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
        AtlasPathSuffix = "Atlas.spriteatlas";

        // Check for Atlasses
        StartAnimAvaiable = Resources.Load(AtlasPath + "Start" + AtlasPathSuffix) != null;
        DefaultAnimAvaiable = Resources.Load(AtlasPath + AtlasPathSuffix) != null;
        EndAnimAvaiable = Resources.Load(AtlasPath + "End" + AtlasPathSuffix) != null;
    }

    #endregion

    #region GET Atlas

    public SpriteAtlas StartAnimAtlas
    {
        get
        {
            if (StartAnimAvaiable && startAnimAtlas == null)
            {
                LoadSpriteAtlas();
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
                LoadSpriteAtlas();
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
                LoadSpriteAtlas();
            }
            return endAnimAtlas;
        }
    }

    #endregion

    #region Manage Atlas

    private void LoadSpriteAtlas()
    {
        if (DefaultAnimAvaiable)
        {
            // Load Sprite from Resources
            if (StartAnimAvaiable)
            {
                startAnimAtlas = ((SpriteAtlas)Resources.Load(AtlasPath + "Start" + AtlasPathSuffix));
            }
            defaultAnimAtlas = ((SpriteAtlas)Resources.Load(AtlasPath + AtlasPathSuffix));
            if (EndAnimAvaiable)
            {
                endAnimAtlas = ((SpriteAtlas)Resources.Load(AtlasPath + "End" + AtlasPathSuffix));
            }

            // Start automatic destroy of Animations
            //destroyRoutine = StartCoroutine(DestroyAtlasses());
        }
    }

    //public void ResetDestroyAtlasses()
    //{
    //    if (destroyRoutine != null)
    //    {
    //        StopCoroutine(destroyRoutine);
    //        StartCoroutine(DestroyAtlasses());
    //    }
    //}

    public IEnumerator DestroyAtlasses()
    {
        // Wait till Animation has started
        yield return new WaitUntil(() => animCon.CurrentAnimation == this.AnimType);
        // Wait till Animation is not played anymore
        yield return new WaitUntil(() => animCon.CurrentAnimation != this.AnimType);
        // Wait till Animation is not played for a while
        yield return new WaitForSeconds(10f);

        // Kill References
        Resources.UnloadAsset(startAnimAtlas);
        startAnimAtlas = null;
        Resources.UnloadAsset(defaultAnimAtlas);
        defaultAnimAtlas = null;
        Resources.UnloadAsset(endAnimAtlas);
        endAnimAtlas = null;
        Resources.UnloadUnusedAssets();
    }

    #endregion
}