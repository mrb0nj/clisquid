namespace CliSquid
{
    public class PromptOption<T>
    {
        private readonly T _promptValue;
        private string _display = "";
        public string Display
        {
            get { return string.IsNullOrWhiteSpace(_display) ? _promptValue.ToString() : _display; }
            set { _display = value; }
        }
        public string Hint { get; set; } = "";
        public bool IsDefault { get; set; }

        public T Value
        {
            get { return _promptValue; }
        }

        public PromptOption(T promptValue)
        {
            _promptValue = promptValue;
        }
    }
}
