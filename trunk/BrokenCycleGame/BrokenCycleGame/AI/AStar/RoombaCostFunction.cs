using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrokenCycleGame.AI
{
    class RoombaCostFunction : CostFunction
    {
        public override float Calculate(Tile start, Tile end)
        {
            return 32.0f;
        }
    }
}
