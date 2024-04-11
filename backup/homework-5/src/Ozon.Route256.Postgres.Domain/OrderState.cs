namespace Ozon.Route256.Postgres.Domain;

public enum OrderState
{
    Unknown = 0,
    Created = 10,
    Paid = 20,
    Boxing = 30,
    WaitForPickup = 40,
    InDelivery = 50,
    WaitForClient = 60,
    Completed = 200,
    Cancelled = 300,
}
