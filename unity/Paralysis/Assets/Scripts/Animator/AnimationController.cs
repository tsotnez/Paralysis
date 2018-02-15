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
    public AnimationState CurrentAnimationState { get; private set; }           // current playing animation state

    // Components
    private AudioSource audioSource;                                            // Reference to audio source for playing sounds
    private SpriteRenderer spriteRenderer;                                      // Sprite Renderer
    private PhotonView photonView;                                              // Network

    // Animation 
    private bool finishedInitialization = false;                                // True if finished loading all the sprites
    private Dictionary<AnimationTypes, SectorAnimation> SectorAnimations;       // Dictionary of Animations

    #region Enums

    // All existing animation types
    public enum AnimationTypes
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

    void Start()
    {
        // initiate dictionarys
        SectorAnimations = new Dictionary<AnimationTypes, SectorAnimation>();

        // initiate Components
        audioSource = GetComponent<AudioSource>();
        photonView = GetComponent<PhotonView>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        InitAnimations();
    }

    public void InitAnimations()
    {
        // Apply values from Inspector to Dictionary
        for (int i = 0; i < AnimPlayType.Length; i++)
        {
            SectorAnimations.Add((AnimationTypes)i, new SectorAnimation(this, (AnimationTypes)i, AnimPlayType[i], StartAnimDuration[i], DefaultAnimDuration[i], EndAnimDuration[i]));
        }

        // Clear Arrays from Inspector
        AnimPlayType = null;
        StartAnimDuration = null;
        DefaultAnimDuration = null;
        EndAnimDuration = null;

        // set finsihed and start first animation
        finishedInitialization = true;
        StartAnimation(AnimationTypes.Idle);
    }

    #endregion

    #region Manage Animations

    public void StartAnimation(AnimationTypes Anim, AnimationKind AnimKind = AnimationKind.DefaultAnimation)
    {
        StopAllCoroutines();
        StartCoroutine(HandleAnimation(SectorAnimations[Anim], AnimKind));
    }

    private IEnumerator HandleAnimation(SectorAnimation Anim, AnimationKind ForceAnimKind)
    {
        if (Anim.DefaultAnimAvaiable)
        {
            // set current animation
            CurrentAnimation = Anim.AnimType;
            CurrentAnimationState = AnimationState.Playing;

            // Handle audio matching to the animation
            PlayAnimationAudio(Anim.AnimType);

            if (ForceAnimKind != AnimationKind.EndAnimation)
            {
                if (Anim.StartAnimAvaiable)
                {
                    yield return PlayAnimation(Anim.StartAnimAtlas, Anim.StartAnimDuration, Anim.AnimType.ToString() + "Start");
                }

                SpriteAtlas atlas = Anim.DefaultAnimAtlas;
                do
                {
                    yield return PlayAnimation(Anim.DefaultAnimAtlas, Anim.DefaultAnimDuration, Anim.AnimType.ToString());
                }
                while (Anim.AnimPlayType == AnimationPlayTypes.Loop);

                if (Anim.EndAnimAvaiable)
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
                if (Anim.EndAnimAvaiable)
                {
                    yield return PlayAnimation(Anim.EndAnimAtlas, Anim.EndAnimDuration, Anim.AnimType.ToString() + "End");
                }
            }
        }
        else
        {
            // If an animation is not found revert to idle to prevent displaying errors
            Debug.Log("Could not start '" + Anim.AnimType.ToString() + "' because it is not present in Resource Folder!" + "\n" + "Idle-Animation has been started instead.");
            StartAnimation(AnimationTypes.Idle);
        }
    }

    IEnumerator PlayAnimation(SpriteAtlas atlas, float delay, string FileNamePrefix)
    {
        // Log Running Animation
        if (DebugLogging)
        {
            Debug.Log("Character: " + CharacterClass + "| Animation: " + FileNamePrefix + " is now running");
            Debug.Log("Delay: " + delay + " - SpriteCount: " + atlas.spriteCount + " --> Delay per Sprite: " + (delay / (float)atlas.spriteCount));
        }

        // Calculate correct delay
        delay = (delay / (float)atlas.spriteCount);

        // Unload assets with no references
        Resources.UnloadUnusedAssets();
        Sprite spr = null;

        // play each animation of the atlas
        string SpriteFileName = "";
        for (int i = 0; i < atlas.spriteCount; i++)
        {
            // ToString parameter "D2" formats the integer with 2 chars (leading 0 if nessessary)
            SpriteFileName = FileNamePrefix + "_" + i.ToString("D2");
            spriteRenderer.sprite = atlas.GetSprite(SpriteFileName);

            if (DebugLogging)
            {
                Debug.Log("Active Sprite: " + SpriteFileName + " at Time: " + DateTime.Now.TimeOfDay);
            }

            // Unload sprite
            Destroy(spr);
            spr = null;
            spr = spriteRenderer.sprite;
            yield return new WaitForSeconds(delay);
        }

        // Unload
        Resources.UnloadUnusedAssets();
        spr = null;
    }

    private void PlayAnimationAudio(AnimationTypes animation)
    {
        AudioClip clip = Resources.Load<AudioClip>("Audio/Characters/" + CharacterClass + "/" + animation.ToString());
        if (clip != null)
        {
            if (audioSource.clip != clip || (audioSource.clip == clip && true))
            {
                //New clip or loop
                audioSource.clip = clip;
                audioSource.Play();
            }
            else if (audioSource.clip == clip && !true)
            {
                //Same clip again
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

}