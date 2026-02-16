using System;

/// <summary>
/// PowerFeature Interface クラス
/// </summary>
public interface IPowerToggleInput
{
    event Action ToggleRequested;
}

/// <summary>
/// PowerFeature Interface クラス
/// </summary>
public interface IPowerStateOutput
{
    void RenderPowerState(bool isOn);
}
