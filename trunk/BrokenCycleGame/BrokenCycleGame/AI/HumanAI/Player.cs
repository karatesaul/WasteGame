using System;
using System.Collections.Generic;
using System.Linq;
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


using BrokenCycleGame.Particlesystem;

namespace BrokenCycleGame
{
    public class Player : Body
    {
        WasteGame game;
        //Adhering to Farseer's engine, we make a fixture for the player sprite.
        public Fixture playerFixture;
        
        //keeps track of the direction the player is facing
        public Vector2 Direction;
        public Vector2 location;
        //the sprite
        public Texture2D playerSprite;
        public PlayerIndex playerNum;
        public Tile[,] tilesInRadius;

        public int health = 100;
        public int maxhealth = 100;
        public int collectedScore = 0;
        public int levelScore = 0;
        //the player's movement speed
        public int Speed = 5;
        //leavingcount keeps track of how long the player has been holding the buttons
        //to leave the level
        int leavingCount = 0;
        //this will be used for events called by the player, such as leaving the screen
        public int eventnumber = 0;
        //keeps track of the time spent doing a melee action
        int meleeTime = 0;
        //determines whether in cooldown or not
        bool readyToAct = true;
        //says whether player is using the hose
        bool firingHose = false;
        bool usingMelee = false;
        //these bools tell the status of a dead or self-removed player
        public bool dead = false;
        public bool removed = false;
        public int radius = 1;
        MeleeWeapon meleeWeapon;
        RangedWeapon rangedPlayerWeapon;
        String directory = "Images/Sweeper/";
        Controller playerController;

        public Player(World gameWorld, WasteGame game, GameWindow Window, PlayerIndex playernum, Texture2D sprite, Controller controller, Vector2 offset, Vector2 location)
            : base(gameWorld)
        {
            this.game = game;
            this.playerController = controller;
            Direction = new Vector2(1, 0);
            rangedPlayerWeapon = new RangedWeapon(game, game.Content, playernum, game.spriteBatch, this);
            meleeWeapon = new MeleeWeapon(gameWorld, game, game.Content.Load<Texture2D>(directory + "BroomRight"), game.Content, playernum, game.spriteBatch, this);
            playerSprite = sprite;
            this.location = location;
            location.X *= playerSprite.Width;
            location.Y *= playerSprite.Height;
            Position = location;
            playerNum = playernum;
            playerFixture = FixtureFactory.AttachRectangle(playerSprite.Width, playerSprite.Height, 1, new Vector2(), this);
            playerFixture.CollisionCategories = Category.Cat1;
            playerFixture.Body.BodyType = BodyType.Dynamic;
            //playerFixture.Body.FixedRotation = true;
            playerFixture.CollidesWith = Category.Cat1 | Category.Cat2 | Category.Cat3 | Category.Cat5 | Category.Cat6 | Category.Cat9;
            playerFixture.OnCollision += _OnCollision;
            //initialize the melee weapon
            //initialize the ranged weapon
        }


        /// <summary>
        /// getTilesInRadius finds all existing alive tiles in a given radius.
        /// If there are no tiles, the radius expands and is recursively called.
        /// </summary>
        /// <returns>A list of tiles inside the given players' radius</returns>
        /// 

