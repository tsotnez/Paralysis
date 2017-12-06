using System;
using System.Collections.Generic;

namespace UnityEngine.EventSystems
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
    [RequireComponent(typeof(EventSystem))]
    public abstract class BaseInputModule : UIBehaviour
    {

        [NonSerialized]
        protected List<RaycastResult> m_RaycastResultCache = new List<RaycastResult>();

        private AxisEventData m_AxisEventData;

        private EventSystem m_EventSystem;
        private BaseEventData m_BaseEventData;

        protected BaseInput m_InputOverride;
        private BaseInput m_DefaultInput;

        public BaseInput input
        {
            get
            {
                if (m_InputOverride != null)
                    return m_InputOverride;

                if (m_DefaultInput == null)
                {
                    var inputs = GetComponents<BaseInput>();
                    foreach (var baseInput in inputs)
                    {
                        // We dont want to use any classes that derrive from BaseInput for default.
                        if (baseInput != null && baseInput.GetType() == typeof(BaseInput))
                        {
                            m_DefaultInput = baseInput;
                            break;
                        }
                    }

                    if (m_DefaultInput == null)
                        m_DefaultInput = gameObject.AddComponent<BaseInput>();
                }

                return m_DefaultInput;
            }
        }

        protected EventSystem eventSystem
        {
            get { return m_EventSystem; }
        }

        #region Unity Lifetime calls

        protected override void OnEnable()
        {
            base.OnEnable();
            m_EventSystem = GetComponent<EventSystem>();
            m_EventSystem.UpdateModules();
        }

        protected override void OnDisable()
        {
            m_EventSystem.UpdateModules();
            base.OnDisable();
        }

        #endregion

        public abstract void Process();

        protected static RaycastResult FindFirstRaycast(List<RaycastResult> candidates)
        {
            for (var i = 0; i < candidates.Count; ++i)
            {
                if (candidates[i].gameObject == null)
                    continue;

                return candidates[i];
            }
            return new RaycastResult();
        }

        protected static MoveDirection DetermineMoveDirection(float x, float y)
        {
            return DetermineMoveDirection(x, y, 0.6f);
        }

        protected static MoveDirection DetermineMoveDirection(float x, float y, float deadZone)
        {
            // if vector is too small... just return
            if (new Vector2(x, y).sqrMagnitude < deadZone * deadZone)
                return MoveDirection.None;

            if (Mathf.Abs(x) > Mathf.Abs(y))
            {
                if (x > 0)
                    return MoveDirection.Right;
                return MoveDirection.Left;
            }
            else
            {
                if (y > 0)
                    return MoveDirection.Up;
                return MoveDirection.Down;
            }
        }

        protected static GameObject FindCommonRoot(GameObject g1, GameObject g2)
        {
            if (g1 == null || g2 == null)
                return null;

            var t1 = g1.transform;
            while (t1 != null)
            {
                var t2 = g2.transform;
                while (t2 != null)
                {
                    if (t1 == t2)
                        return t1.gameObject;
                    t2 = t2.parent;
                }
                t1 = t1.parent;
            }
            return null;
        }

        // walk up the tree till a common root between the last entered and the current entered is foung
        // send exit events up to (but not inluding) the common root. Then send enter events up to
        // (but not including the common root).
        protected void HandlePointerExitAndEnter(PointerEventData currentPointerData, GameObject newEnterTarget)
        {
            // DOC-HINT :::::: multi event system support
            // check if the event data does not belong to the eventsystem associated with the module
            if (!this.CompareEventSystemID(currentPointerData))
                // exit method execution
                return;
            // if we have no target / pointerEnter has been deleted
            // just send exit events to anything we are tracking
            // then exit
            if (newEnterTarget == null || currentPointerData.pointerEnter == null)
            {
                for (var i = 0; i < currentPointerData.hovered.Count; ++i)
                    ExecuteEvents.Execute(currentPointerData.hovered[i], currentPointerData, ExecuteEvents.pointerExitHandler);

                currentPointerData.hovered.Clear();

                if (newEnterTarget == null)
                {
                    currentPointerData.pointerEnter = newEnterTarget;
                    return;
                }
            }

            // if we have not changed hover target
            if (currentPointerData.pointerEnter == newEnterTarget && newEnterTarget)
                return;

            GameObject commonRoot = FindCommonRoot(currentPointerData.pointerEnter, newEnterTarget);

            // and we already an entered object from last time
            if (currentPointerData.pointerEnter != null)
            {
                // send exit handler call to all elements in the chain
                // until we reach the new target, or null!
                Transform t = currentPointerData.pointerEnter.transform;

                while (t != null)
                {
                    // if we reach the common root break out!
                    if (commonRoot != null && commonRoot.transform == t)
                        break;

                    ExecuteEvents.Execute(t.gameObject, currentPointerData, ExecuteEvents.pointerExitHandler);
                    currentPointerData.hovered.Remove(t.gameObject);
                    t = t.parent;
                }
            }

            // now issue the enter call up to but not including the common root
            currentPointerData.pointerEnter = newEnterTarget;
            if (newEnterTarget != null)
            {
                Transform t = newEnterTarget.transform;

                while (t != null && t.gameObject != commonRoot)
                {
                    ExecuteEvents.Execute(t.gameObject, currentPointerData, ExecuteEvents.pointerEnterHandler);
                    currentPointerData.hovered.Add(t.gameObject);
                    t = t.parent;
                }
            }
        }

        protected virtual AxisEventData GetAxisEventData(float x, float y, float moveDeadZone)
        {
            if (m_AxisEventData == null)
                m_AxisEventData = new AxisEventData(eventSystem);

            m_AxisEventData.Reset();
            m_AxisEventData.moveVector = new Vector2(x, y);
            m_AxisEventData.moveDir = DetermineMoveDirection(x, y, moveDeadZone);
            return m_AxisEventData;
        }

        protected virtual BaseEventData GetBaseEventData()
        {
            if (m_BaseEventData == null)
                m_BaseEventData = new BaseEventData(eventSystem);

            m_BaseEventData.Reset();
            return m_BaseEventData;
        }

        public virtual bool IsPointerOverGameObject(int pointerId)
        {
            return false;
        }

        public virtual bool ShouldActivateModule()
        {
            return enabled && gameObject.activeInHierarchy;
        }

        public virtual void DeactivateModule()
        {}

        public virtual void ActivateModule()
        {}

        public virtual void UpdateModule()
        {}

        public virtual bool IsModuleSupported()
        {
            return true;
        }

        // DOC-HINT :::::: multi event system support
        /// <summary>
        /// gets the id of the associated EventSystem. (support for multiple EventSystems)
        /// </summary>
        /// <returns>returns the id of the associated EventSystem or otherwise zero as the default</returns>
        protected override int GetEventSystemID()
        {
            // override as it should always return it's own EventSystem'S id
            return m_EventSystem.EventSystemID;
        }
    }
}
