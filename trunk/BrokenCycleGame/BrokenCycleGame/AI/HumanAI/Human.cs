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
namespace BrokenCycleGame.AI.HumanAI
{
    class Human : Controller
    {
        /// <summary>
        /// The player index of the controller
        /// </summary>
        private PlayerIndex playerIndex;
        private GamePadState prevGamePadState = new GamePadState();
        /// <summary>
        /// Creates a Human controller and associates it with the given PlayerIndex
        /// </summary>
        public Human(PlayerIndex playerIndex)
        {
            this.playerIndex = playerIndex;

        }

        /// <summary>
        /// Checks the gamepad at PlayerIndex for each input and sets the Controller's
        /// boolean values
        /// </summary>
        public override void Update(WasteGame game, Player player)
        {
            KeyboardState keyState = Keyboard.GetState();
            GamePadState gamePadState = GamePad.GetState(playerIndex);
            // Get controller information once per tick
            left  = (keyState.IsKeyDown(Keys.A) || 
                gamePadState.ThumbSticks.Left.X < -.025);
            down  = (keyState.IsKeyDown(Keys.S) ||
                gamePadState.ThumbSticks.Left.Y < -.025);
            up    = (keyState.IsKeyDown(Keys.W) ||
                gamePadState.ThumbSticks.Left.Y > .025);
            right = (keyState.IsKeyDown(Keys.D) ||
                gamePadState.ThumbSticks.Left.X > .025);


            melee = gamePadState.IsButtonDown(Buttons.LeftTrigger) || keyState.IsKeyDown(Keys.LeftControl);
            ranged = gamePadState.IsButtonDown(Buttons.RightTrigger) || keyState.IsKeyDown(Keys.Space);

            attackDown = (gamePadState.ThumbSticks.Right.Y < -.025 || keyState.IsKeyDown(Keys.Down));
            attackUp = (gamePadState.ThumbSticks.Right.Y > .025 || keyState.IsKeyDown(Keys.Up));
            attackLeft = (gamePadState.ThumbSticks.Right.X < -.025 || keyState.IsKeyDown(Keys.Left));
            attackRight = (gamePadState.ThumbSticks.Right.X > .025 || keyState.IsKeyDown(Keys.Right));
            attackUpLeft = (attackUp && attackLeft);
            attackUpRight = (attackUp && attackRight);
            attackDownLeft = (attackDown && attackLeft);
            attackDownRight = (attackDown && attackRight);
            prevGamePadState = gamePadState;
        }
    }
}
