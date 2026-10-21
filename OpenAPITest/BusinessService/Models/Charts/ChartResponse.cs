namespace BusinessService.Models.Charts
{
    /// <summary>
    /// What the Angular app gets back: a Chart.js-friendly shape it can drop straight into the canvas.
    /// </summary>
    public class ChartResponse
    {
        public string Type { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public IReadOnlyList<string> Labels { get; set; } = [];
        public IReadOnlyList<ChartDataset> Datasets { get; set; } = [];
    }

    public class ChartDataset
    {
        public string Label { get; set; } = string.Empty;
        public IReadOnlyList<decimal> Data { get; set; } = [];
    }
}
