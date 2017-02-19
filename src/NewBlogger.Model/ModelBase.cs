using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace NewBlogger.Model
{
    public class ModelBase
    {
        protected ModelBase()
        {
        }

        public String Id { get; set; }

        public DateTime AddTime { get; set; }

        public Boolean IsDelete { get; set; }

    }
}
