using System.Collections.Generic;

namespace dask.adres.sorgu.Entity
{
    public class StreetRootResponse
    {
        public List<StreetResponse> streetResponses { get; set; }

        public StreetRootResponse()
        {
            this.streetResponses = new List<StreetResponse>();
        }

        public partial class StreetResponse : BaseResponse
        {

        }
    }
}
