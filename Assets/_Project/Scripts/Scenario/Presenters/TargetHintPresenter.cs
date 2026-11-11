using UnityEngine;

namespace VRTraining.Scenario
{
    // highlights target of current action and shows marker above
    public class TargetHintPresenter : MonoBehaviour
    {
        [SerializeField] private ScenarioRunner runner;
        [SerializeField] private HintMarker marker;

        private void OnEnable()
        {
            runner.ActionExpected += OnActionExpected;
            runner.ActionCompleted += OnActionCompleted;
            runner.StepFinished += OnStepFinished;
            runner.GroupFinished += OnGroupFinished;
        }

        private void OnDisable()
        {
            runner.ActionExpected -= OnActionExpected;
            runner.ActionCompleted -= OnActionCompleted;
            runner.StepFinished -= OnStepFinished;
            runner.GroupFinished -= OnGroupFinished;
        }

        private void OnActionExpected(StepProgress step, ExpectedAction action)
        {
            Clear();
            var target = ScenarioTarget.Find(action.targetId);
            if (target == null) return;

            target.SetExpected(true);
            // floor zones have no height of their own lift marker to about eye level
            marker.Attach(target.transform, target is ReachTarget ? 1f : 0f);
        }

        private void OnActionCompleted(StepProgress _) => Clear();
        private void OnStepFinished(StepProgress _) => Clear();
        private void OnGroupFinished(GroupProgress _) => Clear();

        private void Clear()
        {
            ScenarioTarget.ClearAllExpected();
            marker.Hide();
        }
    }
}
