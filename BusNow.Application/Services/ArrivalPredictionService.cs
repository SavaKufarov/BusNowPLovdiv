namespace BusNow.Application.Services;

public class ArrivalPredictionService
{
    public int CalculateEtaMinutes(double distanceKm, double avgSpeedKmH = 22)
    {
        if (avgSpeedKmH <= 0)
            return 0;

        var hours = distanceKm / avgSpeedKmH;
        return (int)Math.Ceiling(hours * 60);
    }

    public double CalculateDistanceKm(double lat1, double lon1, double lat2, double lon2)
    {
        double R = 6371;
        double dLat = ToRadians(lat2 - lat1);
        double dLon = ToRadians(lon2 - lon1);

        double a =
            Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
            Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
            Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    private double ToRadians(double angle)
    {
        return angle * Math.PI / 180.0;
    }
}