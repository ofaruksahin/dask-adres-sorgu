using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;

namespace dask.adres.sorgu.Entity
{
    public  class TownRootResponse
    {
        [JsonProperty("yt")]
        public List<TownResponse> townResponses { get; set; }
        public partial class TownResponse : BaseResponse
        {
        }
    }
}
