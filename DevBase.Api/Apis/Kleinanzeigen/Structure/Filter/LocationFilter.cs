using System.Runtime.CompilerServices;
using System.Text;

namespace DevBase.Api.Apis.Kleinanzeigen.Structure.Filter;

public class LocationFilter
{
    private StringBuilder _locationFilter;

    private LocationFilter(StringBuilder filter)
    {
        this._locationFilter = filter;
    }

    public static LocationFilter FromCoordinate(double latitude, double longitude, int distance = -1)
    {
        StringBuilder filter = new StringBuilder();

        filter.Append(string.Format("latitude={0}", latitude));
        filter.Append(string.Format("longitude={0}", longitude));
        
        if (distance >= 0)
            filter.Append(string.Format("distance={0}&distanceUnit=KM", distance));
        
        return new LocationFilter(filter);
    }
    
    public static LocationFilter FromLocationId(int locationID, int distance = -1)
    {
        StringBuilder filter = new StringBuilder();

        filter.Append(string.Format("locationId={0}", locationID));
        
        if (distance >= 0)
            filter.Append(string.Format("distance={0}&distanceUnit=KM", distance));
        
        return new LocationFilter(filter);
    }

    public static LocationFilter FromZipCode(int zipCode, int distance = -1)
    {
        StringBuilder filter = new StringBuilder();

        filter.Append(string.Format("zipcode={0}", zipCode));
        
        if (distance >= 0)
            filter.Append(string.Format("distance={0}&distanceUnit=KM", distance));
        
        return new LocationFilter(filter);
    }

    public override string ToString()
    {
        return this._locationFilter.ToString();
    }
}