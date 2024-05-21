using UnityEngine;

public class AnimatorStateData
{
    public string ClipName { get; }
    public int Trigger { get; }
    public int State { get; }

    public AnimatorStateData(string clipName, string triggerName, string stateName)
    {
        ClipName = clipName;
        Trigger = Animator.StringToHash(triggerName);
        State = Animator.StringToHash(stateName);
    }

    public AnimatorStateData()
    {
        ClipName = null;
        Trigger = -1;
        State = -1;
    }
}
