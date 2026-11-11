using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRTraining.Scenario;

namespace VRTraining.UI
{
    // panel with current group instruction list of steps and line for mistakes
    public class TrainingHudPresenter : MonoBehaviour
    {
        [SerializeField] private ScenarioRunner runner;
        [SerializeField] private Text groupLabel;
        [SerializeField] private Text instructionLabel;
        [SerializeField] private Text feedbackLabel;
        [SerializeField] private RectTransform stepListRoot;
        [SerializeField] private StepRowView stepRowPrefab;
        [SerializeField] private Image progressFill;
        [SerializeField] private float feedbackSeconds = 3f;

        private readonly Dictionary<StepProgress, StepRowView> rows = new Dictionary<StepProgress, StepRowView>();
        private float feedbackHideTime;

        private void OnEnable()
        {
            runner.GroupStarted += OnGroupStarted;
            runner.StepActivated += OnStepActivated;
            runner.StepFinished += OnStepFinished;
            runner.ViolationDetected += OnViolation;
            runner.ScenarioFinished += OnScenarioFinished;
        }

        private void OnDisable()
        {
            runner.GroupStarted -= OnGroupStarted;
            runner.StepActivated -= OnStepActivated;
            runner.StepFinished -= OnStepFinished;
            runner.ViolationDetected -= OnViolation;
            runner.ScenarioFinished -= OnScenarioFinished;
        }

        private void Update()
        {
            if (feedbackLabel.text.Length > 0 && Time.time >= feedbackHideTime) feedbackLabel.text = string.Empty;
        }

        private void OnGroupStarted(GroupProgress group)
        {
            groupLabel.text = $"Группа {runner.GroupIndex + 1}/{runner.Groups.Count}: {group.Definition.title}";
            instructionLabel.text = group.Definition.intro;
            UpdateProgress();

            // rebuild step list for new group
            foreach (var row in rows.Values) Destroy(row.gameObject);
            rows.Clear();
            foreach (var step in group.Steps)
            {
                var row = Instantiate(stepRowPrefab, stepListRoot);
                row.Bind(step);
                rows[step] = row;
            }
        }

        private void OnStepActivated(StepProgress step)
        {
            instructionLabel.text = step.Definition.description;
            rows[step].Refresh(step.Status);
        }

        private void OnStepFinished(StepProgress step)
        {
            rows[step].Refresh(step.Status);
            UpdateProgress();
        }

        // overall progress = finished steps all steps of scenario
        private void UpdateProgress()
        {
            int total = 0, finished = 0;
            foreach (var group in runner.Groups)
                foreach (var step in group.Steps)
                {
                    total++;
                    if (step.IsFinished) finished++;
                }
            progressFill.fillAmount = total == 0 ? 0f : finished / (float)total;
        }

        private void OnViolation(ViolationKind kind, InteractionEvent e)
        {
            feedbackLabel.text = kind == ViolationKind.WrongSequence
                ? "Нарушена последовательность! Группа закрыта."
                : "Ошибка: это не то действие.";
            feedbackLabel.color = StepStatusStyle.Color(StepStatus.Failed);
            feedbackHideTime = Time.time + feedbackSeconds;
        }

        private void OnScenarioFinished() => gameObject.SetActive(false);
    }
}
