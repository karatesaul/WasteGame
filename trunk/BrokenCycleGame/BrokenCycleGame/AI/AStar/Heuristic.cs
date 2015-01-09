using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision;
using FarseerPhysics.Common;


namespace BrokenCycleGame.AI
{
    public abstract class Heuristic
    {
        Tile goalTile;
        public Heuristic(Tile goal)
        {
            this.goalTile = goal;
        }

        public abstract float Estimate(Tile current);
    }
}
