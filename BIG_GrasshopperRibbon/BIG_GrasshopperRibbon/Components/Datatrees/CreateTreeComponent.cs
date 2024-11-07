using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using BIG_UTILITY.LOGGER;
using System.Drawing;

namespace BIG_GrasshopperRibbon
{
    /// <summary>
    /// This CreateTree component creates a new datatree from paths and optional data.
    /// Its behavior is similar to the CreateTree component from Elefront but allows for
    /// the creation of datatrees with no data.
    /// </summary>
    /// <remarks>
    /// Original Author: Antoine Maes (BIG DT)
    /// </remarks>
    public class CreateTreeComponent : BIG_Component
    {
        public CreateTreeComponent() 
            : base("Create Tree",
                  "CT",
                  "Creates a new datatree from paths and optional data",
                  "BIG",
                  "Datatrees")
        {
        }

        protected override Bitmap Icon => Properties.Resources.CreateTree;

        public override Guid ComponentGuid => new Guid("8a712935-fab5-4e9c-af00-3ee54a297cdd");

        protected override string Author => "Antoine Maes";

        protected override string CoAuthor => "";

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            Param_StructurePath pathParam = new Param_StructurePath();
            pathParam.Name = "Paths";
            pathParam.NickName = "P";
            pathParam.Description = "Paths to create the datatree";
            pathParam.Access = GH_ParamAccess.tree;
            pathParam.Optional = false;
            pManager.AddParameter(pathParam);

            Param_GenericObject dataParam = new Param_GenericObject();
            dataParam.Name = "Data";
            dataParam.NickName = "D";
            dataParam.Description = "Data to add to the datatree";
            dataParam.Access = GH_ParamAccess.tree;
            dataParam.Optional = true;
            pManager.AddParameter(dataParam);

        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            Param_GenericObject genericParam = new Param_GenericObject();
            genericParam.Name = "Datatree";
            genericParam.NickName = "D";
            genericParam.Access = GH_ParamAccess.tree;
            pManager.AddParameter(genericParam);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // initialize the input
            GH_Structure<GH_StructurePath> pathTree = new GH_Structure<GH_StructurePath>();
            GH_Structure<IGH_Goo> data = new GH_Structure<IGH_Goo>();

            // get the input
            if (!DA.GetDataTree(0, out pathTree)) return;
            DA.GetDataTree(1, out data);

            // create the datatree and other lists
            GH_Structure<IGH_Goo> datatree;
            List<GH_StructurePath> pathList = new List<GH_StructurePath>();
            List<IGH_Goo> dataList = new List<IGH_Goo>();

            // flatten the trees
            if (pathTree.Branches.Count > 0)
            {
                pathTree.Flatten();
                pathList = pathTree.Branches[0];
            }
            if (data.Branches.Count > 0)
            {
                data.Flatten();
                dataList = data.Branches[0];
            }

            // create the datatree
            datatree = CreateDatatree(pathList, dataList);

            // set the output
            DA.SetDataTree(0, datatree);
        }

        private GH_Structure<IGH_Goo> CreateDatatree(List<GH_StructurePath> pathList, List<IGH_Goo> dataList)
        {
            // new datatree
            GH_Structure<IGH_Goo> datatree = new GH_Structure<IGH_Goo>();

            // if there is no data, create an empty datatree
            if (dataList.Count == 0)
            {
                for (int i = 0; i < pathList.Count; i++)
                {
                    datatree.AppendRange(new List<IGH_Goo>(), pathList[i].Value);
                }
                return datatree;
            }

            // if there is data but it is not of the same length as the paths, return a warning
            else if (pathList.Count != dataList.Count)
            {
                MessageLog.AddWarning("If Data is provided, it must be of same length as Paths");
                return datatree;
            }

            // if data and path have the same length
            for (int i = 0;i < pathList.Count;i++)
            {
                datatree.Append(dataList[i], pathList[i].Value);
            }
            return datatree;
        }
    }
}