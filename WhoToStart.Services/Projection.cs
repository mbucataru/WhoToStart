using System;
using System.Collections.Generic;
using System.Text;

namespace WhoToStart.Services
{
    public class Projection
    {
        required public string Name { get; set; }

        required public double VegasProjection { get; set; }

        required public double DraftSharksProjection { get; set; }

        public double FinalProjection { get; set; }
    }
}
