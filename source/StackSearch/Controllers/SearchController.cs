using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BillingsDotNet.Search;
using StackSearch.Model;

namespace StackSearch.Controllers
{
    public class SearchController : Controller
    {
        private readonly IEnumerable<IIndex> indices;

        public SearchController(IEnumerable<IIndex> indices)
        {
            this.indices = indices;
        }

        public ActionResult Index(SearchViewModel model)
        {
            if (!String.IsNullOrEmpty(model.q))
                model.Results = indices.Select(index => index.Search(model.q));

            return View(model);
        }
    }
}