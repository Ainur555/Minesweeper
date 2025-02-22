using System.Text.Json.Serialization;

namespace Minesweeper.Models.Request
{
    public class GameTurnRequest
    {
        [JsonPropertyName("game_id")]
        public string GameId { get; set; }
        [JsonPropertyName("row")]
        public int Row { get; set; }
        [JsonPropertyName("col")]
        public int Col { get; set; }
     
    }
}
