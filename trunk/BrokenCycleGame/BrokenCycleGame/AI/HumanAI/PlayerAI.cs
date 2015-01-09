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

namespace BrokenCycleGame.AI.HumanAI
{
    class PlayerAI : Controller
    {
        private static readonly Random rand = new Random();

        private int decision_time = 0;

        public override void Update(WasteGame game, Player player)
        {
            // Perform AI Calculations
            if (decision_time > 0) decision_time--;
            else
            {
                decision_time = rand.Next(30) + 30;   
                left = rand.NextDouble() < 0.5;
                right = rand.NextDouble() < 0.5;
                up = rand.NextDouble() < 0.5;
                down = rand.NextDouble() < 0.5;
                removed = rand.NextDouble() > 1.0; 
            }
        }
    }
}
