using CMG.License.Services.Interfaces;
using CMG.License.Shared.DataTypes;
using Deedle;
using System.Linq;

namespace CMG.License.Services.Impls
{
    public class DenialsRptGeneratorService : IDenialsRptGeneratorService
    {
        public void Aggregate(LogFile logFile)
        {
            var groubByProduct = logFile.Denys.ToLookup(l => l.Product, l => l)
                .Select(d => new { product = d.Key,
                max=d.GroupBy(c=>c.TimeStamp).Select(c=>c.Sum(e=>e.Count)).Max()});
            //    .GroupBy(d => d.TimeStamp)
            //        .Select(d => d.Sum(c => c.Count)).ToList();
            //foreach (var product in groubByProduct)
            //{
            //    var g=product.GroupBy(d => d.TimeStamp)
            //        .Select(d => d.Sum(c => c.Count)).ToList();

            //}
                //logFile.Denys.GroupBy(x => x.Product,
                //x => x,
                //(key, g) => new { Product = key, TimeStamp = g.ToList() });
            //var rows=logFile.Denys.Select((x,i) =>
            //{
            //    var sb = new SeriesBuilder<string>();
            //    sb.Add("Index", i);
            //    sb.Add("Time", x.TimeStamp);
            //    sb.Add("Product", x.Product);
            //    sb.Add("Count", x.Count);
            //    return KeyValue.Create(i, sb.Series);
            //});
            //var dfObjects = Frame.FromRows(rows);
            //dfObjects.Print();
        }
    }
}