using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using MustHave.Utilities;

namespace MustHave.UI
{
    [RequireComponent(typeof(ScrollRect))]
    public abstract class ScrollList<T> : UIBehaviour where T : ListItem
    {
        [SerializeField] protected RectTransform listItemSlotPrefab = null;
        [SerializeField] protected T listItemPrefab = null;

        protected HashSet<Transform> itemsInView = new HashSet<Transform>();
        protected ScrollRect scrollRect = null;
        protected RectTransform viewport = null;
        protected RectTransform content = null;
        protected float itemHeight = 0f;
        protected int viewportItemsCount = 0;

        protected abstract T CreateListItemAtIndex(int index, Transform slot);

        protected override void Awake()
        {
            scrollRect = GetComponent<ScrollRect>();
            viewport = scrollRect.viewport;
            content = scrollRect.content;
            content.DestroyAllChildren();
            scrollRect.onValueChanged.AddListener(ShowItemsInViewport);
            itemHeight = listItemSlotPrefab.rect.height;
        }

        protected override void OnRectTransformDimensionsChange()
        {
            SetViewportItemsCount();
        }

        protected void ClearList()
        {
            content.DestroyAllChildren();
            SetViewportItemsCount();
        }

        protected void SetViewportItemsCount()
        {
            if (viewport && viewport.rect.height > 0f)
            {
                viewportItemsCount = (int)((viewport.rect.height + itemHeight - 1) / itemHeight);
            }
        }

        protected void ShowItemsInViewport(Vector2 position)
        {
            foreach (var item in itemsInView)
            {
                item.gameObject.SetActive(false);
            }
            itemsInView.Clear();

            int begIndex = (int)((1f - position.y) * (content.rect.height - viewport.rect.height) / itemHeight);
            begIndex = Mathf.Clamp(begIndex - 1, 0, content.childCount);
            int endIndex = Mathf.Min(begIndex + 2 + viewportItemsCount, content.childCount);
            for (int i = begIndex; i < endIndex; i++)
            {
                Transform slot = content.GetChild(i);
                Transform item = null;
                if (slot.childCount == 0)
                {
                    item = CreateListItemAtIndex(i, slot).transform;
                }
                else
                {
                    item = slot.GetChild(0);
                    item.gameObject.SetActive(true);
                }
                itemsInView.Add(item);
            }
        }
    }
}
