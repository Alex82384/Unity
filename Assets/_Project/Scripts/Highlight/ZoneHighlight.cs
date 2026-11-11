using UnityEngine;

namespace VRTraining.Highlight
{
    // floor ring calm color when idle pulsing green when next target
    public class ZoneHighlight : MonoBehaviour, IHighlightable
    {
        private static readonly int ColorId = Shader.PropertyToID("_BaseColor");

        [SerializeField] private HighlightStyle style;
        [SerializeField] private Renderer ring;
        [SerializeField] private Color idleColor = new Color(1f, 0.85f, 0.55f, 0.7f);

        private MaterialPropertyBlock block;
        private HighlightMode mode;
        private Vector3 baseScale;

        private void Awake()
        {
            block = new MaterialPropertyBlock(); // must not created in field initializer
            baseScale = ring.transform.localScale;
        }

        public void SetMode(HighlightMode newMode) => mode = newMode;

        private void Update()
        {
            bool expected = mode == HighlightMode.Expected;
            float pulse = 0.5f + 0.5f * Mathf.Sin(Time.time * (expected ? style.PulseSpeed : 1.5f));

            Color color = expected ? style.ExpectedColor * style.GlowIntensity : idleColor;
            color.a = expected ? Mathf.Lerp(0.7f, 1f, pulse) : Mathf.Lerp(0.45f, 0.7f, pulse);

            block.SetColor(ColorId, color);
            ring.SetPropertyBlock(block);

            ring.transform.localScale = baseScale * (expected ? Mathf.Lerp(0.97f, 1.05f, pulse) : 1f);
        }
    }
}
