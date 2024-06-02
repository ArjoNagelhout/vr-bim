using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.Exceptions;
using Autodesk.Revit.UI;
using revit_to_vr_common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArgumentException = Autodesk.Revit.Exceptions.ArgumentException;
using ArgumentNullException = Autodesk.Revit.Exceptions.ArgumentNullException;

namespace revit_to_vr_plugin
{
    public class ToposolidEditSketchEditMode : EditMode
    {
        private ToposolidEditSketchEditModeData editSketchData => editModeData as ToposolidEditSketchEditModeData;

        protected override void OnStartEditMode()
        {

        }

        protected override void OnStopEditMode()
        {

        }
    }

    public class ToposolidModifySubElementsEditMode : EditMode
    {
        private ToposolidModifySubElementsEditModeData modifySubElementsData => editModeData as ToposolidModifySubElementsEditModeData;
        private Toposolid toposolid;
        private SlabShapeEditor slabShapeEditor;

        protected override void OnStartEditMode()
        {
            toposolid = GetToposolid();
            slabShapeEditor = toposolid.GetSlabShapeEditor();
            slabShapeEditor.Enable();
            modifySubElementsData.slabShapeData = ToposolidConversion.ConvertSlabShapeData(slabShapeEditor);
        }

        protected override void OnStopEditMode()
        {
            if (editModeData.isCanceled)
            {
                slabShapeEditor.ResetSlabShape();
            }
            slabShapeEditor.Dispose();
            slabShapeEditor = null;
        }

        private Toposolid GetToposolid()
        {
            long id = modifySubElementsData.toposolidId;
            Element toposolidElement = Application.Instance.GetElement(id);
            Debug.Assert(toposolidElement is Toposolid);
            Toposolid toposolid = toposolidElement as Toposolid;
            return toposolid;
        }

        protected override void OnUpdateEditMode(UIApplication uiApp, UpdateEditModeData data)
        {
            Debug.Assert(data is UpdateModifySubElements);
            UpdateModifySubElements d = data as UpdateModifySubElements;

            Debug.Assert(toposolid != null && toposolid.IsValidObject);
            Debug.Assert(slabShapeEditor != null && slabShapeEditor.IsValidObject);
            Debug.Assert(slabShapeEditor.IsEnabled);

            if (d.entries.Count == 0)
            {
                return; // don't need to do anything
            }

            // create transaction (see https://help.autodesk.com/view/RVT/2022/ENU/?guid=Revit_API_Revit_API_Developers_Guide_Basic_Interaction_with_Revit_Elements_Transactions_Transaction_Classes_html)
            Autodesk.Revit.ApplicationServices.Application app = uiApp.Application;
            Document document = uiApp.ActiveUIDocument.Document;
            Debug.Assert(Application.Instance.applicationState.openedDocument.CreationGUID == document.CreationGUID);
            Debug.Assert(document != null);

            using (Transaction transaction = new Transaction(document))
            {
                if (transaction.Start("Toposolid > ModifySubElements") == TransactionStatus.Started)
                {
                    foreach (UpdateModifySubElements.Entry entry in d.entries)
                    {
                        SlabShapeVertex vertex = slabShapeEditor.SlabShapeVertices.get_Item(entry.index);
                        Debug.Assert(vertex != null);
                        try
                        {
                            slabShapeEditor.ModifySubElement(vertex, entry.offset);
                        }
                        catch (Exception e)
                        {
                            transaction.RollBack();
                            UIConsole.Log(e.Message);
                        }
                        finally
                        {
                            UIConsole.Log($"Updated vertex: {entry.index}, {entry.offset}");
                        }
                    }

                    transaction.Commit();
                }
            }
        }
    }
}
