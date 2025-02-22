using System.Text.Json.Serialization;

namespace Minesweeper.Models.Response
{
    public class GameInfoResponse
    {
        [JsonPropertyName("game_id")]
        public string GameId { get; set; }  
        public int Width { get; set; }      
        public int Height { get; set; }   
        public int MinesCount { get; set; }   
        public bool Completed { get; set; }  
        public char[][] Field { get; set; }   
    }
}
