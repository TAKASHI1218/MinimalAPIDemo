using API_Practice.Data;
using API_Practice.Model;
using API_Practice.Model.DTO;
using API_Practice.Repository.IRepository;
using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API_Practice.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private string secretKey;

        /// <summary>
        /// コンテキスト
        /// </summary>
        /// <param name="db"> アプリケーションデータへのアクセスおよび管理を行うためのデータベースコンテキスト。</param>
        /// <param name="mapper"> ドメインモデルと DTO（データ転送オブジェクト）間のマッピングを行うためのオブジェクトマッパー。</param>
        /// <param name="configuration">API シークレットなど、アプリケーション設定を取得するための構成プロバイダー。</param>
        public AuthRepository(ApplicationDbContext db, IMapper mapper,IConfiguration configuration)
        {
            _db = db;
            _mapper = mapper;
            _configuration = configuration;
            secretKey = _configuration.GetValue<string>("ApiSettings:Secret");
        }

        public bool IsUniqueUser(string username)
        {
            var user = _db.LocalUsers.FirstOrDefault(x => x.UserName == username);

            if (user == null) return true;

            return false;
        }

        /// <summary>
        /// 提供されたログイン情報に基づいてユーザーを認証し、認証に成功した場合は JWT トークンを含むLoginResponseDTOを返します。
        /// </summary>
        /// <remarks>
        /// 返される JWT トークンには、ユーザー名とロール（権限）のクレームが含まれ、発行から 7 日間有効です。
        /// </remarks>
        /// <param name="loginRequestDTO">ユーザー名とパスワードを含むログイン情報。</param>
        /// <returns>
        /// 認証に成功した場合は、ユーザー情報と JWT トークンを含む <see cref="LoginResponseDTO"/> を返します。
        /// 認証情報が無効な場合は null を返します。
        /// </returns>
        public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
        {
            var user = _db.LocalUsers.SingleOrDefault(x => x.UserName == loginRequestDTO.UserName
                           && x.Password == loginRequestDTO.Password);

            if (user == null)
            {
                return null;
            }

            // JWT作成のためのハンドラーを準備
            var tokenHandler = new JwtSecurityTokenHandler();
            // Secretをバイト配列に変換
            var key = Encoding.ASCII.GetBytes(secretKey);
            // JWTの中身を設定 - Claims(Name,Role)、期限、署名方式
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name,user.UserName),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            // トークン作成をし、LoginResponseDTOを作成する
            var token = tokenHandler.CreateToken(tokenDescriptor);
            LoginResponseDTO loginResponseDTO = new()
            {
                User = _mapper.Map<UserDTO>(user),
                Token = new JwtSecurityTokenHandler().WriteToken(token)
            };

            return loginResponseDTO;
        }

        /// <summary>
        /// 指定された登録情報を使用して新しいユーザーを登録し、作成されたユーザー情報を返します。
        /// </summary>
        /// <remarks>
        /// 暫定的に登録されたユーザーにはデフォルトで 'admin' ロールが割り当てられます。
        /// セキュリティ上の理由から、返却時にはパスワードはクリアされます。
        /// </remarks>
        /// <param name="requestDTO">ユーザー名、パスワード、名前（表示名）を含むユーザー登録情報。</param>
        /// <returns>
        /// 新しく登録されたユーザーを表す <see cref="UserDTO"/>。
        /// 返却されるオブジェクトではパスワードは空になります。
        /// </returns>
        public UserDTO Register(RegisterationRequestDTO requestDTO)
        {
            LocalUser userObj = new()
            {
                UserName = requestDTO.UserName,
                Password = requestDTO.Password,
                Name = requestDTO.Name,
                Role = "admin"
            };
            _db.LocalUsers.Add(userObj);
            _db.SaveChanges();
            userObj.Password = "";
            return _mapper.Map<UserDTO>(userObj);
        }
    }
}
