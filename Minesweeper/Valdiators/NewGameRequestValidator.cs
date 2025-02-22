using FluentValidation;
using Minesweeper.Models.Request;

namespace Minesweeper.Valdiators
{
    public class NewGameRequestValidator : AbstractValidator<NewGameRequest>
    {
        public NewGameRequestValidator()
        {
            RuleFor(x => x.Width)
                .InclusiveBetween(1, 30)
                .WithMessage("Ширина поля должна быть от 1 до 30.");

            RuleFor(x => x.Height)
                .InclusiveBetween(1, 30)
                .WithMessage("Высота поля должна быть от 1 до 30.");

            RuleFor(x => x.MinesCount)
              .GreaterThan(0)
              .WithMessage("Количество мин должно быть больше 0.")
              .LessThan(x => x.Width * x.Height)
              .WithMessage("Количество мин должно быть меньше общего количества ячеек.")
              .Must((request, minesCount) => minesCount <= request.Width * request.Height - 1)
              .WithMessage("Количество мин должно быть не больше, чем (ширина * высота - 1), чтобы оставалась хотя бы одна свободная ячейка.");
        }
    }
}
