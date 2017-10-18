using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using System.Linq;

[RequireComponent(typeof(SpriteRenderer))]
public class AnimationController : MonoBehaviour
{

    public AnimatorStates[] AnimationType = { 0 };
    public bool[] AnimationLoop = { false };
    public float[] AnimationDuration;
    public SpriteAtlas[] Atlasses;
    public float GeneralSpeed;
    public string CharacterClass = "_master";
    public string CharacterSkin = "basic";

    public bool finishedInitialization = false;  //True if finished loading all the sprites

    public AnimatorStates currentAnimation { get; private set; } // current playing animation state

    string spritesPath;
    float idleHeight;
    float startHeight;
    SpriteRenderer spriteRenderer;

    Dictionary<AnimatorStates, SpriteAtlas> animationSprites;  // Saves all Sprites to the Animations
    Dictionary<AnimatorStates, float> animationDuration;    // Saves all Speed of the Animations
    Dictionary<AnimatorStates, bool> animationLoop;         // Saves if animation should be looped

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
        animationSprites = new Dictionary<AnimatorStates, SpriteAtlas>();
        animationDuration = new Dictionary<AnimatorStates, float>();
        animationLoop = new Dictionary<AnimatorStates, bool>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        spritesPath = "Animations/chars/" + CharacterClass + "/" + CharacterSkin + "/";
        InitAnimations();
    }

    public void InitAnimations()
    {
        // Load all animation sprites on start for better performance
        //for (int i = 0; i < AnimationType.Length; i++)
        //{
        //    // Save sprites to Dictionary
        //    Sprite[] sp = Resources.LoadAll<Sprite>(spritesPath + AnimationType[i].ToString());

        //    if (sp.Length > 0)
        //    {
        //        animationSprites.Add(AnimationType[i], sp);
        //        animationDuration.Add(AnimationType[i], AnimationDuration[i]);
        //        animationLoop.Add(AnimationType[i], AnimationLoop[i]);
        //    }
        //}

        for (int i = 0; i < AnimationType.Length; i++)
        {
            animationSprites.Add(AnimationType[i], Atlasses[i]);
            animationDuration.Add(AnimationType[i], AnimationDuration[i]);
            animationLoop.Add(AnimationType[i], AnimationLoop[i]);
        }

        //Load idle animation
        loadAnimationFromResources(AnimatorStates.Idle);

        // Save idle animation's ground position
        Sprite[] temp = new Sprite[animationSprites[AnimatorStates.Idle].spriteCount];
        animationSprites[AnimatorStates.Idle].GetSprites(temp);

        Sprite idleImage = temp[0];
        idleHeight = idleImage.bounds.extents.y;
        startHeight = transform.localPosition.y;

        finishedInitialization = true;
        StartAnimation(AnimatorStates.Idle);
    }

    public void StartAnimation(AnimatorStates animation)
    {
        if (finishedInitialization && currentAnimation != animation)
        {
            StopAllCoroutines();
            StartCoroutine(PlayAnimation(animation));
        }
    }

    IEnumerator PlayAnimation(AnimatorStates animation)
    {
        currentAnimation = animation;
        Sprite[] sprites = loadAnimationFromResources(animation);

        //fix possible height issues using idle animation as reference(possibly create more)
        if (animationSprites[AnimatorStates.Idle] != null)
        {
            float groundoffset = sprites[0].bounds.extents.y - idleHeight;
            transform.localPosition = new Vector3(transform.localPosition.x, startHeight + groundoffset, transform.localPosition.z);
        }

        //set speed
        float delay = animationDuration[animation] / (float)sprites.Length;

        //play
        while (true)
        {
            foreach (Sprite sp in sprites)
            {
                spriteRenderer.sprite = sp;
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

    private Sprite[] loadAnimationFromResources(AnimatorStates animation)
    {
        Sprite[] sprites = new Sprite[10];
        try
        { sprites = new Sprite[animationSprites[animation].spriteCount]; }
        catch
        {
        }
        //if (animationSprites.ContainsKey(animation))
        //{
            //Animation was loaded already
            animationSprites[animation].GetSprites(sprites);
        //}
        //else
        //{
        //    //Load animation
        //    Sprite[] sp = Resources.LoadAll<Sprite>(spritesPath + animation.ToString());
        //    int index = Array.IndexOf(AnimationType, animation); //index of animation to get correct loop and duration

        //    if (sp.Length > 0)
        //    {
        //        //Save into dictionaries
        //        animationSprites.Add(animation, sp);
        //        animationDuration.Add(animation, AnimationDuration[index]);
        //        animationLoop.Add(animation, AnimationLoop[index]);
        //    }
        //    sprites = sp;
        //}

        return sprites;
    }


}