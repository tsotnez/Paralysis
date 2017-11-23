using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using System.Linq;

[RequireComponent(typeof(SpriteRenderer))]
public abstract class AnimationController : MonoBehaviour
{
    #region Public parameters for Editor-Inspector

    // Default Properties
    public string CharacterClass = "_master";                               // Foldername to the selected char
    public string CharacterSkin = "basic";                                  // Foldername to the selected skin
    public float GeneralSpeed = 1;                                          // General Speed for all animations
    // Animation Properties
    public AnimatorStates[] AnimationType = { 0 };                          // Type of Animation
    public SpriteAtlas[] Atlasses;                                          // Array of all atlasses
    public bool[] AnimationLoop = { false };                                // Shall animation be looped?
    public float[] AnimationDuration;                                       // Duration of each animation
    // AnimationEnd Properties
    public SpriteAtlas[] EndAtlasses;                                       // Array of all atlasses

    #endregion

    // Public GET parameters
    public AnimatorStates CurrentAnimation { get; private set; }            // current playing animation state

    //Private parameters
    private Dictionary<AnimatorStates, SpriteAtlas> animationSprites;       // Saves all Sprites to the Animations
    private Dictionary<AnimatorStates, SpriteAtlas> animationSpriteEnds;    // Saves all Sprite-End-Animations to the Animations
    private Dictionary<AnimatorStates, float> animationDuration;            // Saves all Speed of the Animations
    private Dictionary<AnimatorStates, bool> animationLoop;                 // Saves if animation should be looped
    private AudioSource audioSource;                                        // Reference to audio source for playing sounds

    private bool finishedInitialization = false;                            // True if finished loading all the sprites
    private SpriteRenderer spriteRenderer;                                  // Sprite Renderer

    #region Enums

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
        //Skills
        Skill1,
        Skill2,
        Skill3,
        Skill4,

        // Assassin
        DoubleJump,
        // Knight & Infantry
        BlockMove,
        // Alchemist
        RunStart,
        // ?
        DashFor,
        Walk,

