using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HotbarController : MonoBehaviour {

    [SerializeField]
    private Text championName, hpPercent, staminaPercent;
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
}
