using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper.Core
{
    //Добавил в слой кор, в случае, если нужно будет перенести хранение в бд и для работы с ef
    //ну и для реализации чистой архитектуры
    public class Game
    {
        public string GameId { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int MinesCount { get; set; }
        public char[][] Field { get; set; }
        public bool Completed { get; set; }
    }
}
