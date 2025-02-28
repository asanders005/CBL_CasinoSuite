namespace CBL_CasinoSuite.Data.Models
{
    public class LibraryGameModel
    {
        public string GameName { get; set; }

        public string PageName
        {
            get => $"/Games/{GameName}";
        }

        public string Description { get; set; }

        public string ImgSrc { get; set; }

        public LibraryGameModel(string gameName, string description, string imgSrc)
        {
            GameName = gameName;
            Description = description;
            ImgSrc = imgSrc;
        }
    }
}
