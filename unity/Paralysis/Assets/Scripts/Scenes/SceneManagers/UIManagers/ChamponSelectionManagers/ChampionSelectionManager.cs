using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Parent class for champion selection managers
/// </summary>
public abstract class ChampionSelectionManager : UIManager
{
    public Dictionary<ChampionDatabase.Champions, GameObject> ChampionPrefabs = new Dictionary<ChampionDatabase.Champions, GameObject>();

    protected virtual void Awake()
    {
        GameObject[] prefabs = Resources.LoadAll<GameObject>("Prefabs/Champions");

        foreach (GameObject prefab in prefabs)
        {
            ChampionPrefabs.Add((ChampionDatabase.Champions)Enum.Parse(typeof(ChampionDatabase.Champions), prefab.name, true), prefab);
        }
    }

    /// <summary>
    /// Changes the Champions prefab to fit a preview by removing unnecessary stuff and setting new values
    /// </summary>
    /// <param name="Champion"></param>
    /// <param name="parent"></param>
    protected void ShowPrefab(GameObject Champion, Transform parent, bool flip, bool playAnimation = true)
    {
        Champion.SetActive(false);
        GameObject newPreview = Instantiate(Champion, parent, false);
        GameObject graphics = newPreview.transform.Find("graphics").gameObject;

        Destroy(newPreview.GetComponent<ChampionClassController>());
        Destroy(newPreview.GetComponent<Rigidbody2D>());
        Destroy(newPreview.GetComponent<CharacterStats>());
        Destroy(newPreview.GetComponent<UserControl>());
        Destroy(newPreview.GetComponent<BoxCollider2D>());
        Destroy(newPreview.GetComponent<CircleCollider2D>());
        Destroy(newPreview.transform.Find("GroundCheck").gameObject);
        Destroy(newPreview.transform.Find("Canvas").gameObject);
        Destroy(newPreview.transform.Find("stunnedSymbol").gameObject);
        Destroy(graphics.GetComponent<GraphicsNetwork>());
        Destroy(newPreview.GetComponent<NetworkCharacter>());


        //flip sprite if necessary
        int direction = 1;
        if (flip)
            direction = -1;

        newPreview.transform.localScale = new Vector3(200 * direction, 200, 1); //Scale up
        newPreview.transform.position = new Vector3(newPreview.transform.position.x, newPreview.transform.position.y + 1.4f, newPreview.transform.position.z);
        graphics.GetComponent<ChampionAnimationController>().propGrounded = true;
        if(playAnimation)
            graphics.GetComponent<ChampionAnimationController>().trigBasicAttack1 = true;
        newPreview.SetActive(true);
        Champion.SetActive(true);
    }

    /// <summary>
    /// Remove existent Preview GameObject by destroying all children of a Platform
    /// </summary>
    /// <param name="parent"></param>
    protected void DestroyExistingPreview(Transform parent)
    {
        foreach (Transform child in parent)
        {
            if (child.gameObject.tag == GameConstants.MAIN_PLAYER_TAG)
                Destroy(child.gameObject);
        }
    }

    /// <summary>
    /// Passes info to next manager and starts the actual game
    /// </summary>
    public virtual void startGame(string mapName) { }
    public virtual void startGame() { }
    /// <summary>
    /// Sets the champion for the player
    /// </summary>
    public virtual void setChampion(UserControl.PlayerNumbers targetPlayer, GameObject Champion) { }
    /// <summary>
    /// Sets trinket 1
    /// </summary>
    public virtual void setTrinket1(UserControl.PlayerNumbers targetPlayer, Trinket.Trinkets trinketName) { }
    /// <summary>
    /// Sets trinket 2
    /// </summary>
    public virtual void setTrinket2(UserControl.PlayerNumbers targetPlayer, Trinket.Trinkets trinketName) { }
    /// <summary>
    /// Overwrites a passed trinket with a new one
    /// </summary>
    public virtual void setTrinket(UserControl.PlayerNumbers targetPlayer, Trinket.Trinkets trinketName, Trinket.Trinkets toOverwrite) { }
}
