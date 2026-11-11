using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRTraining.Scenario
{
    // main logic of scenario listens to interactionhub and checks every user action
    // other scripts ui audio highlight just subscribe to events below
    public class ScenarioRunner : MonoBehaviour
    {
        [SerializeField] private ScenarioDefinition definition;
        [SerializeField] private bool autoStart = true;
        [SerializeField] private float groupTransitionDelay = 2f;

        private readonly List<GroupProgress> groups = new List<GroupProgress>();
        private int groupIndex;
        private bool acceptingInput;

        public IReadOnlyList<GroupProgress> Groups => groups;
        public GroupProgress CurrentGroup => groupIndex < groups.Count ? groups[groupIndex] : null;
        public int GroupIndex => groupIndex;
        public ScenarioDefinition Definition => definition;

        // ui audio and highlight scripts subscribe to these
        public event Action<GroupProgress> GroupStarted;
        public event Action<StepProgress> StepActivated;
        public event Action<StepProgress, ExpectedAction> ActionExpected;
        public event Action<StepProgress> ActionCompleted;
        public event Action<StepProgress> StepFinished;
        public event Action<ViolationKind, InteractionEvent> ViolationDetected;
        public event Action<GroupProgress> GroupFinished;
        public event Action ScenarioFinished;

        private void OnEnable() => InteractionHub.Raised += OnInteraction;
        private void OnDisable() => InteractionHub.Raised -= OnInteraction;

        private void Start()
        {
            if (autoStart) StartScenario();
        }

        // resets all progress and starts from first group used for restart too
        public void StartScenario()
        {
            StopAllCoroutines();
            groups.Clear();
            foreach (var group in definition.groups)
                groups.Add(new GroupProgress(group));

            groupIndex = 0;
            StartGroup();
        }

        private void StartGroup()
        {
            var group = CurrentGroup;
            GroupStarted?.Invoke(group);
            ActivateStep(group.CurrentStep);
            acceptingInput = true;
        }

        private void ActivateStep(StepProgress step)
        {
            step.Status = StepStatus.Active;
            StepActivated?.Invoke(step);
            ActionExpected?.Invoke(step, step.CurrentAction);
        }

        private void OnInteraction(InteractionEvent e)
        {
            if (!acceptingInput) return;

            var group = CurrentGroup;
            var step = group.CurrentStep;
            var expected = step.CurrentAction;

            if (Matches(expected, e))
            {
                step.ActionIndex++;
                ActionCompleted?.Invoke(step);

                if (step.CurrentAction != null) ActionExpected?.Invoke(step, step.CurrentAction);
                else FinishStep(step, StepStatus.Success);
            }
            else if (BelongsToLaterStep(group, e))
            {
                // order violation close whole group unfinished steps become skipped
                ViolationDetected?.Invoke(ViolationKind.WrongSequence, e);
                CloseGroup(group);
            }
            else
            {
                // wrong point object button step ends with error
                ViolationDetected?.Invoke(ViolationKind.WrongTarget, e);
                FinishStep(step, StepStatus.Failed);
            }
        }

        private void FinishStep(StepProgress step, StepStatus status)
        {
            step.Status = status;
            StepFinished?.Invoke(step);

            var group = CurrentGroup;
            group.StepIndex++;

            if (group.CurrentStep != null) ActivateStep(group.CurrentStep);
            else CloseGroup(group);
        }

        // marks unfinished steps as skipped and moves on to next group or finishes
        private void CloseGroup(GroupProgress group)
        {
            acceptingInput = false;

            foreach (var step in group.Steps)
            {
                if (step.IsFinished) continue;
                step.Status = StepStatus.Skipped;
                StepFinished?.Invoke(step);
            }

            GroupFinished?.Invoke(group);
            groupIndex++;

            if (groupIndex < groups.Count) StartCoroutine(StartNextGroupAfterDelay());
            else ScenarioFinished?.Invoke();
        }

        private IEnumerator StartNextGroupAfterDelay()
        {
            yield return new WaitForSeconds(groupTransitionDelay);
            StartGroup();
        }

        private static bool Matches(ExpectedAction expected, InteractionEvent e) =>
            expected != null && expected.type == e.Type && expected.targetId == e.TargetId;

        // true if event first action of step comes after current one
        private static bool BelongsToLaterStep(GroupProgress group, InteractionEvent e)
        {
            for (int i = group.StepIndex + 1; i < group.Steps.Count; i++)
            {
                foreach (var action in group.Steps[i].Definition.actions)
                    if (Matches(action, e)) return true;
            }
            return false;
        }
    }
}
