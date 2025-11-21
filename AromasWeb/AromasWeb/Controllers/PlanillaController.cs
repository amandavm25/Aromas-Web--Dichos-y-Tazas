using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AromasWeb.Controllers
{
    public class PlanillaController : Controller
    {
        // GET: PlanillaController
        public ActionResult Index()
        {
            return View();
        }

        // GET: PlanillaController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: PlanillaController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PlanillaController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PlanillaController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PlanillaController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PlanillaController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PlanillaController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
