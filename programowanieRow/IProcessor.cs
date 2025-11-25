using System;

namespace programowanieRow
{
    public interface IProcessor
    {
        void StartWork(
            Obliczenia.IFunction func,
            double a,
            double b,
            int n,
            Action<int> progressCallback,
            Action<double, bool> completedCallback
        );

        void CancelAll();
    }
}