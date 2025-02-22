using FluentValidation;
using Minesweeper.Models.Request;

namespace Minesweeper.Valdiators
{
    public class GameTurnRequestValidator : AbstractValidator<GameTurnRequest>
    {
        public GameTurnRequestValidator() 
        {
            RuleFor(x => x.GameId)
                .NotEmpty()
                .WithMessage("Идентификатор игры не может быть пустым!");

            RuleFor(x => x.Row)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Номер строки не может быть отрицательным!");

            RuleFor(x => x.Col)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Номер колонки не может быть отрицательным!");
        }
    }
}
