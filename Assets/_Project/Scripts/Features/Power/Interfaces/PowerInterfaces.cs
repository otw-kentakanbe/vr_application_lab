using System;

/**
* PowerFeature インターフェースクラス
*/
public interface IPowerToggleInput
{
    event Action ToggleRequested;
}

/**
* PowerFeature インターフェースクラス
*/
public interface IPowerStateOutput
{
    void RenderPowerState(bool isOn);
}

