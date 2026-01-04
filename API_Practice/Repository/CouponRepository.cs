using API_Practice.Data;
using API_Practice.Model;
using API_Practice.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace API_Practice.Repository
{
    public class CouponRepository : ICouponRepository
    {
        private readonly ApplicationDbContext _db;

        public CouponRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<ICollection<Coupon>> GetAllCouponAsync()
        {
            return await _db.Coupons.ToListAsync();
        }

        public async Task<Coupon> GetCouponByIdAsync(int id)
        {
            return await _db.Coupons.Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Coupon> GetCouponByNameAsync(string couponName)
        {
            return await _db.Coupons.Where(x => x.Name.ToLower() == couponName.ToLower()).FirstOrDefaultAsync();
        }

        public async Task CreateCouponAsync(Coupon coupon)
        {
            await _db.Coupons.AddAsync(coupon);
        }

        public void UpdateCoupon(Coupon coupon)
        {
            _db.Coupons.Update(coupon);
        }

        public void RemoveCoupon(Coupon coupon)
        {
            _db.Coupons.Remove(coupon);
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
