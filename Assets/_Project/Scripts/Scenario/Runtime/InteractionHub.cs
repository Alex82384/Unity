using System;

namespace VRTraining.Scenario
{
    public readonly struct InteractionEvent
    {
        public readonly ActionType Type;
        public readonly string TargetId;

        public InteractionEvent(ActionType type, string targetId)
        {
            Type = type;
            TargetId = targetId;
        }

        public override string ToString() => $"{Type}:{TargetId}";
    }

    // targets send their actions here and scenariorunner listens
    // way targets do not know about steps
    public static class InteractionHub
    {
        public static event Action<InteractionEvent> Raised;

        public static void Raise(ActionType type, string targetId) =>
            Raised?.Invoke(new InteractionEvent(type, targetId));
    }
}
