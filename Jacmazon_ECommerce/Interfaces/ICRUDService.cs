namespace Jacmazon_ECommerce.Interfaces
{
    public interface ICRUDService<T> where T : class
    {
        public List<T> GetAll(int currentPage);

        public T? Detail(int? id);

        public List<T> GetCategoryList(int? id);
    }
}
