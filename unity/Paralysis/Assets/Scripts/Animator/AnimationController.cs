using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using System.Linq;

[RequireComponent(typeof(SpriteRenderer))]
public abstract class AnimationController : MonoBehaviour
{
    // Inspector
    public AnimationPlayTypes[] AnimPlayType = { AnimationPlayTypes.Single };   // Shall animation be looped?
    public float[] StartAnimDuration = { 0f };                                  // Duration of Start-Animation
    public float[] DefaultAnimDuration = { 0f };                                // Duration of Default-Animation
    public float[] EndAnimDuration = { 0f };                                    // Duration of End-Animation

    // Logging
    public bool DebugLogging = false;                                           // En-/Disable Logging

    // Default Properties
    public string CharacterClass = "_master";                                   // Foldername to the selected char
    public string CharacterSkin = "basic";                                      // Foldername to the selected skin

    // Public GET parameters
    public AnimationTypes CurrentAnimation { get; private set; }                // current playing animation 
    public AnimationKind CurrentAnimationKind { get; private set; }             // current playing animation kind
    public AnimationState CurrentAnimationState { get; private set; }           // current playing animation state

    // Components
    private AudioSource audioSource;                                            // Reference to audio source for playing sounds
    private SpriteRenderer spriteRenderer;                                      // Sprite Renderer
    private PhotonView photonView;                                              // Network

    // Animation 
    private Dictionary<AnimationTypes, SectorAnimation> SectorAnimations;       // Dictionary of Animations
    private Dictionary<AnimationTypes, Coroutine> AtlasDestroyRoutines;         // Atlas destroy routines 
    private IEnumerator AnimationRoutine = null;                                // Coroutine of HandleAnimation

    #region Enums

    // All existing animation types
    public enum AnimationTypes : byte
    {
        //Default
        Idle = 0,
        Run = 1,
        Jump = 2,
        Fall = 3,
        Dash = 4,
        Hit = 5,
        //Status
        Stunned = 6,
        KnockedBack = 7,
        Block = 8,
        //Attack
        BasicAttack1 = 9,
        BasicAttack2 = 10,
        BasicAttack3 = 11,
        JumpAttack = 12,
        //Skills
        Skill1 = 13,
        Skill2 = 14,
        Skill3 = 15,
        Skill4 = 16,

        // Assassin
        DoubleJump = 17,
        // Knight & Infantry
        BlockMove = 18,
        // Knight
        DashFor = 19,
        // ?
        Walk = 20,

        // End of the Game
        Die = 21
    }

    public enum AnimationKind
    {
        DefaultAnimation = 0, StartAnimation = 1, EndAnimation = 2
    }

    public enum AnimationPlayTypes
    {
        Single = 0, Loop = 1, HoldOnEnd = 2, Nothing = 3
    }

    public enum AnimationState
    {
        Playing = 0, Looping = 1, Waiting = 2
    }

    #endregion

    #region Init

    private void OnEnable()
    {
        if (SectorAnimations == null || AtlasDestroyRoutines == null)
        {
            Init();
            InitAnimations();
        }
        StartWorking();
    }

    private void Init()
    {
        // initiate dictionarys
        SectorAnimations = new Dictionary<AnimationTypes, SectorAnimation>();
        AtlasDestroyRoutines = new Dictionary<AnimationTypes, Coroutine>();

        // initiate Components
        audioSource = GetComponent<AudioSource>();
        photonView = GetComponent<PhotonView>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        photonView = GetComponent<PhotonView>();
    }

    private void InitAnimations()
    {
        // Apply values from Inspector to Dictionary
        for (int i = 0; i < AnimPlayType.Length; i++)
        {
            SectorAnimations.Add((AnimationTypes)i, new SectorAnimation(this, (AnimationTypes)i, AnimPlayType[i], StartAnimDuration[i], DefaultAnimDuration[i], EndAnimDuration[i]));
            AtlasDestroyRoutines.Add((AnimationTypes)i, null);
        }

        // Clear Arrays from Inspector
        AnimPlayType = null;
        StartAnimDuration = null;
        DefaultAnimDuration = null;
        EndAnimDuration = null;
    }

    private void StartWorking()
    {
        // start first animation
        CurrentAnimation = AnimationTypes.Die;
        StartAnimation(AnimationTypes.Idle);
    }

    #endregion

    #region Public Calls

