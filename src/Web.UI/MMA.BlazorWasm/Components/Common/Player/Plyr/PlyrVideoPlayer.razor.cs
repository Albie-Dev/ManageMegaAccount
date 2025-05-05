using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MMA.Domain;

namespace MMA.BlazorWasm.Components.Common.Player.Plyr
{
    public partial class PlyrVideoPlayer
    {
        [Parameter] public List<TrackProperty> Tracks { get; set; } = new List<TrackProperty>();
        [Parameter] public string VideoUrl { get; set; } = string.Empty;
        [Parameter] public List<CSubtitleFileFormatType> SubtitleFileFormatTypes { get; set; } = new List<CSubtitleFileFormatType>() { CSubtitleFileFormatType.VTT };
        public string Type { get; set; } = "video/mp4";
        private string _playerPlyrVideoId { get; set; } = $"plyrplayer{Guid.NewGuid()}";
        private string _subtitleUploadInputId { get; set; } = $"subtitleInput{Guid.NewGuid()}";
        private string _parentDivId { get; set; } = $"videoWrapper{Guid.NewGuid()}";
        


        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var allowedExtensions = SubtitleFileFormatTypes
                    .Select(s => $".{s.ToString().ToLower()}").ToArray();

                await _jsRuntime.InvokeVoidAsync("plyrInterop.initializePlayer",
                    _playerPlyrVideoId,
                    _subtitleUploadInputId,
                    _parentDivId,
                    allowedExtensions);
            }
        }

        private async Task UploadSubtitleAsync()
        {
            await Task.CompletedTask;
        }
    }
}