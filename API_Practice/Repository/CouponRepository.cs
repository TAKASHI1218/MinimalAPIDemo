using API_Practice.Data;
using API_Practice.Model;
using API_Practice.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace API_Practice.Repository
{
    public class CouponRepository : ICouponRepository
    {
        private readonly ApplicationDbContext _db;

        /// <summary>
        /// コンテキスト
        /// </summary>
        /// <param name="db"> アプリケーションデータへのアクセスおよび管理を行うためのデータベースコンテキスト。</param>
        public CouponRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// クーポン一覧を取得するメソッド。
        /// </summary>
        /// <returns>クーポン一覧</returns>
        public async Task<ICollection<Coupon>> GetAllCouponAsync()
        {
            return await _db.Coupons.ToListAsync();
        }

        /// <summary>
        /// 引数のIdのクーポンを取得します。
        /// </summary>
        /// <param name="id">クーポンId</param>
        /// <returns>指定Idのクーポン</returns>
        public async Task<Coupon> GetCouponByIdAsync(int id)
        {
            return await _db.Coupons.Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        /// <summary>
        /// 引数のクーポン名のクーポンを取得します。
        /// </summary>
        /// <param name="couponName">クーポン名</param>
        /// <returns>指定クーポン名のクーポン</returns>
        public async Task<Coupon> GetCouponByNameAsync(string couponName)
        {
            return await _db.Coupons.Where(x => x.Name.ToLower() == couponName.ToLower()).FirstOrDefaultAsync();
        }

        /// <summary>
        /// クーポンを作成します。
        /// </summary>
        /// <param name="coupon">クーポンオブジェクト</param>
        /// <returns>新規作成のクーポン</returns>
        public async Task CreateCouponAsync(Coupon coupon)
        {
            await _db.Coupons.AddAsync(coupon);
        }

        /// <summary>
        /// クーポン情報を更新します。
        /// </summary>
        /// <param name="coupon">クーポンオブジェクト</param>
        public void UpdateCoupon(Coupon coupon)
        {
            _db.Coupons.Update(coupon);
        }

        /// <summary>
        /// クーポンを削除します。
        /// </summary>
        /// <param name="coupon">クーポンオブジェクト</param>
        public void RemoveCoupon(Coupon coupon)
        {
            _db.Coupons.Remove(coupon);
        }

        /// <summary>
        /// DBを更新します。
        /// </summary>
        /// <returns>DBを更新します。</returns>
        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
