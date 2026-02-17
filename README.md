# VR Application Lab
Unity の VR 開発検証用プロジェクト

> [!TIP]
> 実装は `Assets/_Project` 配下に集約

| Start  | Playing |
| ------------- | ------------- |
|<img width="1029" height="539" alt="スクリーンショット 2026-02-16 16 11 13" src="https://github.com/user-attachments/assets/cc25fea2-269c-4ae3-9a51-e0bb3e8599ed" />|<img width="1029" height="538" alt="スクリーンショット 2026-02-16 16 12 04" src="https://github.com/user-attachments/assets/56dff19d-286c-46f2-be16-59d8393db4fc" />|

## Environment
| Item | Description |
|---|---|
| Unity | `6000.3.2f1` |
| Render Pipeline | URP (`17.3.0`) |
| XR | OpenXR (`1.16.1`), XR Interaction Toolkit (`3.3.1`) |

## Architecture & Tech
| Tech | Version | Description |
|---|---|---|
| VContainer | `1.17.0` | 依存性注入（DI）コンテナ。`ProjectLifetimeScope` で Scene コンポーネント / Model / ViewModel を登録 |
| R3 | `1.3.0` | `ReactiveProperty` による状態変化の購読・伝播 |
| UniTask | - | 非同期 API 呼び出し（`UnityWebRequest`） |
| UnityEvent | - | `PowerStateEffectsController` で状態変化時の拡張イベントを Inspector から設定 |
| DOTween | - | ボタン押下時などの簡易アニメーション |
| TextMeshPro | - | UI テキスト表示 |

## MVP (PowerToggle)
| Role | File | Processing |
|---|---|---|
| Presenter | `Assets/_Project/Scripts/Features/Power/PowerStatePresenter.cs` | 入力イベント購読、`AppState` 更新、`IPowerStateOutput.RenderPowerState` 呼び出し、クリック演出トリガー |
| View (Input) | `Assets/_Project/Scripts/Features/Power/PowerToggleInteractor.cs` | XR Hover を受けて `ToggleRequested` を発火 |
| View (Output) | `Assets/_Project/Scripts/Features/Power/PowerStateEffectsController.cs` | 電源状態に応じて `Bloom`/`Vignette` を更新、`UnityEvent` を発火 |
| View (Feedback) | `Assets/_Project/Scripts/Features/Power/PowerToggleClickedView.cs` | `DOTween` でクリック時のジャンプ演出を再生 |
| Interface | `Assets/_Project/Scripts/Features/Power/Interfaces/PowerInterfaces.cs` |  |

## MVVM (WeatherForecast)
| Role | File | Processing |
|---|---|---|
| Model | `Assets/_Project/Scripts/Features/Weather/WeatherForecastModel.cs` | Open-Meteo API 呼び出し、レスポンス整形、都市別キャッシュ（TTL） |
| DTO | `Assets/_Project/Scripts/Features/Weather/Models/Dto/OpenMeteoResponseDto.cs` | Open-Meteo API レスポンスルート定義（internal DTO） |
| DTO | `Assets/_Project/Scripts/Features/Weather/Models/Dto/OpenMeteoHourlyDto.cs` | Open-Meteo API `hourly` フィールド定義（internal DTO） |
| ViewModel | `Assets/_Project/Scripts/Features/Weather/WeatherForecastViewModel.cs` | 選択都市の取得処理、`ReactiveDisplayText`/`ReactiveIsLoading` 更新、キャンセル制御 |
| View | `Assets/_Project/Scripts/Features/Weather/WeatherForecastUI.cs` | 都市ボタン生成、ViewModel の購読、`OnEnable/OnDisable` で購読・キャンセルを明示管理 |
| Config | `Assets/_Project/Scripts/Features/Weather/WeatherForecastConfig.cs` | API URL、TTL 秒数、都市定義（key/name/lat/lon）を保持 |

## Folder Structure
```text
Assets/
  _Project/
    Data/
      Clients/
        Client_A/
          WeatherForecastConfig_Client_A.asset
    Prefabs/
      UI/
        CityButton.prefab
    Scenes/
      Lab.unity
    Scripts/
      Infrastructure/
        DI/
          ProjectLifetimeScope.cs
      Shared/
        State/
          AppState.cs
          AppStateHolder.cs
      Features/
        Power/
          Interfaces/
            PowerInterfaces.cs
          PowerStatePresenter.cs
          PowerToggleInteractor.cs
          PowerStateEffectsController.cs
          PowerToggleClickedView.cs
        Weather/
          Models/
            Dto/
              OpenMeteoResponseDto.cs
              OpenMeteoHourlyDto.cs
          WeatherForecastConfig.cs
          WeatherForecastModel.cs
          WeatherForecastUI.cs
          WeatherForecastViewModel.cs
```
