using System.ComponentModel;

namespace MMA.BlazorWasm
{
    public enum CPortalType
    {
        None = 0,
        [Description(description: "https://localhost:7200")]
        Core = 1,
        [Description(description: "https://localhost:7201")]
        CET = 2,
        [Description(description: "https://localhost:7209")]
        Hub = 3,
        [Description(description: "https://localhost:7220")]
        BlazorWasm = 4,
        [Description(description: "https://localhost:7220")]
        Host = 5
    }
}