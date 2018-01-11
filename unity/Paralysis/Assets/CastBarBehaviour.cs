using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CastBarBehaviour : MonoBehaviour {

    public Image bar;
    public Transform end;
    public Text skillName;

    bool runnning = false;
    float maxTime = 0;
    float currentTime = 0;
	
	// Update is called once per frame
	void Update () {
        if (runnning)
        {
            currentTime = Mathf.Clamp(currentTime + Time.deltaTime, 0,  maxTime);
            bar.fillAmount = currentTime / maxTime;

            Vector3 nextPos = new Vector3();
            nextPos.y = end.transform.localPosition.y;
            nextPos.z = end.transform.localPosition.z;
            nextPos.x = bar.rectTransform.rect.width * bar.fillAmount;

            end.transform.localPosition = nextPos;

            if(currentTime == maxTime)
            {
                runnning = false;

                currentTime = 0;
                bar.fillAmount = 0;
                end.position = new Vector3(0, end.position.y, end.position.z);

                changeActive(false);
            }
        }
    }

    public void startCast(float time, string name)
    {
        skillName.text = name;
        changeActive(true);

        maxTime = time;
        runnning = true;
    }

    void changeActive(bool state)
    {
        GetComponent<Image>().enabled = state;

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(state);
        }
    }
}
