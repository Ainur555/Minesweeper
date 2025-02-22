using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper.DataAccess.Models
{
    public class GameTurnModel
    {
        public string GameId { get; set; }    
        public int Row { get; set; }      
        public int Col { get; set; }        
    }
}
