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

using BrokenCycleGame;

namespace BrokenCycleGame.AI
{
    class EuclidHeuristic : Heuristic
    {
        Tile goalTile;

        public EuclidHeuristic(Tile goal) : base(goal)
        {
            this.goalTile = goal;
        }
        public override float Estimate(Tile current)
        {
            /*
            float CurX = current.Position.X;
            float CurY = current.Position.Y;
            float GoalX = goal.Position.X;
            float GoalY = goal.Position.Y;
             * */
            return (goalTile.Position - current.Position).Length();

            //return (float)Math.Sqrt(Math.Pow(CurX - GoalX, 2) + Math.Pow(CurY - GoalY, 2));
            
        }
    }
}
