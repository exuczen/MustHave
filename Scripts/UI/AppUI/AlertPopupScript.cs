using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MustHave.Utilities;

namespace MustHave.UI
{
    public class AlertPopupScript : UIScript
    {
        public const string ANIMATOR_TRIGGER_SHOW = "show";
        public const string ANIMATOR_TRIGGER_HIDE = "hide";
        public const string BUTTON_OK = "OK";
        public const string BUTTON_YES = "YES";
        public const string BUTTON_NO = "NO";
        public const string WARNING_QUIT_CONFIRM = "Do you really want to quit?";
        public const string WARNING_NOT_IMPLEMENTED = "This feature will be available soon.";

        [SerializeField] private Button _dismissButton = default;
        [SerializeField] private Button[] _buttons = default;
        [SerializeField] protected Text _popupText = default;
        [SerializeField] protected Text _emptyLineText = default;

        private ActionWithText[] _onButtonClickActions = default;
        private Action _dismissButtonAction = default;
        private Animator _animator = default;
        private MonoBehaviour _context = default;
        private int _initialFontSize = default;

        public Animator Animator { get => _animator ?? (_animator = GetComponent<Animator>()); }
        public int FontSize { get => _popupText.fontSize; set { _popupText.fontSize = value; } }

        protected virtual void OnInit() { }

        protected virtual void OnHide() { }

        public void Init(MonoBehaviour context)
        {
            for (int i = 0; i < _buttons.Length; i++)
            {
                int buttonIndex = i;
                _buttons[i].onClick.AddListener(() => {
                    OnButtonClick(buttonIndex);
                });
            }
            _dismissButton.onClick.AddListener(OnDismissButtonClick);
            _initialFontSize = _popupText.fontSize;
            _context = context;
            OnInit();
        }

        public void SetDismissButtonEnabled(bool enabled)
        {
            _dismissButton.interactable = enabled;
        }

        public AlertPopupScript SetButtons(params ActionWithText[] onClickActions)
        {
            int buttonsCount = Mathf.Min(_buttons.Length, onClickActions.Length);
            _onButtonClickActions = new ActionWithText[buttonsCount];

            SetDismissButtonEnabled(buttonsCount == 1);
            if (buttonsCount == 1 && onClickActions.Length > 0 && onClickActions[0] != null)
                _dismissButtonAction = onClickActions[0].action;

            for (int i = 0; i < buttonsCount; i++)
            {
                Button button = _buttons[i];
                _onButtonClickActions[i] = new ActionWithText(onClickActions[i]);
                _buttons[i].GetComponentInChildren<Text>().text = onClickActions[i].text;
                _buttons[i].transform.parent.gameObject.SetActive(true);
            }
            for (int i = buttonsCount; i < _buttons.Length; i++)
            {
                _buttons[i].transform.parent.gameObject.SetActive(false);
            }
            return this;
        }

        public AlertPopupScript SetText(string text)
        {
            //_popupText.text = string.Concat("\n", text, "\n");
            bool textIsNullOrEmpty = string.IsNullOrEmpty(text);
            _popupText.gameObject.SetActive(!textIsNullOrEmpty);
            if (_emptyLineText)
                _emptyLineText.gameObject.SetActive(!textIsNullOrEmpty);
            _popupText.text = text;
            return this;
        }

        public void ShowNotImplementedWarning(Action action = null)
        {
            ShowWithConfirmButton(WARNING_NOT_IMPLEMENTED, action, false);
        }

        public void ShowWithConfirmButton(string text, Action action = null, bool invokeActionOnHide = true)
        {
            SetButtons(ActionWithText.Create(BUTTON_OK, action));
            SetText(text);
            Show();
            _dismissButtonAction = invokeActionOnHide ? action : null;
        }

        public void ShowQuitWarning()
        {
            SetButtons(ActionWithText.Create(BUTTON_NO, null), ActionWithText.Create(BUTTON_YES, Application.Quit)).
            SetText(WARNING_QUIT_CONFIRM).Show();
        }

        public void OnDismissButtonClick()
        {
            if (_dismissButton.interactable)
            {
                HideWithAnimator(() => {
                    _dismissButtonAction?.Invoke();
                    _dismissButtonAction = null;
                });
                _onButtonClickActions = null;
            }
        }

        private void OnButtonClick(int i)
        {
            ActionWithText buttonAction = null;
            if (_onButtonClickActions != null && i >= 0 && i < _onButtonClickActions.Length
                && (buttonAction = _onButtonClickActions[i]) != null)
            {
                if (buttonAction.dismiss)
                {
                    if (buttonAction.DismissWithAnimator)
                    {
                        HideWithAnimator(() => { buttonAction.action?.Invoke(); });
                    }
                    else
                    {
                        Hide();
                        buttonAction.action?.Invoke();
                    }
                }
                else
                {
                    _onButtonClickActions[i].action?.Invoke();
                }
            }
            else
            {
                Hide();
            }
        }

        public override void Show()
        {
            ShowWithAnimator();
        }

        private void ShowWithAnimator()
        {
            base.Show();
            Animator.SetTrigger(ANIMATOR_TRIGGER_SHOW);
        }

        public override void Hide()
        {
            _popupText.fontSize = _initialFontSize;
            base.Hide();
            OnHide();
        }

        private void HideWithAnimator(Action onHide)
        {
            if (gameObject.activeSelf)
            {
                Animator.SetTrigger(ANIMATOR_TRIGGER_HIDE);
                _context?.StartCoroutineActionAfterPredicate(() => {
                    Hide();
                    onHide?.Invoke();
                }, () => gameObject.activeSelf);
            }
        }
    }
}
