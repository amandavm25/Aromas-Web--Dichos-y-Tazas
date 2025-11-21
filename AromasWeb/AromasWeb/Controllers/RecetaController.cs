using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AromasWeb.Controllers
{
    public class RecetaController : Controller
    {
        // GET: RecetaController
        public ActionResult Index()
        {
            return View();
        }

        // GET: RecetaController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: RecetaController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RecetaController/Create
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

        // GET: RecetaController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: RecetaController/Edit/5
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

        // GET: RecetaController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: RecetaController/Delete/5
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
