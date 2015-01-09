using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision;
using FarseerPhysics.Common;
using FarseerPhysics.DebugViews;

using BrokenCycleGame.AI.HumanAI;

namespace BrokenCycleGame
{
    public class GameScreen : Screen
    {
        #region Fields
        DebugViewXNA debugView;
        public Texture2D background;

        public Level level;
        SpriteFont deathFont;
        WasteGame game;
        Texture2D Blue, Red, Green, Yellow;
        //Viewport TopLeft;
        //Viewport TopRight;
        //Viewport LowLeft;
        //Viewport LowRight;
        Viewport DefaultPort;
        Viewport[] viewports; //topleft = 0, topright = 1, lowleft = 2, lowright = 3
        public camera[] cameras;
        SpriteFont font;
        #endregion

        #region Constructor
        public GameScreen(ContentManager content, EventHandler events, WasteGame game, World gameWorld)
            : base(events)
        {
            this.game = game;
            background = content.Load<Texture2D>("Images/Screens/final_level");
            deathFont = content.Load<SpriteFont>("Fonts/deathFont");
            Texture2D playerSprite = content.Load<Texture2D>("Images/PlayerPlaceholder");
            Blue = content.Load<Texture2D>("Images/HealthBars/Blue");
            Red = content.Load<Texture2D>("Images/HealthBars/Red");
            Green = content.Load<Texture2D>("Images/HealthBars/Green");
            Yellow = content.Load<Texture2D>("Images/HealthBars/Yellow");
            font = content.Load<SpriteFont>("Calibri");
            debugView = new DebugViewXNA(gameWorld);
            debugView.AppendFlags(DebugViewFlags.AABB);
            debugView.DefaultShapeColor = Color.Black;
            debugView.SleepingShapeColor = Color.LightGray;
            debugView.LoadContent(game.GraphicsDevice, game.Content);
            DefaultPort = game.GraphicsDevice.Viewport;
            viewports = new Viewport[4];
            viewports[0].Width = DefaultPort.Width / 2;
            viewports[0].Height = DefaultPort.Height / 2;
            viewports[0].X = 0;
            viewports[0].Y = 0;
            viewports[1].Width = DefaultPort.Width / 2;
            viewports[1].Height = DefaultPort.Height / 2;
            viewports[1].X = DefaultPort.Width / 2;
            viewports[1].Y = 0;
            viewports[2].Width = DefaultPort.Width / 2;
            viewports[2].Height = DefaultPort.Height / 2;
            viewports[2].X = 0;
            viewports[2].Y = DefaultPort.Height / 2;
            viewports[3].Width = DefaultPort.Width / 2;
            viewports[3].Height = DefaultPort.Height / 2;
            viewports[3].X = DefaultPort.Width / 2;
            viewports[3].Y = DefaultPort.Height / 2;
        }
        #endregion
        #region Camera Making
        public void makeCameras()
        {
            cameras = new camera[4];
            for (int i = 0; i < 4; i++)
            {
                cameras[i] = new camera(game.currLevel.playerList[i], game);
            }
        }
        
        #endregion

        #region Update
        public override void Update(GameTime Time)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Start))
            {
                //Go to Pause Screen
                screenEvent.Invoke(this, new EventArgs());
                return;
            }


            //update the player
            for (int i = 0; i < 4; i++)
            {
                if (game.players != null)
                {
                    if (game.players[i].isDead())
                    {
                        game.players[i].collectedScore = 0;
                        game.currLevel.score += game.players[i].collectedScore;
                    }


                    else
                        game.players[i].Update();
                }
                cameras[i].update();
            }

            game.currLevel.update(Time);
            base.Update(Time);
        }
        #endregion

        #region Draw
        public override void Draw(SpriteBatch Batch)
        {
            makeCameras();
            Batch.Begin();
            Batch.Draw(background, new Vector2(), Color.White);
            Batch.End();
            for (int i = 0; i < 4; i++)
            {
                game.GraphicsDevice.Viewport = viewports[i];
                game.currLevel.draw(Batch, cameras[i]);
                for (int j = 0; j < 4; j++)
                {
                    if (game.players[j] != null)
                        game.players[j].Draw(Batch, cameras[i]);

                }
            }
            game.GraphicsDevice.Viewport = DefaultPort;
            Batch.Begin();
            for (int i = 0; i < 4; i++)
            {
                if (game.players[i].isDead())
                    Batch.DrawString(deathFont, "You Are Dead", new Vector2(viewports[i].X + 32, viewports[i].Y + viewports[i].Height / 2), Color.Red);
                if (game.players[i].isRemoved())
                    Batch.DrawString(deathFont, "Evacuated Safely", new Vector2(viewports[i].X, viewports[i].Y + viewports[i].Height / 2), Color.Green);
            }
            Batch.DrawString(font, "Level Score: " + game.currLevel.score, new Vector2(game.Window.ClientBounds.Width / 2, 0), Color.Black);
            Batch.DrawString(font, "Player 1 Score: " + game.players[0].collectedScore, new Vector2(10, 30), Color.Black);
            Batch.Draw(Red, new Rectangle(10, 50, game.players[0].health, 25), Color.White);
            Batch.DrawString(font, "Player 1 Health: ", new Vector2(10, 50), Color.Black);


            Batch.Draw(Blue, new Rectangle(game.Window.ClientBounds.Width - 200, 50, game.players[1].health, 25), Color.White);
            Batch.DrawString(font, "Player 2 Health: ", new Vector2(game.Window.ClientBounds.Width - 200, 50), Color.Black);
            Batch.DrawString(font, "Player 2 Score: " + game.players[1].collectedScore, new Vector2(game.Window.ClientBounds.Width - 200, 30), Color.Black);


            Batch.Draw(Green, new Rectangle(10, game.Window.ClientBounds.Height - 50, game.players[2].health, 25), Color.White);
            Batch.DrawString(font, "Player 3 Health: ", new Vector2(10, game.Window.ClientBounds.Height - 50), Color.Black);
            Batch.DrawString(font, "Player 3 Score: " + game.players[2].collectedScore, new Vector2(10, game.Window.ClientBounds.Height - 30), Color.Black);


            Batch.Draw(Yellow, new Rectangle(game.Window.ClientBounds.Width - 200, game.Window.ClientBounds.Height - 50, game.players[3].health, 25), Color.White);
            Batch.DrawString(font, "Player 4 Health: ", new Vector2(game.Window.ClientBounds.Width - 200, game.Window.ClientBounds.Height - 50), Color.Black);
            Batch.DrawString(font, "Player 4 Score: " + game.players[3].collectedScore, new Vector2(game.Window.ClientBounds.Width - 200, game.Window.ClientBounds.Height - 30), Color.Black);


            Batch.End();
            base.Draw(Batch);
        }
        #endregion
    }
}
