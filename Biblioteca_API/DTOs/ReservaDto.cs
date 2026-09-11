namespace Biblioteca_API.DTOs
{
    public class ReservaDto
    {
        public string UserId { get; set; }
        public DateTime ReservationDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public List<int> BookIds { get; set; } = new List<int>();
    }
}