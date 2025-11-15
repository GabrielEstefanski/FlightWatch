<<<<<<< HEAD
using System.Text.Json;

namespace FlightWatch.Infrastructure.ExternalServices.OpenSky.Models;

public class OpenSkyStateVector
{
    public string Icao24 { get; set; } = string.Empty;
    public string? Callsign { get; set; }
    public string OriginCountry { get; set; } = string.Empty;
    public int? TimePosition { get; set; }
    public int LastContact { get; set; }
    public double? Longitude { get; set; }
    public double? Latitude { get; set; }
    public double? BaroAltitude { get; set; }
    public bool OnGround { get; set; }
    public double? Velocity { get; set; }
    public double? TrueTrack { get; set; }
    public double? VerticalRate { get; set; }
    public int[]? Sensors { get; set; }
    public double? GeoAltitude { get; set; }
    public string? Squawk { get; set; }
    public bool Spi { get; set; }
    public int PositionSource { get; set; }
    public int? Category { get; set; }

    public static OpenSkyStateVector FromJsonElements(JsonElement[] data)
    {
        return new OpenSkyStateVector
        {
            Icao24 = GetStringValue(data, 0) ?? string.Empty,
            Callsign = GetStringValue(data, 1)?.Trim(),
            OriginCountry = GetStringValue(data, 2) ?? string.Empty,
            TimePosition = GetIntValue(data, 3),
            LastContact = GetIntValue(data, 4) ?? 0,
            Longitude = GetDoubleValue(data, 5),
            Latitude = GetDoubleValue(data, 6),
            BaroAltitude = GetDoubleValue(data, 7),
            OnGround = GetBoolValue(data, 8),
            Velocity = GetDoubleValue(data, 9),
            TrueTrack = GetDoubleValue(data, 10),
            VerticalRate = GetDoubleValue(data, 11),
            Sensors = null,
            GeoAltitude = GetDoubleValue(data, 13),
            Squawk = GetStringValue(data, 14),
            Spi = GetBoolValue(data, 15),
            PositionSource = GetIntValue(data, 16) ?? 0,
            Category = GetIntValue(data, 17)
        };
    }

    private static string? GetStringValue(JsonElement[] data, int index)
    {
        if (index >= data.Length || data[index].ValueKind == JsonValueKind.Null)
            return null;

        return data[index].ValueKind == JsonValueKind.String 
            ? data[index].GetString() 
            : data[index].ToString();
    }

    private static int? GetIntValue(JsonElement[] data, int index)
    {
        if (index >= data.Length || data[index].ValueKind == JsonValueKind.Null)
            return null;

        return data[index].ValueKind == JsonValueKind.Number
            ? data[index].GetInt32()
            : null;
    }

    private static double? GetDoubleValue(JsonElement[] data, int index)
    {
        if (index >= data.Length || data[index].ValueKind == JsonValueKind.Null)
            return null;

        return data[index].ValueKind == JsonValueKind.Number
            ? data[index].GetDouble()
            : null;
    }

    private static bool GetBoolValue(JsonElement[] data, int index)
    {
        if (index >= data.Length || data[index].ValueKind == JsonValueKind.Null)
            return false;

        if (data[index].ValueKind == JsonValueKind.True)
            return true;

        if (data[index].ValueKind == JsonValueKind.False)
            return false;

        return false;
    }
}

=======
using System.Text.Json;

namespace FlightWatch.Infrastructure.ExternalServices.OpenSky.Models;

public class OpenSkyStateVector
{
    public string Icao24 { get; set; } = string.Empty;
    public string? Callsign { get; set; }
    public string OriginCountry { get; set; } = string.Empty;
    public int? TimePosition { get; set; }
    public int LastContact { get; set; }
    public double? Longitude { get; set; }
    public double? Latitude { get; set; }
    public double? BaroAltitude { get; set; }
    public bool OnGround { get; set; }
    public double? Velocity { get; set; }
    public double? TrueTrack { get; set; }
    public double? VerticalRate { get; set; }
    public int[]? Sensors { get; set; }
    public double? GeoAltitude { get; set; }
    public string? Squawk { get; set; }
    public bool Spi { get; set; }
    public int PositionSource { get; set; }
    public int? Category { get; set; }

    public static OpenSkyStateVector FromJsonElements(JsonElement[] data)
    {
        return new OpenSkyStateVector
        {
            Icao24 = GetStringValue(data, 0) ?? string.Empty,
            Callsign = GetStringValue(data, 1)?.Trim(),
            OriginCountry = GetStringValue(data, 2) ?? string.Empty,
            TimePosition = GetIntValue(data, 3),
            LastContact = GetIntValue(data, 4) ?? 0,
            Longitude = GetDoubleValue(data, 5),
            Latitude = GetDoubleValue(data, 6),
            BaroAltitude = GetDoubleValue(data, 7),
            OnGround = GetBoolValue(data, 8),
            Velocity = GetDoubleValue(data, 9),
            TrueTrack = GetDoubleValue(data, 10),
            VerticalRate = GetDoubleValue(data, 11),
            Sensors = null,
            GeoAltitude = GetDoubleValue(data, 13),
            Squawk = GetStringValue(data, 14),
            Spi = GetBoolValue(data, 15),
            PositionSource = GetIntValue(data, 16) ?? 0,
            Category = GetIntValue(data, 17)
        };
    }

    private static string? GetStringValue(JsonElement[] data, int index)
    {
        if (index >= data.Length || data[index].ValueKind == JsonValueKind.Null)
            return null;

        return data[index].ValueKind == JsonValueKind.String 
            ? data[index].GetString() 
            : data[index].ToString();
    }

    private static int? GetIntValue(JsonElement[] data, int index)
    {
        if (index >= data.Length || data[index].ValueKind == JsonValueKind.Null)
            return null;

        return data[index].ValueKind == JsonValueKind.Number
            ? data[index].GetInt32()
            : null;
    }

    private static double? GetDoubleValue(JsonElement[] data, int index)
    {
        if (index >= data.Length || data[index].ValueKind == JsonValueKind.Null)
            return null;

        return data[index].ValueKind == JsonValueKind.Number
            ? data[index].GetDouble()
            : null;
    }

    private static bool GetBoolValue(JsonElement[] data, int index)
    {
        if (index >= data.Length || data[index].ValueKind == JsonValueKind.Null)
            return false;

        if (data[index].ValueKind == JsonValueKind.True)
            return true;

        if (data[index].ValueKind == JsonValueKind.False)
            return false;

        return false;
    }
}

>>>>>>> 46c33f55e4420f09ba269fe78a84593a0d6687a2
