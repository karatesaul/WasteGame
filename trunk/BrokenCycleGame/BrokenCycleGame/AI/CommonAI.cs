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
    /// <summary>
    /// A TileHeuristic is a heuristic function that is used to rank tiles.
    /// It is used by the getGreatestTile and getLeastTile functions in CommonAI
    /// currently.
    /// </summary>
    /// <param name="T">T is the tile that is currently being scored.</param>
    /// <returns>This should return a double value that represents the score of the given tile.</returns>
    public delegate double TileHeuristic(Tile T);

    /* TO USE TileHeuristic:
     * (1) create a function in whatever class uses getGreatest/LeastTile, like so:
     *      public double H (Tile T) { return 0.0; }
     * (2) Create a tileHeuristic variable in that class:
     *      private TileHeuristic TH;
     * (3) In the constructor of that class, assign the function to the TileHeuristic:
     *      public Class() { TH = H; }
     * (4) Call getGreatest/LeastTile.
     *      CommonAI.getGreatestTile(game.currLevel.tils, TH);
     */

    /// <summary>
    /// CommonAI is a class that contains several static functions which perform
    /// generic AI functionality.  These are generally useful functions that can
    /// be used for any kind of AI.
    /// </summary>
    static class CommonAI
    {


        /// <summary>
        /// Find the nearest tile to the indicated location
        /// </summary>
        public static Tile getNearestTile(Tile[,] tiles, Vector2 location)
        {
            Tile nearest = null;
            double distance = Double.PositiveInfinity;

            // Iterate over every tile in the array.  If it is not dead,
            // check whether it is closer than the nearest currently
            // found tile.  If so, update the values of the nearest tile
            // and the nearest distance.
            if (tiles != null)
            {
                foreach (Tile T in tiles)
                {
                    if (!T.isDead())
                    {
                        double next_distance = (T.Position - location).Length();
                        if (next_distance < distance)
                        {
                            nearest = T;
                            distance = next_distance;
                        }
                    }
                }
            }

            return nearest;
        }

        /// <summary>
        /// Find the tile in the grid with the greatest value according to
        /// the given tile heuristic.
        /// </summary>
        public static Tile getGreatestTile(Tile[,] tiles, TileHeuristic H)
        {
            Tile greatest = null;
            double heuristic = Double.NegativeInfinity;

            // Iterate over every tile in the array.  If it is not dead,
            // check whether it has a higher heuristic value than the
            // currently greatest found tile.  If so, update the values 
            // of the greatest tile and the greatest heuristic.
            if (tiles != null)
            {
                foreach (Tile T in tiles)
                {
                    if (!T.isDead())
                    {
                        double next_heuristic = H(T);
                        if (next_heuristic > heuristic)
                        {
                            greatest = T;
                            heuristic = next_heuristic;
                        }
                    }
                }
            }

            return greatest;
        }

        /// <summary>
        /// Find the tile in the grid with the least value according to
        /// the given tile heuristic.
        /// </summary>
        public static Tile getLeastTile(Tile[,] tiles, TileHeuristic H)
        {
            Tile least = null;
            double heuristic = Double.PositiveInfinity;

            // Iterate over every tile in the array.  If it is not dead,
            // check whether it has a lower heuristic value than the
            // currently least found tile.  If so, update the values 
            // of the least tile and the least heuristic.
            if (tiles != null)
            {
                foreach (Tile T in tiles)
                {
                    if (!T.isDead())
                    {
                        double next_heuristic = H(T);
                        if (next_heuristic < heuristic)
                        {
                            least = T;
                            heuristic = next_heuristic;
                        }
                    }
                }
            }

            return least;
        }

        /// <summary>
        /// Find the number of tiles around the indicated location within 
        /// the given radius that are still living.
        /// </summary>
        public static int getNumLivingTilesAround(Tile[,] tiles, Vector2 location, float radius)
        {
            int number = 0;

            // Iterate over every tile in the array.  If it is not dead,
            // check whether it is closer than the nearest currently
            // found tile.  If so, update the values of the nearest tile
            // and the nearest distance.
            if (tiles != null)
            {
                foreach (Tile T in tiles)
                {
                    if (!T.isDead())
                    {
                        double distance = (T.Position - location).Length();
                        if (distance < radius)
                        {
                            ++number;
                        }
                    }
                }
            }

            return number;
        }

        /// <summary>
        /// Function for summing up force vectors from a collection of bodies.  Assumes
        /// an attractive force.  For repulsive forces multiply the result by -1.
        /// </summary>
        /// <param name="targets">List of bodies that generate forces</param>
        /// <param name="center">The body that is accumulating forces</param>
        /// <param name="ATTRACTION_RADIUS">Maximum distance at which forces take effect</param>
        /// <param name="ATTRACTION_DECAY">Force decay factor (1.0 is linear, >1 is slower, >1 is faster)</param>
        public static Vector2 getSteeringVectorFromCollection(List<Body> targets, Body center, float ATTRACTION_RADIUS, float ATTRACTION_DECAY)
        {
            Vector2 total_forces = Vector2.Zero;

            // Calculate the modified maximum radius once (for calculating
            // force decay factor)
            double mod_max = Math.Pow(ATTRACTION_RADIUS, ATTRACTION_DECAY);

            for(int i = 0; i < targets.Count - 2; i++)
            {
                // Get the vector from the player to the current Body
                Vector2 steer = targets[i].Position - center.Position;
                // If the Body is outside the maximum radius ignore it
                if (steer.Length() > ATTRACTION_RADIUS) { continue; }

                // modify the distance by raising it to the power of the decay
                double mod_distance = Math.Pow(steer.Length(), ATTRACTION_DECAY);

                // The decay factor is as follows: F = (R^N - D^N)/R^N
                //   R = maximum radius
                //   D = distance
                //   N = ATTRACTION_DECAY
                float distance_factor;
                if (mod_distance == 0) distance_factor = 1;
                else distance_factor = (float)((mod_max - mod_distance) / mod_max);

                // Normalize the vector and then decay it
                steer.Normalize();
                steer *= distance_factor;

                // Add the current force to the sum
                total_forces += steer;
                
            }

            return total_forces;
        }
    }
}
