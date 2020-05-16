using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.AAA.Core.Dto
{
    public class BaseDetail
    {
        public long? CreatedBy { get; set; }

        public DateTime? CreatedTime { get; set; }

        public long? ModifiedBy { get; set; }

        public DateTime? ModifiedTime { get; set; }

        public string CreatedDateTimeFormat
        {
            get
            {
                if (CreatedTime != null)
                {
                    return Convert.ToDateTime(CreatedTime).ToString("MM/dd/yyyy HH:mm:ss");
                }
                else
                {
                    return "";
                }
            }
        }

        public string CreatedDateFormat
        {
            get
            {
                if (CreatedTime != null)
                {
                    return Convert.ToDateTime(CreatedTime).ToString("MM/dd/yyyy");
                }
                else
                {
                    return "";
                }
            }
        }


        public string ModifiedDateTimeFormat
        {
            get
            {
                if (ModifiedTime != null)
                {
                    return Convert.ToDateTime(ModifiedTime).ToString("MM/dd/yyyy HH:mm:ss");
                }
                else
                {
                    return "";
                }
            }
        }

        public string ModifiedDateFormat
        {
            get
            {
                if (ModifiedTime != null)
                {
                    return Convert.ToDateTime(ModifiedTime).ToString("MM/dd/yyyy");
                }
                else
                {
                    return "";
                }
            }
        }
    }
}
