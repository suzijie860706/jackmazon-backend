using Jacmazon_ECommerce.DTOs;

namespace Jacmazon_ECommerce.Services
{
    public interface IProductService
    {
        /// <summary>
        /// 取得所有產品資料
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<ProductResponseDto>> GetAllProducts();
    }
}
