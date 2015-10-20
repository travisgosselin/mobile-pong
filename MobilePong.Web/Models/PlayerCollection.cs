using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MobilePong.Web.Models
{
    public class PlayerCollection : List<Player>
    {
        public List<Player> PaddlePlayers
        {
            get { return this.Where(t => t.IsPaddleController).ToList(); }
        }

        public List<Player> BallPlayers
        {
            get { return this.Where(t => !t.IsPaddleController).ToList(); }
        }

        public void CheckCollision(GameSettings settings)
        {
            foreach (var pong in BallPlayers.Select(t => t.Ball))
            {
                // check for collision with the bottom paddle
                /*if (pong.X >= player.PaddlePosition 
                    && (pong.X + (settings.BallRadius * 2)) <= (player.PaddlePosition + settings.PaddleWidth)
                    && pong.Y >= settings.GameHeight - settings.PaddleHeight
                    && pong.Y + (settings.BallRadius * 2) <= settings.GameHeight
                    && pong.YDirectionUp)
                {
                    pong.YDirectionUp = !pong.YDirectionUp;
                    return player;
                }*/

                foreach (var paddle in PaddlePlayers.Select(t => t.Paddle))
                {
                    if (pong.X >= paddle.X - (paddle.Width/2)
                        && pong.X <= paddle.X + (paddle.Width/2)
                        && pong.Y >= settings.GameHeight - settings.PaddleHeight
                        && pong.Y <= settings.GameHeight
                        && !pong.YDirectionUp)
                    {
                        pong.YDirectionUp = !pong.YDirectionUp;
                    }
                }
            }
        }

        public int SmallestHeight
        {
            get { return this.Min(t => t.ScreenHeight); }
        }

        public int SmallestWidth
        {
            get { return this.Min(t => t.ScreenWidth); }
        }
    }
}