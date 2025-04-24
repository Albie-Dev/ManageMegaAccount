using System.ComponentModel;

namespace MMA.BlazorWasm
{
    public enum CPortalType
    {
        None = 0,
        [Description(description: "https://localhost:7200")]
        Core = 1,
        [Description(description: "http://localhost:7201")]
        CET = 2,
        [Description(description: "https://localhost:7209")]
        Hub = 3,
        [Description(description: "http://localhost:7220")]
        BlazorWasm = 4,
        [Description(description: "http://localhost:7220")]
        Host = 5
    }

    public class DropdownOption
    {
        public string Value { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
    }
}