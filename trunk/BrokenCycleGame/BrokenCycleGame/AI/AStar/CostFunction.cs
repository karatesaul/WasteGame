using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrokenCycleGame.AI
{
    public abstract class CostFunction
    {
        public abstract float Calculate(Tile start, Tile end);
    }
}
