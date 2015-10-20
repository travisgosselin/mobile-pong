using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MobilePong.Web.Models
{
    public class Player
    {
        public string ConnectionId { get; set; }
        public string Name { get; set; }
        public bool IsPaddleController { get; set; }
        public int ScreenWidth { get; set; }
        public int ScreenHeight { get; set; }
        public int Goals { get; set; }
        public Ball Ball { get; set; }
        public Paddle Paddle { get; set; }
    }
}