# API_Practice

## 概要
このプロジェクトは、**Minimal API を用いた API 実装の基礎を学習すること**を目的として作成したものです。  
学習講座で提供されたコードをベースに、構成の理解・動作確認・改修を行いながら、Minimal API によるエンドポイント実装の流れを把握しました。

主に以下の機能を実装しています。

### 1. クーポン管理 API
- 登録済みクーポンの取得（GET）
- 新規クーポンの作成（POST）
- 既存クーポンの更新（PUT）
- クーポンの削除（DELETE）

### 2. 認証関連 API
- ユーザー登録のエンドポイント実装
- 登録済みユーザーのログイン処理
- ログイン時のトークン発行処理（JWT）

## MinimalAPIについて
Minimal API は、最小限のコードで高速な HTTP Web API を構築できるシンプルな開発手法です。
従来のようにコントローラーやスキャフォールディングを用意する必要がなく、Program.cs などに直接ルートとハンドラを定義できます。
そのため、Python や Node.js の軽量フレームワークに近い感覚で、素早く RESTful なエンドポイントを実装できるのが特徴です。

## 使用技術
- **フレームワーク**：.NET 10.0  
- **言語**：C# 14 
- **データベース**：SSMS（System.Data.SqlServer）  

## インストールしたパッケージ
1. Swashbuckle.AspNetCore(10.1.0)
2. AutoMapper, AutoMapper.Extensions.Microsoft.DependencyInjiction(12.0.0)
3. FluentValidation(12.1.1),FluentValidation.DependencyInjectionExtensions(12.1.1)
4. Microsoft.EntityFrameworkCore.SqlServer(10.0.1)
5. Microsoft.EntityFrameworkCore.Tools(10.0.1)

## SecretKey について
- Secret には、JWT の署名に使用する 32〜64 文字程度のランダムな英数字＋記号を設定してください。
現在の値は学習・動作確認用のテスト値です。
- 本番環境で使用する Secret は、セキュリティ上の理由から共有してはいけません。
各環境ごとに独自の Secret を生成し、環境変数などで安全に管理してください。

## 起動手順
1. このリポジトリをクローンする
2. SSMSをインストール
3. appsettings.jsonの"DefaultConnection"の接続文字列を適宜変更
4. パッケージマネージャーコンソールで以下を実行
　add-migration AddMinimalAPITablesToDb
5. パッケージマネージャーコンソールで以下を実行
　update-database

## アプリについて
### AuthEndpoints
- POST:/api/regist → ユーザー登録をすると「LocalUsers」テーブルに登録されます
- POST:/api/login  → 「LocalUsers」に登録されたユーザーの場合ログインすることができ認証キーが発行されます
### CouponEndpoints
- GET:/api/coupon  → 「Coupons」テーブルに登録されているクーポン一覧が表示されます
- POST:/api/coupon → 「Coupons」テーブルにクーポンを登録します
- PUT:/api/coupon  → 「Coupons」テーブルに登録されているクーポンの情報を更新します
- GET:/api/coupon/{id} → パラメータのidのクーポンが表示されます
- DELETE:/api/coupon/{id} → パラメータのidのクーポンが削除されます