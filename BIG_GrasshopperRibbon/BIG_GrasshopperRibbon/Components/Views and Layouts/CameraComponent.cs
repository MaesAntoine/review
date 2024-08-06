using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;
using Rhino.Display;
using System.Drawing;

namespace BIG_GrasshopperRibbon
{
    public class CameraComponent : BIG_Component
    {
        public CameraComponent()
            : base(
                  "Camera",
                  "Cam",
                  "Update your view position and direction from Grasshopper",
                  "BIG",
                  "Views and Layouts")
        {
        }

        protected override Bitmap Icon => Properties.Resources.Camera;
        public override Guid ComponentGuid => new Guid("11bbbe73-cc4e-4dbc-918a-3efb2ae700e9");

        protected override string Author => "Antoine Maes";

        protected override string CoAuthor => "";



        private static List<Curve> _originalCameraCurves = CreateCameraCurves();
        private static List<Curve> _modifiedCameraCurves;
        protected static List<Curve> CreateCameraCurves()
        {
            // create a camera representation as curves on the base plane
            int size = 3;
            int si = size / 2;

            Point3d[] boxPoints = {
                new Point3d(-size, -size, -size * 2),
                new Point3d(size, -size, -size * 2),
                new Point3d(size, size, -size * 2),
                new Point3d(-size, size, -size * 2),
                new Point3d(-size, -size, 0),
                new Point3d(size, -size, 0),
                new Point3d(size, size, 0),
                new Point3d(-size, size, 0)
            };

            Point3d[] pyramidPoints = {
                new Point3d(-size, -size, size),
                new Point3d(size, -size, size),
                new Point3d(size, size, size),
                new Point3d(-size, size, size),
                new Point3d(-si, -si, 0),
                new Point3d(si, -si, 0),
                new Point3d(si, si, 0),
                new Point3d(-si, si, 0)
            };

            Curve[] CreateCurves(Point3d[] points)
            {
                return new Curve[]
                {
                    new LineCurve(points[0], points[1]),
                    new LineCurve(points[1], points[2]),
                    new LineCurve(points[2], points[3]),
                    new LineCurve(points[3], points[0]),
                    new LineCurve(points[4], points[5]),
                    new LineCurve(points[5], points[6]),
                    new LineCurve(points[6], points[7]),
                    new LineCurve(points[7], points[4]),
                    new LineCurve(points[0], points[4]),
                    new LineCurve(points[1], points[5]),
                    new LineCurve(points[2], points[6]),
                    new LineCurve(points[3], points[7])
                };
            }

            // build the curves
            Curve[] boxCurve = CreateCurves(boxPoints);
            Curve[] pyramidCurve = CreateCurves(pyramidPoints);

            // merge all curves into one big list of curves
            List<Curve> allCurves = new List<Curve>();
            allCurves.AddRange(pyramidCurve);
            allCurves.AddRange(boxCurve);

            return allCurves;
        }


        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            Param_Point positionParam = new Param_Point();
            positionParam.Name = "Position";
            positionParam.NickName = "P";
            positionParam.Description = "Camera position";
            positionParam.Access = GH_ParamAccess.item;
            positionParam.Optional = false;
            pManager.AddParameter(positionParam);

            Param_Point targetParam = new Param_Point();
            targetParam.Name = "Target";
            targetParam.NickName = "T";
            targetParam.Description = "Camera target";
            targetParam.Access = GH_ParamAccess.item;
            targetParam.Optional = true;
            pManager.AddParameter(targetParam);
            targetParam.SetPersistentData(new Point3d(0, 0, 0));

            Param_Vector upDirectionParam = new Param_Vector();
            upDirectionParam.Name = "Up Direction";
            upDirectionParam.NickName = "U";
            upDirectionParam.Description = "Camera up direction";
            upDirectionParam.Access = GH_ParamAccess.item;
            upDirectionParam.Optional = true;
            pManager.AddParameter(upDirectionParam);
            upDirectionParam.SetPersistentData(new Vector3d(0, 0, 1));

            Param_Number scaleParam = new Param_Number();
            scaleParam.Name = "Size";
            scaleParam.NickName = "S";
            scaleParam.Description = "Size of the curve representation of the camera, as a scale factor.";
            scaleParam.Access = GH_ParamAccess.item;
            scaleParam.Optional = true;
            pManager.AddParameter(scaleParam);
            scaleParam.SetPersistentData(1);

