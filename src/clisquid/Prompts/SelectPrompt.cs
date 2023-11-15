namespace CliSquid.Prompts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CliSquid.Enums;
    using CliSquid.Interfaces;
    using Pastel;

    public class SelectPrompt<T> : ISelectPrompt<T>
    {
        private string _title = "";
        private IList<PromptOption<T>>? _options;
        private bool _optionsInline;
        private int _limit;
        private Configuration _configuration;

        public SelectPrompt()
        {
            _configuration = new Configuration();
        }

        public SelectPrompt(Configuration configuration)
        {
            _configuration = configuration;
        }

        public ISelectPrompt<T> Inline()
        {
            _optionsInline = true;
            return this;
        }

        public ISelectPrompt<T> Limit(int limit)
        {
            _limit = limit;
            return this;
        }

        public ISelectPrompt<T> Options(IList<PromptOption<T>> options)
        {
            _options = options;
            return this;
        }

        public IList<T> SelectMany()
        {
            if (_options == null || !_options.Any())
                throw new Exception("No options provided");

            Action<
                string,
                IList<PromptOption<T>>,
                IList<PromptOption<T>>,
                PromptOption<T>?,
                PromptStatus
            > render = (
                string prompt,
                IList<PromptOption<T>> options,
                IList<PromptOption<T>> selected,
                PromptOption<T>? active,
                PromptStatus status
            ) =>
            {
                Console.CursorVisible = false;
                Prompt.WriteGutter(Prompt.GetGutterPrompt(status));
                Prompt.WriteText(prompt.Pastel(_configuration.Theme.SelectForeground));

                if (status == PromptStatus.Complete)
                {
                    Prompt.WriteGutter(Prompt.GetGutterBar(status));
                    var selectedOptions = String.Join(
                        ", ",
                        selected.Select(s => s.Display).ToArray<string>()
                    );
                    var display = selectedOptions.PadRight(
                        Console.WindowWidth - selectedOptions.Length - Prompt.GUTTER_PAD_RIGHT
                    );
                    Prompt.WriteText(display.Pastel(_configuration.Theme.SelectResultForeground));
                    for (var i = 0; i < options.Count; i++)
                        Prompt.WriteText("".PadRight(Console.WindowWidth));
                    var pos = CursorPosition.GetCursorPosition();
                    Console.SetCursorPosition(0, pos.Top - options.Count);
                }
                else
                {
                    if (_optionsInline)
                        Prompt.WriteGutter(Prompt.GetGutterBar(status));
                    foreach (var option in options)
                    {
                        if (!_optionsInline)
                            Prompt.WriteGutter(Prompt.GetGutterBar(status));

                        var isActive = option == active;
                        var isSelected = selected.Contains(option);
                        var marker = isSelected
                            ? Prompt
                                .CHECK_SELECTED
                                .Pastel(_configuration.Theme.SelectOptionSelected)
                            : Prompt
                                .CHECK_INACTIVE
                                .Pastel(
                                    isActive
                                        ? _configuration.Theme.SelectOptionActive
                                        : _configuration.Theme.SelectOptionInactive
                                );

                        var display = option
                            .Display
                            .Pastel(
                                isActive
                                    ? _configuration.Theme.SelectOptionTextActive
                                    : _configuration.Theme.SelectOptionTextInactive
                            );
                        if (!string.IsNullOrWhiteSpace(option.Hint) && isActive && !_optionsInline)
                            display = string.Format(
                                "{0} {1}",
                                display,
                                option.Hint.Pastel(_configuration.Theme.SelectOptionHint)
                            );

                        if (!_optionsInline)
                            display = display.PadRight(
                                Console.WindowWidth - display.Length - Prompt.GUTTER_PAD_RIGHT - 3
                            );
                        Prompt.WriteText(
                            string.Format(" {0} {1}", marker, display),
                            !_optionsInline
                        );
                    }

                    if (_optionsInline)
                    {
                        var pos = CursorPosition.GetCursorPosition();
                        Prompt.WriteText(string.Empty.PadRight(Console.WindowWidth - pos.Left));
                    }
                }

                var gutterEnd = Prompt.GetGutterEnd(status);
                var gutterEndNote = Prompt.GetGutterEndNote(status, string.Empty);
                gutterEndNote = gutterEndNote.PadRight(
                    Console.WindowWidth - gutterEndNote.Length - Prompt.GUTTER_PAD_RIGHT
                );
                gutterEnd = string.Format("{0}  {1}", gutterEnd, gutterEndNote);
                Prompt.WriteGutter(gutterEnd, newLine: true);
            };

            Action<
                string,
                IList<PromptOption<T>>,
                IList<PromptOption<T>>,
                PromptOption<T>?,
                PromptStatus
            > reRender = (
                string prompt,
                IList<PromptOption<T>> options,
                IList<PromptOption<T>> selected,
                PromptOption<T>? active,
                PromptStatus status
            ) =>
            {
                var pos = CursorPosition.GetCursorPosition();
                var topOffset = _optionsInline ? 3 : _options.Count + 2;
                Console.SetCursorPosition(0, pos.Top - topOffset);
                render(prompt, options, selected, active, status);
            };

            var activeItem = _options.First();
            IList<PromptOption<T>> selectedItems = new List<PromptOption<T>>(
                _options.Where(o => o.IsDefault)
            );
            render(_title, _options, selectedItems, activeItem, PromptStatus.Active);

            Console.Out.Flush();
            ConsoleKeyInfo cki = default;

            do
            {
                Console.TreatControlCAsInput = true;
                cki = Console.ReadKey(true);
                Console.TreatControlCAsInput = false;
                var idx = _options.IndexOf(activeItem);
                switch (cki.Key)
                {
                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.H:
                        if (!_optionsInline)
                            break;
                        if (idx == 0)
                            continue;
                        activeItem = _options[idx - 1];
                        break;
                    case ConsoleKey.RightArrow:
                    case ConsoleKey.L:
                        if (!_optionsInline)
                            break;
                        if (idx + 1 > _options.Count() - 1)
                            continue;
                        activeItem = _options[idx + 1];
                        break;
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.K:
                        if (_optionsInline)
                            break;
                        if (idx == 0)
                            continue;
                        activeItem = _options[idx - 1];
                        break;
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.J:
                        if (_optionsInline)
                            break;
                        if (idx + 1 > _options.Count() - 1)
                            continue;
                        activeItem = _options[idx + 1];
                        break;
                    case ConsoleKey.Spacebar:
                        if (selectedItems.Contains(activeItem))
                            selectedItems.Remove(activeItem);
                        else
                            selectedItems.Add(activeItem);
                        break;
                    case ConsoleKey.C:
                        if ((cki.Modifiers & ConsoleModifiers.Control) != 0)
                            Prompt.CancelToken();
                        break;
                    case ConsoleKey.Escape:
                        Prompt.CancelToken();
                        break;
                }

                if (Prompt.Token.IsCancellationRequested)
                {
                    var pos = CursorPosition.GetCursorPosition();
                    var topOffset = _optionsInline ? 3 : _options.Count + 2;
                    Console.SetCursorPosition(0, pos.Top - topOffset);
                    Console.CursorVisible = true;
                    Prompt.ExitGracefully(_title);
                }

                reRender(_title, _options, selectedItems, activeItem, PromptStatus.Active);
            } while (cki.Key != ConsoleKey.Enter || !selectedItems.Any());

            reRender(_title, _options, selectedItems, null, PromptStatus.Complete);
            Console.CursorVisible = true;

            return selectedItems.Select(p => p.Value).ToList();
        }

        public T SelectOne()
        {
            if (_options == null || !_options.Any())
                throw new Exception("No options provided");

            Action<
                string,
                IList<PromptOption<T>>,
                PromptOption<T>?,
                PromptOption<T>?,
                PromptStatus
            > render = (
                string prompt,
                IList<PromptOption<T>> options,
                PromptOption<T>? selected,
                PromptOption<T>? active,
                PromptStatus status
            ) =>
            {
                Console.CursorVisible = false;
                Prompt.WriteGutter(Prompt.GetGutterPrompt(status));
                Prompt.WriteText(prompt.Pastel(_configuration.Theme.SelectForeground));

                if (status == PromptStatus.Complete)
                {
                    Prompt.WriteGutter(Prompt.GetGutterBar(status));
                    if (selected != null)
                    {
                        var display = selected
                            .Display
                            .PadRight(
                                Console.WindowWidth
                                    - selected.Display.Length
                                    - Prompt.GUTTER_PAD_RIGHT
                            );
                        Prompt.WriteText(
                            display.Pastel(_configuration.Theme.SelectResultForeground)
                        );
                    }
                }
                else
                {
                    if (_optionsInline)
                        Prompt.WriteGutter(Prompt.GetGutterBar(status));

                    foreach (var option in options)
                    {
                        if (!_optionsInline)
                            Prompt.WriteGutter(Prompt.GetGutterBar(status));

                        var isActive = option == active;
                        var marker = (
                            isActive ? Prompt.RADIO_ACTIVE : Prompt.RADIO_INACTIVE
                        ).Pastel(
                            isActive
                                ? _configuration.Theme.SelectOptionActive
                                : _configuration.Theme.SelectOptionInactive
                        );
                        var display = option
                            .Display
                            .Pastel(
                                isActive
                                    ? _configuration.Theme.SelectOptionTextActive
                                    : _configuration.Theme.SelectOptionTextInactive
                            );
                        if (!string.IsNullOrWhiteSpace(option.Hint) && isActive && !_optionsInline)
                            display = string.Format(
                                "{0} {1}",
                                display,
                                option.Hint.Pastel(_configuration.Theme.SelectOptionHint)
                            );

                        if (!_optionsInline)
                            display = display.PadRight(
                                Console.WindowWidth - display.Length - Prompt.GUTTER_PAD_RIGHT - 3
                            );
                        Prompt.WriteText(
                            string.Format(" {0} {1}", marker, display),
                            !_optionsInline
                        );
                    }

                    if (_optionsInline)
                        Prompt.WriteText(string.Empty);
                }

                var gutterEnd = Prompt.GetGutterEnd(status);
                var gutterEndNote = Prompt.GetGutterEndNote(status, string.Empty);
                gutterEndNote = gutterEndNote.PadRight(
                    Console.WindowWidth - gutterEndNote.Length - Prompt.GUTTER_PAD_RIGHT
                );
                gutterEnd = string.Format("{0}  {1}", gutterEnd, gutterEndNote);
                Prompt.WriteGutter(gutterEnd, newLine: true);
            };

            Action<
                string,
                IList<PromptOption<T>>,
                PromptOption<T>?,
                PromptOption<T>?,
                PromptStatus
            > reRender = (
                string prompt,
                IList<PromptOption<T>> options,
                PromptOption<T>? selected,
                PromptOption<T>? active,
                PromptStatus status
            ) =>
            {
                var pos = CursorPosition.GetCursorPosition();

                Console.SetCursorPosition(0, pos.Top - (_optionsInline ? 1 : 2) - _options.Count);

                render(prompt, options, selected, active, status);
            };

            var activeItem = _options.First();
            PromptOption<T>? selectedItem = null;
            render(_title, _options, selectedItem, activeItem, PromptStatus.Active);

            Console.Out.Flush();

            ConsoleKeyInfo cki = default;

            do
            {
                Console.TreatControlCAsInput = true;
                cki = Console.ReadKey(true);
                Console.TreatControlCAsInput = false;
                var idx = _options.IndexOf(activeItem);
                switch (cki.Key)
                {
                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.H:
                        if (!_optionsInline)
                            break;
                        if (idx == 0)
                            continue;
                        activeItem = _options[idx - 1];
                        break;
                    case ConsoleKey.RightArrow:
                    case ConsoleKey.L:
                        if (!_optionsInline)
                            break;
                        if (idx + 1 > _options.Count() - 1)
                            continue;
                        activeItem = _options[idx + 1];
                        break;
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.K:
                        if (_optionsInline)
                            break;
                        if (idx == 0)
                            continue;
                        activeItem = _options[idx - 1];
                        break;
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.J:
                        if (_optionsInline)
                            break;
                        if (idx + 1 > _options.Count() - 1)
                            continue;
                        activeItem = _options[idx + 1];
                        break;
                    case ConsoleKey.Spacebar:
                    case ConsoleKey.Enter:
                        selectedItem = activeItem;
                        break;
                    case ConsoleKey.C:
                        if ((cki.Modifiers & ConsoleModifiers.Control) != 0)
                            Prompt.CancelToken();
                        break;
                    case ConsoleKey.Escape:
                        Prompt.CancelToken();
                        break;
                }

                if (Prompt.Token.IsCancellationRequested)
                {
                    var pos = CursorPosition.GetCursorPosition();
                    var topOffset = _optionsInline ? 3 : _options.Count + 2;
                    Console.SetCursorPosition(0, pos.Top - topOffset);
                    Console.CursorVisible = true;
                    Prompt.ExitGracefully(_title);
                }

                reRender(_title, _options, selectedItem, activeItem, PromptStatus.Active);
            } while (cki.Key != ConsoleKey.Enter || selectedItem == null);

            Console.CursorVisible = true;
            if (Prompt.Token.IsCancellationRequested)
            {
                var pos = CursorPosition.GetCursorPosition();
                var topOffset = _optionsInline ? 3 : _options.Count + 2;
                Console.SetCursorPosition(0, pos.Top - topOffset);
                Prompt.ExitGracefully(_title);
            }

            reRender(_title, _options, selectedItem, null, PromptStatus.Complete);

            return selectedItem.Value;
        }

        ISelectPrompt<T> ISelectPrompt<T>.Title(string title)
        {
            _title = title;
            return this;
        }
    }
}
