using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

namespace MustHave.UI
{
    [RequireComponent(typeof(ScrollView))]
    public class SelectionScrollView<T> : MonoBehaviour, ISelectionScrollViewPool where T : SelectionButton
    {
        public RectTransform Content => content;
        public T SelectedButton => selectedButton;

        [SerializeField]
        protected RectTransform content = null;
        [SerializeField]
        protected RectTransform buttonPoolParent = null;
        [SerializeField]
        protected int buttonPoolCapacity = 10;
        [SerializeField]
        protected T buttonPrefab = null;
        [SerializeField]
        protected bool selectedOnEnable = true;
        [SerializeField, ConditionalHide("selectedOnEnable", true)]
        protected int selectedOnEnableIndex = 0;

        protected List<T> buttons = new();
        protected T selectedButton = null;

        private DeprecatedObjectPool<T> buttonPool = null;

        protected virtual void OnEnable()
        {
            if (selectedOnEnable)
            {
                if (selectedButton != null)
                {
                    selectedButton.InvokeOnClick();
                }
                else
                {
                    SetSelectedOnEnableButton(selectedOnEnableIndex, true);
                }
            }
        }

        public void AddButton(T button)
        {
            buttons.Add(button);
        }

        public void SetSelectedButton(T button, bool invokeAction)
        {
            if (selectedButton && selectedButton != button)
            {
                selectedButton.SetSelected(false, invokeAction);
            }
            selectedButton = button;
            if (selectedButton)
            {
                selectedButton.SetSelected(true, invokeAction);
            }
        }

        public void CreateButtonsPool(int capacity = -1)
        {
            OnCreateButtonsPools();
            buttonPool = new DeprecatedObjectPool<T>(buttonPrefab, buttonPoolParent, capacity < 0 ? buttonPoolCapacity : capacity);
            if (capacity > 0)
            {
                buttonPoolCapacity = capacity;
            }
        }

        public void ReturnButtonsToPool()
        {
            DeselectSelectedButton();

            if (buttonPool != null)
            {
                foreach (var button in buttons)
                {
                    buttonPool.Return(button);
                }
                buttons.Clear();
            }
        }

        public bool SetSelectedButton(int buttonIndex, bool invokeOnClick)
        {
            DeselectSelectedButton();

            if (buttonIndex >= 0 && buttonIndex < buttons.Count)
            {
                selectedButton = buttons[buttonIndex];
                selectedButton.SetSelected(true, true);
                if (invokeOnClick)
                {
                    selectedButton.InvokeOnClick();
                }
                return true;
            }
            return false;
        }

        protected bool SetSelectedOnEnableButton(int buttonIndex, bool invokeOnClick)
        {
            if (SetSelectedButton(buttonIndex, invokeOnClick))
            {
                selectedOnEnableIndex = buttonIndex;
                return true;
            }
            return false;
        }

        protected T GetButtonFromPool(Transform parent)
        {
            var button = buttonPool.GetObject();
            button.transform.SetParent(parent);
            button.SetGameObjectActive(true);
            return button;
        }

        protected virtual void OnCreateButtonsPools()
        {
            content.DestroyAllChildren();
        }

        protected void DeselectSelectedButton()
        {
            if (selectedButton)
            {
                selectedButton.SetSelected(false, true);
                selectedButton = null;
            }
        }

        public bool Show()
        {
            bool active = gameObject.activeSelf;
            gameObject.SetActive(true);
            return !active;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
