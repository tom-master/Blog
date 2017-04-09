using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewBlogger.Model
{
    public partial class Blog
    {
        public void AddTag(Guid tagId) => Tags.ToList().Add(tagId);


        public void RemoveTag(Guid tagId) => Tags.ToList().Remove(tagId);
    }
}
