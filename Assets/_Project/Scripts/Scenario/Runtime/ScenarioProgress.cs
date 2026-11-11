using System.Collections.Generic;

namespace VRTraining.Scenario
{
    // runtime state of one step definition stays immutable progress lives here
    public class StepProgress
    {
        public StepDefinition Definition { get; }
        public StepStatus Status { get; internal set; } = StepStatus.Pending;

        public int ActionIndex { get; internal set; }

        public StepProgress(StepDefinition definition) => Definition = definition;

        public bool IsFinished => Status == StepStatus.Success || Status == StepStatus.Failed || Status == StepStatus.Skipped;

        // action currently expected from user or null when all actions done
        public ExpectedAction CurrentAction =>
            ActionIndex < Definition.actions.Count ? Definition.actions[ActionIndex] : null;
    }

    public class GroupProgress
    {
        public GroupDefinition Definition { get; }
        public List<StepProgress> Steps { get; } = new List<StepProgress>();

        public int StepIndex { get; internal set; }

        public GroupProgress(GroupDefinition definition)
        {
            Definition = definition;
            foreach (var step in definition.steps)
                Steps.Add(new StepProgress(step));
        }

        public StepProgress CurrentStep => StepIndex < Steps.Count ? Steps[StepIndex] : null;
    }
}
