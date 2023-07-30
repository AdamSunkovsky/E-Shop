using Microsoft.EntityFrameworkCore;

namespace E_Shop
{
    public class SaleTranslator
    {
        public double FinalPrice(int SaleType, int Price)
        {
            if(SaleType == 1)
            {
                return Price / 2;
            } else if(SaleType == 2)
            {
                return Price / 3;
            }
            return 0;
        }
    }
}
