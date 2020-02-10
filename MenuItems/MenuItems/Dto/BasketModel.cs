using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MenuItems.Dto
{
    public class BasketModel
    {
        private Dictionary<string, string> _basket;
        public Dictionary<string, string> Basket
        {
            get
            {
                if (_basket != null)
                {
                    return _basket;
                }
                else
                {
                    throw new Exception("Basket is Empty");
                }
            }
            set
            {
                if (value != null)
                {
                   if (_basket == null)
                        _basket = new Dictionary<string, string>();
                    foreach (var item in value)
                    {
                        _basket.Add(item.Key, item.Value);
                    }
                }
            }
        }
    }
}
