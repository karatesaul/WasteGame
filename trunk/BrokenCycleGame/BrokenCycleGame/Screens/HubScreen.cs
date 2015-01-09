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
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision;
using FarseerPhysics.Common;
using FarseerPhysics.DebugViews;

namespace BrokenCycleGame.Screens
{
    class HubScreen : Screen
    {
        Texture2D background;
        Texture2D selected;
        Texture2D notselected;
        List<Level> unlockedLevels;
        List<Level> lockedLevels;
        WasteGame game;
        bool pressedUp = false;
        bool pressedDown = false;
        bool pressedSpace = false;
        bool pressedStart = false;
        public int whichlevel;
        public int eventCallNum;
        const int choices = 5;
        //List levellist
        //List Unlockedlevels

        public HubScreen(ContentManager content, EventHandler events, WasteGame game, World gameWorld)
            : base(events)
        {
            this.game = game;
            whichlevel = 0;
            selected = content.Load<Texture2D>("Images/Screens/SelectedLevel");
            notselected = content.Load<Texture2D>("Images/Screens/NotSelectedLevel");
            background = content.Load<Texture2D>("Images/Screens/mapscene_complete");
        }
        #region update
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
                whichlevel = (whichlevel - 1) % choices;
                if (whichlevel - 1 < 0) whichlevel = 0;
            }
            if (pressedUp && state.IsKeyUp(Keys.Down) && state.IsKeyUp(Keys.Up) &&
                padState.IsButtonUp(Buttons.DPadUp) && padState.IsButtonUp(Buttons.DPadDown) &&
                padState.ThumbSticks.Left.Y > -.025 && padState.ThumbSticks.Left.Y < .025)
            {
                pressedUp = false;
                whichlevel = (whichlevel + 1) % (choices);
                if (whichlevel >= choices)
                    whichlevel = choices;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
                pressedSpace = true;
            if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Start))
            {
                pressedStart = true;
            }
            if (pressedSpace && (Keyboard.GetState().IsKeyUp(Keys.Space) /*|| GamePad.GetState(PlayerIndex.One).IsButtonUp(Buttons.Start)*/))
            {
                eventCallNum = choices;
                screenEvent.Invoke(this, new EventArgs());
                pressedSpace = false;
            }

            if (pressedStart && GamePad.GetState(PlayerIndex.One).IsButtonUp(Buttons.Start))
            {
                eventCallNum = choices;
                screenEvent.Invoke(this, new EventArgs());
                pressedStart = false;
            }

            base.Update(time);
        }
        #endregion

        #region draw
        public override void Draw(SpriteBatch batch)
        {
            base.Draw(batch);

            batch.Begin();

            batch.Draw(background, new Vector2(0, 0), Color.White);

            if (whichlevel == 0)
            {
                batch.Draw(selected, new Vector2(500, 530), Color.White);
            }
            else
            {
                batch.Draw(notselected, new Vector2(500, 530), Color.White);
            }
            if (whichlevel == 1)
            {
                batch.Draw(selected, new Vector2(400, 420), Color.White);
            }
            else
            {
                batch.Draw(notselected, new Vector2(400, 420), Color.White);
            }
            if (whichlevel == 2)
            {
                batch.Draw(selected, new Vector2(120, 300), Color.White);
            }
            else
            {
                batch.Draw(notselected, new Vector2(120, 300), Color.White);
            }
            if (whichlevel == 3)
            {
                batch.Draw(selected, new Vector2(320, 260), Color.White);
            }
            else
            {
                batch.Draw(notselected, new Vector2(320, 260), Color.White);
            }
            if (whichlevel == 4)
            {
                batch.Draw(selected, new Vector2(360, 60), Color.White);
            }
            else
            {
                batch.Draw(notselected, new Vector2(360, 60), Color.White);
            }
            batch.End();
        }
        #endregion
    }
}
