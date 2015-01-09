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

namespace BrokenCycleGame.AI
{
    /// <summary>
    /// InfluenceMap is a record of influence over the current map.
    /// It keeps track of influences in each area -- players exert
    /// positive influence, while enemies exert negative influence.
    /// This can be used by AI to find safe and unsafe areas.
    /// </summary>
    public class InfluenceMap
    {
        /// <summary>
        /// A TileRecord is used to store information in the influence map.
        /// Most of the time it only consists of a pair of a Tile reference
        /// and an influence value.  When spreading influence it consists of
        /// those plus a parent and an intermediate influence value.
        /// </summary>
        private class TileRecord
        {
            public Tile reference;
            public double influence;

            public int x;
            public int y;

            public double spread_influence;
            public Tile parent;
        }
        
        private TileRecord[,] influences;
        // base_influence stores base influence values for each class.
        private Dictionary<Type, double> base_influence;

        /// <summary>
        /// Creates a new InfluenceMap based on the indicated level.
        /// </summary>
        public InfluenceMap(Level level)
        {
            // Create the TileRecord array
            influences = new TileRecord[level.Width, level.Height];
            for (int j = 0; j < level.Height; ++j)
            {
                for (int i = 0; i < level.Width; ++i)
                {
                    // Create TileRecords and link them to the level's tiles
                    influences[i, j] = new TileRecord();
                    influences[i, j].x = i;
                    influences[i, j].y = j;
                    influences[i, j].reference = level.tiles[i, j];
                }
            }

            base_influence = new Dictionary<Type, double>();
        }

        #region Setters / Getters
        /// <summary>
        /// Set the base influence for the given object type.  To, for example,
        /// set the base influence of the Player objects to 16.0:
        /// setBaseInfluence(typeof(Player),16.0);
        /// </summary>
        public void setBaseInfluence(Type objectType, double influence)
        {
            base_influence.Add(objectType, influence);
        }

        /// <summary>
        /// Gets the base influence of the given object type.  If the base influence
        /// for that object type has not been set, returns 0.0.
        /// </summary>
        public double getBaseInfluence(Type objectType)
        {
            double influence;
            if (base_influence.TryGetValue(objectType, out influence))
            {
                return influence;
            }
            return 0.0;
        }

        /// <summary>
        /// Get the influence at the given point on the map.
        /// </summary>
        public double getInfluenceAt(int x, int y)
        {
            return influences[x, y].influence;
        }

        /// <summary>
        /// Get the influence at the given Tile.
        /// </summary>
        public double getInfluenceAt(Tile T)
        {
            return getInfluenceAt((int)T.Position.X / 32, (int)T.Position.Y / 32);
        }

        /// <summary>
        /// Get the entire influence map as an array of doubles.
        /// </summary>
        public double[,] getInfluenceMap()
        {
            double[,] influence_map = new double[influences.GetLength(0), influences.GetLength(1)];

            // Convert the Tile Record array to an array of doubles.
            for (int j = 0; j < influences.GetLength(1); ++j)
            {
                for (int i = 0; i < influences.GetLength(0); ++i)
                {
                    influence_map[i, j] = influences[i, j].influence;
                }
            }

            return influence_map;
        }
        #endregion

        #region Influence Calculation
        /// <summary>
        /// Recalculates the influence map for the current state of the level.
        /// </summary>
        public void calculateInfluence(Level level)
        {
            // Create a list of bodies that cast influence through the level.
            List<Body> influencers = new List<Body>();

            // Add the level's players, roombas, and towers to the influencers list
            influencers.AddRange(level.playerList.Cast<Body>());
            influencers.AddRange(level.roombaList.Cast<Body>());
            influencers.AddRange(level.towerList.Cast<Body>());

            // Reset the influence map
            foreach (TileRecord i in influences)
            {
                i.influence = 0;
            }

            // Iterate over every influencer
            foreach (Body I in influencers)
            {
                // Get the influence that I casts
                double influence = getBaseInfluence(I.GetType());

                // Make sure it casts influence!
                if (influence != 0.0)
                {
                    spreadInfluence((int)I.Position.X / 32, (int)I.Position.Y / 32, influence);
                }
            }
        }

