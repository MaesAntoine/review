using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using BIG_UTILITY.LOGGER;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace BIG_GrasshopperRibbon
{
    /// <summary>
    /// This ShiftPath component works almost like the native ShiftPath component.
    /// The difference is that this component will flatten the datatree if the offset
    /// (as an absolute value) is greater than the path indices length.
    /// </summary>
    /// <remarks>
    /// Original Author: Antoine Maes (BIG DT)
    /// </remarks>
    public class ShiftPathComponent : BIG_Component
    {
        public ShiftPathComponent()
          : base(
                "ShiftPath",
                "SP",
                "A more cooperative ShiftPath component",
                "BIG",
                "Datatrees")
        {
        }

        protected override Bitmap Icon => Properties.Resources.ShiftPaths;
        public override Guid ComponentGuid =>  new Guid("D2010748-DA3E-4B7F-9AD7-75026D2E888C");

        protected override string Author => "Antoine Maes";

        protected override string CoAuthor => "";

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            // create a new parameter that doesn't care about the type of the input
            Param_GenericObject genericParam = new Param_GenericObject();
            genericParam.Name = "Datatree";
            genericParam.NickName = "D";
            genericParam.Description = "Datatree to shift";
            genericParam.Access = GH_ParamAccess.tree;
            genericParam.Optional = false;
            pManager.AddParameter(genericParam);

            // offset parameter integer
            Param_Integer offsetParam = new Param_Integer();
            offsetParam.Name = "Offset";
            offsetParam.NickName = "O";
            offsetParam.Description = "Offset to shift the datatree";
            offsetParam.Access = GH_ParamAccess.item;
            offsetParam.Optional = true;
            pManager.AddParameter(offsetParam);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            // return the shifted datatree
            IGH_Param genericParam = new Param_GenericObject();
            genericParam.Name = "ShiftedDatatree";
            genericParam.NickName = "D";
            genericParam.Access = GH_ParamAccess.tree;
            pManager.AddParameter(genericParam);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // declare the input and output variables
            GH_Structure<IGH_Goo> datatree;
            GH_Structure<IGH_Goo> finalTree; 
            int offset = 0;

            // get the input
            if (!DA.GetDataTree(0, out datatree)) return;
            if (!DA.GetData(1, ref offset)) return;

            finalTree = ShiftDatatree(datatree, offset);

            // set the output
            DA.SetDataTree(0, finalTree);
        }


        private GH_Structure<IGH_Goo> ShiftDatatree(GH_Structure<IGH_Goo> datatree, int offset)
        {
            // if the offset is 0, return the original datatree
            if (offset == 0) return datatree;

            // if the absolute value of the offset is greater than the path indices length, return a flattened tree
            int absoluteOffset = Math.Abs(offset);
            if (absoluteOffset >= datatree.Paths[0].Indices.Length)
            {
                // add runtime remark
                string message = "Offset is greater than the path length. The Datatree flattened to {{0}}";
                MessageLog.AddRemark(message);

                // make a copy of the datatree and flatten it
                GH_Structure<IGH_Goo> copyDatatree = datatree.Duplicate() as GH_Structure<IGH_Goo>;
                copyDatatree.Flatten();
                return copyDatatree;
            }

            // create a new datatree of "any type"
            GH_Structure<IGH_Goo> shiftedDatatree = new GH_Structure<IGH_Goo>();


            // do the shifting
            foreach (GH_Path path in datatree.Paths)
            {
                GH_Path newPath = path;

                if (offset > 0)
                {
                    // shift the path from the left
                    for (int i = 0; i < absoluteOffset; i++)
                    {
                        newPath = newPath.CullFirstElement();
                    }
                }
                else
                {
                    // shift the path from the right
                    for (int i = 0; i < absoluteOffset; i++)
                    {
                        newPath = newPath.CullElement();
                    }
                }

                // build the new datatree
                shiftedDatatree.AppendRange((List<IGH_Goo>)datatree.get_Branch(path), newPath);
            }

            return shiftedDatatree;
        }

    }
}