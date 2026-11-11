using UnityEngine;
using UnityEngine.UI;

namespace VRTraining.Highlight
{
    // outline for ui elements buttons drives unityengine ui outline effect
    [RequireComponent(typeof(Graphic))]
    public class UIOutlineHighlight : MonoBehaviour, IHighlightable
    {
        [SerializeField] private HighlightStyle style;
        [SerializeField] private Vector2 distance = new Vector2(6f, -6f);

        private Outline outline;
        private HighlightMode mode;

        private void Awake()
        {
            outline = gameObject.AddComponent<Outline>();
            outline.effectDistance = distance;
            outline.enabled = false;
        }

        public void SetMode(HighlightMode newMode)
        {
            mode = newMode;
            outline.enabled = mode != HighlightMode.None;
        }

        private void Update()
        {
            if (mode == HighlightMode.None) return;
            Color color = mode == HighlightMode.Expected ? style.ExpectedColor : style.HoverColor;
            // pulse only alpha outline stays readable but visibly alive
            color.a = mode == HighlightMode.Expected ? 0.5f + 0.5f * Mathf.Sin(Time.unscaledTime * style.PulseSpeed) : 1f;
            outline.effectColor = color;
        }
    }
}
