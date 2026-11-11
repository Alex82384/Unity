using UnityEngine;

namespace VRTraining.Highlight
{
    [CreateAssetMenu(menuName = "VR Training/Highlight Style", fileName = "HighlightStyle")]
    public class HighlightStyle : ScriptableObject
    {
        [SerializeField] private Material outlineMaterial;
        [SerializeField] private Color expectedColor = new Color(0.2f, 1f, 0.45f, 1f);
        [SerializeField] private Color hoverColor = Color.white;
        [SerializeField, Min(0f)] private float width = 0.02f;
        [SerializeField, Min(0f)] private float pulseSpeed = 5f;
        [SerializeField, Min(1f)] private float glowIntensity = 1.6f;

        public Material OutlineMaterial => outlineMaterial;
        public Color ExpectedColor => expectedColor;
        public Color HoverColor => hoverColor;
        public float Width => width;
        public float PulseSpeed => pulseSpeed;
        public float GlowIntensity => glowIntensity;
    }
}
