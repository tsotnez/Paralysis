using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HotbarController : MonoBehaviour {

    [SerializeField]
    private Text championName, hpPercent, staminaPercent;
    [SerializeField]
    private Color overlayWhenCooldown;
    public Image spell1Image, spell2Image, spell3Image, spell4Image, basicAttackImage, hpImage, staminaImage, trinket1Image, trinket2Image;

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

    public void initTrinketImages (string trinket1, string trinket2)
    {
        trinket1Image.sprite = Resources.Load<Sprite>("Sprites/AbilityImages/Trinkets/" + trinket1);
        trinket2Image.sprite = Resources.Load<Sprite>("Sprites/AbilityImages/Trinkets/" + trinket2);
    }

    public void setOnCooldown(AnimationController.AnimatorStates skill, float seconds)
    {
        Image skillImage = getImageForSkill(skill);
        StartCoroutine(countDown(skillImage, seconds)); //Start coroutine
    }

    private Image getImageForSkill(AnimationController.AnimatorStates skill)
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
            case AnimationController.AnimatorStates.BasicAttack1:
            case AnimationController.AnimatorStates.BasicAttack2:
            case AnimationController.AnimatorStates.BasicAttack3:
            case AnimationController.AnimatorStates.JumpAttack:
                skillImage = basicAttackImage;
                break;
        }
        return skillImage;
    }

    //Sets trinket on Cooldown
    public void setTrinketOnCooldown(int trinketNumber, int seconds)
    {
        if(trinketNumber == 1)
        {
            StartCoroutine(countDown(trinket1Image, seconds));
        }
        else
        {
            StartCoroutine(countDown(trinket2Image, seconds));
        }
    }

    //Greys out given trinket
    public void greyOutTrinket(int trinketNumber)
    {
        if (trinketNumber == 1)
        {
            trinket1Image.color = overlayWhenCooldown;
        }
        else
        {
            trinket2Image.color = overlayWhenCooldown;
        }
    }
    
    //Sets image on cooldown
    private IEnumerator countDown(Image spell, float seconds)
    {
        Text text = spell.transform.Find("CooldownText").GetComponent<Text>(); //Make cooldown text visible
        text.text = seconds.ToString();
        text.gameObject.SetActive(true);

        float counter = seconds;
        while(counter > 0) //Count down
        {
            spell.color = overlayWhenCooldown;
            text.text = counter.ToString();
            yield return new WaitForSeconds(1);
            counter--;
        }

        spell.color = Color.white; //Reset image and text
        text.gameObject.SetActive(false);

    }

    //The image flashes black for a short time, highlighting that the skill/trinket was used
    public IEnumerator flashBlack(AnimationController.AnimatorStates skill)
    {
        Image spell = getImageForSkill(skill);
        Color colorBefore = spell.color;

        spell.color = new Color(0, 0, 0, .5f);
        yield return new WaitForSeconds(.15f);
        spell.color = colorBefore;
    }
}
