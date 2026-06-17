namespace Logistics.Domain.Primitives.Enums;

/// <summary>
/// The general nature of the commodity carried by a load.
/// </summary>
public enum MaterialType
{
    General,
    Hazmat,
    Refrigerated,
    LiquidBulk,
    Vehicles,
    MachineryEquipment,
    BuildingMaterials,
    Agricultural,
    Other
}
