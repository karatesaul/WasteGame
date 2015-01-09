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
    public abstract class Enemy : Body
    {
        //Ensuring the enemy is in the same gameWorld.
        World gameWorld;
        WasteGame game;
        GameWindow gameWindow;
        //Enemy texture
        Texture2D enemyTex;

        //Every enemy has 50 health
        public int health;
        //Fixture for Farseer purposes
        public Fixture enemyFixture;
        //Enemy positioning variables
        Vector2 Position;
        Vector2 Velocity;
        int speed = 3;
        Random random;
        public Enemy(World gameWorld, WasteGame game, GameWindow gameWindow, Texture2D texture) : base(gameWorld)
        {
            random = new Random();
            this.gameWorld = gameWorld;
            this.gameWindow = gameWindow;
            this.game = game;
            enemyTex = texture;
            setEnemyValues();
            
        }
         
        //Overridable function to set enemy specific values (Position, velocity health etc.)
        protected virtual void setEnemyValues() {
            Position = new Vector2(random.Next(), random.Next());
            Velocity = new Vector2(speed, 0);
            health = 50;

            enemyFixture = FixtureFactory.AttachRectangle(enemyTex.Width, enemyTex.Height, 1, new Vector2(), this);
            enemyFixture.CollisionCategories = Category.Cat3;
            enemyFixture.CollidesWith = Category.Cat1 | Category.Cat3; //Collides with only player at the moment
            enemyFixture.Body.BodyType = BodyType.Dynamic;
            enemyFixture.OnCollision += _OnCollision;
        }

        public virtual bool _OnCollision(Fixture fix1, Fixture fix2, Contact con)
        {
            if (fix2.CollisionCategories == Category.Cat1)
            {
                return true;
            }
            return false;
        }


        public virtual void Update(GameTime gameTime)
        {
            updatePosition();
        }

        //uncommented so program wouldn't complain
        public virtual void Draw(SpriteBatch spriteBatch, camera cam)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null,
                cam.get_transformation(game.GraphicsDevice));
            spriteBatch.Draw(enemyTex, Position, Color.White);
            spriteBatch.End();
        }

        //Uncommented this so that the program wouldn't complain
        public virtual void updatePosition()
        {
            Position += Velocity;
            if (( (Position.Y) > gameWindow.ClientBounds.Height - enemyTex.Height) || ((Position.Y) < 0))
            {
                Velocity.Y *= -1;
            }

            enemyFixture.Body.Position = Position;
        }
    }

}
