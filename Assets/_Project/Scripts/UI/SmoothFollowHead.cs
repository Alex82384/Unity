using UnityEngine;

namespace VRTraining.UI
{
    // keeps panel in front of player follows slowly because ui fixed to head uncomfortable in vr
    public class SmoothFollowHead : MonoBehaviour
    {
        [SerializeField] private float distance = 1.8f;
        [SerializeField] private float heightOffset = -0.4f;
        [SerializeField] private float followSpeed = 3f;

        private void LateUpdate()
        {
            var head = Camera.main;
            if (head == null) return;

            // use only head yaw panel stays upright
            Vector3 forward = head.transform.forward;
            forward.y = 0f;
            if (forward.sqrMagnitude < 0.001f) return;
            forward.Normalize();

            Vector3 targetPosition = head.transform.position + forward * distance + Vector3.up * heightOffset;
            float t = 1f - Mathf.Exp(-followSpeed * Time.unscaledDeltaTime);
            transform.position = Vector3.Lerp(transform.position, targetPosition, t);

            // canvas faces away from forward world ui convention look away from head
            Quaternion targetRotation = Quaternion.LookRotation(transform.position - head.transform.position, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, t);
        }
    }
}