    public void StartAnimation(AnimationTypes Anim, bool forceRestart = false)
    {
        if (CurrentAnimation != Anim || forceRestart)
        {
            if(!PhotonNetwork.offlineMode && GameNetwork.Instance != null)
            {
                if (GameNetwork.Instance.InGame)
                {
                    photonView.RPC("RPC_StartAnimation", PhotonTargets.Others, (byte)Anim);
                    RPC_StartAnimation((byte)Anim);
                }
            }
            else
            {
                RPC_StartAnimation((byte)Anim);
            }
        }
    }

    [PunRPC]
    public void RPC_StartAnimation(byte animType)
    {
        AnimationTypes Anim = (AnimationTypes)animType;

        // set current animation
        CurrentAnimation = Anim;

        // Start CleanUp of the coming Animation
        ReSetDestroyAtlas(Anim);

        // Handle Animation
        if (AnimationRoutine != null)
        {
            StopCoroutine(AnimationRoutine);
            ((IDisposable)AnimationRoutine).Dispose();
        }
        AnimationRoutine = HandleAnimation(SectorAnimations[Anim], AnimationKind.DefaultAnimation);
        StartCoroutine(AnimationRoutine);

    }

    public void StartEndAnimation(AnimationTypes Anim)
    {
        if(!PhotonNetwork.offlineMode && GameNetwork.Instance.InGame)
        {
            photonView.RPC("RPC_StartEndAnimation", PhotonTargets.Others, (byte)Anim);
            RPC_StartEndAnimation((byte)Anim);
        }
        else
        {
            RPC_StartEndAnimation((byte)Anim);
        }
    }

    [PunRPC]
    public void RPC_StartEndAnimation(byte animType)
    {
        AnimationTypes Anim = (AnimationTypes)animType;

        // NO clean here because its the same animation
        // Handle Animation
        if (AnimationRoutine != null)
        {
            StopCoroutine(AnimationRoutine);
            ((IDisposable)AnimationRoutine).Dispose();
        }
        AnimationRoutine = HandleAnimation(SectorAnimations[Anim], AnimationKind.EndAnimation);
        StartCoroutine(AnimationRoutine);
    }

    #endregion

    #region Manage Animations

    private IEnumerator HandleAnimation(SectorAnimation Anim, AnimationKind AnimKind)
    {
        if (Anim.DefaultAnimAvaiable)
        {
            SpriteAtlas atlas = null;
            try
            {
                // set playing
                CurrentAnimationState = AnimationState.Playing;

                // Handle audio matching to the animation
                PlayAnimationAudio(Anim.AnimType);

                if (AnimKind == AnimationKind.DefaultAnimation || AnimKind == AnimationKind.StartAnimation)
                {
                    // Play Start-Animation first
                    if (Anim.StartAnimAvaiable)
                    {
                        CurrentAnimationKind = AnimationKind.StartAnimation;
                        atlas = Anim.StartAnimAtlas;
                        yield return PlayAnimation(atlas, Anim.StartAnimDuration, Anim.AnimType.ToString() + "Start");
                        Resources.UnloadAsset(atlas);
                        atlas = null;
                    }

                    // Set correct states
                    CurrentAnimationKind = AnimationKind.DefaultAnimation;
                    if (Anim.AnimPlayType == AnimationPlayTypes.Loop)
                    {
                        CurrentAnimationState = AnimationState.Looping;
                    }

                    // Start Default Animation
                    atlas = Anim.DefaultAnimAtlas;
                    do
                    {
                        yield return PlayAnimation(Anim.DefaultAnimAtlas, Anim.DefaultAnimDuration, Anim.AnimType.ToString());
                    }
                    while (Anim.AnimPlayType == AnimationPlayTypes.Loop);

                    if (Anim.EndAnimAvaiable || Anim.AnimPlayType == AnimationPlayTypes.HoldOnEnd)
                    {
                        // wait till signal for end is recived
                        CurrentAnimationState = AnimationState.Waiting;
                        while (true)
                        {
                            yield return new WaitForSeconds(0.1f);
                        }
                    }
                    else
                    {
                        // revert to idle
                        StartAnimation(AnimationTypes.Idle);
                    }
                }
                else
                {
                    // Play End-Animation
                    if (Anim.EndAnimAvaiable)
                    {
                        CurrentAnimationKind = AnimationKind.EndAnimation;
                        atlas = Anim.EndAnimAtlas;
                        yield return PlayAnimation(atlas, Anim.EndAnimDuration, Anim.AnimType.ToString() + "End");
                        Resources.UnloadAsset(atlas);
                        atlas = null;
                    }

                    // revert to idle after finishing End-Animation
                    StartAnimation(AnimationTypes.Idle);
                }
            }
            finally
            {
                if (atlas != null)
                {
                    if (DebugLogging) Debug.LogWarning("HandleAnimation: CleanUp of " + atlas.name);
                    Resources.UnloadAsset(atlas);
                    atlas = null;
                    Resources.UnloadUnusedAssets();
                }
            }
        }
        else
        {
            // If an animation is not found revert to idle to prevent displaying errors
            Debug.LogWarning(string.Format("Could not start '{0}' as '{1}' because it is not present in Resource Folder!" + "\n" + "Idle-Animation has been started instead.", Anim.AnimType, AnimKind));
            StartAnimation(AnimationTypes.Idle);
        }
    }

