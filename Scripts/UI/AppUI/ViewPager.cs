using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using MustHave.Utils;

namespace MustHave.UI
{
    [ExecuteInEditMode]
    public class ViewPager : ScrollRect, IPointerDownHandler, IPointerUpHandler
    {
        private HorizontalLayoutGroup layoutGroup = default;
        private Coroutine swipeRoutine = default;

        protected override void Awake()
        {
            vertical = false;
            horizontal = true;
            layoutGroup = content.GetComponent<HorizontalLayoutGroup>();
            layoutGroup.childControlWidth = true;
            layoutGroup.childForceExpandWidth = true;
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            base.OnBeginDrag(eventData);
            inertia = true;
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);
            SwipeToGrid(0.2f, 0.6f);
        }

        private void SwipeToGrid(float minDuration, float maxDuration)
        {
            int childCount = content.childCount;
            if (childCount > 1)
            {
                swipeRoutine = this.SwipeToGrid(layoutGroup, 0.2f, 0.6f, () => {
                    swipeRoutine = null;
                });
            }
        }

        protected override void OnEnable()
        {
            if (EditorApplicationUtils.IsInEditMode)
            {
                OnRectTransformDimensionsChange();
            }
        }

        public void ClearContent()
        {
            content.DestroyAllChildren();
        }

        public void UpdateContent()
        {
            OnRectTransformDimensionsChange();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            int childCount;
            if (content && layoutGroup && (childCount = content.childCount) > 0)
            {
                float contentWidth = viewport.rect.width * childCount + layoutGroup.spacing * (childCount - 1);
                float contentHeight = viewport.rect.height;
                content.sizeDelta = new Vector2(contentWidth, contentHeight);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            //Debug.Log(GetType() + ".OnPointerDown");
            if (swipeRoutine != null)
            {
                StopCoroutine(swipeRoutine);
                inertia = true;
                swipeRoutine = null;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            //Debug.Log(GetType() + ".OnPointerUp: dragging=" + eventData.dragging);
            if (!eventData.dragging && swipeRoutine == null)
            {
                OnEndDrag(eventData);
            }
        }
    }
}
