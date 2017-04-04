using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewBlogger.Model
{
    public partial class Blog : ModelBase
    {
        public Blog(String title, String content, Guid categoryId, params Guid[] tagIds)
        {
            if ((title + "").Length <= 0)
            {
                throw new ArgumentNullException($"{nameof(title)} cannot be null");
            }

            if ((content + "").Length <= 0)
            {
                throw new ArgumentNullException($"{nameof(content)} cannot be null");
            }

            if ((categoryId + "").Length <= 0)
            {
                throw new ArgumentNullException($"{nameof(categoryId)} cannot be null");
            }

            Title = title;

            Content = content;

            CategoryId = categoryId;

            Id = Guid.NewGuid();

            AddTime = DateTime.Now;

            Tags = tagIds;
        }

        public String Title { get; private set; }

        public String Content { get; private set; }

        public Guid CategoryId { get; private set; }

        public Int32 ViewCount { get; private set; }

        public Guid[] Tags { get; private set; }

    }
}
