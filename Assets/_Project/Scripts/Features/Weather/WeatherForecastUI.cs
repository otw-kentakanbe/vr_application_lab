using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using R3;
using VContainer;

public sealed class WeatherForecastUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI outputText;
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private Button buttonPrefab;

    private readonly List<Button> _generatedButtons = new();
    private CancellationTokenSource _cts;
    private CancellationTokenSource _requestCts;
    [Inject] private WeatherForecastViewModel _viewModel;
    private bool _isLoading;

    private void Start()
    {
        if (outputText == null || buttonContainer == null || buttonPrefab == null || _viewModel == null)
        {
            Debug.LogError("[WeatherForecastUI] UI references or dependencies are not assigned.", this);
            enabled = false;
            return;
        }

        _cts = new CancellationTokenSource();
        // ViewModel の ReactiveProperty を監視して反映する
        // Model → ViewModel → View(UI) の流れ
        // - Model: FetchCityAsync の戻り値（文字列）を返すだけ
        // - ViewModel: DisplayText.Value = ... を更新
        // - View: Subscribe で受け取り outputText.text を更新
        _viewModel.DisplayText
            .Subscribe(text => outputText.text = text)
            .AddTo(this);

        BuildCityButtons();
    }

    private void OnDisable()
    {
        CancelAndDisposeRequestCts();

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

        foreach (var city in _viewModel.Cities)
        {
            var button = Instantiate(buttonPrefab, buttonContainer);
            _generatedButtons.Add(button);

            var label = button.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null) label.text = city.DisplayName;

            var capturedCity = city;
            button.onClick.AddListener(() => SelectCityAsync(capturedCity).Forget());
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

    private async UniTaskVoid SelectCityAsync(WeatherForecastConfig.CityConfig city)
    {
        if (_viewModel == null || _cts == null || _isLoading) return;

        _isLoading = true;
        SetButtonsInteractable(false);
        CancelAndDisposeRequestCts();
        _requestCts = CancellationTokenSource.CreateLinkedTokenSource(_cts.Token);

        try
        {
            await _viewModel.SelectCity(city, _requestCts.Token);
        }
        catch (System.OperationCanceledException)
        {
            // Ignore cancellation caused by lifecycle stop (OnDisable).
        }
        finally
        {
            _isLoading = false;
            CancelAndDisposeRequestCts();
            SetButtonsInteractable(true);
        }
    }

    private void CancelAndDisposeRequestCts()
    {
        _requestCts?.Cancel();
        _requestCts?.Dispose();
        _requestCts = null;
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
