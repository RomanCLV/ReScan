﻿namespace ReScanVisualizer.Models
{
    public enum Plan2D
    {
        XY,
        XZ,
        YZ
    }

    public enum Axis
    {
        X,
        Y,
        Z
    }

    public enum RotationAxis
    {
        X,
        Y,
        Z,
        Personalized
    }

    public enum RenderQuality
    {
        VeryLow = -2,
        Low = -1,
        Medium = 0,
        High,
        VeryHigh
    }

    public enum RepetitionMode
    {
        None = 0,
        Translation = 1,
        Rotation = 2
    }
}
