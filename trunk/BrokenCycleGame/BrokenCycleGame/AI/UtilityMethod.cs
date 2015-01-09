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
using BrokenCycleGame.AI.HumanAI;

namespace BrokenCycleGame.AI
{
    class UtilityMethod
    {
        const int MIN_HEALTH = 20;              // Minimum health before player flees

        // Checks if the enemy is within shot range.
        private int checkShoot(float distance, float MIN_TARGET_DIST)
        {
            if (distance > MIN_TARGET_DIST)
                return 1;
            else
                return 0;
        }

        // Checks if the enemy is within melee range.
        private int checkMelee(float distance, float MIN_TARGET_DIST)
        {
            if (distance <= MIN_TARGET_DIST)
                return 3;    // Higher than shoot goal so that players will melee when enemies are close.
            else
                return 0;
        }

        // Checks to see if the player's health is low.
        private int checkHealth(float health)
        {
            if (health <= MIN_HEALTH)
                return 5;    // Higher than shoot and melee to ensure players run.
            else
                return 0;
        }

        /// <param name="targets">List of bodies to attack</param>
        /// <param name="center">The body that performing the attack</param>
        /// <param name="MAX_TARGET_DIST">Maximum distance that attacks can reach (meant for ranged attacks)</param>
        /// <param name="MIN_TARGET_DIST">Minimun distance attacks can reach (meant for melee attacks)</param>
        public int chooseAction(List<Body> targets, Body center, float MAX_TARGET_DIST, float MIN_TARGET_DIST, int health)
        {
            int[] goals = { 0, 0, 0 };   // 0 = melee, 1 = range, 2 = run.
            int topGoal = 2;            // defaults to run.

            // Checks all nearby bodies and determines course of attack.
            for (int i = 0; i < targets.Count; i++)
            {
                // Get the vector from the player to the current Body
                float range = (targets[i].Position - center.Position).Length();
                // If the Body is outside the maximum radius ignore it
                if (range > MAX_TARGET_DIST) { continue; }

                // Determines best goal choice based on range and player health.
                goals[0] += checkMelee(range, MIN_TARGET_DIST);
                goals[1] += checkShoot(range, MIN_TARGET_DIST);
                goals[2] += checkHealth(health);

                for (int j = 0; j < goals.Length; j++)
                {
                    if (goals[topGoal] < goals[j])
                        topGoal = j;
                }
            }

            return topGoal;
        }
    }
}
