using System.Collections.Generic;

namespace dask.adres.sorgu.Entity
{
    public class BuildingRootResponse
    {
        public List<BuildingResponse> buildingResponses { get; set; }

        public BuildingRootResponse()
        {
            this.buildingResponses = new List<BuildingResponse>();
        }

        public partial class BuildingResponse
        {
            public string buildingNumber { get; set; }
            public string buildingCode { get; set; }
            public string siteName { get; set; }
            public string apartmentName { get; set; }
            public string value { get; set; }
        }
    }
}
