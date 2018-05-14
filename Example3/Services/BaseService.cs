using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Crm.Services
{
    public class BaseService : IDisposable
    {
        protected CrmContext db = new CrmContext();
        protected readonly ILog logger;
        public BaseService()
        {
            //logger = LogManager.GetLogger(GetType());

            //     db.Database.Log = (dbLog => logger.Debug(dbLog));
        }
        public void Dispose()
        {
            db.Dispose();
        }
    }
}