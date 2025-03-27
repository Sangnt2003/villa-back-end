namespace DACN_VILLA.DTO.Request
{
    public class UserUpdateRequest
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public string PictureUrl { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
    }
}