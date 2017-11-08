using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using System.Linq;

[RequireComponent(typeof(SpriteRenderer))]
public abstract class AnimationController : MonoBehaviour
{
    // Public parameters for Editor-Inspector
    public AnimatorStates[] AnimationType = { 0 };                          // Type of Animation
    public bool[] AnimationLoop = { false };                                // Shall animation be looped?
    public float[] AnimationDuration;                                       // Duration of each animation
    public SpriteAtlas[] Atlasses;                                          // Array of all atlasses
    public float GeneralSpeed = 1;                                              // General Speed for all animations
    public string CharacterClass = "_master";                               // Foldername to the selected char
    public string CharacterSkin = "basic";                                  // Foldername to the selected skin

    // Public GET parameters
    public AnimatorStates currentAnimation { get; private set; }            // current playing animation state

    //Private parameters
    private Dictionary<AnimatorStates, SpriteAtlas> animationSprites;       // Saves all Sprites to the Animations
    private Dictionary<AnimatorStates, float> animationDuration;            // Saves all Speed of the Animations
    private Dictionary<AnimatorStates, bool> animationLoop;                 // Saves if animation should be looped
    
    private bool finishedInitialization = false;                            // True if finished loading all the sprites
    private SpriteRenderer spriteRenderer;                                  // Sprite Renderer

    // All existing animation types
    public enum AnimatorStates
    {
        //Default
        Idle,
        Run,
        Jump,
        Fall,
        Dash,
        Hit,
        //Status
        Stunned,
        KnockedBack,
        Block,
        //Attack
        BasicAttack1,
        BasicAttack2,
        BasicAttack3,
        JumpAttack,
        JumpAttackEnd,
        //Skills
        Skill1,
        Skill2,
        Skill3,
        Skill4,

        //Assassin
        DoubleJump,
        //Knight
        BlockMove,
        DashFor
    }

    void Start()
    {
        // initiate dictionarys
        animationSprites = new Dictionary<AnimatorStates, SpriteAtlas>();
        animationDuration = new Dictionary<AnimatorStates, float>();
        animationLoop = new Dictionary<AnimatorStates, bool>();

        // initiate Components
        spriteRenderer = GetComponent<SpriteRenderer>();

        InitAnimations();
    }

    public void InitAnimations()
    {
        // Save each animation and their attributes in the dictionarys
        for (int i = 0; i < AnimationType.Length; i++)
        {
            animationSprites.Add(AnimationType[i], Atlasses[i]);
            animationDuration.Add(AnimationType[i], AnimationDuration[i]);
            animationLoop.Add(AnimationType[i], AnimationLoop[i]);
        }

        // Save idle animation's ground position
        Sprite[] temp = new Sprite[animationSprites[AnimatorStates.Idle].spriteCount];
        animationSprites[AnimatorStates.Idle].GetSprites(temp);

        // clear working area
        foreach (Sprite sp in temp) Destroy(sp);

        // set finsihed and start first animation
        finishedInitialization = true;
        StartAnimation(AnimatorStates.Idle);
    }

    public void StartAnimation(AnimatorStates animation)
    {
        if (finishedInitialization && currentAnimation != animation)
        {
            // set current animation
            currentAnimation = animation;

            // get sprite atlas
            SpriteAtlas atlas = animationSprites[animation];
            if (atlas == null) return;

            //set speed
            float delay = animationDuration[animation] / (float)atlas.spriteCount;

            // stop running coroutine
            StopAllCoroutines();
            // start next animation as coroutine
            StartCoroutine(PlayAnimation(animation, atlas, delay));
        }
    }

    IEnumerator PlayAnimation(AnimatorStates animation, SpriteAtlas atlas, float delay)
    {
        while (true)
        {
            // play each animation of the atlas
            for (int i = 0; i < atlas.spriteCount; i++)
            {
                if (i < 10) spriteRenderer.sprite = atlas.GetSprite(animation.ToString() + "_" + "0" + i.ToString()); 
                else spriteRenderer.sprite = atlas.GetSprite(animation.ToString() + "_" + i.ToString());

                yield return new WaitForSeconds(delay);
            }

            if (animationLoop[animation])
                //loop
                yield return null;
            else
                //revert to idle if present
                StartAnimation(AnimatorStates.Idle);
        }
    }
}