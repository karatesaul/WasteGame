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
    class OptionsScreen : Screen
    {
        #region Fields
        WasteGame game;

        public Screen previousScreen; //

        Texture2D background;
        Texture2D directions;

        Texture2D sound;
        Texture2D soundSelected;
        Texture2D window;
        Texture2D windowSelected;

        Texture2D soundLow;
        Texture2D soundLowSelected;
        Texture2D soundHigh;
        Texture2D soundHighSelected;

        Texture2D windowed;
        Texture2D windowedSelected;
        Texture2D fullscreen;
        Texture2D fullscreenSelected;

        bool pressedUp = false;
        bool pressedDown = false;
        bool pressedLeft = false;
        bool pressedRight = false;
        public int eventCallNum = 0;

        int selected = 0;
        int choices = 2;

        int soundOptionSelected = 0;
        int soundChoices = 2;

        int screenSelected = 0;
        int screenChoices = 2;

        #endregion

        #region Constructor
        public OptionsScreen(ContentManager content, EventHandler events, WasteGame game)
            : base(events)
        {
            this.game = game;
            background = content.Load<Texture2D>(@"Images\Screens\final_level");
            directions = content.Load<Texture2D>(@"Images\OptionsButtons\changeoption");
            sound = content.Load<Texture2D>(@"Images\OptionsButtons\sounds");
            soundSelected = content.Load<Texture2D>(@"Images\OptionsButtons\sounds_selected");
            window = content.Load<Texture2D>(@"Images\OptionsButtons\windowed");
            windowSelected = content.Load<Texture2D>(@"Images\OptionsButtons\windowed_selected");

            soundLow = content.Load<Texture2D>(@"Images\OptionsButtons\off");
            soundLowSelected = content.Load<Texture2D>(@"Images\OptionsButtons\off_selected");
            soundHigh = content.Load<Texture2D>(@"Images\OptionsButtons\on");
            soundHighSelected = content.Load<Texture2D>(@"Images\OptionsButtons\on_selected");

            windowed = content.Load<Texture2D>(@"Images\OptionsButtons\windowed");
            windowedSelected = content.Load<Texture2D>(@"Images\OptionsButtons\windowed_selected");
            fullscreen = content.Load<Texture2D>(@"Images\OptionsButtons\fullscreen_nomarks");
            fullscreenSelected = content.Load<Texture2D>(@"Images\OptionsButtons\fullscreen_nomarks_selected");
        }

        #endregion

        #region Draw
        public override void Draw(SpriteBatch batch)
        {
            batch.Begin();
            batch.Draw(background, Vector2.Zero, Color.White);
            switch (selected)
            {
                case 0:
                    batch.Draw(windowSelected, new Vector2(250, 200), Color.White);
                    switch (screenSelected)
                    {
                        case 0:
                            batch.Draw(windowedSelected, new Vector2(433, 200), Color.White);

                            break;
                        case 1:
                            batch.Draw(fullscreenSelected, new Vector2(433, 200), Color.White);
                            
                            break;
                    }
                    batch.Draw(sound, new Vector2(250, 233), Color.White);
                    switch (soundOptionSelected)
                    {
                        case 0: 
                            batch.Draw(soundLow, new Vector2(400, 233), Color.White);
                            break;
                        case 1:
                            batch.Draw(soundHigh, new Vector2(400, 233), Color.White);
                            break;
                    }
                    break;
                case 1:
                    batch.Draw(window, new Vector2(250, 200), Color.White);
                    switch (screenSelected)
                    {
                        case 0:
                            batch.Draw(windowed, new Vector2(433, 200), Color.White);
                            break;
                        case 1:
                            batch.Draw(fullscreen, new Vector2(433, 200), Color.White);
                        break;
                    }
                    batch.Draw(soundSelected, new Vector2(250, 233), Color.White);
                    switch (soundOptionSelected)
                    {
                        case 0:
                            batch.Draw(soundLowSelected, new Vector2(400, 233), Color.White);
                            break;
                        case 1:
                            batch.Draw(soundHighSelected, new Vector2(400, 233), Color.White);
                            break;
                    }
                    break;
                default:
                    break;
            }
            batch.Draw(directions, new Vector2(0, game.Window.ClientBounds.Height - directions.Height),
            Color.White);
            batch.End();
            base.Draw(batch);
        }

        #endregion

        #region Update
        public override void Update(GameTime time)
        {

            KeyboardState state = Keyboard.GetState();
            GamePadState padState = GamePad.GetState(PlayerIndex.One);

            if (state.IsKeyDown(Keys.Down) || padState.IsButtonDown(Buttons.DPadDown) ||
                padState.ThumbSticks.Left.Y < -.025)
                pressedDown = true;
            //move the cursor up (-1)
            if (state.IsKeyDown(Keys.Up) || padState.IsButtonDown(Buttons.DPadUp) ||
                padState.ThumbSticks.Left.Y > .025)
                pressedUp = true;
            if (state.IsKeyDown(Keys.Left) || padState.IsButtonDown(Buttons.DPadLeft) ||
                padState.ThumbSticks.Left.X < -.025)
                pressedLeft = true;
            if (state.IsKeyDown(Keys.Right) || padState.IsButtonDown(Buttons.DPadRight) ||
                padState.ThumbSticks.Left.X > .025)
                pressedRight = true;
            //the cursor doesn't move until the buttons are released
            if (pressedDown && state.IsKeyUp(Keys.Down) && state.IsKeyUp(Keys.Up) && state.IsKeyUp(Keys.Left) && state.IsKeyUp(Keys.Right)
                && padState.IsButtonUp(Buttons.DPadUp) && padState.IsButtonUp(Buttons.DPadDown) && padState.IsButtonUp(Buttons.DPadLeft) && padState.IsButtonUp(Buttons.DPadRight)
                && padState.ThumbSticks.Left.X > -.025 && padState.ThumbSticks.Left.X < .025
                && padState.ThumbSticks.Left.Y > -.025 && padState.ThumbSticks.Right.Y < .025)
            {
                pressedDown = false;
                selected = (selected + 1) % choices;
            }
            if (pressedUp && state.IsKeyUp(Keys.Down) && state.IsKeyUp(Keys.Up) && state.IsKeyUp(Keys.Left) && state.IsKeyUp(Keys.Right)
                && padState.IsButtonUp(Buttons.DPadUp) && padState.IsButtonUp(Buttons.DPadDown) && padState.IsButtonUp(Buttons.DPadLeft) && padState.IsButtonUp(Buttons.DPadRight)
                && padState.ThumbSticks.Left.X > -.025 && padState.ThumbSticks.Left.X < .025
                && padState.ThumbSticks.Left.Y > -.025 && padState.ThumbSticks.Right.Y < .025)
            {
                pressedUp = false;
                selected = (selected - 1) % choices;
                if (selected < 0)
                    selected = choices - 1;
            }
            if (pressedLeft && state.IsKeyUp(Keys.Down) && state.IsKeyUp(Keys.Up) && state.IsKeyUp(Keys.Left) && state.IsKeyUp(Keys.Right)
                && padState.IsButtonUp(Buttons.DPadUp) && padState.IsButtonUp(Buttons.DPadDown) && padState.IsButtonUp(Buttons.DPadLeft) && padState.IsButtonUp(Buttons.DPadRight)
                && padState.ThumbSticks.Left.X > -.025 && padState.ThumbSticks.Left.X < .025
                && padState.ThumbSticks.Left.Y > -.025 && padState.ThumbSticks.Right.Y < .025)
            {
                pressedLeft = false;
                if (selected % choices == 1)
                {
                    soundOptionSelected = (soundOptionSelected - 1) % soundChoices;
                    if (soundOptionSelected < 0)
                        soundOptionSelected = soundChoices - 1;
                }
                else if (selected % choices == 0)
                {
                    screenSelected = (screenSelected - 1) % screenChoices;
                    if (screenSelected < 0)
                        screenSelected = screenChoices - 1;
                    game.graphics.ToggleFullScreen();
                    game.graphics.ApplyChanges();
                }
            }

            if (pressedRight && state.IsKeyUp(Keys.Down) && state.IsKeyUp(Keys.Up) && state.IsKeyUp(Keys.Left) && state.IsKeyUp(Keys.Right)
                && padState.IsButtonUp(Buttons.DPadUp) && padState.IsButtonUp(Buttons.DPadDown) && padState.IsButtonUp(Buttons.DPadLeft) && padState.IsButtonUp(Buttons.DPadRight)
                && padState.ThumbSticks.Left.X > -.025 && padState.ThumbSticks.Left.X < .025
                && padState.ThumbSticks.Left.Y > -.025 && padState.ThumbSticks.Right.Y < .025)
            {
                pressedRight = false;
                if (selected % choices == 1)
                {
                    soundOptionSelected = (soundOptionSelected + 1) % soundChoices;
                }
                else if (selected % choices == 0)
                {
                    screenSelected = (screenSelected + 1) % screenChoices;
                    game.graphics.ToggleFullScreen();
                    game.graphics.ApplyChanges();
                }
            }
            //confirm the selection
            if (state.IsKeyDown(Keys.Escape) || padState.IsButtonDown(Buttons.B))
            {
                screenEvent.Invoke(this, new EventArgs());
                return;
            }


            base.Update(time);
        }
        #endregion
    }
}