        public Tile[,] getTilesInRadius(Tile[,] tiles, int radius)
        {
            Boolean allTilesAlive = false;
            Tile[,] tilesInRadius = new Tile[(2*radius) + 1, (2*radius) + 1];
            int i = 0;
            int j = 0;
            int r = radius;
            int outx = tiles.GetLength(0);
            int outy = tiles.GetLength(1);
            int x, y;
            int posX = (int)Position.X / 32;
            int posY = (int)Position.Y / 32;
            if (radius < 5) return tilesInRadius;
            //Use x and y as loop control, going from -1 to 1 (to be added to position)
            for (x = -r; x < r; x++)
            {
                j++;
                for (y = -r; y < r; y++)
                {
                    //if neither posX+x or posY+y are out of bounds, continue assignment
                    if (((posX + x >= 0) && (posX + x < outx))
                         && ((posY + y >= 0) && (posY + y < outy)))
                    {
                        //Make sure x and y are not the current tile, then set
                        if (!((x == 0) && (y == 0)))
                        {
                            tilesInRadius[i, j] = tiles[posX + x, posY + y];
                           
                        }
                        else
                        {
                            i--;

                        }
                    }
                    i++;
                }
            }
            foreach(Tile t in tilesInRadius)
            {
                if (t != null)
                {
                    if (!t.isDead())
                    {
                        allTilesAlive = true;
                    }
                }
            }
            
            if(!allTilesAlive) getTilesInRadius(tiles, radius++);

            return tilesInRadius;
        }
        public bool _OnCollision(Fixture fix1, Fixture fix2, Contact con)
        {
            switch(fix2.CollisionCategories)
            {

                case Category.Cat1:
                    return true;

                case Category.Cat2:
                    this.Speed = 40;
                    return false;
           
                case Category.Cat3:
                    this.health -= 1;
                    return true;
                case Category.Cat5:
                    return true;
                case Category.Cat6:
                    this.Speed = 1;
                    return false;

                case Category.Cat9:
                    health += 1;
                    Speed = 10;
                    return false;

                default: return false;
            }
        }

                 
    public void Update(){
        if (dead || removed)
        {
            Vector2 tempLinearVelocity = new Vector2(0, 0);
            LinearVelocity = tempLinearVelocity;
            return;
        }
        tilesInRadius = getTilesInRadius(game.currLevel.tiles, radius);
        if (health >= maxhealth) health = 100;
        updatePosition();
        updateDirection();
        updateAttacks();


            playerController.Update(game, this);

            

            //if the player holds down the shoulder buttons for 2 seconds, remove the player from the level
            //but not kill the player
            if (leavingCount != 0 && (GamePad.GetState(playerNum).IsButtonUp(Buttons.LeftShoulder) ||
                GamePad.GetState(playerNum).IsButtonUp(Buttons.RightShoulder)))
                leavingCount = 0;
            if (GamePad.GetState(playerNum).IsButtonDown(Buttons.LeftShoulder) &&
                GamePad.GetState(playerNum).IsButtonDown(Buttons.RightShoulder))
                leavingCount++;
            if (Keyboard.GetState().IsKeyDown(Keys.E)) 
               leavingCount++;
            if (leavingCount > 60)
            {
                removed = true;
                CollisionCategories = Category.None;
            }


            if (health <= 0)
            {
                dead = true;
                collectedScore = 0;
                CollisionCategories = Category.None;    
            }
        }

        private void updatePosition()
        {
            
            Vector2 tempLinearVelocity = new Vector2(0, 0);
            //movement in these cases are WASD or XBox 360 Controller Left Thumbstick
            if (playerController.checkInput(inputs.moveLeft)) tempLinearVelocity.X = -Speed;
            if (playerController.checkInput(inputs.moveRight)) tempLinearVelocity.X = Speed;
            if (playerController.checkInput(inputs.moveUp)) tempLinearVelocity.Y = -Speed;
            if (playerController.checkInput(inputs.moveDown)) tempLinearVelocity.Y = Speed;
            if (playerController.checkInput(inputs.evacuated)) removed = true;

            if (Position.X > ((game.currLevel.Width*playerSprite.Width) - playerSprite.Width)) Position = new Vector2((game.currLevel.Width*playerSprite.Width) - playerSprite.Width, Position.Y );
            if (Position.X < 0) Position = new Vector2(0, Position.Y);
            if (Position.Y >  ((game.currLevel.Height*playerSprite.Height) - playerSprite.Height)) Position = new Vector2(Position.X,(game.currLevel.Height*playerSprite.Height) - playerSprite.Height);
            if (Position.Y < 0) Position = new Vector2(Position.X, 0);

            LinearVelocity = tempLinearVelocity;
            //playerFixture.Body.Position = Position;
        }

