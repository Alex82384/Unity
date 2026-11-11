using UnityEngine;

namespace VRTraining.Scenario
{
    // point of interest reports reach once when head of player enters radius
    // checks distance works with sliding teleport and simulator
    public class ReachTarget : ScenarioTarget
    {
        [SerializeField, Min(0.1f)] private float radius = 0.8f;

        private bool playerInside;

        public override ActionType ActionType => ActionType.Reach;

        private void Update()
        {
            var head = Camera.main;
            if (head == null) return;

            Vector3 offset = head.transform.position - transform.position;
            offset.y = 0f;
            bool inside = offset.sqrMagnitude <= radius * radius;

            // edge trigger report only on entering
            if (inside && !playerInside) ReportAction();
            playerInside = inside;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}
