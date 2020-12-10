using System.Collections.Generic;
using System.Windows.Shapes;

namespace GeoPolygon.Core.Repository
{
    public interface IRepository
    {
        IEnumerable<Polygon> GetMultiPolygonByAddress(string address, ushort frequency);
    }
}
