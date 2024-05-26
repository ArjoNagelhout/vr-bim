using Autodesk.Revit.DB;
using revit_to_vr_common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace revit_to_vr_plugin
{
    public static partial class DataConversion
    {
        public static VRBIM_Line ConvertLine(Line line)
        {
            return new VRBIM_Line()
            {
                direction = ConvertXYZ(line.Direction),
                origin = ConvertXYZ(line.Origin)
            };
        }

        public static VRBIM_Arc ConvertArc(Arc arc)
        {
            return new VRBIM_Arc()
            {
                center = ConvertXYZ(arc.Center),
                normal = ConvertXYZ(arc.Normal),
                radius = (float)arc.Radius,
                xDirection = ConvertXYZ(arc.XDirection),
                yDirection = ConvertXYZ(arc.YDirection)
            };
        }

        public static VRBIM_Curve ConvertCurve(Curve curve)
        {
            VRBIM_Curve output = null;
            switch (curve)
            {
                case Line line:
                    output = ConvertLine(line);
                    break;
                case Arc arc:
                    output = ConvertArc(arc);
                    break;
                default:
                    output = new VRBIM_Curve();
                    break;

            }

            // set properties valid for all curve types
            output.startParameter = (float)curve.GetEndParameter(0);
            output.endParameter = (float)curve.GetEndParameter(1);
            output.startPoint = ConvertXYZ(curve.GetEndPoint(0));
            output.endPoint = ConvertXYZ(curve.GetEndPoint(1));
            output.isBound = curve.IsBound;
            output.isClosed = curve.IsClosed;
            output.isCyclic = curve.IsCyclic;
            output.length = (float)curve.Length;

            if (curve.IsBound)
            {
                // get tesselated curve for display purposes (only valid for bound curves)
                IList<XYZ> curvePoints = curve.Tessellate();
                output.tesselatedCurve = new List<VRBIM_Vector3>(curvePoints.Count);
                foreach (XYZ curvePoint in curvePoints)
                {
                    output.tesselatedCurve.Add(ConvertXYZ(curvePoint));
                }
            }

            return output;
        }
    }
}
