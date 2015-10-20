using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MobilePong.Web.Models
{
    public enum GameStatus
    {
        WaitingForPlayers,
        InProgress,
        Paused,
        Finished
    }
}