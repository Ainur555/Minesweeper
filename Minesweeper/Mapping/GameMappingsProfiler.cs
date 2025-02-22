using AutoMapper;
using Minesweeper.BusinessLogic.Models;
using Minesweeper.Models.Request;
using Minesweeper.Models.Response;

namespace Minesweeper.Mapping
{
    public class GameMappingsProfiler : Profile
    {
        public GameMappingsProfiler()
        {
            CreateMap<GameTurnRequest, GameTurnDto>();
            CreateMap<NewGameRequest, NewGameDto>();
            CreateMap<GameInfoDto, GameInfoResponse>();
        }
    }
}
