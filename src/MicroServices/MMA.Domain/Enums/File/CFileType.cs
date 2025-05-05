using System.ComponentModel;

namespace MMA.Domain
{
    public enum CFileType
    {
        None = 0,
        [Description("https://cdn-icons-png.flaticon.com/512/8343/8343149.png")]
        Image = 1,
        [Description("https://cdn-icons-png.freepik.com/512/3979/3979411.png")]
        Zip = 2,
        [Description("https://png.pngtree.com/png-clipart/20190516/original/pngtree-microsoft-word-logo-icon-png-image_3588805.jpg")]
        Word = 3,
        [Description("https://png.pngtree.com/png-clipart/20190516/original/pngtree-microsoft-excel-logo-icon-png-image_3588803.jpg")]
        Excel = 4,
        [Description("https://images.freeimages.com/fic/images/icons/2795/office_2013_hd/2000/powerpoint.png")]
        PowerPoint = 5,
        [Description("https://cdn.shopify.com/extensions/88b763b5-1c45-40a6-94f4-a2d13490e552/seamless-file-uploads-147/assets/txt_icon.png")]
        TXT = 6,
        [Description("https://getdrawings.com/free-icon/avi-icon-56.png")]
        Video = 7,
        [Description("https://cdn-icons-png.flaticon.com/512/8422/8422322.png")]
        Database = 8,
        [Description("https://cdn-icons-png.freepik.com/512/4722/4722923.png")]
        Audio = 9,
        [Description("https://static.vecteezy.com/system/resources/previews/022/086/609/non_2x/file-type-icons-format-and-extension-of-documents-pdf-icon-free-vector.jpg")]
        PDF = 10,
        [Description("https://cdn-icons-png.freepik.com/512/8422/8422139.png")]
        Html = 11,
        [Description("https://static-00.iconduck.com/assets.00/application-x-msdownload-icon-1577x2048-tann6c6m.png")]
        MicrosoftDownload = 12,
        [Description("https://images.icon-icons.com/3642/PNG/512/otherfile_filetype_icon_227885.png")]
        Other = 13
    }

    public enum CFileStatus
    {
        None = 0,
        Active = 1,
        Deleted = 2,
    }

    public enum CNodeType
    {
        File = 1,
        Directory = 2,
        Root = 3,
        Inbox = 4,
        Trash = 5
    }
}