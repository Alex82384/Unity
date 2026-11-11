using UnityEngine;
using VRTraining.Scenario;

namespace VRTraining.Audio
{
    // plays success violation group-start sounds in response to scenario events
    [RequireComponent(typeof(AudioSource))]
    public class FeedbackAudioPresenter : MonoBehaviour
    {
        [SerializeField] private ScenarioRunner runner;

        private AudioSource source;
        private AudioClip successClip;
        private AudioClip violationClip;
        private AudioClip groupClip;
        private AudioClip finishClip;

        private void Awake()
        {
            source = GetComponent<AudioSource>();
            source.playOnAwake = false;
            source.spatialBlend = 0f; // 2d feedback must heard no matter where user stands

            successClip = ToneFactory.Create("Success", new[] { 660f, 880f }, 0.12f);
            violationClip = ToneFactory.Create("Violation", new[] { 180f, 140f }, 0.22f, square: true);
            groupClip = ToneFactory.Create("Group", new[] { 523f }, 0.25f);
            finishClip = ToneFactory.Create("Finish", new[] { 523f, 659f, 784f }, 0.14f);
        }

        private void OnEnable()
        {
            runner.StepFinished += OnStepFinished;
            runner.ViolationDetected += OnViolation;
            runner.GroupStarted += OnGroupStarted;
            runner.ScenarioFinished += OnScenarioFinished;
        }

        private void OnDisable()
        {
            runner.StepFinished -= OnStepFinished;
            runner.ViolationDetected -= OnViolation;
            runner.GroupStarted -= OnGroupStarted;
            runner.ScenarioFinished -= OnScenarioFinished;
        }

        // skipped steps silent violation itself already played sound
        private void OnStepFinished(StepProgress step)
        {
            if (step.Status == StepStatus.Success) source.PlayOneShot(successClip);
        }

        private void OnViolation(ViolationKind kind, InteractionEvent e) => source.PlayOneShot(violationClip);
        private void OnGroupStarted(GroupProgress _) => source.PlayOneShot(groupClip);
        private void OnScenarioFinished() => source.PlayOneShot(finishClip);
    }
}
