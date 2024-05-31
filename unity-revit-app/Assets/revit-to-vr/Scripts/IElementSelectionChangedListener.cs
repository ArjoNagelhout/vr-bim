namespace RevitToVR
{
    
    public interface IElementSelectionChangedListener
    {
        // gets called on the specific element
        public void OnSelect();

        // gets called on the specific element
        public void OnDeselect();
    }
}