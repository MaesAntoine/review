using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace BIG_GrasshopperRibbon
{

    public class ConstructMaskComponent : BIG_Component
    {

        public ConstructMaskComponent()
          : base("Construct Mask",
                "CM",
                "Construct a mask (string) from a list of strings",
                "BIG",
                "Datatrees")
        {
        }

        protected override Bitmap Icon => Properties.Resources.ConstructMask;
        public override Guid ComponentGuid => new Guid("19D85D8C-DF5F-474D-A00E-232C789F86AF");

        protected override string Author => "Antoine Maes";

        protected override string CoAuthor => "";

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            Param_String indicesParam = new Param_String();
            indicesParam.Name = "Indices";
            indicesParam.NickName = "I";
            indicesParam.Description = "Indices to construct the mask";
            indicesParam.Access = GH_ParamAccess.list;
            indicesParam.Optional = false;
            pManager.AddParameter(indicesParam);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            Param_String maskParam = new Param_String();
            maskParam.Name = "Mask";
            maskParam.NickName = "M";
            maskParam.Description = "Mask constructed from the indices";
            maskParam.Access = GH_ParamAccess.tree;
            pManager.AddParameter(maskParam);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // get the indices list from the input
            var indicesList = new List<string>();
            DA.GetDataList(0, indicesList);

            // Create a mask string from the indices, separated by ";"
            var mask = $"{{{string.Join(";", indicesList)}}}";

            // Set the output
            DA.SetData(0, mask);
        }
    }
}