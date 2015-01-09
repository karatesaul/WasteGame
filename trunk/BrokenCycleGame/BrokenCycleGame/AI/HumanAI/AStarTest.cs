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

using BrokenCycleGame.AI;

namespace BrokenCycleGame.AI.HumanAI
{
    class AStarTest : Controller
    {
        static InfluenceMap i_map;
        ScheduledAStar a_star;

        WasteGame current_game;
        Player current_player;

        Tile target_tile = null;
        Tile current_tile = null;
        List<Tile> current_path = null;
        List<Tile> test_path = null;

        bool stepped_last_frame = false;

        public AStarTest(WasteGame game)
        {
            a_star = new ScheduledAStar();
        }

        public override void Update(WasteGame game, Player player)
        {
            current_game = game;
            current_player = player;

            if (current_tile == null) { current_tile = game.gameScreen.level.tiles[(int)player.Position.X / 32, (int)player.Position.Y / 32]; }

            if (i_map == null) { i_map = new BasicInfluenceMap(game.gameScreen.level); }

            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                target_tile = game.gameScreen.level.tiles[Mouse.GetState().X / 32, Mouse.GetState().Y / 32];
                current_path = a_star.findPath(game.gameScreen.level.tiles, current_tile, target_tile, new PlayerCostFunction(i_map), new playerHeuristic(target_tile, i_map));
            }
            if ((Keyboard.GetState().IsKeyDown(Keys.P)) && (a_star.hasTask) && (!stepped_last_frame))
            {
                current_path = a_star.schedule(1);
                test_path = a_star.getCurrentTestPath();
                stepped_last_frame = true;
            }

            if ((Keyboard.GetState().IsKeyUp(Keys.P)) && (stepped_last_frame))
            {
                stepped_last_frame = false;
            }
        }

        public override void DebugDraw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
            Texture2D pixel = new Texture2D(sb.GraphicsDevice, 1, 1);
            pixel.SetData<Color>(new Color[1] { Color.White });

            // draw current path
            DrawTestPath(sb, pixel);
            DrawBestPath(sb, pixel);
            DrawTargetTile(sb, pixel);
            

            base.DebugDraw(sb);
        }

        private void DrawBestPath(SpriteBatch sb, Texture2D pixel)
        {
            if (current_path != null)
            {
                foreach (Tile T in current_path)
                {
                    int tx = (int)T.Position.X + 8;
                    int ty = (int)T.Position.Y + 8;

                    Color icolor;

                    switch (current_player.playerNum)
                    {
                        case PlayerIndex.Two:
                            icolor = new Color(0.0f, 0.0f, 1.0f);
                            break;
                        case PlayerIndex.Three:
                            icolor = new Color(0.0f, 1.0f, 0.0f);
                            tx += 2;
                            ty += 2;
                            break;
                        case PlayerIndex.Four:
                            icolor = new Color(1.0f, 1.0f, 0.0f);
                            tx += 4;
                            ty += 4;
                            break;
                        default:
                            icolor = new Color(1.0f, 0.0f, 0.0f);
                            break;
                    }


                    sb.Draw(pixel, new Rectangle(tx, ty, 16, 16), icolor);
                }
            }
        }
        private void DrawTestPath(SpriteBatch sb, Texture2D pixel)
        {
            if (test_path != null)
            {
                foreach (Tile T in test_path)
                {
                    int tx = (int)T.Position.X + 10;
                    int ty = (int)T.Position.Y + 10;

                    Color icolor;

                    switch (current_player.playerNum)
                    {
                        case PlayerIndex.Two:
                            icolor = new Color(0.0f, 0.0f, 0.5f);
                            break;
                        case PlayerIndex.Three:
                            icolor = new Color(0.0f, 0.5f, 0.0f);
                            tx += 2;
                            ty += 2;
                            break;
                        case PlayerIndex.Four:
                            icolor = new Color(0.5f, 0.5f, 0.0f);
                            tx += 4;
                            ty += 4;
                            break;
                        default:
                            icolor = new Color(0.5f, 0.0f, 0.0f);
                            break;
                    }


                    sb.Draw(pixel, new Rectangle(tx, ty, 12, 12), icolor);
                }
            }
        }

        private void DrawTargetTile(SpriteBatch sb, Texture2D pixel)
        {
            if (target_tile != null)
            {

                int tx = (int)target_tile.Position.X;
                int ty = (int)target_tile.Position.Y;

                Color icolor;

                switch (current_player.playerNum)
                {
                    case PlayerIndex.Two:
                        icolor = new Color(0.0f, 0.0f, 1.0f);
                        tx += 16;
                        break;
                    case PlayerIndex.Three:
                        icolor = new Color(0.0f, 1.0f, 0.0f);
                        ty += 16;
                        break;
                    case PlayerIndex.Four:
                        icolor = new Color(1.0f, 1.0f, 0.0f);
                        tx += 16;
                        ty += 16;
                        break;
                    default:
                        icolor = new Color(1.0f, 0.0f, 0.0f);
                        break;
                }


                sb.Draw(pixel, new Rectangle(tx, ty, 16, 16), icolor);
            }
        }
    }
}
