using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using MustHave.Utilities;

namespace MustHave.UI
{
    [ExecuteInEditMode]
    public class ViewPagerScript : ScrollRect, IPointerDownHandler, IPointerUpHandler
    {
        private HorizontalLayoutGroup _layoutGroup = default;
        private Coroutine _swipeRoutine = default;

        protected override void Awake()
        {
            vertical = false;
            horizontal = true;
            _layoutGroup = content.GetComponent<HorizontalLayoutGroup>();
            _layoutGroup.childControlWidth = true;
            _layoutGroup.childForceExpandWidth = true;
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
                _swipeRoutine = this.SwipeToGrid(_layoutGroup, 0.2f, 0.6f, () => {
                    _swipeRoutine = null;
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
            if (content && _layoutGroup && (childCount = content.childCount) > 0)
            {
                float contentWidth = viewport.rect.width * childCount + _layoutGroup.spacing * (childCount - 1);
                float contentHeight = viewport.rect.height;
                content.sizeDelta = new Vector2(contentWidth, contentHeight);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            //Debug.Log(GetType() + ".OnPointerDown");
            if (_swipeRoutine != null)
            {
                StopCoroutine(_swipeRoutine);
                inertia = true;
                _swipeRoutine = null;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            //Debug.Log(GetType() + ".OnPointerUp: dragging=" + eventData.dragging);
            if (!eventData.dragging && _swipeRoutine == null)
            {
                OnEndDrag(eventData);
            }
        }
    }
}
