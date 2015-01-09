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
    class NormalTile : Tile
    {
        public NormalTile(World gameWorld, Vector2 location, WasteGame game, Vector2 offset, Level curlevel)
            : base(gameWorld, location, game, offset, curlevel)
        {
            //Loading content in the constructor for simplicity's sake because the content manager is initialized by the time the stage is created
            health = 100;
            prevhealth = health;
            maxhealth = health;

            //A* set
            level = curlevel;
            Neighbors = new Tile[8];

            this.game = game;
            contentName = game.random.NextDouble() > 0.5 ? "Images/Tiles/MarbleTilesBreak" : "Images/Tiles/MarbleTiles1Break";
            tileTex = game.Content.Load<Texture2D>(contentName + "0");
            //breakSound = game.Content.Load<SoundEffect>("Tiles/FloorBreaking");
            location.X *= tileTex.Width;
            location.Y *= tileTex.Height;
            location += offset;
            Position = location;
            tileFixture = FixtureFactory.AttachRectangle(tileTex.Width, tileTex.Height, 1, new Vector2(), this);
            tileFixture.Body.BodyType = BodyType.Dynamic;
            tileFixture.CollisionCategories = Category.Cat2;
            tileFixture.CollidesWith = Category.Cat1 | Category.Cat3;
            tileFixture.OnCollision += _OnCollision;
        }

        public override bool _OnCollision(Fixture fix1, Fixture fix2, Contact con)
        {
            if (fix2.CollisionCategories == Category.Cat3) //If a tile is colliding with a Roomba
            {

            }
            if (fix2.CollisionCategories == Category.Cat1) //Player colliding with tile?
            {
                health -= 3.0f; //Reduce tile health by 1

                //Checks which player is on a certain tile, scoring players appropriately.
                for (int i = 0; i < 4; i++)
                {
                    if (fix2 == game.players[i].playerFixture)
                    {
                        if (this.health <= 0 && !this.dead)
                        {
                            this.dead = true;
                            scorevalue = 100;
                            game.players[i].collectedScore += game.players[i].collectTile(this);
                            tileFixture.CollisionCategories = Category.None;
                        }
                    }

                }
            }

            return false;
        }

        public override void Update()
        {
            int currstage = (int)((maxhealth - health) / (maxhealth / breakstages));
            if (currstage < 0)
            {
                currstage = 0;
            }
            if (currstage > breakstages - 1)
            {
                currstage = (int)breakstages - 1;
            }
            if (currstage > prevhealth)
            {
                tileTex = game.Content.Load<Texture2D>(contentName + currstage.ToString());
            }

            prevhealth = currstage;
        }

        public override void checkNeighbor()
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

        public override Tile[] getConnections()
        {
            return Neighbors;
        }

        public override bool isDead()
        {
            if (dead) return true;
            return false;
        }

        public override void draw(SpriteBatch spriteBatch)
        {
            if (health > 0)
                spriteBatch.Draw(tileTex, new Rectangle((int)Position.X, (int)Position.Y, tileTex.Width, tileTex.Height), Color.White);
            base.draw(spriteBatch);
        }
    }
}
