using API_Practice.Model.DTO;
using FluentValidation;

namespace API_Practice.Model.Validation
{
    public class CouponCreateValidation:AbstractValidator<CouponCreateDTO>
    {
        public CouponCreateValidation()
        {
            RuleFor(model => model.Name).NotEmpty();
            RuleFor(model => model.Percent).InclusiveBetween(1, 100);
        }
    }
}
