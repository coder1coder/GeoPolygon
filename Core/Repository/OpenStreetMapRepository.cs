using BAMCIS.GeoJSON;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace GeoPolygon.Core.Repository
{
    class OpenStreetMapRepository : IRepository
    {
        public IEnumerable<System.Windows.Shapes.Polygon> GetMultiPolygonByAddress(string address, ushort frequency)
        {
            string s = "https://nominatim.openstreetmap.org/search?q=" + address + "&format=json&polygon_geojson=1";

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            var request = (HttpWebRequest)WebRequest.Create(s);
            request.ContentType = "application/json; charset=utf-8";
            request.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.106 Safari/537.36";
            var response = request.GetResponse() as HttpWebResponse;
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                var jarr = JArray.Parse(reader.ReadToEnd());

                if (jarr.HasValues)
                {
                    var jo = JObject.Parse(jarr.Children().First().ToString());
                    if (jo.ContainsKey("geojson"))
                    {
                        GeoJson data = JsonConvert.DeserializeObject<GeoJson>(jo["geojson"].ToString());
                        switch (data.Type)
                        {
                            case GeoJsonType.MultiPolygon:
                                MultiPolygon mp = (MultiPolygon)data;

                                foreach (Polygon p in mp.Coordinates)
                                {
                                    var polygon = new System.Windows.Shapes.Polygon();

                                    foreach (LinearRing lr in p.Coordinates)
                                    {
                                        for (int i = 0; i < lr.Coordinates.Count(); i++)
                                        {
                                            if (i % frequency != 0) continue;

                                            var point = lr.Coordinates.ElementAt(i);

                                            polygon.Points.Add(
                                                new System.Windows.Point(
                                                    point.Latitude,
                                                    point.Longitude
                                                )
                                            );
                                        }

                                        yield return polygon;
                                    }
                                }

                            break;
                        }
                    }
                }
            }
        }
    }
}
