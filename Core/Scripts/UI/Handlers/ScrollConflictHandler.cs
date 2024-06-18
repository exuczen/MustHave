using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MustHave.UI
{
    [RequireComponent(typeof(ScrollRect))]
    public class ScrollConflictHandler : UIBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        private ScrollRect parentScrollRect = default;
        private ScrollRect scrollRect = default;
        private ScrollRect activeScrollRect = default;

        private bool ParentScrollRectVertical => parentScrollRect ? parentScrollRect.vertical && !parentScrollRect.horizontal : false;
        private bool ParentScrollRectHorizontal => parentScrollRect ? parentScrollRect.horizontal && !parentScrollRect.vertical : false;
        private bool ScrollRectVertical => scrollRect ? scrollRect.vertical && !scrollRect.horizontal : false;
        private bool ScrollRectHorizontal => scrollRect ? scrollRect.horizontal && !scrollRect.vertical : false;

        protected override void Awake()
        {
            parentScrollRect = transform.GetComponentInParents<ScrollRect>();
            activeScrollRect = scrollRect = GetComponent<ScrollRect>();
            //Debug.Log(GetType() + ".Awake: scrollRect=" + scrollRect);
            //Debug.Log(GetType() + ".Awake: parentScrollRect=" + parentScrollRect);
        }

        private void SetScrollRectsEnabled(bool enabled)
        {
            if (parentScrollRect)
            {
                parentScrollRect.enabled = enabled;
            }
            if (scrollRect)
            {
                scrollRect.enabled = enabled;
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            float horizontal = eventData.position.x - eventData.pressPosition.x;
            float vertical = eventData.position.y - eventData.pressPosition.y;
            float absHorizontal = Mathf.Abs(horizontal);
            float absVertical = Mathf.Abs(vertical);

            if (absHorizontal > absVertical)
            {
                if (ScrollRectVertical && ParentScrollRectHorizontal)
                {
                    activeScrollRect = parentScrollRect;
                }
                else if (ScrollRectHorizontal && ParentScrollRectVertical)
                {
                    activeScrollRect = scrollRect;
                }
            }
            else //if (absHorizontal <= absVertical)
            {
                if (ScrollRectVertical && ParentScrollRectHorizontal)
                {
                    activeScrollRect = scrollRect;
                }
                else if (ScrollRectHorizontal && ParentScrollRectVertical)
                {
                    activeScrollRect = parentScrollRect;
                }
            }
            SetScrollRectsEnabled(false);
            if (activeScrollRect)
            {
                activeScrollRect.enabled = true;
                activeScrollRect.OnBeginDrag(eventData);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (activeScrollRect)
            {
                activeScrollRect.OnDrag(eventData);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (parentScrollRect)
            {
                parentScrollRect.OnEndDrag(eventData);
            }
            if (scrollRect)
            {
                scrollRect.OnEndDrag(eventData);
            }

            activeScrollRect = scrollRect;
            this.StartCoroutineActionAfterFrames(() => {
                SetScrollRectsEnabled(true);
            }, 1);
        }
    }
}
