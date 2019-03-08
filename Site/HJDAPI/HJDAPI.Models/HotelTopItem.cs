namespace HJDAPI.Models
{
    public class HotelTopItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ReviewCount { get; set; }
        public string CommentContent { get; set; }
        public int CommentRate { get; set; }
        public string PicUrl { get; set; }
        public int Rank { get; set; }
        public int Score { get; set; }
    }
}