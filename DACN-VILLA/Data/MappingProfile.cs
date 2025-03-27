using AutoMapper;
using DACN_VILLA.DTO.Request;
using DACN_VILLA.DTO.Respone;
using DACN_VILLA.DTO.Response;
using DACN_VILLA.DTO;
using DACN_VILLA.Model;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Location mappings
        CreateMap<Location, LocationCreateRequest>().ReverseMap();
        

        CreateMap<Location, LocationUpdateRequest>().ReverseMap();
        CreateMap<Location, LocationResponse>().ReverseMap();
        CreateMap<Wishlist, WishlistCreateRequest>().ReverseMap();
        CreateMap<Wishlist, WishlistResponse>()
        .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Villa != null ? src.Villa.Name : string.Empty))
        .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Villa != null ? src.Villa.Description : string.Empty))
        .ForMember(dest => dest.PricePerNight, opt => opt.MapFrom(src => src.Villa != null ? src.Villa.PricePerNight : 0))
        .ForMember(dest => dest.ListedPrice, opt => opt.MapFrom(src => src.Villa != null ? src.Villa.ListedPrice : 0))
        .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.Villa != null && src.Villa.VillaImages != null
            ? src.Villa.VillaImages.Select(img => img.ImageUrl).ToList()
            : new List<string>()))
        .ForMember(dest => dest.Capacity, opt => opt.MapFrom(src => src.Villa != null ? src.Villa.Capacity : 0))
        .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Villa != null ? src.Villa.Rating : 0))
        .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Villa != null ? src.Villa.Address : string.Empty))
        .ForMember(dest => dest.VillaServices, opt => opt.MapFrom(src => src.Villa != null && src.Villa.VillaServices != null
            ? src.Villa.VillaServices.Select(vs => vs.Service.Name).ToList()
            : new List<string>()));

        CreateMap<Villa, VillaResponse>()
            .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.VillaImages.Select(img => img.ImageUrl).ToList()))
            .ForMember(dest => dest.VillaServices, opt => opt.MapFrom(src => src.VillaServices != null
                                                                      ? src.VillaServices.Select(vs => vs.Service.Name).ToList()
                                                                      : new List<string>()))
            .ForMember(dest => dest.LocationId, opt => opt.MapFrom(src => src.Location.Id))
            .ForMember(dest => dest.Reviews, opt => opt.MapFrom(src => src.Reviews))
            .ForMember(dest => dest.AvailableFrom, opt => opt.MapFrom(src => DateTime.Now))
            .ForMember(dest => dest.AvailableTo, opt => opt.MapFrom(src => DateTime.Now.AddDays(30)));
        CreateMap<NotificationRequest, Notification>()
    .ForMember(dest => dest.Id, opt => opt.Ignore()) // Bỏ qua tạo ID trong AutoMapper
    .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()); // Bỏ qua tạo CreatedAt trong AutoMapper
        CreateMap<Notification, NotificationResponse>();
        // Booking mappings
        CreateMap<Booking, BookingResponse>()
            .ForMember(dest => dest.BookingId, opt => opt.MapFrom(src => src.Id)) // Mapping Id -> BookingId
            .ForMember(dest => dest.VillaName, opt => opt.MapFrom(src => src.Villa.Name)) // Mapping Villa.Name
            .ForMember(dest => dest.VillaLocation, opt => opt.MapFrom(src => src.Villa.Location.Name)) // Mapping Villa.Location
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FullName)) // Mapping User.Name
            .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User.Email))
            .ForMember(dest => dest.VillaId, opt => opt.MapFrom(src => src.Villa.Id));
        CreateMap<Villa, VillaResponse>()
            .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.VillaImages.Select(img => img.ImageUrl).ToList()))
            .ForMember(dest => dest.VillaServices, opt => opt.MapFrom(src => src.VillaServices != null
                                                                      ? src.VillaServices.Select(vs => vs.Service.Name).ToList()
                                                                      : new List<string>()))
            .ForMember(dest => dest.LocationId, opt => opt.MapFrom(src => src.Location.Id))
            .ForMember(dest => dest.AvailableFrom, opt => opt.MapFrom(src => DateTime.Now))
            .ForMember(dest => dest.AvailableTo, opt => opt.MapFrom(src => DateTime.Now.AddDays(30)));

        CreateMap<VillaCreateRequest, Villa>()
            .ForMember(dest => dest.VillaImages, opt => opt.Ignore())
            .ForMember(dest => dest.VillaServices, opt => opt.Ignore())
            .ForMember(dest => dest.Bookings, opt => opt.Ignore())
            .ForMember(dest => dest.Reviews, opt => opt.Ignore())
            .ForMember(dest => dest.Wishlists, opt => opt.Ignore());
        // BookingProcess mappings
        CreateMap<BookingProcess, BookingProcessResponse>()
            .ForMember(dest => dest.Note, opt => opt.MapFrom(src => src.Note))
            .ForMember(dest => dest.ApprovalStatus, opt => opt.MapFrom(src => src.ApprovalStatus))
            .ForMember(dest => dest.ProcessedAt, opt => opt.MapFrom(src => src.ProcessedAt));

        // Discount mappings
        CreateMap<Discount, DiscountResponse>().ReverseMap();

        // Service mappings
        CreateMap<Services, ServiceResponse>().ReverseMap();
        CreateMap<VillaServices, VillaServiceResponse>().ReverseMap();

        // Review mappings
        CreateMap<Review, ReviewDTO>().ReverseMap();

        // User mappings
        CreateMap<User, UserCreateRequest>().ReverseMap();
        CreateMap<User, UserResponse>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));
    }
}
