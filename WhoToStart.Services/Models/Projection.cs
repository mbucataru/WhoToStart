using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WhoToStart.Services.Models
{
    public class Projection
    {
        public int Id { get; set; }
        required public string Name { get; set; }
        required public string Team { get; set; }
        required public string Position { get; set; }
        required public string Format { get; set; }
        // TO-DO: Setup Week tracking: Not valid for now.
        public int Week { get; set; }

        public double VegasProjection { get; set; }

        public double DraftSharksProjection { get; set; }

        public double FinalProjection { get; set; }
        

        [NotMapped]
        public string? MissingDataWarning
        {
            get
            {
                if (!IsMissingData) return null;

                if (VegasProjection == 0)
                    return "This Final Projection is higher than it should be, as there are no Vegas Projections for this player. This problem is typically resolved on Thursday.";

                return "This Final Projection is lower than it should be, as there are no DraftSharks Projections for this player. This problem is typically resolved on Thursday.";
            }
        }

        [NotMapped]
        private bool IsMissingData
        {
            get => VegasProjection == 0 || DraftSharksProjection == 0;
        }


    }
}
