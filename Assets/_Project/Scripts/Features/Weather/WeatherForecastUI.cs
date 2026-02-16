using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

/// <summary>
/// WeatherForecastUI クラス
/// - ViewModel からデータを受け取り、UI に表示するためのロジックを担当する
/// - ViewModel の ReactiveProperty<string> を監視して、天気予報のテキストを _outputText に反映する
/// - 都市選択のボタンを動的に生成し、クリックされたときに ViewModel の SelectCityAsync を呼び出す
/// </summary>
public sealed class WeatherForecastUI : MonoBehaviour
{
    private const string LogPrefix = "[WeatherForecastUI]";

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI _outputText;
    [SerializeField] private Transform _buttonContainer;
    [SerializeField] private Button _buttonPrefab;

    private readonly List<Button> _generatedButtons = new();
    private CancellationTokenSource _cts;
    private bool _isInitialized;
    private IDisposable _displayTextSubscription;
    private IDisposable _isLoadingSubscription;
    [Inject] private WeatherForecastViewModel _forecastViewModel;

    private void Start()
    {
        if (!ValidateDependencies()) return;

        _isInitialized = true;
        BindViewModel();
        InitializeUiLifecycle();
    }

    // 再有効化時を考慮して、UI ライフサイクルを管理するための共通処理
    private void OnEnable()
    {
        // when starting, _isInitialized is false, so skip EnsureActiveUiLifecycle to avoid unnecessary initialization.
        if (!_isInitialized) return;

        BindViewModel();
        EnsureActiveUiLifecycle();
    }

    // _cts 再生成、ボタン再構築、isLoading に応じた interactable 同期を行う共通処理
    private void EnsureActiveUiLifecycle()
    {
        _cts ??= new CancellationTokenSource();
        BuildCityButtons();
        // prevent from clicking buttons while loading data.
        SetButtonsInteractable(!_forecastViewModel.ReactiveIsLoading.Value);
    }

    private void OnDisable()
    {
        UnbindViewModel();
        CancelAndDisposeCts();
        CleanupButtons();
    }

    private void BuildCityButtons()
    {
        CleanupButtons();

        foreach (var city in _forecastViewModel.Cities)
        {
            var button = Instantiate(_buttonPrefab, _buttonContainer);
            _generatedButtons.Add(button);

            var capturedCity = city;
            button.onClick.AddListener(() =>
            {
                // if _cts is null, it means the UI is not active, so ignore the click.
                if (_cts == null) return;
                _forecastViewModel.SelectCityAsync(capturedCity, _cts.Token).Forget();
            });

            var label = button.GetComponentInChildren<TextMeshProUGUI>();
            if (label == null) continue;

            label.text = city.DisplayName;
        }
    }

    private void CleanupButtons()
    {
        foreach (var button in _generatedButtons)
        {
            if (button == null) continue;

            button.onClick.RemoveAllListeners();
        }
        _generatedButtons.Clear();

        if (_buttonContainer == null) return;
        for (var i = _buttonContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(_buttonContainer.GetChild(i).gameObject);
        }
    }

    private void SetButtonsInteractable(bool interactable)
    {
        foreach (var button in _generatedButtons)
        {
            if (button == null) continue;

            button.interactable = interactable;
        }
    }

    private bool ValidateDependencies()
    {
        if (_outputText == null || _buttonContainer == null || _buttonPrefab == null || _forecastViewModel == null)
        {
            Debug.LogError($"{LogPrefix} UI references or dependencies are not assigned.", this);
            enabled = false;
            return false;
        }

        return true;
    }

    private void BindViewModel()
    {
        if (_displayTextSubscription == null)
        {
            _displayTextSubscription = _forecastViewModel.ReactiveDisplayText
                .Subscribe(latestText => _outputText.text = latestText);
        }

        if (_isLoadingSubscription == null)
        {
            _isLoadingSubscription = _forecastViewModel.ReactiveIsLoading
                .Subscribe(isLoading => SetButtonsInteractable(!isLoading));
        }
    }

    private void InitializeUiLifecycle()
    {
        EnsureActiveUiLifecycle();
    }

    private void CancelAndDisposeCts()
    {
        if (_cts == null) return;

        _cts.Cancel();
        _cts.Dispose();
        _cts = null;
    }

    private void UnbindViewModel()
    {
        _displayTextSubscription?.Dispose();
        _displayTextSubscription = null;

        _isLoadingSubscription?.Dispose();
        _isLoadingSubscription = null;
    }

}
