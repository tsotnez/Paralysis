using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NetworkHostGameManager : UIManager {

    protected override void Start()
    {
        base.Start();
        EventSystem.current.firstSelectedGameObject = firstSelected;
    }
}
