using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasketWebPanel.ViewModels
{
    public class SearchPostsViewModel : BaseViewModel
    {
        public SearchPostsViewModel()
        {
            Posts = new List<PostBindinModel>();
        }

        public List<PostBindinModel> Posts { get; set; }
    }

    public class PostBindinModel
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public bool IsDeleted { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
    }
}