using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

using Microsoft.AspNet.SignalR;

using MobilePong.Web.Hubs;

namespace MobilePong.Web.Models
{
    public class Game
    {
        public GameOverview View { get; set; }
        
        private IHubContext HubContext
        {
            get
            {
                return GlobalHost.ConnectionManager.GetHubContext<GameHub>();
            }
        }

        public Game()
        {
            View = new GameOverview();
        }

        public void Join(string connectionId, string username, bool isPaddle, int width, int height)
        {
            // check to see if the player exists
            var player = View.Players.FirstOrDefault(t => t.ConnectionId == connectionId || t.Name.ToLower() == username.ToLower());
            if (player != null)
            {
                player.ConnectionId = connectionId;
                player.Name = username;
                player.ScreenHeight = height;
                player.ScreenWidth = width;
                player.IsPaddleController = isPaddle;
            }
            else
            {
                // create the new player
                player = new Player { 
                    ConnectionId = connectionId, 
                    Name = username, 
                    ScreenHeight = height, 
                    ScreenWidth = width, 
                    IsPaddleController = isPaddle 
                };
                View.Players.Add(player);
            }

            // if the game is in progress already then just start the game for this user only
            if (View.Status == GameStatus.InProgress)
            {
                if (isPaddle) player.Paddle = new Paddle(View.Settings);
                else player.Ball = new Ball(View.Settings);

                View.Players.ForEach(t => HubContext.Clients.Client(t.ConnectionId).UpdateGame(View));
                HubContext.Clients.Client(player.ConnectionId).StartGame(View, player.IsPaddleController);
            }
            else if (View.Status == GameStatus.WaitingForPlayers && View.Players.Count > 1)
            {
                View.Players.ForEach(t => HubContext.Clients.Client(t.ConnectionId).UpdateGame(View));
                Task.Factory.StartNew(GameLoop);
            }
            else
            {
                View.Players.ForEach(t => HubContext.Clients.Client(t.ConnectionId).UpdateGame(View));
            }
        }

        #region - Entry Actions -

        public void PaddleMove(string connectionId, int xPosition)
        {
            var player = View.Players.FirstOrDefault(t => t.ConnectionId == connectionId);
            if (player != null)
            {
                 player.Paddle.PaddleMove(xPosition);
            }
        }

        public void PaddleExpand(string connectionId)
        {
            var player = View.Players.FirstOrDefault(t => t.ConnectionId == connectionId);
            if (player != null)
            {
                player.Paddle.PaddleExpand();
            }
        }
        
        public void SpeedBurst(string connectionId)
        {
            var player = View.Players.FirstOrDefault(t => t.ConnectionId == connectionId);
            if (player != null)
            {
                player.Ball.SpeedBurst();
            }
        }

        public void MoveBall(string connectionId, bool isLeft)
        {
            var player = View.Players.FirstOrDefault(t => t.ConnectionId == connectionId);
            if (player != null)
            {
                player.Ball.XDirectionLeft = isLeft;
            }
        }

        #endregion

        private void GameLoop()
        {
            // initialize starting components
            View.Settings.GameHeight = View.Players.SmallestHeight;
            View.Settings.GameWidth = View.Players.SmallestWidth;
            View.Players.PaddlePlayers.ForEach(t => t.Paddle = new Paddle(View.Settings));
            View.Players.BallPlayers.ForEach(t => t.Ball = new Ball(View.Settings));
            View.Status = GameStatus.InProgress;

            // broadcast game start to all
            View.Players.ForEach(t => HubContext.Clients.Client(t.ConnectionId).StartGame(View, t.IsPaddleController));

            // game loop until status changed
            while (View.Status == GameStatus.InProgress)
            {
                // ensure players are still present
                if (View.Players.Count == 0)
                {
                    View.Status = GameStatus.Finished;
                    break;
                }

                // make movements of moveable pieces
                View.Players.BallPlayers.ForEach(t => t.Ball.Move());
                View.Players.BallPlayers.ForEach(t => t.Ball.Move());
                View.Players.PaddlePlayers.ForEach(t => t.Paddle.Move());
                View.Players.CheckCollision(View.Settings);

                // goal has been made
                foreach (var player in View.Players.BallPlayers)
                {
                    if (player.Ball.CheckBeyondGoal())
                    {
                        View.Players.ForEach(t => HubContext.Clients.Client(t.ConnectionId).Goal(player));
                        player.Ball.ResetPosition();
                        player.Goals += 1;
                    }
                }

                // sleep based on game speed and rebroadcast pon gmovement
                Thread.Sleep(View.Settings.GameSpeed);
                View.Players.ForEach(t => HubContext.Clients.Client(t.ConnectionId)
                    .Move(new { 
                        Balls = View.Players.BallPlayers.Select(b => new { Name = b.Name, BallX = b.Ball.X, BallY = b.Ball.Y }), 
                        Paddles = View.Players.PaddlePlayers.Select(b => new { Name = b.Name, PaddleX = b.Paddle.X, PaddleWidth = b.Paddle.Width }) 
                    }));
            }

            // portray ending of game
            View.Players.ForEach(t => HubContext.Clients.Client(t.ConnectionId).UpdateGame(View));
            View = new GameOverview();
        }

        public void Reset()
        {
            View.Status = GameStatus.Finished;
        }

        public void Disconnected(string connectionId)
        {
            var playerToRemove = View.Players.FirstOrDefault(t => t.ConnectionId == connectionId);
            if (playerToRemove != null)
            {
                View.Players.Remove(playerToRemove);
            }
        }

        public void Reconnected(string connectionId)
        {
            // todo
        }
    }
}