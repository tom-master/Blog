using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
 

namespace NewBlogger.Test
{
    public class Class1
    {

        public Class1()
        {

        }

    
        public void AddCategory()
        {
            var model = new TestModel();

            //var result = nameof(model.Name);
        }
    }

    public class TestModel
    {
        public String Name { get; set; }
    }
}
