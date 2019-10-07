using System;

namespace MustHave.UI
{
    public class ActionWithText
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

        public ActionWithText(string text, Action action, bool dismiss = true, bool dismissWithAnimator = true)
        {
            this.text = text;
            this.action = action;
            this.dismiss = dismiss;
            _dismissWithAnimator = dismiss && dismissWithAnimator;
        }

        public ActionWithText(ActionWithText src) : this(src.text, src.action, src.dismiss, src._dismissWithAnimator) { }

        public static ActionWithText CreateCopy(ActionWithText src)
        {
            return Create(src.text, src.action, src.dismiss, src._dismissWithAnimator);
        }

        public static ActionWithText Create(string text, Action action, bool dismiss = true, bool dismissWithAnimator = true)
        {
            return new ActionWithText(text, action, dismiss, dismissWithAnimator);
        }
    }
}