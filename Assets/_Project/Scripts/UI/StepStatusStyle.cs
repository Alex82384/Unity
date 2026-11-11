using UnityEngine;
using VRTraining.Scenario;

namespace VRTraining.UI
{
    public static class StepStatusStyle
    {
        public static string Label(StepStatus status)
        {
            switch (status)
            {
                case StepStatus.Active:  return "Текущий";
                case StepStatus.Success: return "Выполнен";
                case StepStatus.Failed:  return "Выполнен с ошибкой";
                case StepStatus.Skipped: return "Пропущен";
                default:                 return "Ожидает";
            }
        }

        public static Color Color(StepStatus status)
        {
            switch (status)
            {
                case StepStatus.Active:  return new Color(1f, 0.85f, 0.25f);
                case StepStatus.Success: return new Color(0.35f, 0.9f, 0.45f);
                case StepStatus.Failed:  return new Color(1f, 0.4f, 0.35f);
                case StepStatus.Skipped: return new Color(0.65f, 0.65f, 0.7f);
                default:                 return new Color(0.85f, 0.88f, 0.95f);
            }
        }
    }
}
