﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class gAnimationController : MonoBehaviour
{
    //animations located in /Assets/Resources/Animations
    SpriteRenderer spriteRenderer;
    string idleAnimation; // idle is needed to align all other animations to the "ground"
    public AnimatorStates[] AnimationType = { 0 };
    public bool[] AnimationLoop = { true };
    public float[] AnimationDuration;
    public float GeneralSpeed;
    public string CharacterClass, CharacterSkin;//in Resources folder

    string spritesPath;
    float idleHeight;
    float startHeight;

    Dictionary<AnimatorStates, Sprite[]> animationSprites; // Saves all Sprites to the Animations
    Dictionary<AnimatorStates, float> animationDuration; // Saves all Speed of the Animations
    Dictionary<AnimatorStates, bool> animationLoop; // Saves if animation should be looped

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
        BlockMove
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spritesPath = "Animations/chars/" + CharacterClass + "/" + CharacterSkin + "/";
        InitAnimations();
    }

    public void InitAnimations()
    {
        // Load all animation sprites on start for better performance
        for (int i = 0; i < AnimationType.Length; i++)
        {
            // Save sprites to Dictionary
            animationSprites.Add(AnimationType[i], Resources.LoadAll<Sprite>(spritesPath + AnimationType[i].ToString()));
            animationDuration.Add(AnimationType[i], AnimationDuration[i]);
            animationLoop.Add(AnimationType[i], AnimationLoop[i]);
        }

        // Save idle animation's ground position
        Sprite idleImage = animationSprites[AnimatorStates.Idle][0];
        idleHeight = idleImage.bounds.extents.y;
        startHeight = transform.localPosition.y;
    }

    public void StartAnimation(AnimatorStates animation)
    {
        StopAllCoroutines();
        StartCoroutine(PlayAnimation(animation));
    }

    IEnumerator PlayAnimation(AnimatorStates animation)
    {
        Sprite[] sprites = animationSprites[animation];
        //fix possible height issues using idleanimation as reference(possibly create more)
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
            else if (idleAnimation != "")
                //revert to idle if present
                StartAnimation(AnimatorStates.Idle);
            else
                //stop
                yield break;
        }
    }
}
