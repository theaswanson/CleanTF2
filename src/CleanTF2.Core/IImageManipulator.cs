﻿namespace CleanTF2.Core
{
    public interface IImageManipulator
    {
        IImageManipulator Composite(string secondImage);
        Task Save(string saveTo);
        IImageManipulator Resize(int width, int height, bool ignoreAspectRatio = true);
        IImageManipulator WithImage(string path);
    }
}
