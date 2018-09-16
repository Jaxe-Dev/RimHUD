namespace RimHUD.Data
{
    internal abstract class Option<T>
    {
        public T Default { get; }
        public T Value { get; set; }

        public string Label { get; }
        public string Tooltip { get; }

        protected Option(T @default, string label, string tooltip)
        {
            Default = @default;
            Value = @default;

            Label = label;
            Tooltip = tooltip;
        }
    }
}
