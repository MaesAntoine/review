using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using BIG_UTILITY.LOGGER;
using BIG_UTILITY.SYSTEM;
using System.Drawing;
using System.Diagnostics;
using Grasshopper.Kernel.Parameters.Hints;
using BIG_SQLLogger;
using System.Linq;

namespace BIG_GrasshopperRibbon
{
    public class ColorTreeStructure : BIG_Component
    {
        public ColorTreeStructure()
            : base(
                  "Color Tree Structure",
                  "CTS",
                  "Creates colors for the values of a certain path index",
                  "BIG",
                  "Datatrees")
        {
        }

        override protected Bitmap Icon => Properties.Resources.ColorTreeStructure;

        public override Guid ComponentGuid => new Guid("5ede7f2a-16ce-4761-b25f-6c802e3e7b41");

        protected override string Author => "Antoine Maes";

        protected override string CoAuthor => "";

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            // Add an input that can take any type of grasshopper data
            Param_GenericObject dataParam = new Param_GenericObject();
            dataParam.Name = "Data";
            dataParam.NickName = "D";
            dataParam.Description = "Tree to evaluate";
            dataParam.Access = GH_ParamAccess.tree;
            dataParam.Optional = false;
            pManager.AddParameter(dataParam);

            // Add an input to specify the path index
            Param_Integer pathIndexParam = new Param_Integer();
            pathIndexParam.Name = "Path Index";
            pathIndexParam.NickName = "i";
            pathIndexParam.Description = "Index of the path to evaluate";
            pathIndexParam.Access = GH_ParamAccess.item;
            pathIndexParam.Optional = true;
            pManager.AddParameter(pathIndexParam);

            // Add an input to specify the colors
            Param_Colour colorParam = new Param_Colour();
            colorParam.Name = "Colors";
            colorParam.NickName = "C";
            colorParam.Description = "Colors that build the gradient";
            colorParam.Access = GH_ParamAccess.list;
            colorParam.Optional = true;
            colorParam.SetPersistentData(ToGhColor(COLOR.AllColors));
            pManager.AddParameter(colorParam);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            Param_Colour colorParam = new Param_Colour();
            colorParam.Name = "Colors";
            colorParam.NickName = "C";
            colorParam.Description = "Colors for the values of the path index";
            colorParam.Access = GH_ParamAccess.tree;
            pManager.AddParameter(colorParam);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // initialize the inputs and outputs
            GH_Structure<IGH_Goo> data = new GH_Structure<IGH_Goo>();
            GH_Integer pathIndex = new GH_Integer();
            List<GH_Colour> colors = new List<GH_Colour>();
            List<GH_Colour> newGhColors = new List<GH_Colour>();

            // get the inputs
            if (!DA.GetDataTree(0, out data)) return;
            DA.GetData(1, ref pathIndex);
            DA.GetDataList(2, colors);

            // Handle edge cases
            if (data.Paths[0].Length <= pathIndex.Value) { MessageLog.AddError("Path index is higher than the path length"); return; }
            if (colors.Count < 1) { MessageLog.AddError("You provided an empty list of colors"); return; }


            // for each branch paths, get the value at the path index, duplicate by the number of elements in the branch
            List<double> pathIndices = new List<double>();
            CreatePathIndices(data, pathIndex, pathIndices);

            // if all pathIndices are 0, newColors should be a list of the first color duplicated
            newGhColors = createNewGhColors(colors, pathIndices);

            // add the indices using the list to tree function
            GH_Structure<GH_Colour> newTree = ListToTree(newGhColors, data);
            DA.SetDataTree(0, newTree);
        }

        private static void CreatePathIndices(GH_Structure<IGH_Goo> data, GH_Integer pathIndex, List<double> pathIndices)
        {
            int index = pathIndex.Value;
            foreach (GH_Path path in data.Paths)
            {
                for (int i = 0; i < data.get_Branch(path).Count; i++)
                {
                    pathIndices.Add(path.Indices[index]);
                }
            }
        }

        private List<GH_Colour> createNewGhColors(List<GH_Colour> colors, List<double> pathIndices)
        {
            List<GH_Colour> newGhColors;
            if (pathIndices.All(x => x == 0))
            {
                // if all pathIndices are 0, newColors should be a list of the first color duplicated
                newGhColors = Enumerable.Repeat(colors[0], pathIndices.Count).ToList();
            }
            else
            {
                // remap the indices and create new gh colors
                List<double> remappedValues = NUMBER.Remap(pathIndices, pathIndices.Min(), pathIndices.Max(), 0.00, 1.00);
                List<Color> newColors = COLOR.ValuesToSpectrumColors(remappedValues, ToColor(colors), true);
                newGhColors = ToGhColor(newColors);
            }

            return newGhColors;
        }

        private List<GH_Colour> ToGhColor(List<Color> colors)
        {
            List<GH_Colour> ghColors = new List<GH_Colour>();
            foreach (Color color in colors)
            {
                ghColors.Add(new GH_Colour(color));
            }
            return ghColors;
        }

        private List<Color> ToColor(List<GH_Colour> ghColors)
        {
            List<Color> colors = new List<Color>();
            foreach (GH_Colour ghColor in ghColors)
            {
                colors.Add(ghColor.Value);
            }
            return colors;
        }


        private GH_Structure<GH_Colour> ListToTree(List<GH_Colour> list, GH_Structure<IGH_Goo> tree)
        {

            GH_Structure<GH_Colour> newTree = new GH_Structure<GH_Colour>();

            int count = 0;
            for (int i=0; i < tree.PathCount; i++)
            {
                GH_Path path = tree.Paths[i];
                var elements = tree.get_Branch(path);

                for (int j=0; j < elements.Count; j++)
                {
                    newTree.Append(list[count], path);
                    count++;
                }
            }
            return newTree;
        }
    }
}
