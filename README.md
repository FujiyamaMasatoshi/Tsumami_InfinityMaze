# TsumamiAdventure

## ゲームの概要

* 時間制限内に3D迷路ゲームをクリアしつづける
* ゴールポイントに到達すると上のステージに進み、下から襲ってくるマグマから逃げ、ポイントを稼ぐゲーム
<image src="src_readme/game_image.png">

### プレイ動画
プレイ動画は以下のURL(YouTube)から視聴可能です。

URL: https://youtu.be/hEtkUWpbypI

### 実行ファイルURL

MacOSに対応した実行ファイルは以下のURLからダウンロードできます。
確認済み動作環境 (M3Pro MacBook Pro, M2 MacBook Air)

URL: https://drive.google.com/drive/folders/1ycmDWViFcNKX285dXzNuINVLluRMfbSa?usp=sharing

## こだわり

* 大規模言語モデルによるアシスタントAI
    
    * 大規模言語モデル(LLM)をゲーム内に取り入れた
    * 本作では、プレイヤの進行法とプレイヤとゴールまでの道のりからゴールに近づいていることなどを教えてくれるプレイヤーアシスタントとして実装

* 迷路攻略アクション性を追加
    * 床に穴をあける
    * コインの設置
    * 3段ジャンプ
    * 時間制限
    を取り入れることでアクション性を追加

## 操作方法
* 移動: WASDキー
* カメラ視点: マウス (or A, Dキーでも自動的にカメラが移動)
* ジャンプ(3段ジャンプ): スペースキー
