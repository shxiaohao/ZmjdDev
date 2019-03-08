using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using HJD.AccessService.Contract;
using HJD.AccessService.Contract.Model;
using HJD.AccessService.Contract.Params;

namespace HJD.AccessService.Contract
{
    [ServiceContract(Namespace = "http://www.zmjiudian.com/")]
    public interface IAccessExService
    {

        #region 索引相关处理

        [OperationContract]
        void AddIndexDoc(string id, SearchType type);

        [OperationContract]
        void RemoveIndexDoc(string id, SearchType type);

        #endregion
    }
}
