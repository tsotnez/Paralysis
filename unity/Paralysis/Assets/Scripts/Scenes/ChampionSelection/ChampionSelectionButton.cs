using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

/// <summary>
/// Parent class of all button scripts used in ChampionSelection Scenes including the ViewChampions scene.
/// </summary>
public class ChampionSelectionButton : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, ISubmitHandler
{
    //True while this button is selected, so locked in (only one at a time for a champion and 2 for a trinket)
    [HideInInspector]
    public bool currentlySelected = false;

    protected Image img;
    protected LocalChampionSelectionPortrait portrait;
    protected Text text;

    //[SerializeField]
    //protected PlayerTargetValue targetValue = PlayerTargetValue.Trinket; //Will this Button define trinket, champion or skin

    //protected enum PlayerTargetValue
    //{
    //    Trinket, Champion, Skin
    //}

    protected virtual void Start()
    {
        img = GetComponent<Image>();
        portrait = transform.parent.Find("portrait").gameObject.GetComponent<LocalChampionSelectionPortrait>();
        text = GetComponentInChildren<Text>();
    }


    /// <summary>
    /// Lose currently selected status
    /// </summary>
    public virtual void loseFocus()
    {
        currentlySelected = false;
        text.enabled = false;
    }

    /// <summary>
    /// Show frame image and transition to colored image
    /// </summary>
    public virtual void Selecting()
    {
    }

    /// <summary>
    /// Hideframe image and transition to shadow image
    /// </summary>
    public virtual void Deselecting()
    {
              //  if (!isSelectedByOtherPlayer() && !currentlySelected) //Only transition to shadow image if no other player higlights this button
    }

    /// <summary>
    /// Set the champion/trinket behind this button as selected champion/trinket and show preview by calling methods from manager class
    /// </summary>
    public virtual void onClick()
    {
        if (currentlySelected == false)
        {


        }
    }

    #region Interfaces
    public void OnSelect(BaseEventData eventData)
    {
        Selecting();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        Deselecting();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Selecting();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Deselecting();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onClick();
    }

    public void OnSubmit(BaseEventData eventData)
    {
        onClick();
    }
    #endregion
}
