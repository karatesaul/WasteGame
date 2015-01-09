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
    class TitleScreen : Screen
    {
        #region Fields
        Texture2D background;
        Texture2D OptionsBox;
        Texture2D credits;
        Texture2D creditsSelected;
        Texture2D startGame;
        Texture2D OptionsBoxSelected;
        Texture2D startGameSelected;
        Texture2D exitGame;
        Texture2D exitGameSelected;
        bool pressedUp = false;
        bool pressedDown = false;
        bool pressedSpace = false;
        bool pressedStart = false;
        bool pressedTab = false;
        bool pressedY = false;
        public int eventCallNum = 0;
        WasteGame game;
        //keeps track of what menu option is selected - basically the cursor
        int selected = 0;
        const int choices = 4;

        #endregion

        #region Constructor
        public TitleScreen(ContentManager content, EventHandler events, WasteGame game)
            : base(events)
        {
            this.game = game;
            credits = content.Load<Texture2D>(@"Images\MenuButtons\credits_button");
            creditsSelected = content.Load<Texture2D>(@"Images\MenuButtons\credit_button_selected");
            background = content.Load<Texture2D>(@"Images\BrokenCycleTitle");
            OptionsBox = content.Load<Texture2D>(@"Images\MenuButtons\options_button");
            startGame = content.Load<Texture2D>(@"Images\MenuButtons\start_button");
            OptionsBoxSelected = content.Load<Texture2D>(@"Images\MenuButtons\options_button_selected");
            startGameSelected = content.Load<Texture2D>(@"Images\MenuButtons\start_button_selected");
            exitGame = content.Load<Texture2D>(@"Images\MenuButtons\quit_button");
            exitGameSelected = content.Load<Texture2D>(@"Images\MenuButtons\quit_button_selected");
        }

        #endregion

        #region Update
        public override void Update(GameTime time)
        {
            KeyboardState state = Keyboard.GetState();
            GamePadState padState = GamePad.GetState(PlayerIndex.One);
            //move the cursor down (+1)
            if (state.IsKeyDown(Keys.Down) || padState.IsButtonDown(Buttons.DPadDown) ||
                padState.ThumbSticks.Left.Y < -.025)
                pressedDown = true;
            //move the cursor up (-1)
            if (state.IsKeyDown(Keys.Up) || padState.IsButtonDown(Buttons.DPadUp) ||
                padState.ThumbSticks.Left.Y > .025)
                pressedUp = true;
            //the cursor doesn't move until the buttons are released
            if (pressedDown && state.IsKeyUp(Keys.Down) && state.IsKeyUp(Keys.Up) &&
                padState.IsButtonUp(Buttons.DPadUp) && padState.IsButtonUp(Buttons.DPadDown) &&
                padState.ThumbSticks.Left.Y > -.025 && padState.ThumbSticks.Left.Y < .025)
            {
                pressedDown = false;
                selected = (selected + 1) % choices;
            }
            if (pressedUp && state.IsKeyUp(Keys.Down) && state.IsKeyUp(Keys.Up) &&
                padState.IsButtonUp(Buttons.DPadUp) && padState.IsButtonUp(Buttons.DPadDown) &&
                padState.ThumbSticks.Left.Y > -.025 && padState.ThumbSticks.Left.Y < .025)
            {
                pressedUp = false;
                selected = (selected - 1) % choices;
                if (selected < 0)
                    selected = choices - 1;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Tab))
            {
                pressedTab = true;
            }
            if(GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Y))
            {
                pressedY = true;
            }

            if (pressedTab && Keyboard.GetState().IsKeyUp(Keys.Tab))
            {
                eventCallNum = 5;
                screenEvent.Invoke(this, new EventArgs());

                pressedTab = false;
            }

            if (pressedY && GamePad.GetState(PlayerIndex.One).IsButtonUp(Buttons.Y))
            {
                eventCallNum = 5;
                screenEvent.Invoke(this, new EventArgs());
                pressedY = false;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
                pressedSpace = true;
            if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Start))
            {
                pressedStart = true;
            }
            if (pressedSpace && (Keyboard.GetState().IsKeyUp(Keys.Space) /*|| GamePad.GetState(PlayerIndex.One).IsButtonUp(Buttons.Start)*/))
            {
                eventCallNum = selected;
                screenEvent.Invoke(this, new EventArgs());
                pressedSpace = false;
            }

            if (pressedStart && GamePad.GetState(PlayerIndex.One).IsButtonUp(Buttons.Start))
            {
                eventCallNum = selected;
                screenEvent.Invoke(this, new EventArgs());
                pressedStart = false;
            }

            base.Update(time);
        }

        #endregion

        #region Draw
        public override void Draw(SpriteBatch batch)
        {

            batch.Begin();
            batch.Draw(background, Vector2.Zero, Color.White);
            switch (selected)
            {
                case 0: //mainMenu selected
                    batch.Draw(startGameSelected, new Vector2(200, 200), Color.White);
                    batch.Draw(OptionsBox, new Vector2(200, 250), Color.White);
                    batch.Draw(exitGame, new Vector2(200, 300), Color.White);
                    batch.Draw(credits, new Vector2(200, 350), Color.White);
                    break;

                case 1: //options selected
                    batch.Draw(startGame, new Vector2(200, 200), Color.White);
                    batch.Draw(OptionsBoxSelected, new Vector2(200, 250), Color.White);
                    batch.Draw(exitGame, new Vector2(200, 300), Color.White);
                    batch.Draw(credits, new Vector2(200, 350), Color.White);
                    break;

                case 2:
                    batch.Draw(startGame, new Vector2(200, 200), Color.White);
                    batch.Draw(OptionsBox, new Vector2(200, 250), Color.White);
                    batch.Draw(exitGameSelected, new Vector2(200, 300), Color.White);
                    batch.Draw(credits, new Vector2(200, 350), Color.White);
                    break;

                case 3:
                    batch.Draw(startGame, new Vector2(200, 200), Color.White);
                    batch.Draw(OptionsBox, new Vector2(200, 250), Color.White);
                    batch.Draw(exitGame, new Vector2(200, 300), Color.White);
                    batch.Draw(creditsSelected, new Vector2(200, 350), Color.White);
                    break;
                default: //anything else - should not happen
                    break;
            }
            batch.End();
            base.Draw(batch);
        }
        #endregion
    }
}