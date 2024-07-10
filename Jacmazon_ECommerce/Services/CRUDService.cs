using Jacmazon_ECommerce.Interfaces;
using Jacmazon_ECommerce.Models;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;

namespace Jacmazon_ECommerce.Services
{
    public class CRUDService<T> : ICRUDService<T> where T : class
    {
        protected readonly ICRUDRepository<T> cRUD;
        protected readonly AdventureWorksLt2019Context _context;

        public CRUDService(ICRUDRepository<T> cRUD, AdventureWorksLt2019Context context)
        {
            this.cRUD = cRUD;
            _context = context;
        }

        public List<T> GetAll(int currentPage)
        {
            return cRUD.GetAll(currentPage);
        }

        public T? Detail(int? id)
        {
            if (id == null || id < 1 || !id.HasValue) return null;

            return cRUD.Detail(Convert.ToInt32(id));
        }

        public List<T> GetCategoryList(int? id)
        {
            return cRUD.GetCategoryList(id);
        }

    }
}
