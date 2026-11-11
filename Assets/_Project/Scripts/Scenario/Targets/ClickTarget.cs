using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace VRTraining.Scenario
{
    [RequireComponent(typeof(XRSimpleInteractable))]
    public class ClickTarget : ScenarioTarget
    {
        // seconds between two clicks so grip and trigger together count once
        private const float ClickCooldown = 0.4f;

        private XRSimpleInteractable interactable;
        private XRBaseInputInteractor hoveringInteractor;
        private float lastClickTime = -10f;

        public override ActionType ActionType => ActionType.Click;

        protected override void Awake()
        {
            base.Awake();
            interactable = GetComponent<XRSimpleInteractable>();
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
            hoveringInteractor = null;
        }

        // the default xri bindings select with grip and activate with trigger
        // trigger is the natural button for clicking so both work
        private void Update()
        {
            if (hoveringInteractor == null) return;

            if (hoveringInteractor.activateInput.ReadWasPerformedThisFrame())
            {
                Click();
            }
        }

        private void OnSelected(SelectEnterEventArgs args)
        {
            Click();
        }

        private void OnHoverEntered(HoverEnterEventArgs args)
        {
            hoveringInteractor = args.interactorObject as XRBaseInputInteractor;
            SetHovered(true);
        }

        private void OnHoverExited(HoverExitEventArgs args)
        {
            hoveringInteractor = null;
            SetHovered(false);
        }

        private void Click()
        {
            if (Time.time - lastClickTime < ClickCooldown) return;

            lastClickTime = Time.time;
            ReportAction();
        }
    }
}
