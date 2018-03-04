using UnityEngine;
using System.Collections;

public class FrameRateCounter : MonoBehaviour
{
    #if UNITY_EDITOR
    float deltaTime = 0.0f;
    void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
    }

    void OnGUI()
    {
        if (Time.timeScale != 0)
        {
            int w = Screen.width, h = Screen.height;
            GUIStyle style = new GUIStyle();
            Rect rect = new Rect(0,5, w, 15);
            style.alignment = TextAnchor.LowerLeft;
            style.fontSize = h * 2 / 130;
            style.normal.textColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            float msec = deltaTime * 1000.0f;
            float fps = 1.0f / deltaTime;
            string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
            GUI.Label(rect, text, style);
        }
    }
    #endif
}