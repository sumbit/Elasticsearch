using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreDemo.Models
{
    public interface IEsClientProvider
    {
        ElasticClient GetClient();
    }
}
