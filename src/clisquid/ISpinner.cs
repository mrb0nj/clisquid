namespace CliSquid
{
    using System;

    public interface ISpinner<T>
    {
        ISpinner<T> Title(string title);
        ISpinner<T> SetSpinnerType(SpinnerType spinnerType);
        T Spin(Func<ISpinner<T>, T> work);
        void SetStatus(string status);
    }
}
