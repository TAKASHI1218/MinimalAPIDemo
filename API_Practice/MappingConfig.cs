using AutoMapper;
using API_Practice.Model;
using API_Practice.Model.DTO;

namespace API_Practice
{
    public class MappingConfig:Profile
    {
        public MappingConfig()
        {
            CreateMap<Coupon, CouponCreateDTO>().ReverseMap();
            CreateMap<Coupon, CouponDTO>().ReverseMap();
            CreateMap<Coupon, CouponUpdateDTO>().ReverseMap();
        }
    }
}
