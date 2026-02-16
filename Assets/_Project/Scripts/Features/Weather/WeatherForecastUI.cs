using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using R3;
using VContainer;

/**
* WeatherForecastUI クラス
* - ViewModel からデータを受け取り、UI に表示するためのロジックを担当する
* - ViewModel の ReactiveProperty<string> を監視して、天気予報のテキストを outputText に反映する
* - 都市選択のボタンを動的に生成し、クリックされたときに ViewModel の SelectCityAsync を呼び出す
*/
public sealed class WeatherForecastUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI outputText;
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private Button buttonPrefab;

    private readonly List<Button> _generatedButtons = new();
    private CancellationTokenSource _cts;
    private bool _isInitialized;
    [Inject] private WeatherForecastViewModel _forecastViewModel;

    private void Start()
    {
        if (outputText == null || buttonContainer == null || buttonPrefab == null || _forecastViewModel == null)
        {
            Debug.LogError("[WeatherForecastUI] UI references or dependencies are not assigned.", this);
            enabled = false;
            return;
        }

        // ViewModel の ReactiveDisplayText を監視して、UI の outputText に反映する
        // ReactiveProperty を Subscribe して、値が変わるたびに outputText.text を更新する
        // * text は、_forecastViewModel.ReactiveDisplayText（ReactiveProperty<string>）から流れてくる現在値/更新値
        // AddTo(this) を呼ぶことで、この MonoBehaviour が破棄されるときに自動的に購読解除される
        _forecastViewModel.ReactiveDisplayText
            .Subscribe(text => outputText.text = text)
            .AddTo(this);
        _forecastViewModel.ReactiveIsLoading
            .Subscribe(isLoading => SetButtonsInteractable(!isLoading))
            .AddTo(this);

        _isInitialized = true;
        EnsureActiveUiLifecycle();
    }

    // 再有効化時を考慮して、UI ライフサイクルを管理するための共通処理
    private void OnEnable()
    {
        // when starting, _isInitialized is false, so skip EnsureActiveUiLifecycle to avoid unnecessary initialization.
        if (!_isInitialized) return;

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
        if (_cts != null)
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
        }

        CleanupButtons();
    }

    private void BuildCityButtons()
    {
        CleanupButtons();

        foreach (var city in _forecastViewModel.Cities)
        {
            var button = Instantiate(buttonPrefab, buttonContainer);
            _generatedButtons.Add(button);

            var label = button.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null) label.text = city.DisplayName;

            var capturedCity = city;
            button.onClick.AddListener(() =>
            {
                // if _cts is null, it means the UI is not active, so ignore the click.
                if (_cts == null) return;
                _forecastViewModel.SelectCityAsync(capturedCity, _cts.Token).Forget();
            });
        }
    }

    private void CleanupButtons()
    {
        foreach (var button in _generatedButtons)
        {
            if (button != null) button.onClick.RemoveAllListeners();
        }
        _generatedButtons.Clear();

        if (buttonContainer == null) return;
        for (var i = buttonContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(buttonContainer.GetChild(i).gameObject);
        }
    }

    private void SetButtonsInteractable(bool interactable)
    {
        foreach (var button in _generatedButtons)
        {
            if (button != null)
            {
                button.interactable = interactable;
            }
        }
    }

}
