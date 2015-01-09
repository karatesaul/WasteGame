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
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;

namespace BrokenCycleGame
{

    /*
     For the purposes of this game, the collision categories will be as follows:
     * 
     * Category 1: 
     */
    public class Tile : Body
    {
        #region Fields
        public float health;
        protected float prevhealth;
        protected float maxhealth;
        public int scorevalue = 100;
        public Fixture tileFixture;
        protected Texture2D tileTex;

        /*
         * A* considerations:
         * Array of Neighbors and Level tile was created in 
         * (so Tile can access the full tile array)
         */
        protected Tile[] Neighbors;
        protected Level level;

        protected float breakstages = 7;

        protected bool dead = false;

        //SoundEffect breakSound;

        protected WasteGame game;

        protected String contentName;

        #endregion

        #region Constructor

        public Tile(World gameWorld, Vector2 location, WasteGame game, Vector2 offset, Level curlevel)
            : base(gameWorld)
        {
        }

        #endregion

        #region Collision Method
        public virtual bool _OnCollision(Fixture fix1, Fixture fix2, Contact con)
        {
            return false;
        }

        #endregion

        #region Update
        public virtual void Update()
        {
        }
        #endregion

        #region Checking Neighbors
        /// <summary>
        /// Adds all neighboring tiles in as neighbors.  Uses Level's tile array
        /// to check indices
        /// 
        /// </summary>

        public virtual void checkNeighbor()
        {
            int i = 0;
            int outx = level.tiles.GetLength(0);
            int outy = level.tiles.GetLength(1);
            int x, y;
            int posX = (int)Position.X;
            int posY = (int)Position.Y;

            //Use x and y as loop control, going from -1 to 1 (to be added to position)
            for (x = -1; x <= 1; x++)
            {
                for (y = -1; y <= 1; y++)
                {
                    //if neither posX+x or posY+y are out of bounds, continue assignment
                    if (((posX + x >= 0) && (posX + x < outx))
                         && ((posY + y >= 0) && (posY + y < outy)))
                    {
                        //Make sure x and y are not the current tile, then set
                        if (x != 0 && y != 0)
                        {
                            Neighbors[i] = level.tiles[posX + x, posY + y];
                        }
                        else
                        {
                            i--;
                        }
                    }
                    i++;
                }
            }
        }

        public virtual Tile[] getConnections()
        {
            return Neighbors;
        }

        /*
        /// <summary>
        /// When Tile dies, update its status in all neighboring tiles
        /// using checkNeighbor()
        /// Ma be unnecessay if we're not actually deleting the nodes, but usefull
        /// if walls included and destroyable.
        /// </summary>
        public void UpdateNeighbors()
        { 
           foreach (Tile T in Neighbors){
               if (T != null)
               {
                   T.checkNeighbor();
               }
           }
        }*/

        public virtual bool isDead()
        {
            if (dead) return true;
            return false;
        }

        #endregion

        #region Draw
        public virtual void draw(SpriteBatch spriteBatch)
        {
        }
        #endregion
    }
}
