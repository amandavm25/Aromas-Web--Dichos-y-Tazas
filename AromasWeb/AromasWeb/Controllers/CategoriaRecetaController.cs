using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AromasWeb.Controllers
{
    public class CategoriaRecetaController : Controller
    {
        // GET: CategoriaRecetaController
        public ActionResult Index()
        {
            return View();
        }

        // GET: CategoriaRecetaController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: CategoriaRecetaController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CategoriaRecetaController/Create
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

        // GET: CategoriaRecetaController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: CategoriaRecetaController/Edit/5
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

        // GET: CategoriaRecetaController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CategoriaRecetaController/Delete/5
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
