using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace dask.adres.sorgu.Entity
{
    public class NeigborhoodRootResponse
    {
        [JsonProperty("yt")]
        public List<NeigborhoodResponse> neigborhoodResponses { get; set; }
        public partial class NeigborhoodResponse : BaseResponse
        {
        }
    }
}
