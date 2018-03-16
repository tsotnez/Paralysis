﻿using System;
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

    public void setOnCooldown(Skill.SkillType skill, float seconds)
    {
        Image skillImage = getImageForSkill(skill);
        StartCoroutine(countDown(skillImage, seconds)); //Start coroutine
    }

    private Image getImageForSkill(Skill.SkillType skill)
    {
        Image skillImage = null;
        switch (skill) //Get image 
        {
            case Skill.SkillType.Skill1:
                skillImage = spell1Image;
                break;
            case Skill.SkillType.Skill2:
                skillImage = spell2Image;
                break;
            case Skill.SkillType.Skill3:
                skillImage = spell3Image;
                break;
            case Skill.SkillType.Skill4:
                skillImage = spell4Image;
                break;
            case Skill.SkillType.BasicAttack1:
            case Skill.SkillType.BasicAttack2:
            case Skill.SkillType.BasicAttack3:
            case Skill.SkillType.JumpAttack:
                skillImage = basicAttackImage;
                break;
        }
        return skillImage;
    }

    public void trinketUsed(int trinketNumber)
    {
        Image target;

        if (trinketNumber == 1)
            target = trinket1Image;
        else
            target = trinket2Image;

        target.color = overlayWhenCooldown;
    }

    public void trinketDurationOver(int trinketNumber, float cooldown)
    {
        Image target;

        if (trinketNumber == 1)
            target = trinket1Image;
        else
            target = trinket2Image;

        target.color = Color.white;
        StartCoroutine(countDown(target, cooldown));
    }
    
    //Sets image on cooldown
    private IEnumerator countDown(Image spell, float seconds)
    {
        Text text = spell.transform.Find("CooldownText").GetComponent<Text>(); //Make cooldown text visible
        text.text = seconds.ToString();
        text.gameObject.SetActive(true);
        Image overlay = spell.transform.Find("Overlay").gameObject.GetComponent<Image>();
        overlay.enabled = true;

        float counter = seconds;
        while(counter > 0) //Count down
        {
            overlay.fillAmount = counter / seconds;
            text.text = counter.ToString();
            yield return new WaitForSeconds(1);
            counter--;
        }

        overlay.enabled = false;
        text.gameObject.SetActive(false);

    }

    //The image flashes black for a short time, highlighting that the skill/trinket was used
    public IEnumerator flashBlack(Skill.SkillType skill)
    {
        Image spell = getImageForSkill(skill);
        if (spell != null)
        {
            Color colorBefore = spell.color;

            spell.color = new Color(0, 0, 0, .5f);
            yield return new WaitForSeconds(.15f);
            spell.color = colorBefore;
        }
    }

    /// <summary>
    /// Resets everything so the game may be restarted
    /// </summary>
    public void resetValues()
    {
        StopAllCoroutines();

        resetImage(spell1Image);
        resetImage(spell2Image);
        resetImage(spell3Image);
        resetImage(spell4Image);
        resetImage(basicAttackImage);
        resetImage(trinket1Image, true);
        resetImage(trinket2Image, true);
    }

    private void resetImage(Image spell, bool trinket = false)
    {
        Text text;
        Image overlay;

        overlay = spell.transform.Find("Overlay").gameObject.GetComponent<Image>();

        if (spell != basicAttackImage)
        {
            text = spell.transform.Find("CooldownText").GetComponent<Text>();
            text.gameObject.SetActive(false);
        }

        overlay.enabled = false;

        spell.color = Color.white;
    }
}
