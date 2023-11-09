namespace CliSquid
{
    using System.Collections.Generic;

    public interface ISelectPrompt<T>
    {
        ISelectPrompt<T> Title(string title);
        ISelectPrompt<T> Options(IList<PromptOption<T>> options);
        ISelectPrompt<T> Limit(int limit);
        ISelectPrompt<T> Inline();
        T SelectOne();
        IList<T> SelectMany();
    }
}