    private IEnumerator PlayAnimation(SpriteAtlas atlas, float delay, string FileNamePrefix)
    {
        Sprite spr = null;

        try
        {
            // Log Running Animation
            if (DebugLogging)
            {
                Debug.Log(string.Format("Character: {0}| Animation: {1} is now running", CharacterClass, FileNamePrefix));
                Debug.Log(string.Format("Delay: {0} - SpriteCount: {1} --> Delay per Sprite: {2}", delay, atlas.spriteCount, (delay / (float)atlas.spriteCount)));
            }

            // Calculate correct delay
            delay = (delay / (float)atlas.spriteCount);

            // Unload assets with no references
            Resources.UnloadUnusedAssets();

            // play each animation of the atlas
            string SpriteFileName = "";
            for (int i = 0; i < atlas.spriteCount; i++)
            {
                // ToString parameter "D2" formats the integer with 2 chars (leading 0 if nessessary)
                SpriteFileName = FileNamePrefix + "_" + i.ToString("D2");
                spriteRenderer.sprite = atlas.GetSprite(SpriteFileName);

                if (DebugLogging)
                {
                    Debug.Log(string.Format("Active Sprite: {0} at Time: {1}", SpriteFileName, DateTime.Now.TimeOfDay));
                }

                // Unload sprite
                Destroy(spr);
                spr = null;
                spr = spriteRenderer.sprite;
                yield return new WaitForSeconds(delay);
            }
        }
        finally
        {
            if (DebugLogging) Debug.LogWarning("PlayAnimation: CleanUp of " + atlas.name);
            Resources.UnloadAsset(atlas);
            spr = null;
            atlas = null;
            Resources.UnloadUnusedAssets();
        }
    }

    #endregion

    #region Audio

    private void PlayAnimationAudio(AnimationTypes animation)
    {
        AudioClip clip = Resources.Load<AudioClip>("Audio/Characters/" + CharacterClass + "/" + animation.ToString());
        if (clip != null)
        {
            if (audioSource.clip != clip || (audioSource.clip == clip && true))
            {
                // New clip or loop
                audioSource.clip = clip;
                audioSource.Play();
            }
            else if (audioSource.clip == clip && !true)
            {
                // Same clip again
                audioSource.Stop();
                audioSource.Play();
            }
            else
                audioSource.Stop();
        }
        else
            audioSource.Stop();
    }

    #endregion
    
    #region Atlas Destroy

    private IEnumerator HandleDestroyAtlas(SectorAnimation AnimForDestroy)
    {
        // Wait till Animation has started
        yield return new WaitUntil(() => CurrentAnimation == AnimForDestroy.AnimType);
        // Wait till Animation is not played anymore
        yield return new WaitUntil(() => CurrentAnimation != AnimForDestroy.AnimType);
        // Wait till Animation is not in use anymore
        yield return new WaitForSeconds(3f);

        // Destroy Atlasses
        AnimForDestroy.DestroySpriteAtlasses();
    }

    private void ReSetDestroyAtlas(AnimationTypes Anim)
    {
        // Stop Coroutine if running
        if (AtlasDestroyRoutines.ContainsKey(Anim) && AtlasDestroyRoutines[Anim] != null)
        {
            StopCoroutine(AtlasDestroyRoutines[Anim]);
        }
        // Start destroy Coroutine
        AtlasDestroyRoutines[Anim] = StartCoroutine(HandleDestroyAtlas(SectorAnimations[Anim]));
    }

    #endregion

}