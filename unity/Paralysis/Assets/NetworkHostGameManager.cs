using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NetworkHostGameManager : UIManager {

    public GameObject firstSelected;

    protected override void Start()
    {
        base.Start();
        EventSystem.current.firstSelectedGameObject = firstSelected;
    }
}
