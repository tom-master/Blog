using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewBlogger.Model
{
    public class Category : ModelBase
    {
        public Category(){}
        public Category(String name)
        {
            if ((name + "").Length <= 0)
            {
                throw new ArgumentNullException($"{nameof(name)} cannot be null");
            }

            Name = name;

            Id = Guid.Parse("f7e371e3-9ad2-4ef9-9c6c-559409b67840");

            AddTime=DateTime.Now;
        }


        public String Name { get; private set; }

    }
}
