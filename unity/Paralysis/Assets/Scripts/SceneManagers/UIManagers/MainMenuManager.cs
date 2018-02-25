using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MenuPagesManager {
    public static int MainMenuDefaultPageIndex = -1;

    protected override void Start()
    {
        DefaultPageIndex = MainMenuDefaultPageIndex;
        base.Start();

    }
}
