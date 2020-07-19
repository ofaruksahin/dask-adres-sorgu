using dask.adres.sorgu.Entity;
using dask.adres.sorgu.Interface;
using HtmlAgilityPack;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

namespace dask.adres.sorgu
{
    public class Dask : IDask
    {
        string url = "https://adreskodu.dask.gov.tr/site-element/control";
        string _token { get; set; }
        string token
        {
            get
            {
                if (String.IsNullOrEmpty(_token))
                    _token = "";
                return _token;
            }
            set { }
        }

        IRestClient client { get; set; }
        IRestRequest request { get; set; }
        IRestResponse response { get; set; }

        public Dask()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        private T Execute<T>(string ashx, Method method, IDictionary<string, string> args = null)
        {
            try
            {
                client = new RestClient($"{url}/{ashx}");
                client.Timeout = -1;
                request = new RestRequest(method);
                request.AddHeader("Host", "adreskodu.dask.gov.tr");
                request.AddHeader("Connection", "keep-alive");
                request.AddHeader("Accept", "*/*");
                request.AddHeader("DNT", "1");
                request.AddHeader("X-Request-With", "application/xml");
                request.AddHeader("Sec-Fetch-Site", "same-origin");
                request.AddHeader("Sec-Fetch-Mode", "cors");
                request.AddHeader("Sec-Fetch-Dest", "empty");
                request.AddHeader("Referer", "https://adreskodu.dask.gov.tr");
                request.AddHeader("Accept-Encoding", "gzip, deflate, br");
                request.AddHeader("Accept-Language", "tr-TR,tr;q=0.9,en-US;q=0.8,en;q=0.7");
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                if (args != null)
                {
                    IEnumerable<string> keys = args.Keys.ToList();
                    foreach (string key in keys)
                    {
                        string val;
                        args.TryGetValue(key, out val);
                        if (val != null)
                            request.AddParameter(key, val);
                    }
                }
                response = client.Execute(request);
                Encoding encoding = Encoding.GetEncoding("windows-1253");
                string content = encoding.GetString(response.RawBytes);
                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        T result = default(T);
                        if ((content.StartsWith("{") && content.EndsWith("}")) || (content.StartsWith("[") && content.EndsWith("]")))
                            result = JsonConvert.DeserializeObject<T>(content);
                        else
                            result = (T)Convert.ChangeType(content, typeof(T));
                        return result;
                    default:
                        throw new HttpRequestException($"Http Error => {response.StatusCode} {DateTime.Now}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return default(T);
            }
        }

        private string GetToken()
        {
            return Execute<string>("y.ashx", Method.GET);
        }

        private KeyValuePair<string, string> GetTokenPair()
        {
            return new KeyValuePair<string, string>(token.Substring(0, token.Length - 1), token.Substring(token.Length - 1, 1));
        }

        public CityRootResponse GetCities()
        {
            if (String.IsNullOrEmpty(token))
                throw new ArgumentNullException(nameof(token));
            IDictionary<string, string> args = new Dictionary<string, string>();
            args.Add(GetTokenPair());
            args.Add("t", "il");
            args.Add("u", "ddl_il");
            CityRootResponse cityRootResponse = null;
            cityRootResponse = Execute<CityRootResponse>("load.ashx", Method.POST, args);
            cityRootResponse?.cityResponses?.RemoveAll(x => String.IsNullOrEmpty(x.value));
            return cityRootResponse;
        }

        public DistrictRootResponse GetDistricts(string city_id)
        {
            if (String.IsNullOrEmpty(token))
                throw new ArgumentNullException(nameof(token));
            IDictionary<string, string> args = new Dictionary<string, string>();
            args.Add(GetTokenPair());
            args.Add("t", "ce");
            args.Add("u", city_id);
            DistrictRootResponse countryRootResponse = null;
            countryRootResponse = Execute<DistrictRootResponse>("load.ashx", Method.POST, args);
            countryRootResponse?.districtResponses?.RemoveAll(x => String.IsNullOrEmpty(x.value));
            return countryRootResponse;
        }

        public TownRootResponse GetTowns(string district_id)
        {
            if (String.IsNullOrEmpty(token))
                throw new ArgumentNullException(nameof(token));
            IDictionary<string, string> args = new Dictionary<string, string>();
            args.Add(GetTokenPair());
            args.Add("t", "vl");
            args.Add("u", district_id);
            TownRootResponse townRootResponse = null;
            townRootResponse = Execute<TownRootResponse>("load.ashx", Method.POST, args);
            townRootResponse?.townResponses?.RemoveAll(x => String.IsNullOrEmpty(x.value));
            return townRootResponse;
        }

        public NeigborhoodRootResponse GetNeigborhoods(string town_id)
        {
            if (String.IsNullOrEmpty(token))
                throw new ArgumentNullException(nameof(town_id));
            IDictionary<string, string> args = new Dictionary<string, string>();
            args.Add(GetTokenPair());
            args.Add("t", "mh");
            args.Add("u", town_id);
            NeigborhoodRootResponse neigborhoodRootResponse = null;
            neigborhoodRootResponse = Execute<NeigborhoodRootResponse>("load.ashx", Method.POST, args);
            neigborhoodRootResponse?.neigborhoodResponses?.RemoveAll(x => String.IsNullOrEmpty(x.value));
            return neigborhoodRootResponse;
        }

        public StreetRootResponse GetStreet(string neigborhood_id)
        {
            if (String.IsNullOrEmpty(token))
                throw new ArgumentNullException(nameof(token));
            IDictionary<string, string> args = new Dictionary<string, string>();
            args.Add(GetTokenPair());
            args.Add("t", "sf");
            args.Add("u", neigborhood_id);
            string response = Execute<string>("load.ashx", Method.POST, args);
            if (String.IsNullOrEmpty(response))
                return new StreetRootResponse();
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(response);
            List<HtmlNode> tableRows = htmlDocument.DocumentNode.SelectNodes("//tbody//tr//td").ToList();
            tableRows?.RemoveAll(x => x.InnerText == "CADDE" || x.InnerText == "SOKAK" || x.InnerText == "BULVAR" || x.InnerText == "MEYDAN" || x.InnerText == "KÜME EVLER" || x.InnerText == "KÖY SOKAĞI" || x.InnerText == "SEÇ");
            StreetRootResponse streetRootResponse = new StreetRootResponse();
            for (int i = 0; i < tableRows.Count; i++)
            {
                HtmlNode firstNode = tableRows[i];
                streetRootResponse.streetResponses.Add(new StreetRootResponse.StreetResponse
                {
                    text = firstNode.InnerText,
                    value = firstNode.ParentNode.Id.Substring(1, firstNode.ParentNode.Id.ToString().Length - 1)
                });
            }
            return streetRootResponse;
        }

        public BuildingRootResponse GetBuildings(string street_id)
        {
            if (String.IsNullOrEmpty(token))
                throw new ArgumentNullException(nameof(token));
            IDictionary<string, string> args = new Dictionary<string, string>();
            args.Add(GetTokenPair());
            args.Add("t", "dk");
            args.Add("u", street_id);
            args.Add("term", "");
            string response = Execute<string>("load.ashx", Method.POST, args);
            if (!String.IsNullOrEmpty(response))
            {
                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(response);
                List<HtmlNode> tableRows = htmlDocument.DocumentNode.SelectNodes("//tbody/tr//td").ToList();
                tableRows.RemoveAll(x => x.InnerText == "SEÇ");
                BuildingRootResponse buildingRootResponse = new BuildingRootResponse();
                for (int i = 0; i < tableRows.Count; i += 4)
                {
                    HtmlNode buildingNumberNode = tableRows[i];
                    HtmlNode buildingCodeNode = tableRows[i + 1];
                    HtmlNode siteNameNode = tableRows[i + 2];
                    HtmlNode apartmentNameNode = tableRows[i + 3];
                    buildingRootResponse.buildingResponses.Add(new BuildingRootResponse.BuildingResponse
                    {
                        buildingNumber = buildingNumberNode.InnerText,
                        buildingCode = buildingCodeNode.InnerText,
                        siteName = siteNameNode.InnerText == "&nbsp;" ? "" : siteNameNode.InnerText,
                        apartmentName = apartmentNameNode.InnerText == "&nbsp;" ? "" : apartmentNameNode.InnerText,
                        value = buildingNumberNode.ParentNode.Id.Substring(1, buildingNumberNode.ParentNode.Id.ToString().Length - 1)
                    });
                }
                return buildingRootResponse;
            }
            return new BuildingRootResponse();
        }

        public DoorRootResponse GetDoors(string building_id)
        {
            if (String.IsNullOrEmpty(token))
                throw new ArgumentNullException(nameof(token));
            IDictionary<string, string> args = new Dictionary<string, string>();
            args.Add(GetTokenPair());
            args.Add("t", "ick");
            args.Add("u", building_id);
            args.Add("term", "");
            string response = Execute<string>("load.ashx", Method.POST, args);
            if (!String.IsNullOrEmpty(response))
            {
                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(response);
                List<HtmlNode> tableRows = htmlDocument.DocumentNode.SelectNodes("//tbody//tr//td").ToList();
                tableRows.RemoveAll(x => x.InnerText == "SEÇ");
                DoorRootResponse doorRootResponse = new DoorRootResponse();
                for (int i = 0; i < tableRows.Count; i++)
                {
                    HtmlNode firstNode = tableRows[i];
                    doorRootResponse.doorResponses.Add(new DoorRootResponse.DoorResponse
                    {
                        name = firstNode.InnerText == "&nbsp;" ? "" : firstNode.InnerText,
                        value = firstNode.ParentNode.Id.Substring(1, firstNode.ParentNode.Id.ToString().Length - 1)
                    });
                }
                return doorRootResponse;
            }
            return new DoorRootResponse();
        }

        public void Dispose()
        {
            if (client != null)
                client = null;
            if (request != null)
                request = null;
            if (response != null)
                response = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.SuppressFinalize(this);
        }

        ~Dask()
        {
            Dispose();
        }
    }
}
