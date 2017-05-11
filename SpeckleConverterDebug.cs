﻿using System;
using System.Collections.Generic;
using System.Linq;

using Grasshopper.Kernel;
using Rhino.Geometry;

using GH_IO.Serialization;
using System.Diagnostics;
using Grasshopper.Kernel.Parameters;

using SpeckleGhRhConverter;


using Grasshopper;
using Grasshopper.Kernel.Data;

using Newtonsoft.Json;
using System.Dynamic;
using SpeckleCommon;

namespace SpeckleGrasshopper
{
    public class EncodeToSpeckle : GH_Component
    {
        public EncodeToSpeckle()
          : base("Speckle Converter", "Speckle Converter",
              "Speckle Converter",
              "Speckle", "Debug")
        {
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("{c4442de1-c440-40ba-8da7-33c89eb1a529}"); }
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Object", "O", "Objects to convert.", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Conversion Result String", "S", "Conversion result string.", GH_ParamAccess.item);
            pManager.AddGenericParameter("Conversion Result", "R", "Conversion result object.", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            object myObj = new object();
            DA.GetData(0, ref myObj);

            var result = GhRhConveter.fromGhRhObject(myObj);
            DA.SetData(0, JsonConvert.SerializeObject(result, Formatting.Indented));
            DA.SetData(1, result);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }
    }

    public class DecodeSpeckle : GH_Component
    {
        GhRhConveter c = new GhRhConveter();

        public DecodeSpeckle()
          : base("Speckle Encoder", "Speckle Encoder",
              "Speckle Encoder",
              "Speckle", "Debug")
        {
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("{63502b81-8029-402a-8990-6ea6c8986237}"); }
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Object", "O", "Objects to convert.", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Conversion Result", "R", "Conversion result.", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            object myObj = new object();
            DA.GetData(0, ref myObj);

            var result = c.encodeObject(((Grasshopper.Kernel.Types.GH_ObjectWrapper)myObj).Value as dynamic);
            DA.SetData(0, result);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }
    }
}
