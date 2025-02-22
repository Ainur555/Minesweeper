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
    /// <summary>
    /// Класс-сервис, реализующий основную логику игры
    /// </summary>
    /// <param name="_distributedCache"></param>
    public class MinesweeperService(IDistributedCache _distributedCache) : IMinesweeperService
    {
        private readonly Dictionary<string, GameData> _games = new();
        private const string KeyPrefix = "minesweeper:game:";

        /// <summary>
        /// Логика создания игры
        /// </summary>
        /// <param name="request">NewGameDto</param>
        /// <returns></returns>
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

        /// <summary>
        /// Обработка хода игрока
        /// </summary>
        /// <param name="request">GameTurnDto</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
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

        /// <summary>
        /// Сохранение игры в кэш
        /// </summary>
        /// <param name="game">GameData</param>
        private void SaveGame(GameData game)
        {
            _distributedCache.SetStringAsync(
                key: GetGameKey(game.GameId),
                value: JsonSerializer.Serialize(game),
                options: new DistributedCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromHours(1)
                });
        }

        /// <summary>
        /// Получение игры из кэша
        /// </summary>
        /// <param name="gameId">string</param>
        /// <returns>GameData</returns>
        private GameData? GetGame(string gameId)
        {
            var json = _distributedCache.GetString(GetGameKey(gameId));

            if (string.IsNullOrEmpty(json))
            {
                return null;
            }

            return JsonSerializer.Deserialize<GameData>(json);
        }

        /// <summary>
        /// Генерация мин на поле
        /// </summary>
        /// <param name="width">int</param>
        /// <param name="height">int</param>
        /// <param name="minesCount">int</param>
        /// <returns>List<MinePosition>int</returns>
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

        /// <summary>
        ///  Инициализирует игровое поле заданного размера.
        /// </summary>
        /// <param name="width">int</param>
        /// <param name="height">int</param>
        /// <returns>char[][]</returns>
        private char[][] InitializeField(int width, int height)
        {
            return Enumerable.Range(0, height)
                .Select(_ => Enumerable.Repeat(' ', width).ToArray())
                .ToArray();
        }

        /// <summary>
        /// Рекурсивно открывает ячейку на игровом поле
        /// </summary>
        /// <param name="game">GameData</param>
        /// <param name="row">int</param>
        /// <param name="col">int</param>
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

        /// <summary>
        /// Подсчитывает количество мин, расположенных вокруг заданной ячейки.
        /// </summary>
        /// <param name="game">GameData</param>
        /// <param name="row">int</param>
        /// <param name="col">int</param>
        /// <returns>int</returns>
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

        /// <summary>
        /// Проверка на то, выиграл ли игрок
        /// </summary>
        /// <param name="game">GameData </param>
        /// <returns>bool</returns>
        private bool CheckWin(GameData game)
        {
            int openedCells = game.Field.Sum(row => row.Count(cell => cell != ' '));
            return openedCells == game.Width * game.Height - game.MinesCount;
        }

        /// <summary>
        /// Раскрывает все мины на игровом поле, заменяя символ ячейки на 'X'.
        /// </summary>
        /// <param name="game">GameData</param>
        private void RevealAllMines(GameData game)
        {
            foreach (var mine in game.Mines)
            {
                game.Field[mine.Row][mine.Col] = 'X';
            }
        }

        /// <summary>
        /// Отмечает все мины на игровом поле символом 'M'.
        /// </summary>
        /// <param name="game">GameData</param>
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


