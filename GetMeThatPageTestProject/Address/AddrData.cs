namespace GetMeThatPageTestProject.Address
{
    internal class AddrData
    {
        public static readonly string[] relativePaths = new string[]
           {
                "./subfolder/file.txt",
                "../parent_folder/image.jpg",
                "./folder/../file.txt",
                "../../other_folder/document.docx",
                "../subfolder/another_subfolder/",
                "../file.txt",
                "./subfolder/../image.png",
                "./../folder/file.txt",
                "./subfolder/../other_folder/data.csv",
                "./../../file.txt",
                "./../example_folder/example_file.txt",
                "./../images/pic.jpg",
                "./../data/data.csv",
                "./../documents/report.docx",
                "./../backup/backup.zip",
                "./../media/video.mp4",
                "./../project/code.py",
                "./../downloads/file.pdf",
                "./../music/song.mp3",
                "./../archives/archive.tar.gz",
                "././file.txt",
                ".././../file.txt",
                "/../../file.txt",
                "./.././../file.txt",
                ".././file.txt"

           };
    }
}
