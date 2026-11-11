using System.Collections.Generic;
using UnityEngine;
using VRTraining.Highlight;

namespace VRTraining.Scenario
{
    // base class for everything step use zone object button
    // every target has id and registers itself found by id from scenario
    // child classes only decide when action happened
    public abstract class ScenarioTarget : MonoBehaviour
    {
        private static readonly Dictionary<string, ScenarioTarget> Registry = new Dictionary<string, ScenarioTarget>();

        [SerializeField] private string targetId;

        private IHighlightable highlight;
        private bool isExpected;

        public string TargetId => targetId;
        public abstract ActionType ActionType { get; }

        // finds registered target by id null if not in scene
        public static ScenarioTarget Find(string id) =>
            id != null && Registry.TryGetValue(id, out var target) ? target : null;

        public static void ClearAllExpected()
        {
            foreach (var target in Registry.Values) target.SetExpected(false);
        }

        protected virtual void Awake() => highlight = GetComponent<IHighlightable>();

        protected virtual void OnEnable() => Registry[targetId] = this;

        protected virtual void OnDisable()
        {
            if (Registry.TryGetValue(targetId, out var current) && current == this)
                Registry.Remove(targetId);
        }

        // marks target as thing to use now pulsing outline
        public void SetExpected(bool expected)
        {
            isExpected = expected;
            highlight?.SetMode(expected ? HighlightMode.Expected : HighlightMode.None);
        }

        // hover feedback never overrides expected highlight
        protected void SetHovered(bool hovered)
        {
            if (isExpected) return;
            highlight?.SetMode(hovered ? HighlightMode.Hover : HighlightMode.None);
        }

        protected void ReportAction() => InteractionHub.Raise(ActionType, targetId);
    }
}
