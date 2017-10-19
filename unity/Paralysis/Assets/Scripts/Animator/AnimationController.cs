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
        for (int i = 0; i < AnimationType.Length; i++)
        {
            animationSprites.Add(AnimationType[i], Atlasses[i]);
            animationDuration.Add(AnimationType[i], AnimationDuration[i]);
            animationLoop.Add(AnimationType[i], AnimationLoop[i]);
        }

        // Save idle animation's ground position
        Sprite[] temp = new Sprite[animationSprites[AnimatorStates.Idle].spriteCount];
        animationSprites[AnimatorStates.Idle].GetSprites(temp);

        Sprite idleImage = temp[0];
        idleHeight = idleImage.bounds.extents.y;
        startHeight = transform.localPosition.y;

        // clear working area
        foreach (Sprite sp in temp) Destroy(sp);
        Destroy(idleImage);

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

        SpriteAtlas atlas = animationSprites[animation];
        if (atlas == null) yield break; 

        //fix possible height issues using idle animation as reference(possibly create more)
        //if (animationSprites[AnimatorStates.Idle] != null)
        //{
        //    float groundoffset = sprites[0].bounds.extents.y - idleHeight;
        //    transform.localPosition = new Vector3(transform.localPosition.x, startHeight + groundoffset, transform.localPosition.z);
        //}

        //set speed
        float delay = animationDuration[animation] / (float)atlas.spriteCount;

        //play
        while (true)
        {
            for (int i = 0; i < atlas.spriteCount; i++)
            {
                string imageNumber;
                if (i < 10) imageNumber = "0" + i.ToString();
                else imageNumber = i.ToString();
                spriteRenderer.sprite = atlas.GetSprite(animation.ToString() + "_" + imageNumber);
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