using Jacmazon_ECommerce.Models;
using System.Configuration;

namespace Jacmazon_ECommerce.Extensions
{
    public abstract class PagedResultBase
    {
        public int CurrentPage { get; set; } //目前頁面
        public int PageCount { get; set; } //總頁數
        public int PageSize { get; set; } //每頁多少筆資料
        public int RowCount { get; set; } //資料總數
        //顯示第一筆資料的數字
        public int FirstRowOnPage
        {
            get { return (CurrentPage - 1) * PageSize + 1; }
        }

        //顯示最後一筆資料的數字
        public int LastRowOnPage
        {
            get { return Math.Min(CurrentPage * PageSize, RowCount); }
        }
    }

    public class PagedResult<T> : PagedResultBase where T : class
    {
        protected readonly AdventureWorksLt2019Context _context;
        protected readonly IConfiguration _configuration;

        private IList<T>? results_;
        public IList<T>? Results
        {
            get => results_;
            set
            {
                results_ = value;
                //設定分頁相關參數
                RowCount = _context.Set<T>().Count();
                if (results_ != null && results_.Count != 0)
                {
                    // 計算總頁數
                    PageCount = RowCount / PageSize + (RowCount % PageSize > 0 ? 1 : 0);
                }
            }
        }

        public PagedResult(AdventureWorksLt2019Context context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            //Config
            PageSize = Convert.ToInt16(configuration["PageSize"]);
            //Data
            results_ = new List<T>();
          
        }
    }
}
