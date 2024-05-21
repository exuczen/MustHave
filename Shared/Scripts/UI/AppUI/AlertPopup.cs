using System;
using UnityEngine;
using UnityEngine.UI;
using MustHave.Utils;

namespace MustHave.UI
{
    public class AlertPopup : UIScript
    {
        public bool Active => gameObject.activeSelf;
        public Animator Animator => animator;
        public int FontSize { get => popupText.fontSize; set { popupText.fontSize = value; } }

        public const string ANIMATOR_TRIGGER_SHOW = "show";
        public const string ANIMATOR_TRIGGER_HIDE = "hide";
        public const string BUTTON_OK = "OK";
        public const string BUTTON_YES = "YES";
        public const string BUTTON_NO = "NO";
        public const string WARNING_QUIT_CONFIRM = "Do you really want to quit?";
        public const string WARNING_NOT_IMPLEMENTED = "This feature will be available soon.";

        [SerializeField] private Button dismissButton = default;
        [SerializeField] private Button[] buttons = default;
        [SerializeField] protected Text popupText = default;
        [SerializeField] protected Text emptyLineText = default;
        //[SerializeField] private CanvasGroup canvasGroup = null;

        private AlertButtonData[] buttonsData = default;
        private Action dismissButtonAction = default;
        private Animator animator = default;
        private MonoBehaviour context = default;
        private int initialFontSize = default;

        private Action onShowQuitWarning = default;
        private Action onDismissQuitWarning = default;

        public void Init(MonoBehaviour context)
        {
            buttonsData = new AlertButtonData[buttons.Length];
            for (int i = 0; i < buttons.Length; i++)
            {
                int buttonIndex = i;
                buttons[i].onClick.AddListener(() => {
                    OnButtonClick(buttonIndex);
                });
            }
            animator = GetComponent<Animator>();
            dismissButton.onClick.AddListener(OnDismissButtonClick);
            initialFontSize = popupText.fontSize;
            this.context = context;
        }

        public void SetQuitWarningActions(Action onShow, Action onDismiss)
        {
            onShowQuitWarning = onShow;
            onDismissQuitWarning = onDismiss;
        }

        public void SetDismissButtonEnabled(bool enabled)
        {
            dismissButton.interactable = enabled;
        }

        public AlertPopup SetButtons(params AlertButtonData[] buttonsData)
        {
            int buttonsCount = Mathf.Min(buttons.Length, buttonsData.Length);

            SetDismissButtonEnabled(buttonsCount == 1);
            if (buttonsCount == 1 && buttonsData.Length > 0 && buttonsData[0].Action != null)
            {
                dismissButtonAction = buttonsData[0].Action;
            }
            for (int i = 0; i < buttonsCount; i++)
            {
                this.buttonsData[i] = new AlertButtonData(buttonsData[i]);
                buttons[i].GetComponentInChildren<Text>().text = buttonsData[i].Text;
                buttons[i].transform.parent.SetGameObjectActive(true);
            }
            for (int i = buttonsCount; i < buttons.Length; i++)
            {
                buttons[i].transform.parent.SetGameObjectActive(false);
            }
            return this;
        }

        public AlertPopup SetText(string text)
        {
            bool textIsNullOrEmpty = string.IsNullOrEmpty(text);
            popupText.SetGameObjectActive(!textIsNullOrEmpty);
            if (emptyLineText)
                emptyLineText.SetGameObjectActive(!textIsNullOrEmpty);
            popupText.text = text;
            return this;
        }

        public void ShowNotImplementedWarning(Action action = null)
        {
            ShowWithConfirmButton(WARNING_NOT_IMPLEMENTED, action, true, false);
        }

        public void ShowWithConfirmButton(string text, Action action, bool instantAction = true, bool invokeActionOnDismiss = true)
        {
            SetButtons(AlertButtonData.Create(BUTTON_OK, action, instantAction));
            SetText(text);
            Show();
            dismissButtonAction = invokeActionOnDismiss ? action : null;
        }

        public void ShowWithYesNoButtons(string text, Action onAccept, Action onReject, bool instantAction, bool invokeRejectActionOnDismiss)
        {
            SetButtons(AlertButtonData.Create(BUTTON_YES, onAccept, instantAction), AlertButtonData.Create(BUTTON_NO, onReject, instantAction));
            SetText(text);
            Show();
            dismissButtonAction = invokeRejectActionOnDismiss ? onReject : null;
        }

        public void ShowQuitWarning()
        {
            onShowQuitWarning?.Invoke();
            SetButtons(
                AlertButtonData.Create(BUTTON_NO, onDismissQuitWarning),
                AlertButtonData.Create(BUTTON_YES, () => {
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
                })).SetText(WARNING_QUIT_CONFIRM).Show();
        }

        public void OnDismissButtonClick()
        {
            if (dismissButton.interactable)
            {
                HideWithAnimator(() => {
                    dismissButtonAction?.Invoke();
                    dismissButtonAction = null;
                });
            }
        }

        private void OnButtonClick(int i)
        {
            AlertButtonData buttonData;
            Action onHide = null;
            if (i >= 0 && i < buttonsData.Length && (buttonData = buttonsData[i]).Action != null)
            {
                if (buttonData.ActionInstant)
                {
                    buttonData.Action.Invoke();
                }
                else
                {
                    onHide = buttonData.Action;
                }
            }
            HideWithAnimator(onHide);
        }

        public override void Show()
        {
            base.Show();
            animator.SetTrigger(ANIMATOR_TRIGGER_SHOW);
        }

        public override void Hide()
        {
            for (int i = 0; i < buttonsData.Length; i++)
            {
                buttonsData[i] = default;
            }
            popupText.fontSize = initialFontSize;
            base.Hide();
        }

        private void HideWithAnimator(Action onHide)
        {
            if (gameObject.activeSelf)
            {
                animator.SetTrigger(ANIMATOR_TRIGGER_HIDE);
                context.StartCoroutineActionAfterPredicate(() => {
                    onHide?.Invoke();
                    Hide();
                }, () => gameObject.activeSelf);
            }
        }
    }
}
