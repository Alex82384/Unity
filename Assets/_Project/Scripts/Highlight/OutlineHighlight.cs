using System.Collections.Generic;
using UnityEngine;

namespace VRTraining.Highlight
{
    // outline for 3d objects adds outline material outlinehull shader to all renderers
    // while object highlighted and removes afterwards colors in highlightstyle
    public class OutlineHighlight : MonoBehaviour, IHighlightable
    {
        private static readonly int ColorId = Shader.PropertyToID("_OutlineColor");
        private static readonly int WidthId = Shader.PropertyToID("_OutlineWidth");
        private static readonly int VerticalWeightId = Shader.PropertyToID("_VerticalWeight");
        private static readonly int LiftId = Shader.PropertyToID("_Lift");

        [SerializeField] private HighlightStyle style;
        [SerializeField, Min(0.1f)] private float widthScale = 1f;
        [SerializeField, Range(0f, 1f)] private float verticalWeight = 1f;
        [SerializeField, Min(0f)] private float lift;
        [SerializeField] private Renderer[] renderers;

        private readonly Dictionary<Renderer, Material[]> originalMaterials = new Dictionary<Renderer, Material[]>();
        private MaterialPropertyBlock block;
        private HighlightMode mode = HighlightMode.None;

        private void Awake()
        {
            block = new MaterialPropertyBlock(); // must not created in field initializer
            if (renderers == null || renderers.Length == 0)
                renderers = GetComponentsInChildren<Renderer>();
        }

        private void OnDisable() => SetMode(HighlightMode.None);

        public void SetMode(HighlightMode newMode)
        {
            if (newMode == mode) return;
            mode = newMode;

            if (mode == HighlightMode.None) RemoveOutline();
            else ApplyOutline();
        }

        private void Update()
        {
            if (mode == HighlightMode.None) return;
            UpdateColorAndWidth();
        }

        private void ApplyOutline()
        {
            foreach (var r in renderers)
            {
                if (r == null || originalMaterials.ContainsKey(r)) continue;

                var original = r.sharedMaterials;
                originalMaterials[r] = original;

                var extended = new Material[original.Length + 1];
                original.CopyTo(extended, 0);
                extended[original.Length] = style.OutlineMaterial;
                r.sharedMaterials = extended;
            }
            UpdateColorAndWidth();
        }

        private void RemoveOutline()
        {
            foreach (var pair in originalMaterials)
            {
                if (pair.Key == null) continue;
                pair.Key.sharedMaterials = pair.Value;
                pair.Key.SetPropertyBlock(null);
            }
            originalMaterials.Clear();
        }

        private void UpdateColorAndWidth()
        {
            float pulse = mode == HighlightMode.Expected ? 0.5f + 0.5f * Mathf.Sin(Time.time * style.PulseSpeed) : 1f;
            Color color = mode == HighlightMode.Expected ? style.ExpectedColor * style.GlowIntensity : style.HoverColor;
            float width = style.Width * widthScale * (mode == HighlightMode.Expected ? Mathf.Lerp(0.6f, 1.2f, pulse) : 1f);

            block.SetColor(ColorId, color);
            block.SetFloat(WidthId, width);
            block.SetFloat(VerticalWeightId, verticalWeight);
            block.SetFloat(LiftId, lift);

            foreach (var r in originalMaterials.Keys)
            {
                if (r == null) continue;
                // outline material last slot of renderer
                r.SetPropertyBlock(block, r.sharedMaterials.Length - 1);
            }
        }
    }
}
