using System.Text.Json.Serialization;

namespace BackEnd.Models.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum GameStatus {
    Waiting=1,
    InProgress=2,
    Finished=3
}