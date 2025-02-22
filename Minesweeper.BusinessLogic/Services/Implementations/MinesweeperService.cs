using Microsoft.Extensions.Caching.Distributed;
using Minesweeper.BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Minesweeper.BusinessLogic.Services.Implementations
{
    public class MinesweeperService(IDistributedCache _distributedCache) : IMinesweeperService
    {
        private readonly Dictionary<string, GameData> _games = new();
        private const string KeyPrefix = "minesweeper:game:";

        public GameInfoDto CreateGame(NewGameDto request)
        {
            var gameId = Guid.NewGuid().ToString();
            var mines = GenerateMines(request.Width, request.Height, request.MinesCount);
            var field = InitializeField(request.Width, request.Height);

            var gameData = new GameData
            {
                GameId = gameId,
                Width = request.Width,
                Height = request.Height,
                MinesCount = request.MinesCount,
                Mines = mines,
                Field = field,
                Completed = false
            };

            SaveGame(gameData);
            return ToGameInfoDto(gameData);
        }

        public GameInfoDto MakeMove(GameTurnDto request)
        {
            var gameData = GetGame(request.GameId);
            if (gameData == null)
                throw new ArgumentException("Game not found.");

            if (gameData.Completed)
                throw new InvalidOperationException("Game is already completed.");

            if (request.Row < 0 || request.Row >= gameData.Height || request.Col < 0 || request.Col >= gameData.Width)
                throw new ArgumentException("Invalid cell coordinates.");

            if (gameData.Field[request.Row][request.Col] != ' ')
                throw new InvalidOperationException("Cell is already opened.");

            if (gameData.Mines.Any(m => m.Row == request.Row && m.Col == request.Col))
            {
                gameData.Field[request.Row][request.Col] = 'X';
                gameData.Completed = true;
                RevealAllMines(gameData);
            }
            else
            {
                OpenCell(gameData, request.Row, request.Col);
                if (CheckWin(gameData))
                {
                    gameData.Completed = true;
                    MarkAllMines(gameData);
                }
            }

            SaveGame(gameData);
            return ToGameInfoDto(gameData);
        }

        private string GetGameKey(string gameId) => KeyPrefix + gameId;

        private void SaveGame(GameData game)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
            };

            var json = JsonSerializer.Serialize(game);
            _distributedCache.SetString(GetGameKey(game.GameId), json, options);
        }

        private GameData? GetGame(string gameId)
        {
            var json = _distributedCache.GetString(GetGameKey(gameId));
            if (string.IsNullOrEmpty(json))
                return null;
            return JsonSerializer.Deserialize<GameData>(json);
        }

        private List<MinePosition> GenerateMines(int width, int height, int minesCount)
        {
            var mines = new List<MinePosition>();
            var random = new Random();
            while (mines.Count < minesCount)
            {
                var row = random.Next(height);
                var col = random.Next(width);
        
                if (!mines.Any(m => m.Row == row && m.Col == col))
                {
                    mines.Add(new MinePosition { Row = row, Col = col });
                }
            }
            return mines;
        }

        private char[][] InitializeField(int width, int height)
        {
            return Enumerable.Range(0, height)
                .Select(_ => Enumerable.Repeat(' ', width).ToArray())
                .ToArray();
        }

        private void OpenCell(GameData game, int row, int col)
        {
            if (row < 0 || row >= game.Height || col < 0 || col >= game.Width || game.Field[row][col] != ' ')
                return;

            int minesCount = CountAdjacentMines(game, row, col);
            game.Field[row][col] = minesCount == 0 ? '0' : (char)('0' + minesCount);

            if (minesCount == 0)
            {
                for (var i = -1; i <= 1; i++)
                {
                    for (var j = -1; j <= 1; j++)
                    {
                        if (i != 0 || j != 0)
                            OpenCell(game, row + i, col + j);
                    }
                }
            }
        }

        private int CountAdjacentMines(GameData game, int row, int col)
        {
            int count = 0;
            for (var i = -1; i <= 1; i++)
            {
                for (var j = -1; j <= 1; j++)
                {
                    int newRow = row + i;
                    int newCol = col + j;
                    if (newRow >= 0 && newRow < game.Height && newCol >= 0 && newCol < game.Width)
                    {
                        if (game.Mines.Any(m => m.Row == newRow && m.Col == newCol) &&
                            !(newRow == row && newCol == col))
                            count++;
                    }
                }
            }
            return count;
        }

        private bool CheckWin(GameData game)
        {
            int openedCells = game.Field.Sum(row => row.Count(cell => cell != ' '));
            return openedCells == game.Width * game.Height - game.MinesCount;
        }

        private void RevealAllMines(GameData game)
        {
            foreach (var mine in game.Mines)
            {
                game.Field[mine.Row][mine.Col] = 'X';
            }
        }

        private void MarkAllMines(GameData game)
        {
            foreach (var mine in game.Mines)
            {
                game.Field[mine.Row][mine.Col] = 'M';
            }
        }

        private GameInfoDto ToGameInfoDto(GameData game)
        {
            return new GameInfoDto
            {
                GameId = game.GameId,
                Width = game.Width,
                Height = game.Height,
                MinesCount = game.MinesCount,
                Field = game.Field,
                Completed = game.Completed
            };
        }
    }  
}


