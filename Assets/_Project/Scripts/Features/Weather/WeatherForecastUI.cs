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
    [Inject] private WeatherForecastViewModel _forecastViewModel;
    private bool _isLoading;

    private void Start()
    {
        if (outputText == null || buttonContainer == null || buttonPrefab == null || _forecastViewModel == null)
        {
            Debug.LogError("[WeatherForecastUI] UI references or dependencies are not assigned.", this);
            enabled = false;
            return;
        }

        _cts = new CancellationTokenSource();

        // ViewModel の ReactiveDisplayText を監視して、UI の outputText に反映する
        // ReactiveProperty を Subscribe して、値が変わるたびに outputText.text を更新する
        // * text は、_forecastViewModel.ReactiveDisplayText（ReactiveProperty<string>）から流れてくる現在値/更新値
        // AddTo(this) を呼ぶことで、この MonoBehaviour が破棄されるときに自動的に購読解除される
        _forecastViewModel.ReactiveDisplayText
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

        foreach (var city in _forecastViewModel.Cities)
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
        if (_forecastViewModel == null || _cts == null || _isLoading) return;

        _isLoading = true;
        SetButtonsInteractable(false);

        // if there is an ongoing request, cancel it before starting a new one.
        CancelAndDisposeRequestCts();
        _requestCts = CancellationTokenSource.CreateLinkedTokenSource(_cts.Token);

        try
        {
            // unitask の await で、キャンセルトークンを渡すことで、リクエストがキャンセルされたときに OperationCanceledException がスローされる
            await _forecastViewModel.SelectCity(city, _requestCts.Token);
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
