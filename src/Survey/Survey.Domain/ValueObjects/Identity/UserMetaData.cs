using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Survey.Domain.ValueObjects.Identity
{
    public record UserMetaData
    {
        [Column("user_agent", TypeName = "nvarchar(300)")]
        public string? UserAgent { get; private set; }
        [MaxLength(20)]
        public string? IP { get; private set; }

        [MaxLength(100)]
        public string? Country { get; private set; }
        [MaxLength(10)]
        [Column("country_code")]
        public string? CountryCode { get; private set; }
        [MaxLength(100)]
        public string? City { get; private set; }
        [MaxLength(100)]
        [Column("time_zone")]
        public string? TimeZone { get; private set; }
        [Precision(8, 6)]
        [Column("location_lat")]
        public decimal? LocationLat { get; private set; }
        [Precision(9, 6)]
        [Column("location_lng")]
        public decimal? LocationLng { get; private set; }

        protected UserMetaData() { }
        private UserMetaData(string? userAgent, string? ip, string? country, string? countryCode, string? city, string? timezone, decimal? locationLat, decimal? locationLng)
        {
            UserAgent = userAgent;
            IP = ip;
            Country = country;
            CountryCode = countryCode;
            City = city;
            TimeZone = timezone;
            LocationLat = locationLat;
            LocationLng = locationLng;
        }

        public static UserMetaData Of(string? userAgent, string? ip, string? country, string? countryCode, string? city, string? timezone, decimal? locationLat, decimal? locationLng)
        {
            return new UserMetaData(userAgent, ip, country, countryCode, city, timezone, locationLat, locationLng);
        }
    }
}
