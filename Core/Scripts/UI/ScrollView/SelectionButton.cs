using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace MustHave.UI
{
    [RequireComponent(typeof(Button))]
    public abstract class SelectionButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPoolable
    {
        [Header("------------------ SelectionButton ------------------")]
        [SerializeField]
        protected Button button = null;
        [SerializeField]
        protected Image contentImage = null;
        [SerializeField]
        private Image selectionImage = null;
        [SerializeField]
        private Image highlightImage = null;

        private Action<bool> onSelected = null;
        private UnityAction onClick = null;

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (highlightImage)
            {
                highlightImage.SetGameObjectActive(true);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (highlightImage)
            {
                highlightImage.SetGameObjectActive(false);
            }
        }

        public void Setup<T>(SceneControllers sceneControllers, SelectionScrollView<T> panel, Action<T> onClick, Action<bool> onSelected = null) where T : SelectionButton
        {
            bool panelIsFakeScrollView = panel is FakeScrollView;
            if (!panelIsFakeScrollView)
            {
                panel.AddButton(this as T);
            }
            this.onClick = () => {
                panel.SetSelectedButton(this as T, true);
                onClick?.Invoke(this as T);
                OnClick(sceneControllers);
            };
            button.onClick.AddListener(this.onClick);
            this.onSelected = onSelected;
        }

        public void InvokeOnClick()
        {
            onClick?.Invoke();
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void SetSelected(bool selected, bool invokeAction)
        {
            SetSelectionImageEnabled(selected);
            if (invokeAction)
            {
                onSelected?.Invoke(selected);
            }
        }

        public virtual void Clear()
        {
            onSelected = null;
            onClick = null;
            button.onClick.RemoveAllListeners();

            SetSelectionImageEnabled(false);
        }

        public virtual void OnReturnToPool()
        {
            Clear();
            Hide();
        }

        public void SetContentImage(Sprite sprite)
        {
            contentImage.sprite = sprite;
        }

        public void SetNameOfSpriteName()
        {
            if (contentImage.sprite)
            {
                name = contentImage.sprite.name;
            }
        }

        protected virtual void OnClick(SceneControllers sceneControllers)
        {
        }

        protected void SetSelectionImageEnabled(bool enabled)
        {
            if (selectionImage)
            {
                if (selectionImage.gameObject == gameObject)
                {
                    selectionImage.enabled = enabled;
                }
                else
                {
                    selectionImage.SetGameObjectActive(enabled);
                }
            }
        }
    }
}
