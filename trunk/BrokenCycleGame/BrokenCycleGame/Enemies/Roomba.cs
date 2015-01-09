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

using BrokenCycleGame.AI;

namespace BrokenCycleGame
{
    public class Roomba : Enemy
    {

        public ScheduledAStar astar = new ScheduledAStar();
        //Ensuring the enemy is in the same gameWorld.
        List<Body> bodyList;
        WasteGame game;
        //Enemy texture
        Texture2D roombaTex;

        //Every roomba has 100 health
        public int health;
        public Texture2D healthbar;
        //Fixture for Farseer purposes
        public Fixture roombaFixture;
        //Enemy positioning variables
        public Boolean dead;
        public int Speed = 10;
        int countingAStar = 0;
        Tile[,] tiles;
        Random random;
        Heuristic h;
        bool atLoc = true;
        List<Tile> pathway;

        public Roomba(World gameWorld, WasteGame game, Tile[,] tiles, GameWindow gameWindow, Texture2D texture, Vector2 location)
            : base(gameWorld, game, gameWindow, texture)
        {
            random = new Random();
            roombaTex = texture;
            healthbar = game.Content.Load<Texture2D>("Images/HealthBars/Red");
            //new Vector2(random.Next(0, game.Window.ClientBounds.Width - roombaTex.Width), random.Next(0, game.Window.ClientBounds.Height - roombaTex.Height));
            bodyList = new List<Body>();
            this.game = game;
            health = 100;
            this.tiles = tiles;
            location.X *= texture.Width;
            location.Y *= texture.Height;
            Position = location;
            roombaFixture = FixtureFactory.AttachCircle(texture.Width / 2, 1, this);
            roombaFixture.CollisionCategories = Category.Cat3;
            roombaFixture.CollidesWith = Category.Cat1 | Category.Cat2 | Category.Cat3 | Category.Cat4 | Category.Cat5 | Category.Cat6;
            roombaFixture.Body.FixedRotation = true;
            roombaFixture.Body.BodyType = BodyType.Dynamic;
            roombaFixture.OnCollision += _OnCollision;
        }


        
        public override bool _OnCollision(Fixture fix1, Fixture fix2, Contact con)
        {
            if (fix2.CollisionCategories == Category.Cat1)
            {
                return true;
            }

            if (fix2.CollisionCategories == Category.Cat2)
            {
                this.Speed = 10;
                return false;
            }

            if (fix2.CollisionCategories == Category.Cat3)
            { 
                return true;
            }

            if (fix2.CollisionCategories == Category.Cat4)
            {
                health -= 10;
                return true;
            
            }

            if (fix2.CollisionCategories == Category.Cat5)
            {
                return true;
            }

            if (fix2.CollisionCategories == Category.Cat6)
            {
                this.Speed = 1;
                return false;
            }
            return false;
        }
        public override void Update(GameTime gameTime)
        {
            updatePosition();
            dead =checkisDead();
            if (dead)
            {
                CollisionCategories = Category.None;
            }
        }

        Boolean checkisDead()
        {
            return this.health <= 0;
        }

        public override void Draw(SpriteBatch spriteBatch, camera cam)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null,
                cam.get_transformation(game.GraphicsDevice));

            if (!dead)
            {
                spriteBatch.Draw(roombaTex, Position - new Vector2(roombaTex.Width / 2, roombaTex.Height / 2), Color.White);
                spriteBatch.Draw(healthbar, new Rectangle((int)(Position.X - roombaTex.Width/2), (int)(Position.Y - roombaTex.Height/2), health/2, 10), Color.White);
            }
            spriteBatch.End();
        }

        public override void updatePosition()
        {
            Vector2 tempLinearVelocity = new Vector2(0, 0);
            //if atLoc is false, steer along the A* path
            if (!atLoc)
            {
                if (pathway != null)
                {
                    if (astar.hasTask) astar.schedule(1);
                    tempLinearVelocity =
                       CommonAI.getSteeringVectorFromCollection(pathway.Cast<Body>().ToList<Body>(), this, 400.0f, 5.0f);
                }
                atLoc = true;
            }
            else
            {
                if (!dead)
                {
                    tempLinearVelocity = CommonAI.getSteeringVectorFromCollection(game.players.Cast<Body>().ToList<Body>(), this, 400.0f, 5.0f);
                    pathway = getPath(CurrentTile(), getTile());
                    countingAStar++;
                }
                else tempLinearVelocity = new Vector2(0, 0);
            }

                if (Position.X > (game.currLevel.Width * 32) - roombaTex.Width) Position = new Vector2((game.currLevel.Width * 32) - roombaTex.Width , Position.Y);
                if (Position.X < 0) Position = new Vector2(0, Position.Y);
                if (Position.Y > (game.currLevel.Height * 32) - roombaTex.Height) Position = new Vector2(Position.X, (game.currLevel.Height * 32) - roombaTex.Height);
                if (Position.Y < 0) Position = new Vector2(Position.X, 0);

                
                LinearVelocity = tempLinearVelocity;
        }

        public List<Tile> getPath(Tile current, Tile goal) {
            List<Tile> pathway = astar.findPath(tiles, current, goal, new RoombaCostFunction(), new roombaHeuristic(current));
            atLoc = false;
            return pathway;
        }

        public void FollowPath(List<Tile> pathway, int index) {
            if (index < pathway.Count())
            {
                Tile next = pathway.ElementAt(index);
                Position += next.Position;
                index++;
            }
            else
            {
                atLoc = true;
            }
        }

        public Tile CurrentTile()
        {
            Tile thisTile = tiles[((int)this.Position.X/32), (((int)this.Position.Y/32))];
            return thisTile;
        }

        public int findNearestPlayer()
        {
            double distance = 10000;
            int PlayerToFace = 0;
            for (int i = 0; i < 3; i++)
            {
                if (!game.players[i].isDead() && !game.players[i].isRemoved())
                {
                    double newDistance = (Position - game.players[i].Position).Length();
                    if (newDistance < distance)
                    {
                        distance = newDistance;
                        PlayerToFace = i;
                    }
                }
                else PlayerToFace = 0;
            }
            return PlayerToFace;
        }

        public Tile getTile()
        {
            Tile newTile;
            
            int playerToFace = findNearestPlayer();
            newTile = tiles[(int)MathHelper.Clamp(((int)game.players[playerToFace].Position.X / 32), 0, (int)game.players[playerToFace].Position.X / 32), (int)MathHelper.Clamp(((int)game.players[playerToFace].Position.Y / 32), 0, ((int)game.players[playerToFace].Position.Y / 32))];

            if (newTile != null)
                return newTile;
            else return tiles[0,0];
        }
    }
}
