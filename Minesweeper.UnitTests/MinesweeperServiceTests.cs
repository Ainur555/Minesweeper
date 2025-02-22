using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Minesweeper.BusinessLogic.Models;
using Minesweeper.BusinessLogic.Services;
using Minesweeper.BusinessLogic.Services.Implementations;
using System.Text.Json;

namespace Minesweeper.UnitTests
{
    public class MinesweeperServiceTests
    {
        private readonly IDistributedCache _cache;
        private readonly MinesweeperService _service;

        public MinesweeperServiceTests()
        {
            var options = Options.Create(new MemoryDistributedCacheOptions());
            _cache = new MemoryDistributedCache(options);
            _service = new MinesweeperService(_cache);
        }

        [Fact]
        public void CreateGame_ShouldReturnValidGame()
        {
            // Arrange
            var newGameDto = new NewGameDto { Width = 5, Height = 5, MinesCount = 5 };

            // Act
            var game = _service.CreateGame(newGameDto);

            // Assert
            Assert.False(string.IsNullOrEmpty(game.GameId));
            Assert.Equal(5, game.Width);
            Assert.Equal(5, game.Height);
            Assert.Equal(5, game.MinesCount);
            Assert.False(game.Completed);
            Assert.Equal(5, game.Field.Length);
            Assert.All(game.Field, row => Assert.Equal(5, row.Length));
     
            foreach (var row in game.Field)
            {
                Assert.All(row, cell => Assert.Equal(' ', cell));
            }
        }
      

        [Fact]
        public void MakeMove_InvalidCoordinates_ShouldThrowException()
        {
            // Arrange
            var newGameDto = new NewGameDto { Width = 3, Height = 3, MinesCount = 1 };
            var game = _service.CreateGame(newGameDto);
            var moveDto = new GameTurnDto { GameId = game.GameId, Row = -1, Col = 0 };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _service.MakeMove(moveDto));
        }

        [Fact]
        public void MakeMove_GameNotFound_ShouldThrowException()
        {
            // Arrange
            var moveDto = new GameTurnDto { GameId = "non-existent", Row = 0, Col = 0 };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _service.MakeMove(moveDto));
        }
      
    }
}