using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HotbarController : MonoBehaviour {

    [SerializeField]
    private Text championName, hpPercent, staminaPercent;
    [SerializeField]
    private Color overlayWhenCooldown;
    public Image spell1Image, spell2Image, spell3Image, spell4Image, basicAttackImage, hpImage, staminaImage;

    public void setChampionName(string name)
    {
        championName.text = name;
    }

    public void setFillAmounts(float hp, float stamina)
    {
        hpImage.fillAmount = hp;
        staminaImage.fillAmount = stamina;

        hpPercent.text = hp * 100 + "%";
        staminaPercent.text = stamina * 100 + "%";
    }

    public void initAbilityImages(string name)
    {
        spell1Image.sprite = Resources.Load<Sprite>("Sprites/AbilityImages/" + name + "/Skill1");
        spell2Image.sprite = Resources.Load<Sprite>("Sprites/AbilityImages/" + name + "/Skill2");
        spell3Image.sprite = Resources.Load<Sprite>("Sprites/AbilityImages/" + name + "/Skill3");
        spell4Image.sprite = Resources.Load<Sprite>("Sprites/AbilityImages/" + name + "/Skill4");
        basicAttackImage.sprite = Resources.Load<Sprite>("Sprites/AbilityImages/" + name + "/basic attack");
    }

    public void setOnCooldown(AnimationController.AnimatorStates skill, float seconds)
    {
        Image skillImage = null;

        switch (skill) //Get image 
        {
            case AnimationController.AnimatorStates.Skill1:
                skillImage = spell1Image;
                break;
            case AnimationController.AnimatorStates.Skill2:
                skillImage = spell2Image;
                break;
            case AnimationController.AnimatorStates.Skill3:
                skillImage = spell3Image;
                break;
            case AnimationController.AnimatorStates.Skill4:
                skillImage = spell4Image;
                break;
            default:
                return;
        }

        StartCoroutine(countDown(skillImage, seconds)); //Start coroutine
    }

    private IEnumerator countDown(Image spell, float seconds)
    {
        Text text = spell.transform.Find("CooldownText").GetComponent<Text>(); //Make cooldown text visible
        text.text = seconds.ToString();
        text.gameObject.SetActive(true);

        spell.color = overlayWhenCooldown; //Enable overlay to grey out skill

        float counter = seconds;
        while(counter > 0) //Count down
        {
            text.text = counter.ToString();
            yield return new WaitForSeconds(1);
            counter--;
        }

        spell.color = Color.white; //Reset image and text
        text.gameObject.SetActive(false);

    }
}
