using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MobilePong.Web.Models
{
    public class GameOverview
    {
        public GameSettings Settings { get; set; }
        public GameStatus Status { get; set; }
        public PlayerCollection Players { get; set; }

        public GameOverview(GameSettings settings = null)
        {
            if (settings == null)
            {
                settings = new GameSettings();
            }

            Settings = settings;
            Players = new PlayerCollection();
            Status = GameStatus.WaitingForPlayers;
        }
    }
}