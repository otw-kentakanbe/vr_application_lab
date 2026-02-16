using System;

/**
* PowerFeature Interface クラス
*/
public interface IPowerToggleInput
{
    event Action ToggleRequested;
}

/**
* PowerFeature Interface クラス
*/
public interface IPowerStateOutput
{
    void RenderPowerState(bool isOn);
}

