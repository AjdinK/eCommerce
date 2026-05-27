using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eCommerce.Model.Responses;

namespace eCommerce.Model.Messages
{
    public class ProductUpdated
    {
        public ProductResponse Data { get; set; }
        public int Id { get; set; }
    }
}
