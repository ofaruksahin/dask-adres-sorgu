using System.Collections.Generic;

namespace dask.adres.sorgu.Entity
{
    public class DoorRootResponse
    {
        public List<DoorResponse> doorResponses { get; set; }

        public DoorRootResponse()
        {
            this.doorResponses = new List<DoorResponse>();
        }

        public partial class DoorResponse 
        {
            public string name { get; set; }
            public string value { get; set; }
        }
    }
}
