using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrokenCycleGame.AI
{
    class PlayerCostFunction : CostFunction
    {
        private InfluenceMap influenceMap;

        public PlayerCostFunction(InfluenceMap i_map)
        {
            influenceMap = i_map;
        }

        public override float Calculate(Tile start, Tile end)
        {
            float influenceCost = 0;
            if(influenceMap != null)
            influenceCost = -(float)influenceMap.getInfluenceAt(end);
            float distanceCost = (end.Position-start.Position).Length();

            float tileValue = 100 - end.scorevalue;

            return (influenceCost < 0 ? -influenceCost : 0) + distanceCost + tileValue;
        }
    }
}
