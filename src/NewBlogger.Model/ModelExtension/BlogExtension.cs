using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewBlogger.Model
{
    public partial class Blog
    {
        public void AddViewCount()
        {
            ViewCount = ViewCount + 1;
        }
    }
}
