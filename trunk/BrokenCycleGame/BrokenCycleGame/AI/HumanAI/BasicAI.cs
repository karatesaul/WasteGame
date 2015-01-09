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
    /// <summary>
    /// BasicAI is a very basic AI class which attempts to clean tiles, avoid enemies,
    /// and avoid other players using steering techniques.
    /// </summary>
    class BasicAI : Controller
    {
        const bool DEBUG_DRAW_ON = false;

        double weakTilesFarFromEnemy(Tile T)
        {
            double score = 0.0;

            double distance = (T.Position - current_player.Position).Length();

            double health = T.health;

            List<Body> target_list = current_game.currLevel.roombaList.Cast<Body>().ToList<Body>();
            target_list.AddRange(current_game.currLevel.towerList.Cast<Body>().ToList<Body>());

            double enemy_density = 0.0;

            foreach (Body B in target_list)
            {
                enemy_density += 1 / (T.Position - B.Position).LengthSquared();
            }

            score = distance * 1.0 + health * 10 + 100000 * enemy_density;

            return score;
        }

        double weakTilesWithSmallInfluence(Tile T)
        {
            double score = 100 - T.scorevalue;

            double distance = (T.Position - current_player.Position).Length() * 100 / 32;

            double health = T.health;

            double influence = 0;
            if(i_map != null)
            influence = i_map.getInfluenceAt(T);

            if (influence < 0)
            {
                score = distance * 1.0 + health * 1.0 - 100 * influence;
            }
            else
            {
                score = distance * 1.0 + health * 1.0 + 20 * influence;
            }

            return score;
        }

        TileHeuristic tile_heuristic;

        WasteGame current_game;
        Player current_player;

        static InfluenceMap i_map;
        static ScheduledAStar a_star = null;

        static bool influenceCalculatedThisFrame = false;
        static bool influenceMapDrawnThisFrame = false;
        static bool drawInfluenceMap = false;
        static bool toggleDebugDrawHeld = false;

        // Steering constants and variables
        const float NEAR_TILE_ATTRACTION_FACTOR = 1.0f;

        const float NEAR_ENEMY_REPULSION_RADIUS = 160.0f;
        const float NEAR_ENEMY_REPULSION_FACTOR = 2.0f;
        const float NEAR_ENEMY_REPULSION_DECAY = 2.0f;

        const float NEAR_PLAYER_REPULSION_RADIUS = 480.0f;
        const float NEAR_PLAYER_REPULSION_FACTOR = 1.5f;
        const float NEAR_PLAYER_REPULSION_DECAY = 2.0f;

        Tile target_tile = null;
        List<Tile> current_path = null;

        int Timer = 300;

        public BasicAI()
        {
            if (a_star == null) { a_star = new ScheduledAStar(); }

            tile_heuristic = weakTilesWithSmallInfluence;
            i_map = null;
            //influenceCalculatedThisFrame = false;
        }

        /// <summary>
        /// Get the steering vector for fleeing from enemies.
        /// </summary>
        private Vector2 getEnemySteeringVector(Level level, Player player)
        {
            // Cast the level's roomba list to a Body list so that it can be used
            // in getSteeringVectorFromCollection
            List<Body> target_list = level.roombaList.Cast<Body>().ToList<Body>();
            target_list.AddRange(level.towerList.Cast<Body>().ToList<Body>());
            return -NEAR_ENEMY_REPULSION_FACTOR * CommonAI.getSteeringVectorFromCollection(target_list, player, NEAR_ENEMY_REPULSION_RADIUS, NEAR_ENEMY_REPULSION_DECAY);
        }

        /// <summary>
        /// Get the steering vector for fleeing from other players.
        /// </summary>
        private Vector2 getPlayerSteeringVector(WasteGame game, Player player)
        {
            // Add all players that are not the current player to the target list

            List<Body> target_list = new List<Body>();

            foreach (Player P in game.players)
            {
                if (P == player || P.isDead()) continue;
                target_list.Add(P);
            }

            return -NEAR_PLAYER_REPULSION_FACTOR * CommonAI.getSteeringVectorFromCollection(target_list, player, NEAR_PLAYER_REPULSION_RADIUS, NEAR_PLAYER_REPULSION_DECAY);
        }


        public override void Update(WasteGame game, Player player)
        {

            if (Timer == 0)
            {
                target_tile = null;
                Timer = 300;
            }
            current_game = game;
            current_player = player;

            UpdateInfluenceMap(game, player);

            if ((target_tile == null) || (target_tile.isDead()))
            {
                Timer = 300;
                target_tile = CommonAI.getLeastTile(game.currLevel.tiles, tile_heuristic);

                current_path = a_star.findPath(game.currLevel.tiles, game.currLevel.tiles[(int)current_player.Position.X / 32, (int)current_player.Position.Y / 32], target_tile, new PlayerCostFunction(i_map), new playerHeuristic(target_tile, i_map));
            }

            UpdatePath(game, player);

            WeaponChoice(game, player);

            //UpdateSteeringVector(game, player);
        }


        private void UpdateInfluenceMap(WasteGame game, Player player)
        {
            // All BasicAI's share one influence map.  Update it only once.
            if (!influenceCalculatedThisFrame)
            {
                if (i_map == null) { 
                i_map = new BasicInfluenceMap(game.currLevel);
                }

                i_map.calculateInfluence(game.currLevel);
                influenceCalculatedThisFrame = true;
            }

            // Toggle debug drawing of influence map
            // When the player presses ctrl+I.
            if (!toggleDebugDrawHeld)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.LeftControl) && Keyboard.GetState().IsKeyDown(Keys.I))
                {
                    drawInfluenceMap = !drawInfluenceMap;
                    toggleDebugDrawHeld = true;
                }
            }
            if (Keyboard.GetState().IsKeyUp(Keys.LeftControl) || Keyboard.GetState().IsKeyUp(Keys.I))
            {
                toggleDebugDrawHeld = false;
            }

            // Set drawn to false so that once Draw() is reached the map is drawn only once.
            influenceMapDrawnThisFrame = false;
        }
        private void UpdateSteeringVector(WasteGame game, Player player)
        {
            if (target_tile != null)
            {
                Vector2 near_tile_steer = target_tile.Position - player.Position;
                near_tile_steer.Normalize();
                near_tile_steer *= (float)NEAR_TILE_ATTRACTION_FACTOR;


                Vector2 near_enemy_steer = getEnemySteeringVector(game.currLevel, player);
                //Vector2 near_enemy_steer = Vector2.Zero;

                Vector2 near_player_steer = getPlayerSteeringVector(game, player);
                //Vector2 near_player_steer = Vector2.Zero;

                int num_tiles_left = CommonAI.getNumLivingTilesAround(game.currLevel.tiles, target_tile.Position, 160.0f);
                Vector2 steer;

                if (num_tiles_left < 5)
                {
                    steer = near_tile_steer;
                }
                else if (num_tiles_left < 10)
                {
                    steer = near_tile_steer * 2 + near_enemy_steer;
                }
                else if (num_tiles_left < 15)
                {
                    steer = near_tile_steer * 2 + near_enemy_steer + near_player_steer;
                }
                else
                {
                    steer = near_tile_steer + near_enemy_steer + near_player_steer;
                }

                if (steer.X < 0) { left = true; right = false; }
                else if (steer.X > 0) { left = false; right = true; }
                else { left = false; right = false; }

                if (steer.Y < 0) { up = true; down = false; }
                else if (steer.Y > 0) { up = false; down = true; }
                else { up = false; down = false; }

                if (steer == Vector2.Zero) { right = true; }

                if ((player.Position + steer - target_tile.Position).Length() >= (player.Position - target_tile.Position).Length())
                {
                    // If the steering vector does not get the player closer to the target tile, invalidate it and search for another.
                    target_tile = null;
                }
            }
        }
        private void UpdatePath(WasteGame game, Player player)
        {
            if (a_star.hasTask) 
            { 
                current_path = a_star.schedule(1);

                if ((current_path != null) && (current_path.Count > 0))
                {
                    Tile nearest_tile = current_path[0];
                    float nearest_distance = float.PositiveInfinity;
                    float current_distance;

                    foreach (Tile T in current_path)
                    {
                        current_distance = (T.Position - current_player.Position).Length();
                        if (current_distance < nearest_distance)
                        {
                            nearest_tile = T;
                            nearest_distance = current_distance;
                        }
                    }

                    for (int i = current_path.IndexOf(nearest_tile) - 1; i >= 0; --i)
                    {
                        current_path.RemoveAt(i);
                    }
                }
            }

            if (current_path != null)
            {
                Vector2 steer = Vector2.Zero;

                if (current_path.Count == 0) { /*target_tile = null;*/ return; }

                while (true)
                {

                    Timer--;
                    float current_distance = (target_tile.Position - current_player.Position).Length() - 16;
                    float next_distance = (target_tile.Position - current_path[0].Position).Length();

                    if (current_distance <= next_distance)
                    {
                        current_path.RemoveAt(0);
                        if (current_path.Count == 0) break;
                        continue;
                    }
                    else
                    {
                        steer = current_path[0].Position - current_player.Position;
                        break;
                    }
                }

                if (steer.X < 0) { left = true; right = false; }
                else if (steer.X > 0) { left = false; right = true; }
                else { left = false; right = false; }

                if (steer.Y < 0) { up = true; down = false; }
                else if (steer.Y > 0) { up = false; down = true; }
                else { up = false; down = false; }
            }
            else
            {
                if (target_tile != null)
                {
                    target_tile = target_tile.getConnections()[0];
                }
            }
        }

        private void WeaponChoice(WasteGame game, Player player)
        {
            UtilityMethod utilMeth = new UtilityMethod();
            List<Body> target_list = game.currLevel.roombaList.Cast<Body>().ToList<Body>();
            target_list.AddRange(game.currLevel.towerList.Cast<Body>().ToList<Body>());
            float closestEnemy = 1000;
            int focus = 0;
            float distance = 0;
            float dirX = 0;
            float dirY = 0;

            ranged = false;
            melee = false;

            if (target_list.Count > 0)
            {
                for (int i = 0; i < target_list.Count; i++)
                {
                    distance = (target_list[i].Position - player.Position).Length();
                    if (closestEnemy > distance)
                    {
                        closestEnemy = distance;
                        focus = i;
                    }
                }

                dirX = target_list[focus].Position.X - player.Position.X;
                dirY = target_list[focus].Position.Y - player.Position.Y;

                if (dirX < 0 && dirY < 0) { attackUpLeft = true;}
                else if (dirX > 0 && dirY > 0) { attackDownRight = true; }
                else if (dirX < 0 && dirY > 0) { attackUpRight = true; }
                else if (dirX > 0 && dirY < 0) { attackDownLeft = true; }
                else if (dirX == 0 && dirY > 0) { attackRight = true; }
                else if (dirX > 0 && dirY == 0) { attackDown = true; }
                else if (dirX < 0 && dirY == 0) { attackUp = true; }
                else if (dirX == 0 && dirY < 0) { attackLeft = true; }

                Vector2 meleeRange = new Vector2(player.Position.X + player.Direction.X * 48 / 10, player.Position.Y + player.Direction.Y * 48 / 10);
                Vector2 rangeRange = new Vector2(player.Position.X + player.Direction.X * 48 / 5, player.Position.Y + player.Direction.Y * 48 / 5);

                float r = (rangeRange - player.Position).Length();
                float m = (meleeRange - player.Position).Length();

                int weapon = utilMeth.chooseAction(target_list, player, r, m, player.health);

                if (weapon == 0)
                {
                    melee = true;
                    ranged = false;
                }
                else if (weapon == 1)
                {
                    ranged = true;
                    melee = false;
                }
                else
                {
                    ranged = false;
                    melee = false;
                }
            }
        }

        public override void DebugDraw(SpriteBatch sb)
        {
            if (DEBUG_DRAW_ON)
            {
                Texture2D pixel = new Texture2D(sb.GraphicsDevice, 1, 1);
                pixel.SetData<Color>(new Color[1] { Color.White });

                // Draw target tile
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

                // draw current path

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
                                break;
                            case PlayerIndex.Four:
                                icolor = new Color(1.0f, 1.0f, 0.0f);
                                break;
                            default:
                                icolor = new Color(1.0f, 0.0f, 0.0f);
                                break;
                        }


                        sb.Draw(pixel, new Rectangle(tx, ty, 16, 16), icolor);
                    }
                }

                influenceCalculatedThisFrame = false;

                // Draw influence map (only once per frame)
                if ((i_map != null) && (drawInfluenceMap) && (!influenceMapDrawnThisFrame))
                {
                    i_map.Draw(sb);
                    influenceMapDrawnThisFrame = true;
                }
            }
        }
    }
}
