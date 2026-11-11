using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace VRTraining.Scenario
{
    [RequireComponent(typeof(XRGrabInteractable))]
    public class GrabTarget : ScenarioTarget
    {
        private XRGrabInteractable interactable;

        public override ActionType ActionType => ActionType.Grab;

        protected override void Awake()
        {
            base.Awake();
            interactable = GetComponent<XRGrabInteractable>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            interactable.selectEntered.AddListener(OnSelected);
            interactable.hoverEntered.AddListener(OnHoverEntered);
            interactable.hoverExited.AddListener(OnHoverExited);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            interactable.selectEntered.RemoveListener(OnSelected);
            interactable.hoverEntered.RemoveListener(OnHoverEntered);
            interactable.hoverExited.RemoveListener(OnHoverExited);
        }

        private void OnSelected(UnityEngine.XR.Interaction.Toolkit.SelectEnterEventArgs _) => ReportAction();
        private void OnHoverEntered(UnityEngine.XR.Interaction.Toolkit.HoverEnterEventArgs _) => SetHovered(true);
        private void OnHoverExited(UnityEngine.XR.Interaction.Toolkit.HoverExitEventArgs _) => SetHovered(false);
    }
}
