namespace BusNow.Core.Entities;

public class Vehicle
{
    public int Id { get; set; }

    public int TransportLineId { get; set; }
    public int? RouteId { get; set; }
    public int CurrentStopOrderIndex { get; set; } = 1;
    public bool IsForwardDirection { get; set; } = true;
    public string RegistrationNumber { get; set; } = null!;
    public string VehicleType { get; set; } = null!;
    public double CurrentLatitude { get; set; }
    public double CurrentLongitude { get; set; }
    public DateTime LastUpdateTime { get; set; }
    public bool IsActive { get; set; } = true;

    public TransportLine? TransportLine { get; set; }
    public Route? Route { get; set; }
    public ICollection<ArrivalPrediction> ArrivalPredictions { get; set; } = new List<ArrivalPrediction>();
}