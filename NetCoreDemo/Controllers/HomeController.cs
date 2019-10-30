using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Microsoft.AspNetCore.Mvc;
using Nest;
using NetCoreDemo.Models;
using SqlSugar;

namespace NetCoreDemo.Controllers
{
    public class HomeController : Controller
    {
        private SqlSugarClient _sqlClient()
        {
            SqlSugarClient db = new SqlSugarClient(
               new ConnectionConfig()
               {
                   ConnectionString = "server=.;uid=sa;pwd=123456;database=zhongkeyuan",
                   DbType = DbType.SqlServer,//设置数据库类型
                   IsAutoCloseConnection = true,//自动释放数据务，如果存在事务，在事务结束后释放
                   InitKeyType = InitKeyType.Attribute //从实体特性中读取主键自增列信息
               });


            //用来打印Sql方便你调式    
            db.Aop.OnLogExecuting = (sql, pars) =>
            {
                Console.WriteLine(sql + "\r\n" +
                db.Utilities.SerializeObject(pars.ToDictionary(it => it.ParameterName, it => it.Value)));
                Console.WriteLine();
            };
            return db;
        }
        private readonly ElasticClient _client;
        public HomeController(IEsClientProvider clientProvider)
        {
            _client = clientProvider.GetClient();
        }
        [HttpPost]
        [Route("value/index")]
        public async Task<BulkResponse> Index([FromBody]List<Post> post)
        {
            return await _client.IndexManyAsync(post, "");
        }

        [HttpPost]
        [Route("value/search")]
        public IReadOnlyCollection<X_ArticleParagraph> Search(string type)
        {
            var response = _client.Get(new DocumentPath<X_ArticleParagraph>("OJ11daf3f70ccdee87"), pd => pd.Index("test_artp3"));
            var list = _client.MultiGetAsync(m => m.GetMany<X_ArticleParagraph>(new List<string> { "OJ11daf3f70ccdee87", "OJ11d92b6505e64785", "OJ11dad19c06f72761", "OJ11d9c2bf6df931f9" })); 
            return _client.Search<X_ArticleParagraph>(s => s
            .Index("test_artp3")
                .From(0)
                .Size(10)
                .Query(q => q.Match(m => m.Field(f => f.Type).Query(type)))).Documents;
        }

        public async Task<IActionResult> IndexTest()
        {
            int pagesize = 20000;
            using (var db = _sqlClient())
            {
                var list = db.Queryable<X_ArticleParagraph>().ToList();
                int countArr = list.Count() / pagesize;
                for (int i = 0; i <= countArr; i++)
                {
                    await registerTask(list.Skip(i * pagesize).Take(pagesize));
                }
                return Json(new { result = "ok" });
            }
        }

        public async Task<bool> registerTask(IEnumerable<X_ArticleParagraph> list)
        {
            return await Task.Run(() =>
            {
                _client.IndexManyAsync(list, "test_artp3");
                return true;
            });
        }
        public IActionResult IndexCount()
        {
            CountRequest ss = new CountRequest("test_artp");
            ss.QueryOnQueryString = "三维人脸模板和二维平面技术";
            var result = _client.Count<X_ArticleParagraph>(s => s
              .Index("test_artp")
              .Query(q => q.QueryString(p => p.Query("三维人脸模板和二维平面技术")))
            ).Count;
            Func<CountDescriptor<X_ArticleParagraph>, ICountRequest> ext;
            var resutl1 = _client.Count<X_ArticleParagraph>(s => s
              .Query(q => q
                .Bool(ssd => ssd
                    .Must(d => d
                        .Match(f => f
                            .Field(ass => ass.Fid).Query(""))
                        )
                    )
                )
            ).Count;
            return Json(new { result = _client.Count(ss).Count, result1 = result });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