        /// <summary>
        /// Spread influence starting at a given tile.  Influence spreads
        /// radially out from the starting tile and decreases with the
        /// manhattan distance from the original tile.  Influence stops
        /// spreading when the distance is equal to the original influence
        /// value (the spread influence is equal to zero
        /// </summary>
        private void spreadInfluence(int centerx, int centery, double influence)
        {
            // Make sure the position requested is on the map!
            if (!pointOnMap(centerx, centery)) return;
            
            // Should use a similar method to A* to spread influence
            // simply spreads radially decrementing influence until it reaches zero

            List<TileRecord> open = new List<TileRecord>();
            List<TileRecord> closed = new List<TileRecord>();

            influences[centerx, centery].spread_influence = influence;
            open.Add(influences[centerx, centery]);

            while (open.Count > 0)
            {
                // Get the next tile from the open list
                TileRecord current_tile = open[0];
                open.RemoveAt(0);
                closed.Add(current_tile);

                if (Math.Abs(current_tile.spread_influence) > 0)
                {
                    // Add the 4 neighbors to the open list if they are not already in the closed list
                    spreadInfluenceToNeighbor(current_tile, -1, 0, open, closed);
                    spreadInfluenceToNeighbor(current_tile, 1, 0, open, closed);
                    spreadInfluenceToNeighbor(current_tile, 0, -1, open, closed);
                    spreadInfluenceToNeighbor(current_tile, 0, 1, open, closed);
                }

                // cleanup -- add influence
                current_tile.influence += current_tile.spread_influence;
            }
        }

        /// <summary>
        /// Spreads influence to a neighboring tile.  Automatically checks
        /// whether that tile is within bounds and not on either the open
        /// or closed list already, and automatically decrements the 
        /// influence value whether it is positive or negative.
        /// </summary>
        /// <param name="current_tile">Tile to spread from</param>
        /// <param name="addx">x of neighbor relative to current tile</param>
        /// <param name="addy">y of neighbor relative to current tile</param>
        /// <param name="open">open list for spreading influence</param>
        /// <param name="closed">closed list for spreading influence</param>
        private void spreadInfluenceToNeighbor(TileRecord current_tile, int addx, int addy, List<TileRecord> open, List<TileRecord> closed)
        {
            // Make sure the requested neighbor is on the map!
            if (!pointOnMap(current_tile.x + addx, current_tile.y + addy)) return;

            // As long as the requested neighbor is valid, find it from the Tile Record array.
            TileRecord next_tile = influences[current_tile.x + addx, current_tile.y + addy];

            // Make sure the neighbor is not in either the closed list or the open list.
            if ((!closed.Contains(next_tile)) && (!open.Contains(next_tile)))
            {
                // If the influence is positive, increment it and if
                // the influence is negative, decrement it.
                if (current_tile.spread_influence > 0)
                {
                    next_tile.spread_influence = current_tile.spread_influence - 1;
                }
                else
                {
                    next_tile.spread_influence = current_tile.spread_influence + 1;
                }
                // Add the neighbor to the open list.
                open.Add(next_tile);
            }
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Returns true if the indicated point is on the map and false otherwise.
        /// </summary>
        public bool pointOnMap(int x, int y)
        {
            if ((x < 0) ||
                (x >= influences.GetLength(0)) ||
                (y < 0) ||
                (y >= influences.GetLength(1)))
            {
                return false;
            }

            return true;
        }
        #endregion

        #region Drawing
        /// <summary>
        /// Draw a debug overlay of the influence map
        /// </summary>
        public void Draw(SpriteBatch sb)
        {
            Texture2D pixel = new Texture2D(sb.GraphicsDevice, 1, 1);
            pixel.SetData<Color>(new Color[1] { Color.White });

            foreach (TileRecord T in influences)
            {
                float tx = T.reference.Position.X;
                float ty = T.reference.Position.Y;

                float ivalue = (float)Math.Min(Math.Max(T.influence,-8.0f),8.0f)/8.0f;
                Color icolor = new Color (ivalue < 0 ? -ivalue : 0,
                                          ivalue > 0 ? ivalue : 0,
                                          0,
                                          0.5f);

                sb.Draw(pixel, new Rectangle((int)tx, (int)ty, 32, 32), icolor);
            }

        }
        #endregion
    }
}
