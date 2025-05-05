using System.ComponentModel;

namespace MMA.Domain
{
    public enum CSubtitleFileFormatType
    {
        None = 0,
        [Description(".vtt - WebVTT")]
        VTT = 1,

        [Description(".srt - SubRip")]
        SRT = 2,

        [Description(".ass - Advanced SubStation Alpha")]
        ASS = 3,

        [Description(".ssa - SubStation Alpha")]
        SSA = 4,

        [Description(".sub - MicroDVD/SubViewer")]
        SUB = 5,

        [Description(".idx - Index file")]
        IDX = 6,

        [Description(".sbv - YouTube SubViewer")]
        SBV = 7,

        [Description(".lrc - Lyrics")]
        LRC = 8,

        [Description(".txt - Plain Text")]
        TXT = 9,

        [Description(".xml - XML-based subtitles")]
        XML = 10,

        [Description(".stl - EBU Subtitles")]
        STL = 11,

        [Description(".dfxp - Distribution Format Exchange Profile")]
        DFXP = 12,

        [Description(".ttml - Timed Text Markup Language")]
        TTML = 13,

        [Description(".cap - Captions from broadcast")]
        CAP = 14,

        [Description(".smil - Multimedia Integration Format")]
        SMIL = 15
    }
}