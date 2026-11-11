namespace VRTraining.Highlight
{
    public enum HighlightMode
    {
        None,
        Expected,
        Hover
    }

    // every highlight script implements scenario code does not care how object highlighted
    // another outline asset for example highlight plus could added same way
    public interface IHighlightable
    {
        void SetMode(HighlightMode mode);
    }
}
