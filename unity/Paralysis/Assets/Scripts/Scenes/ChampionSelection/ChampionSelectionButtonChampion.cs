using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Parent class of all buttons supposed to set champions
/// </summary>
public class ChampionSelectionButtonChampion : ChampionSelectionButton {

    protected Transform popup;
    protected Transform skills;

    protected GameObject OnClickAnimation; //GO to be instantiated when player choses a champion (--> epicness increased)

    protected GameObject ChampionPrefab; //Prefab of that champion
    public ChampionDatabase.Champions Champion; //The champion this button sets


    protected override void Start()
    {
        base.Start();
        OnClickAnimation = Resources.Load<GameObject>("Prefabs/UI/LocalChampionSelection/ClickAnimation");
    }

    protected virtual void showSkillPopUps()
    {
        //Set Skill Images and text
        setSkillInfos();
        popup.gameObject.SetActive(true);
    }

    public override void Selecting()
    {
        base.Selecting();      
        portrait.switchTo(1);
    }

    public override void Deselecting()
    {
        base.Deselecting();

        if(!currentlySelected)
            portrait.switchTo(0);
    }

    public override void onClick()
    {
        base.onClick();
        StartCoroutine(handleOnclickAnimation()); //Show on click aniamtion for champion
    }

    public override void loseFocus()
    {
        base.loseFocus();
        portrait.switchTo(0);
    }

    /// <summary>
    /// Sets skill texts
    /// </summary>
    /// <param name="skills"></param>
    protected void setSkillInfos()
    {
        int counter = 1;
        foreach (Transform preview in skills)
        {
            ChampionClassController controller = null;
            if (ChampionPrefab != null)
                controller = ChampionPrefab.GetComponent<ChampionClassController>();
            else
                Debug.Log(Champion);

            switch (counter)
            {
                case 1:
                    preview.Find("Image").gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/AbilityImages/" + controller.className + "/Skill1");
                    preview.Find("SkillName").gameObject.GetComponent<Text>().text = controller.GetSkillByType(Skill.SkillType.Skill1).skillName;
                    preview.Find("Desc").gameObject.GetComponent<Text>().text = controller.GetSkillByType(Skill.SkillType.Skill1).skillDescription.Replace("\n", "");
                    break;
                case 2:
                    preview.Find("Image").gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/AbilityImages/" + controller.className + "/Skill2");
                    preview.Find("SkillName").gameObject.GetComponent<Text>().text = controller.GetSkillByType(Skill.SkillType.Skill2).skillName;
                    preview.Find("Desc").gameObject.GetComponent<Text>().text = controller.GetSkillByType(Skill.SkillType.Skill2).skillDescription.Replace("\n", "");
                    break;
                case 3:
                    preview.Find("Image").gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/AbilityImages/" + controller.className + "/Skill3");
                    preview.Find("SkillName").gameObject.GetComponent<Text>().text = controller.GetSkillByType(Skill.SkillType.Skill3).skillName;
                    preview.Find("Desc").gameObject.GetComponent<Text>().text = controller.GetSkillByType(Skill.SkillType.Skill3).skillDescription.Replace("\n", "");
                    break;
                case 4:
                    preview.Find("Image").gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/AbilityImages/" + controller.className + "/Skill4");
                    preview.Find("SkillName").gameObject.GetComponent<Text>().text = controller.GetSkillByType(Skill.SkillType.Skill4).skillName;
                    preview.Find("Desc").gameObject.GetComponent<Text>().text = controller.GetSkillByType(Skill.SkillType.Skill4).skillDescription.Replace("\n", "");
                    break;
            }
            counter++;
        }
    }

    /// <summary>
    /// Instantiates Object which plays the on click animation and destroys it after animation has finished
    /// </summary>
    /// <returns></returns>
    private IEnumerator handleOnclickAnimation()
    {
        GameObject anim = Instantiate(OnClickAnimation, transform.parent, false);
        yield return new WaitForSeconds(.333f);
        Destroy(anim);
    }
}
