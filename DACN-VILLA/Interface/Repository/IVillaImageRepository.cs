using DACN_VILLA.Model;

namespace DACN_VILLA.Interface.Repository
{
    public interface IVillaImageRepository
    {
        Task AddAsync(VillaImage villaImage);  // Phương thức để thêm ảnh vào cơ sở dữ liệu
        Task<VillaImage> GetByIdAsync(Guid id);  // Phương thức để lấy ảnh theo ID
        Task<List<VillaImage>> GetByVillaIdAsync(Guid villaId);  // Lấy danh sách ảnh theo VillaId
    }
}
