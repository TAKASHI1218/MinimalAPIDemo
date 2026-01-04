using API_Practice.Model.DTO;
using FluentValidation;

namespace API_Practice.Model.Validation
{
    public class CouponUpdateValidation:AbstractValidator<CouponUpdateDTO>
    {
        public CouponUpdateValidation()
        {
            RuleFor(model => model.Id).NotEmpty().GreaterThan(0);
            RuleFor(model => model.Name).NotEmpty();
            RuleFor(model => model.Percent).InclusiveBetween(1, 100);
        }
    }
}
