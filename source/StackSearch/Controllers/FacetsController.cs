using System;
using System.Web.Mvc;
using BillingsDotNet.Search;
using StackSearch.Model;

namespace StackSearch.Controllers
{
    public class FacetsController : Controller
    {
        private readonly IndexTankIndex index;

        public FacetsController(IndexTankIndex index)
        {
            this.index = index;
        }

        public ActionResult Index(SearchViewModel model)
        {
            if (!String.IsNullOrEmpty(model.q))
                model.Result = index.Search(model.q, model.f);

            return View(model);
        }
    }
}