            Param_Boolean isViewParallelParam = new Param_Boolean();
            isViewParallelParam.Name = "Is View Parallel";
            isViewParallelParam.NickName = "P";
            isViewParallelParam.Description = "Set the VIEWPORT to parallel projection mode?";
            isViewParallelParam.Access = GH_ParamAccess.item;
            isViewParallelParam.Optional = true;
            pManager.AddParameter(isViewParallelParam);
            isViewParallelParam.SetPersistentData(false);

            Param_Boolean isActiveParam = new Param_Boolean();
            isActiveParam.Name = "Is Active";
            isActiveParam.NickName = "A";
            isActiveParam.Description = "Is the camera active?";
            isActiveParam.Access = GH_ParamAccess.item;
            isActiveParam.Optional = true;
            pManager.AddParameter(isActiveParam);
            isActiveParam.SetPersistentData(false);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            Param_Point positionParam = new Param_Point();
            positionParam.Name = "Position";
            positionParam.NickName = "P";
            positionParam.Description = "Camera position";
            positionParam.Access = GH_ParamAccess.item;
            pManager.AddParameter(positionParam);

            Param_Point targetParam = new Param_Point();
            targetParam.Name = "Target";
            targetParam.NickName = "T";
            targetParam.Description = "Camera target";
            targetParam.Access = GH_ParamAccess.item;
            pManager.AddParameter(targetParam);

            Param_Plane planeParam = new Param_Plane();
            planeParam.Name = "Perpendicular Plane";
            planeParam.NickName = "P";
            planeParam.Description = "A perpendicular plane to the camera at the target location";
            planeParam.Access = GH_ParamAccess.item;
            pManager.AddParameter(planeParam);

            Param_Curve cameraCurveParam = new Param_Curve();
            cameraCurveParam.Name = "Camera curves";
            cameraCurveParam.NickName = "C";
            cameraCurveParam.Description = "Representation of a camera as curves";
            cameraCurveParam.Access = GH_ParamAccess.list;
            pManager.AddParameter(cameraCurveParam);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // get the inputs
            Point3d position = new Point3d();
            Point3d target = new Point3d();
            Vector3d upDirection = new Vector3d();
            bool shouldViewBeParallel = false;
            bool active = false;
            double sizeScale = 1;


            // get the inputs
            if (!DA.GetData(0, ref position)) return;
            DA.GetData(1, ref target);
            DA.GetData(2, ref upDirection);
            DA.GetData(3, ref sizeScale);
            DA.GetData(4, ref shouldViewBeParallel);
            DA.GetData(5, ref active);


            // get the viewport to modify
            RhinoViewport viewport = Rhino.RhinoDoc.ActiveDoc.Views.ActiveView.ActiveViewport;
            Plane targetPlane = getPerpendicularPlane(target, position, upDirection);

            // handle is active
            if (active)
            {
                viewport.SetCameraLocations(target, position);
                viewport.CameraUp = upDirection;
            }
            else
            {
                // make a representation of a camera
                TransformCamera(targetPlane, position, sizeScale);
                DA.SetDataList(3, _modifiedCameraCurves);
            }

            // set the camera projection mode
            HandleViewportMode(shouldViewBeParallel, viewport);

            DA.SetData(0, position);
            DA.SetData(1, target);
            DA.SetData(2, targetPlane);
        }


        private static void HandleViewportMode(bool shouldViewBeParallel, RhinoViewport viewport)
        {
            if (shouldViewBeParallel && !viewport.IsParallelProjection)
            {
                viewport.ChangeToParallelProjection(true);
            }
            else if (!shouldViewBeParallel && viewport.IsParallelProjection)
            {
                viewport.ChangeToPerspectiveProjection(true, 50.0);
            }
        }

        private void TransformCamera(Plane plane, Point3d origin, double scale)
        {
            _modifiedCameraCurves = new List<Curve>();
            foreach (Curve curve in _originalCameraCurves)
            {
                _modifiedCameraCurves.Add(curve.DuplicateCurve());
            }

            plane.Origin = origin;
            Transform xOrient = Transform.PlaneToPlane(Plane.WorldXY, plane);
            Transform xScale = Transform.Scale(plane.Origin, scale);
            Transform xForm = xScale * xOrient;

            foreach (Curve curve in _modifiedCameraCurves)
            {
                curve.Transform(xForm);
            }
        }

        private Plane getPerpendicularPlane(Point3d target, Point3d position, Vector3d upVector)
        {
            Vector3d camDirection = target - position;
            Vector3d vectorX = Vector3d.CrossProduct(upVector, camDirection);
            vectorX.Unitize();

            Plane targetPlane = new Plane(target, vectorX, Vector3d.CrossProduct(camDirection, vectorX));
            return targetPlane;
        }
    }
}
