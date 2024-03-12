using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Showdata
{
    public class CustomerResponse
    {
        public int status { get; set; }
        public List<Datum> data { get; set; }
    }
    public class Datum
    {
        public string REFERENCE_NO { get; set; }
        public string STATUS { get; set; }
        public string NAME_EN { get; set; }
        public string NAME_AR { get; set; }
        public string ID_TYPE { get; set; }
        public string ID_NUMBER { get; set; }
        public string MOBILE { get; set; }
        public string PHONE { get; set; }
    }

}