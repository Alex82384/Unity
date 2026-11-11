using UnityEngine;
using UnityEngine.UI;

namespace VRTraining.Scenario
{
    [RequireComponent(typeof(Button))]
    public class UIButtonTarget : ScenarioTarget
    {
        private Button button;

        public override ActionType ActionType => ActionType.PressUIButton;

        protected override void Awake()
        {
            base.Awake();
            button = GetComponent<Button>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            button.onClick.AddListener(ReportAction);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            button.onClick.RemoveListener(ReportAction);
        }
    }
}
