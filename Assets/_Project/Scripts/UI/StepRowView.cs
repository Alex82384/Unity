using UnityEngine;
using UnityEngine.UI;
using VRTraining.Scenario;

namespace VRTraining.UI
{
    public class StepRowView : MonoBehaviour
    {
        [SerializeField] private Image background;
        [SerializeField] private Image marker;
        [SerializeField] private Text titleLabel;
        [SerializeField] private Text statusLabel;

        public void Bind(StepProgress step)
        {
            titleLabel.text = step.Definition.title;
            Refresh(step.Status);
        }

        public void Refresh(StepStatus status)
        {
            Color color = StepStatusStyle.Color(status);

            statusLabel.text = StepStatusStyle.Label(status);
            statusLabel.color = color;
            marker.color = color;

            // active step gets soft tinted row eye finds immediately
            Color tint = color;
            tint.a = status == StepStatus.Active ? 0.18f : 0f;
            background.color = tint;
        }
    }
}
