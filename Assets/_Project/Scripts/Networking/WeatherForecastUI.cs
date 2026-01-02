using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using R3;

public sealed class WeatherForecastUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI outputText;
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private Button buttonPrefab;

    [Header("Config")]
    [SerializeField] private WeatherForecastConfig config;

    private readonly List<Button> _generatedButtons = new();
    private CancellationTokenSource _cts;
    private WeatherForecastViewModel _viewModel;

    private void Start()
    {
        if (outputText == null || config == null || buttonContainer == null || buttonPrefab == null)
        {
            Debug.LogError("[WeatherForecastUI] UI or config references are not assigned.", this);
            enabled = false;
            return;
        }

        _cts = new CancellationTokenSource();
        // ViewModel の ReactiveProperty を監視して反映する
        // Model → ViewModel → View(UI) の流れ
        // - Model: FetchCityAsync の戻り値（文字列）を返すだけ
        // - ViewModel: DisplayText.Value = ... を更新
        // - View: Subscribe で受け取り outputText.text を更新
        _viewModel = new WeatherForecastViewModel(config);
        _viewModel.DisplayText
            .Subscribe(text => outputText.text = text)
            .AddTo(this);

        BuildCityButtons();
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
        if (_viewModel != null)
        {
            _viewModel.Dispose();
            _viewModel = null;
        }
    }

    private void BuildCityButtons()
    {
        CleanupButtons();

        foreach (var city in _viewModel.Cities)
        {
            var button = Instantiate(buttonPrefab, buttonContainer);
            _generatedButtons.Add(button);

            var label = button.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null) label.text = city.DisplayName;

            var capturedCity = city;
            button.onClick.AddListener(() => _viewModel.SelectCity(capturedCity, _cts.Token).Forget());
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

}
