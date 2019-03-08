using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http; 
using HJDAPI.Controllers.Adapter;

namespace HJDAPI.Controllers
{
    public class ChannelController : BaseApiController
    {
        [HttpGet]
        public bool RemoveChannelInfoCache(long CID)
        {
            return ChannelAdapter.RemoveChannelInfoCache(CID);
        }

         
    }
}
