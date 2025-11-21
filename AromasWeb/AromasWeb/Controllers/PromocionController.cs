using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AromasWeb.Controllers
{
    public class PromocionController : Controller
    {
        // GET: PromocionController
        public ActionResult Index()
        {
            return View();
        }

        // GET: PromocionController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: PromocionController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PromocionController/Create
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

        // GET: PromocionController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PromocionController/Edit/5
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

        // GET: PromocionController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PromocionController/Delete/5
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
