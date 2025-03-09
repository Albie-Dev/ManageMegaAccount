namespace MMA.Domain
{
    public enum CFileType
    {
        None = 0,
        Image = 1,
        Zip = 2,
        Word = 3,
        Excel = 4,
        PowerPoint = 5,
        TXT = 6,
        Video = 7
    }

    public enum CFileStatus
    {
        None = 0,
        Active = 1,
        Deleted = 2
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