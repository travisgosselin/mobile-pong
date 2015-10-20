using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MobilePong.Web.Models
{
    public static class GameCollection
    {
        public static List<Game> Games = new List<Game>();

        public static void StartGame()
        {
            var game = new Game();
            Games.Add(game);


        }
    }
}