        private void updateDirection()
        {
            //update the direction the player faces
            //this method will also update which direction the sprite faces
            //this configuration allows for 8 directions

            //Hi, Alec added this in so the A.I looked better.  "Player"  either A.I. or Human wil now turn the direction they're facing.
            if (playerController.checkInput(inputs.moveUp)) playerSprite = game.Content.Load<Texture2D>(@"Images/Sweeper/sweeperup");
            else if (playerController.checkInput(inputs.moveDown)) playerSprite = game.Content.Load<Texture2D>(@"Images/Sweeper/sweeperdown");
            else if (playerController.checkInput(inputs.moveLeft)) playerSprite = game.Content.Load<Texture2D>(@"Images/Sweeper/sweeperleft");
            else if (playerController.checkInput(inputs.moveRight)) playerSprite = game.Content.Load<Texture2D>(@"Images/Sweeper/sweeperright");

            if (playerController.checkDirectionInput(attackInputs.attackDown))
            {
                Direction.Y = 5;
                Direction.X = 0;
                playerSprite = game.Content.Load<Texture2D>(@"Images/Sweeper/sweeperdown");
                meleeWeapon.broom = game.Content.Load<Texture2D>(directory + "BroomDown");
            }
            if (playerController.checkDirectionInput(attackInputs.attackLeft))
            {
                Direction.X = -5;
                Direction.Y = 0;
                playerSprite = game.Content.Load<Texture2D>(@"Images/Sweeper/sweeperleft");
                meleeWeapon.broom = game.Content.Load<Texture2D>(directory + "BroomLeft");
            }
            if (playerController.checkDirectionInput(attackInputs.attackUp))
            {
                Direction.Y = -5;
                Direction.X = 0;
                playerSprite = game.Content.Load<Texture2D>(@"Images/Sweeper/sweeperup");
                meleeWeapon.broom = game.Content.Load<Texture2D>(directory + "BroomUp");
            }
            if (playerController.checkDirectionInput(attackInputs.attackRight))
            {
                Direction.X = 5;
                Direction.Y = 0;
                playerSprite = game.Content.Load<Texture2D>(@"Images/Sweeper/sweeperright");
                meleeWeapon.broom = game.Content.Load<Texture2D>(directory + "BroomRight");

            }
            if (playerController.checkDirectionInput(attackInputs.attackDownLeft))
            {
                Direction.X = -5;
                Direction.Y = 5;
                meleeWeapon.broom = game.Content.Load<Texture2D>(directory + "BroomDownLeft");
            }
            if (playerController.checkDirectionInput(attackInputs.attackDownRight))
            {
                Direction.X = 5;
                Direction.Y = 5;
                meleeWeapon.broom = game.Content.Load<Texture2D>(directory + "BroomDownRight");
            }
            if (playerController.checkDirectionInput(attackInputs.attackUpLeft))
            {
                Direction.X = -5;
                Direction.Y = -5;
                meleeWeapon.broom = game.Content.Load<Texture2D>(directory + "BroomUpLeft");
            }
            if (playerController.checkDirectionInput(attackInputs.attackUpRight))
            {
                Direction.X = 5;
                Direction.Y = -5;
                meleeWeapon.broom = game.Content.Load<Texture2D>(directory + "BroomUpRight");
            }
            
        }

        private void updateAttacks()
        {
            if(playerController.checkInput(inputs.rangedAttack))
            {
            rangedPlayerWeapon.Update(Position, Direction);
            }
            else{
                rangedPlayerWeapon.particleEngine.KillParticles(rangedPlayerWeapon.particleEngine.particles);
            }
            if(playerController.checkInput(inputs.meleeAttack))
            meleeWeapon.Update(Position, Direction);
        }

        public bool isDead()
        {
            return dead;
        }
        public bool isRemoved()
        {
            return removed;
        }
        public int collectTile(Tile tile)
        {
            return tile.scorevalue;
        }

        public void Draw(SpriteBatch batch, camera cam)
        {
            batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null,
                cam.get_transformation(game.GraphicsDevice));
            if (!removed && !dead)
            {
                batch.Draw(playerSprite, Position - new Vector2(playerSprite.Width / 2, playerSprite.Height / 2), Color.White);
                playerController.DebugDraw(batch);
            }
            batch.End();
            if (playerController.checkInput(inputs.rangedAttack) && !removed && !dead)
            {
                rangedPlayerWeapon.Draw(batch, cam);
            }
            if (playerController.checkInput(inputs.meleeAttack) && !removed && !dead)
            {
                meleeWeapon.Draw(batch, cam);
            }
        }
    }
}
