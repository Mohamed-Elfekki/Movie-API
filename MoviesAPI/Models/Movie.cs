namespace MoviesAPI.Models
{
    public class Movie
    {
        public int Id { get; set; }

        [MaxLength(250)]
        public string Title { get; set; }

        public int Year { get; set; }

        public double Rate { get; set; }

        [MaxLength(2500)]
        public string StoreLine { get; set; }

        public byte[] Poster { get; set; }


        //1:M
        public byte GenreId { get; set; }
        public Genre Genre { get; set; }
    }
}
