using API_Practice.Model;

namespace API_Practice.Repository.IRepository
{
    public interface ICouponRepository
    {
        Task<ICollection<Coupon>> GetAllCouponAsync();

        Task<Coupon> GetCouponByIdAsync(int id);

        Task<Coupon> GetCouponByNameAsync(string couponName);

        Task CreateAsync(Coupon coupon);

        void UpdateCoupon(Coupon coupon);

        void RemoveCoupon(Coupon coupon);

        Task SaveAsync();
    }
}
