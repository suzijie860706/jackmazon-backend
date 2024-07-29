//using Jacmazon_ECommerce.Interfaces;
//using Jacmazon_ECommerce.Data;
//using Jacmazon_ECommerce.Models;
//using Jacmazon_ECommerce.Models.AdventureWorksLT2016Context;

//namespace Jacmazon_ECommerce.Repositories
//{
//    public class ProductCategoryRepository : ICRUDRepository<ProductCategory>
//    {
//        protected readonly AdventureWorksLt2016Context context;
//        protected readonly IConfiguration configuration;

//        public ProductCategoryRepository(AdventureWorksLt2016Context context,
//            IConfiguration configuration)
//        {
//            this.context = context;
//            this.configuration = configuration;
//        }

//        public List<ProductCategory> GetAll(int currentPage)
//        {
//            int skip_count = 0;
//            int page_size = Convert.ToInt16(configuration["PageSize"]);
//            if (currentPage != 1) skip_count = currentPage * page_size;

//            IQueryable<ProductCategory> productCategories = context.ProductCategories.Skip(skip_count).Take(page_size);
//            return productCategories.ToList();
//        }

//        public ProductCategory? Detail(int id)
//        {
//            ProductCategory? productCategory = context.ProductCategories.Find(id);
//            return productCategory;
//        }
//    }
//}
