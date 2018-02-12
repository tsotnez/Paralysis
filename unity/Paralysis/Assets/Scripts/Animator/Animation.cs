﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.U2D;

[System.Serializable]
class Animation : MonoBehaviour
{
    // Animation Parameters
    public AnimationController.AnimatorStates AnimationType;
    public bool Loop = false;               // Is Animation Looping
    public int StartAnimDuration = 0;       // Duration of StartAnimation (Duration/SpriteCount)
    public int DefaultAnimDuration = 0;     // Duration of Animation (Duration/SpriteCount)
    public int EndAnimDuration = 0;         // Duration of EndAnimation (Duration/SpriteCount)

    // Animation available
    public bool StartAnimAvaiable { get; private set; }
    public bool DefaultAnimAvaiable { get; private set; }
    public bool EndAnimAvaiable { get; private set; }

    // Cache for Atlasses
    SpriteAtlas startAnimAtlas = null;
    SpriteAtlas defaultanimAtlas = null;
    SpriteAtlas endAnimAtlas = null;

    // FS Properties
    string AtlasPath;
    string AtlasPathSuffix;

    // Stuff
    AnimationController animCon;
    Coroutine destroyRoutine = null;

    #region Enum

    public enum AnimType { Start, Default, End }

    #endregion

    #region default

    private void Start()
    {
        // Build Path
        AtlasPath = "Animations\\" + animCon.CharacterClass + "\\" + animCon.CharacterSkin + "\\" + this.AnimationType.ToString();
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
            else
            {
                ResetDestroyAtlasses();
            }
            return startAnimAtlas;
        }
    }

    public SpriteAtlas DefaultAnimAtlas
    {
        get
        {
            if (DefaultAnimAvaiable && startAnimAtlas == null)
            {
                LoadSpriteAtlas();
            }
            else
            {
                ResetDestroyAtlasses();
            }
            return defaultanimAtlas;
        }
    }

    public SpriteAtlas EndAnimAtlas
    {
        get
        {
            if (EndAnimAvaiable && startAnimAtlas == null)
            {
                LoadSpriteAtlas();
            }
            else
            {
                ResetDestroyAtlasses();
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
            defaultanimAtlas = ((SpriteAtlas)Resources.Load(AtlasPath + AtlasPathSuffix));
            if (EndAnimAvaiable)
            {
                endAnimAtlas = ((SpriteAtlas)Resources.Load(AtlasPath + "End" + AtlasPathSuffix));
            }

            // Start automatic destroy of Animations
            destroyRoutine = StartCoroutine(DestroyAtlasses());
        }
    }

    private void ResetDestroyAtlasses()
    {
        if (destroyRoutine != null)
        {
            StopCoroutine(destroyRoutine);
            StartCoroutine(DestroyAtlasses());
        }
    }

    private IEnumerator DestroyAtlasses()
    {
        // Wait till Animation has started
        yield return new WaitUntil(() => animCon.CurrentAnimation == this.AnimationType);
        // Wait till Animation is not played anymore
        yield return new WaitUntil(() => animCon.CurrentAnimation != this.AnimationType);
        // Wait till Animation is not played for a while
        yield return new WaitForSeconds(10f);

        // Kill References
        Resources.UnloadUnusedAssets();
        startAnimAtlas = null;
        defaultanimAtlas = null;
        endAnimAtlas = null;
    }

    #endregion
}