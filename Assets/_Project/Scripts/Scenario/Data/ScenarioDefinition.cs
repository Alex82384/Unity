using System;
using System.Collections.Generic;
using UnityEngine;

namespace VRTraining.Scenario
{
    [Serializable]
    public class ExpectedAction
    {
        public ActionType type;
        public string targetId;
    }

    [Serializable]
    public class StepDefinition
    {
        public string id;
        public string title;
        [TextArea, Tooltip("Instruction shown to the user while the step is active.")]
        public string description;
        public List<ExpectedAction> actions = new List<ExpectedAction>();
    }

    [Serializable]
    public class GroupDefinition
    {
        public string title;
        [TextArea, Tooltip("Message displayed when the group is activated (order of expected actions).")]
        public string intro;
        public List<StepDefinition> steps = new List<StepDefinition>();
    }

    // whole scenario as data only has ids of targets no scene references
    // steps changed without touching code or scene
    [CreateAssetMenu(menuName = "VR Training/Scenario Definition", fileName = "Scenario")]
    public class ScenarioDefinition : ScriptableObject
    {
        public string title;
        public List<GroupDefinition> groups = new List<GroupDefinition>();
    }
}
