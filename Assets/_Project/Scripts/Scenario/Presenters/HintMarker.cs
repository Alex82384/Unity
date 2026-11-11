using UnityEngine;

namespace VRTraining.Scenario
{
    // diamond above next target moves up and down rotates and follows target
    public class HintMarker : MonoBehaviour
    {
        [SerializeField] private float clearance = 0.3f;
        [SerializeField] private float bobAmplitude = 0.05f;
        [SerializeField] private float bobSpeed = 3f;
        [SerializeField] private float spinSpeed = 100f;

        private Transform follow;
        private float heightAbove;

        private void Awake() => gameObject.SetActive(false);

        // shows marker over target extra lifts higher eg over floor zones
        public void Attach(Transform target, float extra = 0f)
        {
            follow = target;
            heightAbove = TopOf(target) + clearance + extra;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            follow = null;
            gameObject.SetActive(false);
        }

        private void LateUpdate()
        {
            if (follow == null) return;

            Vector3 position = follow.position + Vector3.up * (heightAbove + Mathf.Sin(Time.time * bobSpeed) * bobAmplitude);
            transform.position = position;
            transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.World);
        }

        // distance from target pivot to top renderers or rect of ui element
        private static float TopOf(Transform target)
        {
            if (target is RectTransform rect) return rect.rect.height * 0.5f * rect.lossyScale.y;

            var renderers = target.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0) return 0f;

            var bounds = renderers[0].bounds;
            foreach (var r in renderers) bounds.Encapsulate(r.bounds);
            return Mathf.Max(0f, bounds.max.y - target.position.y);
        }
    }
}
