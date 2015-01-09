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
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;

using BrokenCycleGame.Particlesystem;
using BrokenCycleGame.AI;

namespace BrokenCycleGame
{
    public class Tower : Enemy
    {
        World gameWorld;
        Joint joint;
        WasteGame game;
        GameWindow gameWindow;
        Texture2D towerTex;

        public int health;
        public Fixture towerFixture;
        //the direction the tower is facing
        public Vector2 Direction;
        Vector2 Origin;
        public bool dead;
        Vector2 location;
        TowerWeapon towerWeapon;
        UtilityMethod utilMeth;
        Random rand = new Random();

        public Tower(World world, WasteGame game, GameWindow window, Texture2D texture, Vector2 offset, Vector2 location)
            : base(world, game, window, texture)
        {
            utilMeth = new UtilityMethod();
            towerWeapon = new TowerWeapon(game, game.Content, game.spriteBatch, this);
            gameWorld = world;
            this.game = game;
            gameWindow = window;
            towerTex = texture;
            health = 100;
            this.location = location;
            location.X *= 32;
            location.Y *= 32;
            Position = location;
            towerFixture = FixtureFactory.AttachCircle((texture.Width / 2), 1, this, Vector2.Zero);
            towerFixture.CollisionCategories = Category.Cat3;
            towerFixture.Body.BodyType = BodyType.Dynamic;
            towerFixture.CollidesWith = Category.Cat3 | Category.Cat1;
            towerFixture.OnCollision += _OnCollision;
            towerFixture.Body.AngularDamping = 1f;
            towerFixture.Body.LinearDamping = 1f;
            towerFixture.Body.FixedRotation = true;

            Direction = Vector2.Zero;
            dead = false;
        }

        public virtual bool _OnCollision(Fixture fix1, Fixture fix2, Contact con)
        {
            if (fix2.CollisionCategories == Category.Cat1)
            {
                return true;
            }

            if (fix2.CollisionCategories == Category.Cat3)
            {
                return true;
            }
            return false;
        }

        public override void Update(GameTime time)
        {
            //update the direction
            updateDirection();
            //update the spray
            Vector2 weaponRange = new Vector2(location.X + Direction.X * 48 / 5, location.Y + Direction.Y * 48 / 5);
            float distance = weaponRange.Length();
            //int fireAction = utilMeth.chooseAction(~~Only closest/weakest player~~, this.towerFixture.Body, weaponRange.Length(), -1, 100);
            int fireAction = utilMeth.chooseAction(game.players.Cast<Body>().ToList<Body>(), this.towerFixture.Body, distance, -1, 100);
            if (fireAction == 1)
                towerWeapon.Update(Position, Direction);
            if (health <= 0)
            {
                dead = true;
                CollisionCategories = Category.None;
            }

        }

        public override void Draw(SpriteBatch batch, camera cam)
        {
            batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null,
                cam.get_transformation(game.GraphicsDevice));
            Vector2 tempPos = Position;
            float _towerRotation = Rotation;
            towerWeapon.Draw(batch, cam);
            batch.Draw(towerTex, Position, null, Color.White, (Rotation), new Vector2(towerTex.Width / 2, towerTex.Height / 2), 1f, SpriteEffects.None, 0f);
            batch.End();
        }

        private void updateDirection()
        {
            //find the nearest player and face towards them
            Vector2 weaponRange = new Vector2(location.X + Direction.X * 48 / 5, location.Y + Direction.Y * 48 / 5);
            float distance = weaponRange.Length();
            int playerToFace = 0;
            int playerHealth = 0;
            for (int i = 0; i < 4; i++)
            {
                if (!game.players[i].isDead() || !game.players[i].isRemoved())
                {
                    double newDistance = (Position - game.players[i].Position).Length();
                    if (newDistance <= distance)
                    {
                        //if (playerHealth > game.players[i].health)
                        //{
                        playerHealth = game.players[i].health;
                        playerToFace = i;
                        //}
                    }
                }
            }

            //reduce the direction to a unit vector
            Vector2 delta = Position - game.players[playerToFace].Position;
            Direction = delta / delta.Length();

            //towerFixture.Body.ApplyTorque(delta.LengthSquared()/64f);
            Rotation = (MathHelper.ToRadians(((float)Math.Atan2((double)delta.Y, (double)delta.X))) * 64f);
            ApplyForce(delta);
        }
    }
}
