namespace CliSquid
{
    using System;

    public interface IUserInputPrompt<T>
    {
        IUserInputPrompt<T> Title(string title);
        IUserInputPrompt<T> Placeholder(string placeholder);
        IUserInputPrompt<T> InitialValue(string initialValue);
        IUserInputPrompt<T> Validate(Func<string, Tuple<bool, string>> validator);
        T Read();
    }
}
