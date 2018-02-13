﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using System.Linq;

[RequireComponent(typeof(SpriteRenderer))]
public abstract class AnimationController : MonoBehaviour
{
    //Logging
    public bool DebugLogging = false;                                       // En-/Disable Logging

    // Default Properties
    public string CharacterClass = "_master";                               // Foldername to the selected char
    public string CharacterSkin = "basic";                                  // Foldername to the selected skin
    public float GeneralSpeed = 1;                                          // General Speed for all animations

    // Public GET parameters
    public AnimatorStates CurrentAnimation { get; private set; }            // current playing animation 
    public AnimationState CurrentAnimationState { get; private set; }       // current playing animation state

    private AudioSource audioSource;                                        // Reference to audio source for playing sounds
    private bool finishedInitialization = false;                            // True if finished loading all the sprites
    private SpriteRenderer spriteRenderer;                                  // Sprite Renderer
    private Coroutine AnimationRoutine;                                     // Animation Routine
    private PhotonView photonView;

    private Dictionary<AnimatorStates, SectorAnimation> Animations;
    public SectorAnimation[] test;
    #region Enums

    // All existing animation types
    public enum AnimatorStates
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

    public enum TypeOfAnimation
    {
        Animation = 0, StartAnimation = 1, EndAnimation = 2
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
        Animations = new Dictionary<AnimatorStates, SectorAnimation>();
        audioSource = GetComponent<AudioSource>();
        photonView = GetComponent<PhotonView>();

        // initiate Components
        spriteRenderer = GetComponent<SpriteRenderer>();

        InitAnimations();
    }

    public void InitAnimations()
    {
        // set finsihed and start first animation
        finishedInitialization = true;
        StartAnimation(AnimatorStates.Idle);
    }

    #endregion

    #region Manage Animations

    public void StartAnimation(AnimatorStates animation)
    {
        if (finishedInitialization && CurrentAnimation != animation)
        {
            if (true)
            {
                StartAnimation(animation, TypeOfAnimation.StartAnimation);
            }
            else
            {
                StartAnimation(animation, TypeOfAnimation.Animation);
            }
        }
    }

    public void StartAnimation(AnimatorStates animation, TypeOfAnimation AnimationType, AnimationPlayTypes ForceAnimationPlayType = AnimationPlayTypes.Nothing)
    {
        if(!PhotonNetwork.offlineMode && GameNetwork.Instance.InGame)
        {
            photonView.RPC("RPC_StartAnimation", PhotonTargets.All, (short)animation, (short)AnimationType, (short)ForceAnimationPlayType);
        }
        else
        {
            RPC_StartAnimation((short)animation, (short)AnimationType, (short)ForceAnimationPlayType);
        }
    }

    [PunRPC]
    public void RPC_StartAnimation(short animationN, short AnimationTypeN, short ForceAnimationPlayTypeN)
    {
        AnimatorStates animation = (AnimatorStates)animationN;
        TypeOfAnimation AnimationType = (TypeOfAnimation)AnimationTypeN;
        AnimationPlayTypes ForceAnimationPlayType = (AnimationPlayTypes)ForceAnimationPlayTypeN;

        if (true)
        {
            // set current animation
            CurrentAnimation = animation;
            CurrentAnimationState = AnimationState.Playing;

            // get sprite atlas
            SpriteAtlas atlas = null;
            switch (AnimationType)
            {
                case TypeOfAnimation.Animation:
                    atlas = Animations[animation].DefaultAnimAtlas;
                    break;
                case TypeOfAnimation.StartAnimation:
                    atlas = Animations[animation].StartAnimAtlas;
                    break;
                case TypeOfAnimation.EndAnimation:
                    atlas = Animations[animation].EndAnimAtlas;
                    break;
            }

            // Calculating AnimationPlayType
            AnimationPlayTypes AnimationPlayType;
            if (ForceAnimationPlayType == AnimationPlayTypes.Nothing)
            {
                if (true)
                {
                    AnimationPlayType = AnimationPlayTypes.Loop;
                }
                else if (AnimationType == TypeOfAnimation.Animation)
                {   // if found a matching End-Animation to the choosen animation set the PlayType to HoldOnEnd
                    AnimationPlayType = AnimationPlayTypes.HoldOnEnd;
                }
                else
                {
                    AnimationPlayType = AnimationPlayTypes.Single;
                }
            }
            else
            {
                AnimationPlayType = ForceAnimationPlayType;
            }

            // stop running coroutine
            if (AnimationRoutine != null) StopCoroutine(AnimationRoutine);
            // start next animation as coroutine
            AnimationRoutine = StartCoroutine(PlayAnimation(animation, atlas, 0, AnimationType, AnimationPlayType));
        }
        else
        {
            // If an animation is not found revert to idle to prevent displaying errors
            Debug.Log("Could not start " + AnimationType.ToString() + " '" + animation.ToString() + "' because it is not present in Dictionary!" + "\n" + "Idle-Animation has been started instead.");
            StartAnimation(AnimatorStates.Idle);
        }
    }

