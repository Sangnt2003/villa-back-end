using DACN_VILLA.DTO.Response;

namespace DACN_VILLA.DTO.Respone
{
    public class UserListResponse
    {
        public List<UserResponse> Users { get; set; }
        public int TotalPages { get; set; }
    }
}
