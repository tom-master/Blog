using System;
using System.Collections.Generic;
using System.Text;

namespace NewBlogger.Model
{
    public class Tag : ModelBase
    {


        public Tag(String name)
        {
            if ((name + "").Length <= 0)
            {
                throw new ArgumentNullException($"{nameof(name)} cannot be null");
            }

            Name = name;

            Id = Guid.NewGuid();

            AddTime = DateTime.Now;
        }


        public String Name { get; private set; }
    }
}
