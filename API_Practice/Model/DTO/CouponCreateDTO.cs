namespace API_Practice.Model.DTO
{
    public class CouponCreateDTO
    {
        public string Name { get; set; }

        public int Percent { get; set; }

        public bool IsActive { get; set; }
    }
}
