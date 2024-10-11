using System.Text.Json.Serialization;

namespace ISA3Demos.Models.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum QuestionHardness
    {
        Easy = 1,
        Medium = 2,
        Hard = 3
    }
}
