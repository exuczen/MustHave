using System;

namespace MustHave.UI
{
    public struct AlertButtonData
    {
        public string Text;
        public Action Action;
        public bool ActionInstant;

        public AlertButtonData(string text, Action action, bool actionInstant = true)
        {
            Text = text;
            Action = action;
            ActionInstant = actionInstant;
        }

        public AlertButtonData(AlertButtonData src) : this(src.Text, src.Action, src.ActionInstant) { }

        public static AlertButtonData CreateCopy(AlertButtonData src)
        {
            return Create(src.Text, src.Action, src.ActionInstant);
        }

        public static AlertButtonData Create(string text, Action action, bool actionInstant = true)
        {
            return new AlertButtonData(text, action, actionInstant);
        }
    } 
}
