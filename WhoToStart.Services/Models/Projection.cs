using System;
using System.Collections.Generic;
using System.Text;

namespace WhoToStart.Services.Models
{
    public class Projection
    {
        public int Id { get; set; }
        required public string Name { get; set; }
        required public string Team { get; set; }
        required public string Position { get; set; }
        // TO-DO: Setup Week tracking: Not valid for now.
        public int Week { get; set; }

        public double VegasProjection { get; set; }

        public double DraftSharksProjection { get; set; }

        public double FinalProjection { get; set; }
    }
}
