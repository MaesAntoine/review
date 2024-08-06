using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Grasshopper.GUI.Canvas;
using Rhino.Display;
using BIG_SQLLogger;
using BIG_UTILITY.LOGGER;
using System.Drawing;

namespace BIG_GrasshopperRibbon
{
    public class MeshOutlineComponent : BIG_Component
    {

        private bool isMenuItemChecked = false;
        private string menuItemText = "One Object";

        public MeshOutlineComponent()
            : base("View Mesh Outlines",
                  "VMO",
                  "Gets the outline curves of meshes using a projection plane.\nRight click of more Options",
                  "BIG",
                  "Mesh")
        {
        }

        protected override Bitmap Icon => Properties.Resources.MeshOutlines;
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        public override Guid ComponentGuid => new Guid("758fa630-fe62-45f6-8989-c1be9788d8ea");
        protected override string Author => "Antoine Maes";
        protected override string CoAuthor => "";


        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            Param_Geometry geometryParam = new Param_Geometry();
            geometryParam.Name = "Mesh";
            geometryParam.NickName = "M";
            geometryParam.Description = "Mesh to display the outlines of";
            geometryParam.Access = GH_ParamAccess.list;
            geometryParam.Optional = false;
            pManager.AddParameter(geometryParam);

            Param_Plane planeParam = new Param_Plane();
            planeParam.Name = "Plane";
            planeParam.NickName = "P";
            planeParam.Description = "The plane to display the mesh outlines on";
            planeParam.Access = GH_ParamAccess.item;
            planeParam.Optional = true;
            pManager.AddParameter(planeParam);
            planeParam.SetPersistentData(Plane.WorldXY);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            Param_Curve curveParam = new Param_Curve();
            curveParam.Name = "Outlines";
            curveParam.NickName = "O";
            curveParam.Description = "The outlines of the meshes";
            curveParam.Access = GH_ParamAccess.list;
            pManager.AddParameter(curveParam);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            DisplayComponentMode();
            CheckDisplayMode();

            // Input variables
            List<Mesh> inputMeshes = new List<Mesh>();
            Plane plane = Plane.WorldXY;

            // Get the input data
            if (!DA.GetDataList(0, inputMeshes)) return;
            DA.GetData(1, ref plane);

            // Handle the component mode
            if (isMenuItemChecked) { inputMeshes = MergeMeshes(inputMeshes); }

            // get outlines of the meshes with the plane
            List<Polyline> outlines = GetOutlines(plane, inputMeshes);

            // Set the output data
            DA.SetDataList(0, outlines);
        }

        private static List<Polyline> GetOutlines(Plane plane, List<Mesh> outputMeshes)
        {
            List<Polyline> outlines = new List<Polyline>();
            foreach (var mesh in outputMeshes)
            {
                outlines.AddRange(mesh.GetOutlines(plane));
            }
            return outlines;
        }

        private List<Mesh> MergeMeshes(List<Mesh> outputMeshes)
        {
            
            Mesh joinedMesh = new Mesh();
            foreach (var mesh in outputMeshes)
            {
                joinedMesh.Append(mesh);
            }
            outputMeshes.Clear();
            outputMeshes.Add(joinedMesh);

            return outputMeshes;
        }


        private static void CheckDisplayMode()
        {
            // if the viewport is not in parallel projection, add a runtime warning
            if (!Rhino.RhinoDoc.ActiveDoc.Views.ActiveView.ActiveViewport.IsParallelProjection)
            {
                MessageLog.AddWarning("The viewport is not in parallel projection. \nThe outlines will not be accurate.");
            }
        }


        private void DisplayComponentMode()
        {
            if (isMenuItemChecked)
            {
                Message = menuItemText;
            }
            else
            {
                Message = "Per Object";
            }
        }

        public override bool AppendMenuItems(ToolStripDropDown menu)
        {
            // load the default items and then add a separator
            base.AppendMenuItems(menu);
            Menu_AppendSeparator(menu);

            // add a custom item
            Menu_AppendItem(
                menu,
                menuItemText,
                Menu_MyCustomItemClicked,
                true,
                isMenuItemChecked
                );

            // return true to show the custom menu
            return true;
        }

        private void Menu_MyCustomItemClicked(object sender, EventArgs e)
        {
            isMenuItemChecked = !isMenuItemChecked;

            if (sender is ToolStripMenuItem item)
            {
                item.Checked = isMenuItemChecked;
                ExpireSolution(true);
            }
        }
    }
}