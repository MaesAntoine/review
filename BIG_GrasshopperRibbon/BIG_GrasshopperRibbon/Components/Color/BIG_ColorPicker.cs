

using BIG_UTILITY.SYSTEM;
using Grasshopper.Kernel.Parameters;
using System;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using System.Drawing;
using Grasshopper.Kernel.Attributes;
using BIG_GrasshopperRibbon;
using System.Collections.Generic;


namespace BIG_GrasshopperRibbon
{
    public class BIG_ColorPicker : BIG_Component
    {
        private enum Colors
        {
            Red,
            Orange,
            Yellow,
            Lime,
            Green,
            Turquoise,
            LightBlue,
            Blue,
            Blueberry,
            Violet,
            Purple,
            Magenta
        }

        public class ColorPickerAttributes : GH_ComponentAttributes
        {


            public Color Color { get; set; } = GH_Skin.palette_hidden_standard.Fill;
            public Color DefaultColor { get; } = GH_Skin.palette_hidden_standard.Fill;

            public ColorPickerAttributes(IGH_Component component)
              : base(component)
            { }

            protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
            {
                if (channel == GH_CanvasChannel.Objects)
                {
                    // Cache the existing fill color style.
                    //DefaultColor style = GH_Skin.palette_hidden_standard;

                    // Swap out fill color for hidden (no preview), standard components.
                    GH_Skin.palette_hidden_standard.Fill = Color;

                    // Render component
                    base.Render(canvas, graphics, channel);

                    // Put the original color back.
                    GH_Skin.palette_hidden_standard.Fill = DefaultColor;
                }
                else
                    base.Render(canvas, graphics, channel);
            }
        }

        private Color color;
        private ColorPickerAttributes attributes;



        public BIG_ColorPicker() : base(
            "Color Picker",
            "CP",
            "Component to pick colors from a preset number of BIG colors",
            "BIG",
            "Color")
        {
        }

        protected override Bitmap Icon => Properties.Resources.ColorPicker;

        public override Guid ComponentGuid => new Guid("e55d128a-5409-4d9e-a63a-3d073d2161b1");

        protected override string Author => "Andreas Bak";

        protected override string CoAuthor => "";
        public override void CreateAttributes()
        {
            if (attributes == null)
            {
                attributes = new ColorPickerAttributes(this);
            }
            m_attributes = attributes;

        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            Param_Enum colorPickParam = new Param_Enum(typeof(Colors));
            colorPickParam.Name = "ColorPick";
            colorPickParam.NickName = "CP";
            colorPickParam.Description = "Connect valuelist for color options or pick by index. If no value is supplied, all colors will be returned";
            colorPickParam.Optional = true;
            colorPickParam.Access = GH_ParamAccess.list;
            pManager.AddParameter(colorPickParam);

        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            Param_Colour colorParam = new Param_Colour();
            colorParam.Name = "Color";
            colorParam.NickName = "C";
            colorParam.Description = "Picked color";
            colorParam.Access = GH_ParamAccess.list;
            pManager.AddParameter(colorParam);
        }

        protected override void BeforeSolveInstance()
        {
            CreateAttributes();
            base.BeforeSolveInstance();
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<int> colourPickIntegers = new List<int>();
            DA.GetDataList("ColorPick", colourPickIntegers);
            List<Color> colors = new List<Color>();

            if (colourPickIntegers.Count == 0)
            {
                colors.Add(COLOR.BIG_Red);
                colors.Add(COLOR.BIG_Orange);
                colors.Add(COLOR.BIG_Yellow);
                colors.Add(COLOR.BIG_Lime);
                colors.Add(COLOR.BIG_Green);
                colors.Add(COLOR.BIG_Turquoise);
                colors.Add(COLOR.BIG_LightBlue);
                colors.Add(COLOR.BIG_Blue);
                colors.Add(COLOR.BIG_Blueberry);
                colors.Add(COLOR.BIG_Violet);
                colors.Add(COLOR.BIG_Purple);
                colors.Add(COLOR.BIG_Magenta);
            }

            else if (colourPickIntegers.Count > 0)
            {
                foreach (int colorPickInteger in colourPickIntegers)
                {
                    //Parse int to enum
                    Colors colourPick = (Colors)colorPickInteger;
                    switch (colourPick)
                    {
                        case Colors.Red: colors.Add(COLOR.BIG_Red); break;
                        case Colors.Orange: colors.Add(COLOR.BIG_Orange); break;
                        case Colors.Yellow: colors.Add(COLOR.BIG_Yellow); break;
                        case Colors.Lime: colors.Add(COLOR.BIG_Lime); break;
                        case Colors.Green: colors.Add(COLOR.BIG_Green); break;
                        case Colors.Turquoise: colors.Add(COLOR.BIG_Turquoise); break;
                        case Colors.LightBlue: colors.Add(COLOR.BIG_LightBlue); break;
                        case Colors.Blue: colors.Add(COLOR.BIG_Blue); break;
                        case Colors.Blueberry: colors.Add(COLOR.BIG_Blueberry); break;
                        case Colors.Violet: colors.Add(COLOR.BIG_Violet); break;
                        case Colors.Purple: colors.Add(COLOR.BIG_Purple); break;
                        case Colors.Magenta: colors.Add(COLOR.BIG_Magenta); break;
                    }
                }


            }

            if (colors.Count == 1)
            {
                attributes.Color = colors[0];
            }
            else
            {
                attributes.Color = attributes.DefaultColor;
            }

            DA.SetDataList("Color", colors);
        }
    }
}
