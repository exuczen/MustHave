using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using MustHave.Utilities;

namespace MustHave.UI
{
    [RequireComponent(typeof(ScrollRect))]
    public class ScrollConflictHandler : UIBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        private ScrollRect _parentScrollRect = default;
        private ScrollRect _scrollRect = default;
        private ScrollRect _activeScrollRect = default;

        private bool ParentScrollRectVertical => _parentScrollRect ? _parentScrollRect.vertical && !_parentScrollRect.horizontal : false;
        private bool ParentScrollRectHorizontal => _parentScrollRect ? _parentScrollRect.horizontal && !_parentScrollRect.vertical : false;
        private bool ScrollRectVertical => _scrollRect ? _scrollRect.vertical && !_scrollRect.horizontal : false;
        private bool ScrollRectHorizontal => _scrollRect ? _scrollRect.horizontal && !_scrollRect.vertical : false;

        protected override void Awake()
        {
            _parentScrollRect = transform.GetComponentInParents<ScrollRect>();
            _activeScrollRect = _scrollRect = GetComponent<ScrollRect>();
            //Debug.Log(GetType() + ".Awake: _scrollRect=" + _scrollRect);
            //Debug.Log(GetType() + ".Awake: _parentScrollRect=" + _parentScrollRect);
        }

        private void SetScrollRectsEnabled(bool enabled)
        {
            if (_parentScrollRect)
                _parentScrollRect.enabled = enabled;
            if (_scrollRect)
                _scrollRect.enabled = enabled;
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
                    _activeScrollRect = _parentScrollRect;
                }
                else if (ScrollRectHorizontal && ParentScrollRectVertical)
                {
                    _activeScrollRect = _scrollRect;
                }
            }
            else //if (absHorizontal <= absVertical)
            {
                if (ScrollRectVertical && ParentScrollRectHorizontal)
                {
                    _activeScrollRect = _scrollRect;
                }
                else if (ScrollRectHorizontal && ParentScrollRectVertical)
                {
                    _activeScrollRect = _parentScrollRect;
                }
            }
            SetScrollRectsEnabled(false);
            if (_activeScrollRect)
            {
                _activeScrollRect.enabled = true;
                _activeScrollRect.OnBeginDrag(eventData);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_activeScrollRect)
            {
                _activeScrollRect.OnDrag(eventData);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_parentScrollRect)
                _parentScrollRect.OnEndDrag(eventData);
            if (_scrollRect)
                _scrollRect.OnEndDrag(eventData);

            _activeScrollRect = _scrollRect;
            this.StartCoroutineActionAfterFrames(() => {
                SetScrollRectsEnabled(true);
            }, 1);
        }
    }
}
