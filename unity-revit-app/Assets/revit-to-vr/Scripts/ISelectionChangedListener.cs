namespace RevitToVR
{
    
    public interface ISelectionChangedListener
    {
        // gets called on the specific element
        public void OnSelect();

        // gets called on the specific element
        public void OnDeselect();
    }
}