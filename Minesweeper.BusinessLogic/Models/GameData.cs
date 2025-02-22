using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper.BusinessLogic.Models
{
    internal class GameData
    {
        public string GameId { get; set; } = null!;
        public int Width { get; set; }
        public int Height { get; set; }
        public int MinesCount { get; set; }
        public List<MinePosition> Mines { get; set; } = new();
        public char[][] Field { get; set; } = null!;
        public bool Completed { get; set; }
    }
}
