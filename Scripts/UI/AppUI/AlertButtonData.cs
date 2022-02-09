using System;

namespace MustHave.UI
{
    public class AlertButtonData
    {
        public string text = default;
        public Action action = default;
        public bool dismiss = default;
        private bool _dismissWithAnimator = default;

        public bool DismissWithAnimator
        {
            set { dismiss = _dismissWithAnimator = value; }
            get { return dismiss && _dismissWithAnimator; }
        }

        public AlertButtonData(string text, Action action, bool dismiss = true, bool dismissWithAnimator = true)
        {
            this.text = text;
            this.action = action;
            this.dismiss = dismiss;
            _dismissWithAnimator = dismiss && dismissWithAnimator;
        }

        public AlertButtonData(AlertButtonData src) : this(src.text, src.action, src.dismiss, src._dismissWithAnimator) { }

        public static AlertButtonData CreateCopy(AlertButtonData src)
        {
            return Create(src.text, src.action, src.dismiss, src._dismissWithAnimator);
        }

        public static AlertButtonData Create(string text, Action action, bool dismiss = true, bool dismissWithAnimator = true)
        {
            return new AlertButtonData(text, action, dismiss, dismissWithAnimator);
        }
    }
}