namespace FlightWatch.Application.Helpers;

public static class AircraftCategoryHelper
{
    public static string GetCategoryDescription(int? category)
    {
        return category switch
        {
            0 => "Unknown",
            1 => "No ADS-B Info",
            2 => "Light (<15500 lbs)",
            3 => "Small (15500-75000 lbs)",
            4 => "Large (75000-300000 lbs)",
            5 => "High Vortex (B-757)",
            6 => "Heavy (>300000 lbs)",
            7 => "High Performance",
            8 => "Rotorcraft",
            9 => "Glider",
            10 => "Lighter-than-air",
            11 => "Parachutist",
            12 => "Ultralight",
            13 => "Reserved",
            14 => "UAV/Drone",
            15 => "Space Vehicle",
            16 => "Emergency Vehicle",
            17 => "Service Vehicle",
            18 => "Point Obstacle",
            19 => "Cluster Obstacle",
            20 => "Line Obstacle",
            _ => "Unknown"
        };
    }

    public static string? GetCategoryShort(int? category)
    {
        return category switch
        {
            2 => "Light",
            3 => "Small",
            4 => "Large",
            5 => "High Vortex",
            6 => "Heavy",
            7 => "High Perf",
            8 => "Helicopter",
            9 => "Glider",
            10 => "Balloon",
            11 => "Parachute",
            12 => "Ultralight",
            14 => "Drone",
            15 => "Spacecraft",
            16 => "Emergency",
            17 => "Service",
            _ => null
        };
    }
}

