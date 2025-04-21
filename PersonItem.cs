using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonApi_sync_REST
{
    /// <summary>
    ///  Клас моделі представляє дані, якими керує наша програма.
    /// </summary>
    [Table("Persons")]
    [PrimaryKey(nameof(Unzr))]
    public class PersonItem
    {

        public enum GenderEnum
        {
            male,
            female
        }

        [Base64String, MaxLength(30), JsonProperty(propertyName: nameof(Name), Required = Required.Always)]
        public string Name { set; get; } = string.Empty;

        [Base64String, MaxLength(50), JsonProperty(propertyName: nameof(Surname), Required = Required.Always)]
        public string Surname { set; get; } = string.Empty;

        [Base64String, MaxLength(50), JsonProperty(propertyName: nameof(Patronym))]
        public string? Patronym { set; get; }

        [DataType(DataType.Date), JsonProperty(propertyName: nameof(DateOfBirth), Required = Required.Always)]
        public DateOnly DateOfBirth { set; get; }

        [EnumDataType(typeof(GenderEnum)), JsonProperty(propertyName: nameof(Gender), Required = Required.Always)]
        public GenderEnum Gender { set; get; }

        [Base64String, MaxLength(10), JsonProperty(propertyName: nameof(Rnokpp))]
        public string? Rnokpp { set; get; }

        [Base64String, JsonProperty(propertyName: nameof(PassportNumber))]
        public string? PassportNumber { set; get; }

        [Required, Base64String, MaxLength(10), JsonProperty(propertyName: nameof(Unzr), Required = Required.Always)]
        public required string Unzr { init; get; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity), DataType(DataType.DateTime)]
        public DateTime Inserted { get; init; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed), DataType(DataType.DateTime)]
        public DateTime LastUpdated { get; set; }
    }
}
