using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CastBarEndBehaviour : MonoBehaviour {

    public Image bar;

	
	// Update is called once per frame
	void Update () {
        Vector3 nextPos = new Vector3();
        nextPos.y = transform.localPosition.y;
        nextPos.z = transform.localPosition.z;
        nextPos.x = bar.rectTransform.rect.width * bar.fillAmount;

        transform.localPosition = nextPos;
    }
}
