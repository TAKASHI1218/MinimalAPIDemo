using API_Practice.Model;
using API_Practice.Model.DTO;
using API_Practice.Repository.IRepository;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace API_Practice.Endpoints
{
    public static class CouponEndpoints
    {
        /// <summary>
        /// アプリケーション内でクーポンを管理するための API エンドポイントを構成します。
        /// </summary>
        /// <remarks>
        /// このメソッドは、クーポンの一覧取得、個別取得、作成、更新、削除といった
        /// 各種エンドポイントを登録します。
        /// アプリケーション起動時に呼び出すことで、クーポン関連の API ルートが有効になります。
        /// </remarks>
        /// <param name="app">クーポンエンドポイントを追加する対象の <see cref="WebApplication"/> インスタンス。</param>
        public static void ConfigureCouponEndpoints(this WebApplication app)
        {
            // -- クーポン一覧を取得 -- //
            app.MapGet("api/coupon", GetAllCoupon)
                .WithName("GetCouponList").Produces<APIResponse>(200);

            // -- Idを指定してクーポンを取得 -- //
            app.MapGet("api/coupon/{id:int}", GetCouponById)
                .WithName("GetCouponById").Produces<APIResponse>(200);

            // -- クーポン作成 -- //　
            // 作成に成功した場合は"201"作成できなかった場合は"400"を返す
            // CouponCreateDTOを受け取りCouponDTOを返す
            app.MapPost("api/coupon", CreateCoupon)
                .WithName("CreateCoupon").Accepts<CouponCreateDTO>("application/json").Produces<APIResponse>(201).Produces(400);

            // -- クーポン更新 -- //　
            app.MapPut("api/coupon", UpdateCoupon)
                .WithName("UpdateCoupon").Accepts<CouponUpdateDTO>("application/json").Produces<APIResponse>(200).Produces(400);

            // -- クーポンの削除 -- //　
            app.MapDelete("api/coupon/{id:int}", DeleteCoupon)
                .WithName("DeleteCoupon").Produces<APIResponse>(204).Produces(400);
        }

        /// <summary>
        /// データストアに保存されているすべてのクーポンを取得し、HTTP 200（OK）レスポンスとして返します。
        /// </summary>
        /// <param name="_couponRepo">クーポンデータへアクセスするためのリポジトリ。</param>
        /// <param name="_logger">操作ログを記録するためのロガーインスタンス。</param>
        /// <returns>
        /// 非同期操作を表すタスク。タスクの結果は、すべてのクーポン一覧を含む API レスポンスをHTTP 200（OK）として返します。
        /// </returns>
        private async static Task<IResult> GetAllCoupon(ICouponRepository _couponRepo, ILogger<Program> _logger)
        {
            APIResponse response = new();
            response.Result = await _couponRepo.GetAllCouponAsync();
            response.IsSuucess = true;
            response.StatusCode = HttpStatusCode.OK;
            return Results.Ok(response);
        }

        /// <summary>
        /// 指定された一意の ID を持つクーポンを取得し、その結果を HTTP レスポンスとして返します。
        /// </summary>
        /// <param name="id">取得対象となるクーポンの一意の ID。存在する有効なクーポン ID である必要があります。</param>
        /// <param name="_couponRepo">クーポンデータへアクセスするためのリポジトリ。</param>
        /// <param name="_logger">操作ログを記録するためのロガーインスタンス。</param>
        /// <returns>
        /// 非同期操作を表すタスク。タスクの結果には、クーポンが見つかった場合はそのデータを含む HTTP レスポンスが返されます。
        /// </returns>
        private async static Task<IResult> GetCouponById(int id, ICouponRepository _couponRepo, ILogger<Program> _logger)
        {
            _logger.Log(LogLevel.Information, "Idからクーポン取得");
            APIResponse response = new();
            response.Result = await _couponRepo.GetCouponByIdAsync(id);
            response.IsSuucess = true;
            response.StatusCode = HttpStatusCode.OK;
            return Results.Ok(response);
        }

        /// <summary>
        /// 指定されたデータに基づいて新しいクーポンを作成し、その処理結果を返します。
        /// </summary>
        /// <remarks>
        /// クーポン名が既に存在する場合、または入力データがバリデーションに失敗した場合は、
        /// エラー情報を含む BadRequest レスポンスを返します。
        /// 正常に作成できた場合は、作成されたクーポンの詳細をレスポンスとして返します。
        /// </remarks>
        /// <param name="coupon_C_DTO">作成するクーポンの情報を保持する DTO。すべてのバリデーション要件を満たす必要があります。</param>
        /// <param name="_couponRepo">クーポンデータへアクセスするためのリポジトリ。</param>
        /// <param name="_validator">クーポンデータが必要な条件を満たしているか検証するためのバリデータ。</param>
        /// <param name="_mapper">DTO とドメインモデル間の変換を行うためのマッパー。</param>
        /// <param name="_logger">操作ログを記録するためのロガーインスタンス。</param>
        /// <returns>
        /// 作成に成功した場合は、作成されたクーポンデータを含む API レスポンスを返します。
        /// 失敗した場合は、エラー内容と適切なステータスコードを含むレスポンスを返します。
        /// </returns>
        private async static Task<IResult> CreateCoupon([FromBody] CouponCreateDTO coupon_C_DTO,
                                                        ICouponRepository _couponRepo,
                                                        IValidator<CouponCreateDTO> _validator,
                                                        IMapper _mapper,
                                                        ILogger<Program> _logger)
        {
            _logger.Log(LogLevel.Information, "クーポン作成");

            APIResponse response = new() { IsSuucess = false, StatusCode = HttpStatusCode.BadRequest };

            // FluentValidationによるバリデーション処理
            var validationResult = await _validator.ValidateAsync(coupon_C_DTO);
            if (!validationResult.IsValid)
            {
                response.ErrorMessage.Add(validationResult.Errors.FirstOrDefault().ToString());
                return Results.BadRequest(response);
            }

            // 独自実装によるバリデーション処理
            if (await _couponRepo.GetCouponByNameAsync(coupon_C_DTO.Name) != null)
            {
                response.ErrorMessage.Add("すでに使用されているクーポン名です");
                return Results.BadRequest(response);
            }

            // 1. CouponにCouponCreateDTOをマップ、作成日登録
            Coupon coupon = _mapper.Map<Coupon>(coupon_C_DTO);
            coupon.Created = DateTime.Now;

            // 2. Couponに追加
            await _couponRepo.CreateCouponAsync(coupon);
            await _couponRepo.SaveAsync();

            // 3. 外部公開用のデータ表示のためCoponDTOにCouponをマップ
            CouponDTO couponDTO = _mapper.Map<CouponDTO>(coupon);

            // 201を返しGetCouponByIdルートよりデータ取得可能
            response.Result = couponDTO;
            response.IsSuucess = true;
            response.StatusCode = HttpStatusCode.OK;
            return Results.Ok(response);

            // 参考：過去の処理
            // return Results.CreatedAtRoute("GetCouponById", new { coupon.Id }, couponDTO);
            // return Results.Created($"/api/coupon/{coupon.Id}",coupon);
        }

        /// <summary>
        /// 指定されたデータを使用して既存のクーポンを更新し、その処理結果を返します。
        /// </summary>
        /// <remarks>
        /// 更新処理の前に入力データのバリデーションを行います。
        /// バリデーションに失敗した場合は、エラー内容を含む BadRequest レスポンスを返します。
        /// このメソッドは ASP.NET Core の Minimal API エンドポイントで使用されることを想定しています。
        /// </remarks>
        /// <param name="coupon_U_DTO">更新後のクーポン情報を保持する DTO。すべてのバリデーションルールを満たす必要があります。</param>
        /// <param name="_couponRepo">クーポンデータへアクセスするためのリポジトリ。</param>
        /// <param name="_validator">クーポンデータが必要な条件を満たしているか検証するためのバリデータ。</param>
        /// <param name="_mapper">DTO とドメインモデル間の変換を行うためのマッパー。</param>
        /// <param name="_logger">操作ログを記録するためのロガーインスタンス。</param>
        /// <returns>
        /// 更新に成功した場合は更新後のクーポンデータを含む成功レスポンスを返し、
        /// 失敗した場合はエラー内容を含む BadRequest レスポンスを返します。
        /// </returns>
        private async static Task<IResult> UpdateCoupon([FromBody] CouponUpdateDTO coupon_U_DTO,
                                                       ICouponRepository _couponRepo,
                                                       IValidator<CouponUpdateDTO> _validator,
                                                       IMapper _mapper,
                                                       ILogger<Program> _logger)
        {
            _logger.Log(LogLevel.Information, "クーポン更新");

            APIResponse response = new() { IsSuucess = false, StatusCode = HttpStatusCode.BadRequest };

            // FluentValidationによるバリデーション処理
            var validationResult = await _validator.ValidateAsync(coupon_U_DTO);
            if (!validationResult.IsValid)
            {
                response.ErrorMessage.Add(validationResult.Errors.FirstOrDefault().ToString());
                return Results.BadRequest(response);
            }

            Coupon coupon = _mapper.Map<Coupon>(coupon_U_DTO);
            coupon.LastUpdated = DateTime.Now;
            _couponRepo.UpdateCoupon(coupon);
            await _couponRepo.SaveAsync();

            response.Result = _mapper.Map<CouponDTO>(await _couponRepo.GetCouponByIdAsync(coupon_U_DTO.Id));
            response.IsSuucess = true;
            response.StatusCode = HttpStatusCode.OK;
            return Results.Ok(response);
        }

        /// <summary>
        /// 指定された ID を持つクーポンをデータストアから削除します。
        /// </summary>
        /// <param name="id">削除対象となるクーポンの一意の ID。</param>
        /// <param name="_couponRepo">クーポンデータへアクセスするためのリポジトリ。</param>
        /// <param name="_logger">操作ログを記録するためのロガーインスタンス。</param>
        /// <returns>
        /// 削除処理の結果を含む API レスポンス。クーポンが削除された場合は 204（No Content）を返し、
        /// 指定されたクーポンが存在しない場合は 400（Bad Request）を返します。
        /// </returns>
        private async static Task<IResult> DeleteCoupon(int id, ICouponRepository _couponRepo, ILogger<Program> _logger)
        {
            _logger.Log(LogLevel.Information, "クーポン削除");

            APIResponse response = new() { IsSuucess = false, StatusCode = HttpStatusCode.BadRequest };

            Coupon counponFromStore = await _couponRepo.GetCouponByIdAsync(id);
            if (counponFromStore != null)
            {
                _couponRepo.RemoveCoupon(counponFromStore);
                await _couponRepo.SaveAsync();
                response.IsSuucess = true;
                response.StatusCode = HttpStatusCode.NoContent;
                return Results.Ok(response);
            }
            else
            {
                response.ErrorMessage.Add("存在しないクーポンです");
                return Results.BadRequest(response);
            }
        }
    }
}
