using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class ColorBar : MonoBehaviour {

    [SerializeField]
    public GameObject coolSkill;
    private Color fullcolor;
    private Color lowcolor;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void SkillUsed()
    {
       Image content = coolSkill.GetComponent<Image>();

        //content.color = Color.Lerp(fullcolor, lowcolor, content.)
    }

}
