using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewBlogger.Model
{
    public class Category : ModelBase
    {

        public Category(String name)
        {
            if ((name + "").Length <= 0)
            {
                throw new ArgumentNullException($"{nameof(name)} cannot be null");
            }

            Name = name;

            Id = Guid.NewGuid().ToString();

            AddTime=DateTime.Now;
        }


        public String Name { get; private set; }

    }
}
