namespace DACN_VILLA.Helper
{
    public static class AddressHelper
    {
        public static string NormalizeAddress(string address)
        {
            // Chuyển tất cả các ký tự thành chữ thường
            address = address.ToLower();

            // Loại bỏ dấu câu và khoảng trắng thừa
            address = address.Replace(",", "").Replace(".", "").Trim();

            return address;
        }
    }
}