    IEnumerator PlayAnimation(AnimatorStates animation, SpriteAtlas atlas, float delay, TypeOfAnimation AnimationType, AnimationPlayTypes AnimationPlayType)
    {
        // Log Running Animation
        if (DebugLogging)
        {
            Debug.Log("Character: " + CharacterClass + "| Animation: " + animation.ToString() + " with AnimationType: " + AnimationType.ToString() + " and AnimationPlayType: " + AnimationPlayType.ToString() + " is now running");
            Debug.Log("Delay: " + delay + " - SpriteCount: " + atlas.spriteCount + " --> Delay per Sprite: " + (delay / (float)atlas.spriteCount));
        }
        bool debugTmp = false;


        // Handle audio
        PlayAnimationAudio(animation);

        // Calculate correct delay
        delay = (delay / (float)atlas.spriteCount);

        // Add end to the filename if its an EndAnimation
        string SpriteNameAddition = "";
        if (AnimationType == TypeOfAnimation.StartAnimation)
            SpriteNameAddition = "Start";
        else if (AnimationType == TypeOfAnimation.EndAnimation)
            SpriteNameAddition = "End";

        while (true)
        {
            if (DebugLogging)
            {
                if (debugTmp)
                {
                    Debug.Log("Character: " + CharacterClass + "| Looping: " + animation.ToString());
                }
                else
                {
                    debugTmp = true;
                }
            }
            // Unload assets with no references
            Resources.UnloadUnusedAssets();
            Sprite spr = null;

            // play each animation of the atlas
            for (int i = 0; i < atlas.spriteCount; i++)
            {
                // ToString parameter "D2" formats the integer with 2 chars (leading 0 if nessessary)
                spriteRenderer.sprite = atlas.GetSprite(animation.ToString() + SpriteNameAddition + "_" + i.ToString("D2"));

                if (DebugLogging)
                {
                    Debug.Log("Active Sprite: " + animation.ToString() + SpriteNameAddition + "_" + i.ToString("D2") + " at Time: " + DateTime.Now.TimeOfDay);
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

            if (AnimationType == TypeOfAnimation.StartAnimation)
            {
                StartAnimation(animation, TypeOfAnimation.Animation);
                break;
            }
            else if (AnimationPlayType == AnimationPlayTypes.HoldOnEnd)
            {
                // wait till signal for end is recived
                CurrentAnimationState = AnimationState.Waiting;
                while (true)
                {
                    yield return new WaitForSeconds(0.1f);
                }
            }
            else if (AnimationPlayType == AnimationPlayTypes.Loop)
            {
                // loop
                CurrentAnimationState = AnimationState.Looping;
                yield return null;
            }
            else
            {
                // revert to idle
                StartAnimation(AnimatorStates.Idle);
                break;
            }
        }
    }

    private void PlayAnimationAudio(AnimatorStates animation)
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