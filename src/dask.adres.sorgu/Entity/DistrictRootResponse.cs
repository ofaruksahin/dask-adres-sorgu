using Newtonsoft.Json;
using System.Collections.Generic;

namespace dask.adres.sorgu.Entity
{
    public class DistrictRootResponse
    {
        [JsonProperty("yt")]
        public List<DistrictResponse> districtResponses { get; set; }
        public partial class DistrictResponse : BaseResponse
        {

        }
    }
}
