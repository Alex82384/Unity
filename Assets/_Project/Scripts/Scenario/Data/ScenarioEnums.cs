namespace VRTraining.Scenario
{
    // kinds of user actions step expect see task description
    public enum ActionType
    {
        Reach,
        Grab,
        Click,
        PressUIButton
    }

    public enum StepStatus
    {
        Pending,
        Active,
        Success,
        Failed,
        Skipped
    }

    public enum ViolationKind
    {
        WrongTarget,
        WrongSequence
    }
}
