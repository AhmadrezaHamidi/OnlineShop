using System.Text.Json.Serialization;

namespace Ahmad.OnlineShop.Persistence.EF.Services.Sms;

/// <summary>Payload ارسال به API sms.ir</summary>
public sealed record SmsBulkRequest(
    [property: JsonPropertyName("lineNumber")]   long         LineNumber,
    [property: JsonPropertyName("messageText")]  string       MessageText,
    [property: JsonPropertyName("mobiles")]      List<string> Mobiles,
    [property: JsonPropertyName("sendDateTime")] string?      SendDateTime = null
);

/// <summary>Response موفق از API sms.ir</summary>
public sealed record SmsBulkResponse(
    [property: JsonPropertyName("status")]  int    Status,
    [property: JsonPropertyName("message")] string Message
);
