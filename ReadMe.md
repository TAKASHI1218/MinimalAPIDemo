# API_Practice

## 概要
このプロジェクトは、Minimal API を用いた API 実装を学習する目的で作成したものです。
学習講座で提供されたコードをベースに構成を理解しつつ、動作確認や改修を通じてエンドポイント実装の仕組みを深く理解しました。
また、教材コードに対して独自にコメントを追加したり、構成を再整理・補足したりすることで、より体系的に理解できるよう工夫しています。
今後は、応用的な設計改善や機能追加にも取り組み、さらに発展させていく予定です。

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
- 本番環境で使用する Secret は、セキュリティ上の理由から絶対に共有してはいけません。
各環境ごとに独自の Secret を生成し、環境変数などで安全に管理してください。

## 起動手順
1. このリポジトリをクローンする
2. SSMSをインストール
3. appsettings.jsonの"DefaultConnection"の接続文字列を適宜変更
4. パッケージマネージャーコンソールで以下を実行
　add-migration AddCouponToDb
5. パッケージマネージャーコンソールで以下を実行
　update-dababase