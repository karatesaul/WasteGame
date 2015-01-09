using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrokenCycleGame.AI
{
    class roombaHeuristic : Heuristic
    {
        Tile goalTile;
        public roombaHeuristic(Tile goal) : base(goal)
        {
            this.goalTile = goal; 
        }


        public override float Estimate(Tile current)
        {
            return (goalTile.Position - current.Position).Length();
        }
    }
}
