﻿namespace DACN_VILLA.DTO.Request
{
    public class LocationUpdateRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
    }
}
