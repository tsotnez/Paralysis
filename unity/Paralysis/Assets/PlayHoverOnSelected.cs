using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayHoverOnSelected : MonoBehaviour {

	public void MouseEnter()
    {
        transform.Find("Hover").gameObject.SetActive(true);
    }

    public void MouseExit()
    {
        transform.Find("Hover").gameObject.SetActive(false);
    }
}
