using System;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace UnityEngine.UI
{
    /* =======================
     * change-log:     
	 * [2017/01/04]
	 *		updated sources to 5.5.0     
     * [2015/06/09]
     *      updated sources from 4.6 branch to 5.0.2f 
     * [2015/03/24]
     *      added comments to the multi event system support code
     *      added multi event system support (affected code is marked with "DOC-HINT :::::: multi event system support" comment)
    */
    // Button that's meant to work with mouse or touch-based devices.
    [AddComponentMenu("UI/Button", 30)]
    public class Button : Selectable, IPointerClickHandler, ISubmitHandler
    {
        [Serializable]
        public class ButtonClickedEvent : UnityEvent {}

        // Event delegates triggered on click.
        [FormerlySerializedAs("onClick")]
        [SerializeField]
        private ButtonClickedEvent m_OnClick = new ButtonClickedEvent();

        protected Button()
        {}

        public ButtonClickedEvent onClick
        {
            get { return m_OnClick; }
            set { m_OnClick = value; }
        }

        private void Press()
        {
            if (!IsActive() || !IsInteractable())
                return;

            m_OnClick.Invoke();
        }

        // Trigger all registered callbacks.
        public virtual void OnPointerClick(PointerEventData eventData)
        {
			// DOC-HINT :::::: multi event system support
            // check if the event data does not belong to the eventsystem associated with the module
            if (!this.CompareEventSystemID(eventData))
                // exit method execution
                return;

            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            Press();
        }

        public virtual void OnSubmit(BaseEventData eventData)
        {
            // DOC-HINT :::::: multi event system support
            // check if the event data does not belong to the eventsystem associated with the module
            if (!this.CompareEventSystemID(eventData))
                // exit method execution
                return;

            Press();

            // if we get set disabled during the press
            // don't run the coroutine.
            if (!IsActive() || !IsInteractable())
                return;

            DoStateTransition(SelectionState.Pressed, false);
            StartCoroutine(OnFinishSubmit());
        }

        private IEnumerator OnFinishSubmit()
        {
            var fadeTime = colors.fadeDuration;
            var elapsedTime = 0f;

            while (elapsedTime < fadeTime)
            {
                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }

            DoStateTransition(currentSelectionState, false);
        }
    }
}
