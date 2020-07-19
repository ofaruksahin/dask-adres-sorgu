using Newtonsoft.Json;
using System.Collections.Generic;

namespace dask.adres.sorgu.Entity
{
    public class CityRootResponse
    {
        [JsonProperty("yt")]
        public List<CityResponse> cityResponses { get; set; }

        public partial class CityResponse : BaseResponse
        {
        }
    }
}
