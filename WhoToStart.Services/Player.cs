using System;
using System.Collections.Generic;
using System.Text;

namespace WhoToStart.Services
{
    public class Player
    {
        required public string Name { get; set; }
        required public string Team { get; set; }
        required public string Position { get; set; }

        public int Id { get; set; }
    }
}
