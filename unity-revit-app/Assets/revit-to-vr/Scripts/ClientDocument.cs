using System;
using System.Collections.Generic;
using System.Linq;
using revit_to_vr_common;
using UnityEngine;

namespace RevitToVR
{
    
    // to be used by the renderer / interaction spawners
    public interface IClientDocumentListener
    {
        void OnOpen();

        void ElementAdded(long elementId, VRBIM_Element element);

        void ElementRemoved(long elementId);

        void ElementSelected(long elementId);

        void ElementDeselected(long elementId);

        void OnClose();
    }

    public class ClientDocument : IDisposable
    {
        public IClientDocumentListener Listener;

        // key = element id
        public Dictionary<long, VRBIM_Element> elements = new Dictionary<long, VRBIM_Element>();
        
        public HashSet<long> selectedElementIds = new HashSet<long>();
        
        public void Apply(DocumentOpenedEvent e)
        {
            Listener?.OnOpen();
        }

        public void Apply(DocumentClosedEvent e)
        {
            Listener?.OnClose();
        }

        public VRBIM_Element GetElement(long elementId)
        {
            Debug.Assert(elements.ContainsKey(elementId));
            return elements[elementId];
        }

        public void Apply(SelectionChangedEvent e)
        {
            HashSet<long> new_ = e.selectedElementIds.ToHashSet();
            HashSet<long> current = selectedElementIds;

            HashSet<long> toAdd = new HashSet<long>(new_);
            toAdd.ExceptWith(current);
            
            HashSet<long> toRemove = new HashSet<long>(current);
            toRemove.ExceptWith(new_);

            foreach (long id in toAdd)
            {
                Listener?.ElementSelected(id);
            }

            foreach (long id in toRemove)
            {
                Listener?.ElementDeselected(id);
            }

            selectedElementIds = new_;
        }

        public void Apply(DocumentChangedEvent e)
        {
            foreach (long deletedElementId in e.deletedElementIds)
            {
                Debug.Log($"deletedElementId: {deletedElementId}");
                if (elements.ContainsKey(deletedElementId))
                {
                    elements.Remove(deletedElementId);
                    Listener?.ElementRemoved(deletedElementId);
                }
            }
            
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
                if (selectedElementIds.Contains(key))
                {
                    Listener?.ElementSelected(key);
                }
            }
        }

        public void Dispose()
        {
            
        }
    }
}