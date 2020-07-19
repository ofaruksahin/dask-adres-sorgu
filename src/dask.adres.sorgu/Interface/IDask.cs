using dask.adres.sorgu.Entity;
using System;

namespace dask.adres.sorgu.Interface
{
    public interface IDask : IDisposable
    {
        CityRootResponse GetCities();
        DistrictRootResponse GetDistricts(string city_id);
        TownRootResponse GetTowns(string district_id);
        NeigborhoodRootResponse GetNeigborhoods(string town_id);
        StreetRootResponse GetStreet(string neigborhood_id);
        BuildingRootResponse GetBuildings(string street_id);
        DoorRootResponse GetDoors(string building_id);
    }
}