        // End of the Game
        Die
    }

    public enum TypeOfAnimation
    {
        Animation, EndAnimation
    }

    public enum AnimationPlayTypes
    {
        Single, Loop, HoldOnEnd, Nothing
    }

    #endregion

    #region Init

    void Start()
    {
        // initiate dictionarys
        animationSprites = new Dictionary<AnimatorStates, SpriteAtlas>();
        animationSpriteEnds = new Dictionary<AnimatorStates, SpriteAtlas>();
        animationDuration = new Dictionary<AnimatorStates, float>();
        animationLoop = new Dictionary<AnimatorStates, bool>();
        audioSource = GetComponent<AudioSource>();

        // initiate Components
        spriteRenderer = GetComponent<SpriteRenderer>();

        InitAnimations();
    }

    public void InitAnimations()
    {
        // Save each ANIMATION and their attributes in the dictionarys
        for (int i = 0; i < AnimationType.Length; i++)
        {
            animationSprites.Add(AnimationType[i], Atlasses[i]);
            animationDuration.Add(AnimationType[i], AnimationDuration[i]);
            animationLoop.Add(AnimationType[i], AnimationLoop[i]);

            // Save each END-ANIMATION
            if (EndAtlasses[i] != null)
            {
                animationSpriteEnds.Add(AnimationType[i], EndAtlasses[i]);
            }
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

    #endregion

    #region Manage Animations

    public void StartAnimation(AnimatorStates animation, TypeOfAnimation AnimationType = TypeOfAnimation.Animation, AnimationPlayTypes ForceAnimationPlayType = AnimationPlayTypes.Nothing)
    {
        if (finishedInitialization && (CurrentAnimation != animation || (CurrentAnimation == animation && AnimationType == TypeOfAnimation.EndAnimation)))
        {
            if (AnimationDictionaryHasAnimation(animation, AnimationType))
            {
                // set current animation
                CurrentAnimation = animation;

                // get sprite atlas
                SpriteAtlas atlas = null;
                if (AnimationType == TypeOfAnimation.Animation)
                    atlas = animationSprites[animation];
                else if (AnimationType == TypeOfAnimation.EndAnimation)
                    atlas = animationSpriteEnds[animation];

                //set speed
                float delay = animationDuration[animation] / (float)atlas.spriteCount;

                // Calculating AnimationPlayType
                AnimationPlayTypes AnimationPlayType;
                if (ForceAnimationPlayType == AnimationPlayTypes.Nothing)
                {
                    if (AnimationType == TypeOfAnimation.Animation && AnimationDictionaryHasAnimation(animation, TypeOfAnimation.EndAnimation))
                    {   // if found a matching End-Animation to the choosen animation set the PlayType to HoldOnEnd
                        AnimationPlayType = AnimationPlayTypes.HoldOnEnd;
                    }
                    else if (animationLoop[animation])
                    {
                        AnimationPlayType = AnimationPlayTypes.Loop;
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
                StopAllCoroutines();
                // start next animation as coroutine
                StartCoroutine(PlayAnimation(animation, atlas, delay, AnimationType, AnimationPlayType));
            }
            else
            {
                // If an animation is not found revert to idle to prevent displaying errors
                Debug.Log("Could not start " + AnimationType.ToString() + " '" + animation.ToString() + "' because it is not present in Dictionary!" + "\n" + "Idle-Animation has been started instead.");
                StartAnimation(AnimatorStates.Idle);
            }
        }
    }

    private bool AnimationDictionaryHasAnimation(AnimatorStates Animation, TypeOfAnimation AnimationType)
    {
        if (AnimationType == TypeOfAnimation.Animation)
        {
            if (animationSprites.ContainsKey(Animation) && animationSprites[Animation] != null)
                return true;
        }
        else
        {
            if (animationSpriteEnds.ContainsKey(Animation) && animationSpriteEnds[Animation] != null)
                return true;
        }

        return false;
    }

    IEnumerator PlayAnimation(AnimatorStates animation, SpriteAtlas atlas, float delay, TypeOfAnimation AnimationType, AnimationPlayTypes AnimationPlayType)
    {
        //Handle audio
        AudioClip clip = Resources.Load<AudioClip>("Audio/Characters/" + CharacterClass + "/" + animation.ToString());
        if (clip != null)
        {
            if (audioSource.clip != clip || (audioSource.clip == clip && animationLoop[animation]))
            {
                //New clip or loop
                audioSource.clip = clip;
                audioSource.Play();
            }
            else if (audioSource.clip == clip && !animationLoop[animation])
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

        // Add end to the filename if its an EndAnimation
        string AtlasNameAddition = "";
        if (AnimationType == TypeOfAnimation.EndAnimation)
            AtlasNameAddition = "End";



        while (true)
        {
            Resources.UnloadUnusedAssets(); // Unload assets with no references
            Texture2D tex = null;
            Sprite spr = null;
            // play each animation of the atlas
            for (int i = 0; i < atlas.spriteCount; i++)
            {
                if (i < 10)
                    spriteRenderer.sprite = atlas.GetSprite(animation.ToString() + AtlasNameAddition + "_" + "0" + i.ToString());
                else
                    spriteRenderer.sprite = atlas.GetSprite(animation.ToString() + AtlasNameAddition + "_" + i.ToString());

                Destroy(spr); //Unload sprite 
                spr = null;
                Resources.UnloadAsset(tex); //Unload texture from RAM after it was shown for long enough
                tex = null;
                tex = spriteRenderer.sprite.texture;
                spr = spriteRenderer.sprite;
                yield return new WaitForSeconds(delay);
            }

            if (AnimationPlayType == AnimationPlayTypes.HoldOnEnd)
                while (true)
                {
                    yield return new WaitForSeconds(0.1f);
                }
            else if (AnimationPlayType == AnimationPlayTypes.Loop)
                //loop
                yield return null;
            else
                //revert to idle if present
                StartAnimation(AnimatorStates.Idle);
        }
    }

    #endregion

}