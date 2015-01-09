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

namespace BrokenCycleGame
{
    public enum inputs
    {
        moveLeft, moveRight, moveUp, moveDown, meleeAttack, rangedAttack, evacuated
    }

    public enum attackInputs
    {
        attackLeft, attackRight, attackUp, attackDown, attackUpLeft, attackUpRight, attackDownLeft, attackDownRight
    }
    /// <summary>
    /// Controller class
    /// used for sending commands to the players
    /// Inherited by both Human (the class for receiving input from the gamepad)
    /// and AI classes.
    /// </summary>
    public abstract class Controller
    {
        // These booleans represent whether each given input is asserted during
        // each given game tick.
        protected bool left = false;
        protected bool right = false;
        protected bool up = false;
        protected bool down = false;
        protected bool melee = false;
        protected bool ranged = false;
        protected bool removed = false;

        protected bool attackDown = false;
        protected bool attackLeft = false;
        protected bool attackUp = false;
        protected bool attackRight = false;
        protected bool attackUpLeft = false;
        protected bool attackUpRight = false;
        protected bool attackDownLeft = false;
        protected bool attackDownRight = false;
        /// <summary>
        /// Update() is called by the Player once per tick.  It needs to be defined
        /// by each inheriting class.  Inside Update, all of the input booleans need
        /// to be set.  References to the game and the Player are passed to Update
        /// in case the AI needs to use them.
        /// </summary>
        public abstract void Update(WasteGame game, Player player);

        public virtual void DebugDraw(SpriteBatch sb)
        {
            // By default draw nothing
            // This can be used to draw debug output if necessary
        }

        /// <summary>
        /// checkInput() is used to check each individual input.  It simply switches
        /// the requested input enum and returns the requested boolean value.  It
        /// cannot be overriden by any inheriting classes.
        /// </summary>
        public Boolean checkInput(inputs input)
        {
            switch (input)
            {
                case inputs.moveLeft: return left;
                case inputs.moveRight: return right;
                case inputs.moveUp: return up;
                case inputs.moveDown: return down;
                case inputs.meleeAttack: return melee;
                case inputs.rangedAttack: return ranged;
                case inputs.evacuated: return removed;
                default: return false;
            }
        }

        public Boolean checkDirectionInput(attackInputs input)
        {
            switch (input)
            {
                case attackInputs.attackDown: return attackDown;
                case attackInputs.attackUp: return attackUp;
                case attackInputs.attackLeft: return attackLeft;
                case attackInputs.attackRight: return attackRight;
                case attackInputs.attackUpLeft: return attackUpLeft;
                case attackInputs.attackUpRight: return attackUpRight;
                case attackInputs.attackDownLeft: return attackDownLeft;
                case attackInputs.attackDownRight: return attackDownRight;
                default: return false;
            }
        }
    }
}
