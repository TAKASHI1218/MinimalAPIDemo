using API_Practice.Model;
using API_Practice.Model.DTO;
using API_Practice.Repository.IRepository;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace API_Practice.Endpoints
{
    public static class AuthEndpoints
    {
        /// <summary>
        /// アプリケーション内の認証処理のための API エンドポイントを構成します。
        /// </summary>
        /// <remarks>
        /// このメソッドは、クーポンの一覧取得、個別取得、作成、更新、削除といった
        /// 各種エンドポイントを登録します。
        /// アプリケーション起動時に呼び出すことで、認証菅れ関連の API ルートが有効になります。
        /// </remarks>
        /// <param name="app">クーポンエンドポイントを追加する対象の <see cref="WebApplication"/> インスタンス。</param>
        public static void ConfigureAuthEndpoints(this WebApplication app)
        {
            // ログイン
            app.MapPost("api/login", Login)
                .WithName("Login").Accepts<LoginRequestDTO>("application/json").Produces<LoginResponseDTO>(200).Produces(400);
            
            // ユーザー登録
            app.MapPost("api/regist", Register)
                .WithName("Regist").Accepts<RegisterationRequestDTO>("application/json").Produces<UserDTO>(200).Produces(400);
        }

        /// <summary>
        /// 提供されたログイン情報を使用してユーザー認証を試みます。
        /// </summary>
        /// <param name="model">ユーザーの認証情報を含むログインリクエストデータ。</param>
        /// <param name="_authRepo">ユーザーの認証情報を検証するための認証リポジトリ。</param>
        /// <param name="_logger">ログイン試行および関連情報を記録するためのロガー。</param>
        /// <returns>
        /// 認証処理の結果を表す <see cref="IResult"/>。
        /// 認証に成功した場合はユーザー情報を含む成功レスポンスを返し、失敗した場合はエラーメッセージを含む BadRequest レスポンスを返します。
        /// </returns>
        private async static Task<IResult> Login([FromBody] LoginRequestDTO model,
                                            IAuthRepository _authRepo,
                                            ILogger<Program> _logger)
        {
            _logger.Log(LogLevel.Information, "ログイン");

            APIResponse response = new() { IsSuucess = false, StatusCode = HttpStatusCode.BadRequest };
            var loginResponse = _authRepo.Login(model);

            if (loginResponse == null)
            {
                response.ErrorMessage.Add("登録されていないユーザーです");
                return Results.BadRequest(response);
            }

            response.Result = loginResponse;
            response.IsSuucess = true;
            response.StatusCode = HttpStatusCode.OK;
            return Results.Ok(response);
        }

        /// <summary>
        /// 指定された登録情報を使用して新しいユーザーアカウントを登録します。
        /// </summary>
        /// <remarks>
        /// ユーザー名が既に登録されている場合、または必須の登録項目が不足している場合は、
        /// BadRequest を返し、失敗理由を示すエラーメッセージを含みます。
        /// </remarks>
        /// <param name="model">新規ユーザーの登録データ。</param>
        /// <param name="_authRepo">ユーザー名の重複確認およびアカウント作成を行うための認証リポジトリ。</param>
        /// <param name="_logger">登録処理に関するイベントや情報を記録するためのロガー。</param>
        /// <returns>
        /// 登録処理の結果を表す <see cref="IResult"/>。
        /// 登録に成功した場合はユーザー情報を含む成功レスポンスを返し、
        /// 失敗した場合はエラー情報を含む BadRequest レスポンスを返します。
        /// </returns>
        private async static Task<IResult> Register([FromBody] RegisterationRequestDTO model,
                                                    IAuthRepository _authRepo,
                                                    ILogger<Program> _logger)
        {
            _logger.Log(LogLevel.Information, "ユーザ登録");

            APIResponse response = new() { IsSuucess = false, StatusCode = HttpStatusCode.BadRequest };

            bool ifUserNameIsUnique = _authRepo.IsUniqueUser(model.UserName);

            if (!ifUserNameIsUnique)
            {
                response.ErrorMessage.Add("すでに登録済みのユーザー名です");
                return Results.BadRequest(response);
            }

            var registerResponse = _authRepo.Register(model);
            if(registerResponse == null || string.IsNullOrEmpty(registerResponse.UserName))
            {
                response.ErrorMessage.Add("ユーザー名、その他の項目が入力されていません");
                return Results.BadRequest(response);
            }

            response.Result = registerResponse;
            response.IsSuucess = true;
            response.StatusCode = HttpStatusCode.OK;
            return Results.Ok(response);
        }
    }
}
