using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrokenCycleGame.AI
{
    public class playerHeuristic : Heuristic
    {
        Tile goalTile;
        InfluenceMap influenceMap;
        public playerHeuristic(Tile goal, InfluenceMap i_map) : base(goal)
        {
            this.goalTile = goal;
            this.influenceMap = i_map;
        }

        public override float Estimate(Tile current)
        {
            float euclidDistance = (goalTile.Position - current.Position).Length();

            /*float tileHealth = current.health;

            float tileInfluence = (float)influenceMap.getInfluenceAt(current);

            float estimate = euclidDistance + (100 - tileHealth) + (40 + tileInfluence) * 128;

            float tileInfluence = (float)influenceMap.getInfluenceAt(current);*/

            float estimate = euclidDistance * 100/32;

            return estimate;
        }
    }
}
