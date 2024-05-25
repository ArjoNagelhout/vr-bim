using System;
using System.Collections.Generic;
using revit_to_vr_common;

namespace RevitToVR
{
    
    // to be used by the renderer / interaction spawners
    public interface IClientDocumentListener
    {
        void OnOpen();

        void ElementAdded(long elementId, VRBIM_Element element);

        void ElementRemoved(long elementId);

        void OnClose();
    }
    
    public class ClientDocument : IDisposable
    {
        public IClientDocumentListener Listener;

        // key = element id
        public Dictionary<long, VRBIM_Element> elements = new Dictionary<long, VRBIM_Element>();
        
        public void Apply(DocumentOpenedEvent e)
        {
            Listener?.OnOpen();
        }

        public void Apply(DocumentClosedEvent e)
        {
            Listener?.OnClose();
        }
        
        public void Apply(DocumentChangedEvent e)
        {
            foreach (KeyValuePair<long, VRBIM_Element> changedElement in e.changedElements)
            {
                long key = changedElement.Key;
                if (elements.ContainsKey(key))
                {
                    elements.Remove(key);
                    Listener?.ElementRemoved(key);
                }
                
                elements.Add(key, changedElement.Value);
                Listener?.ElementAdded(key, changedElement.Value);
            }
        }

        public void Dispose()
        {
            
        }
    }
}