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

namespace BrokenCycleGame
{
    public class PauseScreen : Screen
    {
        #region Fields
        WasteGame game;
        SpriteFont pausedFont;
        Texture2D background;
        Texture2D resumeGame;
        Texture2D resumeGameSelected;
        Texture2D option;
        Texture2D optionSelected;
        Texture2D mainMenu;
        Texture2D mainMenuSelected;
        bool pressedUp = false;
        bool pressedDown = false;
        bool pressedA = false;
        bool pressedEnter = false;
        public int eventCallNum = 0;
        int selected = 0;
        int choices = 3;
        #endregion

        #region Constructor
        public PauseScreen(ContentManager Content, EventHandler screenEvent, WasteGame game)
            : base(screenEvent)
        {
            this.game = game;
            this.screenEvent = screenEvent;
            //load all of the sprites
            //the resume game and main menu sprites are using existing sprites as placeholders
            pausedFont = Content.Load<SpriteFont>("Fonts/deathFont");
            background = Content.Load<Texture2D>("Images/Screens/mapscene_complete");
            resumeGame = Content.Load<Texture2D>("Images/MenuButtons/start_button");
            resumeGameSelected = Content.Load<Texture2D>("Images/MenuButtons/start_button_selected");
            option = Content.Load<Texture2D>("Images/MenuButtons/options_button");
            optionSelected = Content.Load<Texture2D>("Images/MenuButtons/options_button_selected");
            mainMenu = Content.Load<Texture2D>("Images/MenuButtons/quit_button");
            mainMenuSelected = Content.Load<Texture2D>("Images/MenuButtons/quit_button_selected");
        }
        #endregion

        #region Update
        public override void Update(GameTime time)
        {
            KeyboardState keyState = Keyboard.GetState();
            GamePadState padState = GamePad.GetState(PlayerIndex.One);
            //move the cursor up (-1)
            if (keyState.IsKeyDown(Keys.Up) || padState.IsButtonDown(Buttons.DPadUp)
                || padState.ThumbSticks.Left.Y > .025)
                pressedUp = true;
            //move the cursor down (+1)
            if (keyState.IsKeyDown(Keys.Down) || padState.IsButtonDown(Buttons.DPadDown)
                || padState.ThumbSticks.Left.Y < -.025)
                pressedDown = true;
            if (pressedUp && keyState.IsKeyUp(Keys.Down) && keyState.IsKeyUp(Keys.Up) &&
                padState.IsButtonUp(Buttons.DPadUp) && padState.IsButtonUp(Buttons.DPadDown) &&
                padState.ThumbSticks.Left.Y > -.025 && padState.ThumbSticks.Left.Y < .025)
            {
                pressedUp = false;
                selected = (selected - 1) % choices;
                if (selected < 0)
                    selected = choices - 1;
            }
            if (pressedDown && keyState.IsKeyUp(Keys.Down) && keyState.IsKeyUp(Keys.Up) &&
                padState.IsButtonUp(Buttons.DPadUp) && padState.IsButtonUp(Buttons.DPadDown) &&
                padState.ThumbSticks.Left.Y > -.025 && padState.ThumbSticks.Left.Y < .025)
            {
                pressedDown = false;
                selected = (selected + 1) % choices;
            }
            if (padState.IsButtonDown(Buttons.A))
            {
                pressedA = true;
            }
            if (keyState.IsKeyDown(Keys.Enter))
            {
                pressedEnter = true;
            }
            if (pressedA &&  padState.IsButtonUp(Buttons.A))
            {
                pressedA = false;
                eventCallNum = selected;
                screenEvent.Invoke(this, new EventArgs());
            }

            if (pressedEnter && keyState.IsKeyUp(Keys.Enter))
            {
                pressedEnter = false;
                eventCallNum = selected;
                screenEvent.Invoke(this, new EventArgs());
            }

            base.Update(time);
        }
        #endregion

        #region Draw
        public override void Draw(SpriteBatch batch)
        {
            batch.Begin();
            batch.Draw(background, Vector2.Zero, Color.White);
            batch.DrawString(pausedFont, "Paused", new Vector2(300, 100), Color.Red);
            switch (selected)
            {
                case 0: //resume game selected
                    batch.Draw(resumeGameSelected, new Vector2(300, 200), Color.White);
                    batch.Draw(option, new Vector2(300, 250), Color.White);
                    batch.Draw(mainMenu, new Vector2(300, 300), Color.White);
                    break;
                case 1: //options selected
                    batch.Draw(resumeGame, new Vector2(300, 200), Color.White);
                    batch.Draw(optionSelected, new Vector2(300, 250), Color.White);
                    batch.Draw(mainMenu, new Vector2(300, 300), Color.White);
                    break;
                case 2: //main menu selected
                    batch.Draw(resumeGame, new Vector2(300, 200), Color.White);
                    batch.Draw(option, new Vector2(300, 250), Color.White);
                    batch.Draw(mainMenuSelected, new Vector2(300, 300), Color.White);
                    break;
                default:
                    break;
            }
            batch.End();
            base.Draw(batch);
        }
        #endregion
    }
}
