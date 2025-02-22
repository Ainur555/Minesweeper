using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper.DataAccess.Models
{
    public class GameInfoModel
    {
        public string GameId { get; set; }    
        public int Width { get; set; }     
        public int Height { get; set; }      
        public int MinesCount { get; set; }  
        public bool Completed { get; set; } 
        public char[][] Field { get; set; }  
    }
}
