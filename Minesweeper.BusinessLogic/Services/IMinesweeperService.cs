using Minesweeper.BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper.BusinessLogic.Services
{
    public interface IMinesweeperService
    {
        GameInfoDto CreateGame(NewGameDto request);
        GameInfoDto MakeMove(GameTurnDto request);
    }
}
