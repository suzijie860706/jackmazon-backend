using Jacmazon_ECommerce.Models;

namespace Jacmazon_ECommerce.Interfaces
{
    public interface ICRUDRepository<T> where T : class
    {
        List<T> GetAll(int currentPage);
        T? Detail(int id);

        public virtual List<T> GetCategoryList(int? id)
        {
            return null;
        }
        //public ICRUD<T> Update(T entity);
        //public ICRUD<T> Delete(T entity);
        //public ICRUD<T> Update(string entityName);
        //public ICRUD<T> Delete(string entityName);
    }
}
