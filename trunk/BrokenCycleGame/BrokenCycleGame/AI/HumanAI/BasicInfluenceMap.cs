using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrokenCycleGame.AI.HumanAI
{
    /// <summary>
    /// This is an influence map with base influences set
    /// for BasicAI.
    /// </summary>
    public class BasicInfluenceMap : InfluenceMap
    {
        public BasicInfluenceMap(Level level) : base(level)
        {
            setBaseInfluence(typeof(Player), 8.0);
            setBaseInfluence(typeof(Roomba), -8.0);
            setBaseInfluence(typeof(Tower), -8.0);
            setBaseInfluence(typeof(Wall), -4.0);
        }
    }
}
