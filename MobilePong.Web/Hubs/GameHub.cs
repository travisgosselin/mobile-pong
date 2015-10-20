using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.AspNet.SignalR.Hubs;

using MobilePong.Web.Models;

namespace MobilePong.Web.Hubs
{
    public class GameHub : Hub
    {
        private static Game Game = null;

        public void Join(string username, int width,int height, bool isPaddle)
        {
            if (Game == null || Game.View.Status == GameStatus.Finished)
            {
                Game = new Game();
            }

            Game.Join(Context.ConnectionId, username, isPaddle, width, height);
        }

        public void MovePaddle(int xPosition)
        {
            if (Game != null)
            {
                Game.PaddleMove(Context.ConnectionId, xPosition);
            }
        }

        public void PaddleExpand()
        {
            if (Game != null)
            {
                Game.PaddleExpand(Context.ConnectionId);
            }
            
        }

        public void SpeedBurst()
        {
            if (Game != null)
            {
                Game.SpeedBurst(Context.ConnectionId);
            }
        }

        public void MoveBall(bool isLeft)
        {
            if (Game != null)
            {
                Game.MoveBall(Context.ConnectionId, isLeft);
            }
        }

        public void Reset()
        {
            if (Game != null)
            {
                Game.Reset();
            }
        }

        #region - SignalR Events -

        public override Task OnConnected()
        {
            return base.OnConnected();
        }

        public override Task OnDisconnected()
        {
            if (Game != null)
            {
                Game.Disconnected(Context.ConnectionId);
            }

            return base.OnDisconnected();
        }

        public override Task OnReconnected()
        {
            if (Game != null)
            {
                Game.Reconnected(Context.ConnectionId);
            }

            return base.OnReconnected();
        }

        #endregion
    }
}