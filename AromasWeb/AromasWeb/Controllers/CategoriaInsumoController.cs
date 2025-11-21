using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AromasWeb.Controllers
{
    public class CategoriaInsumoController : Controller
    {
        // GET: CategoriaInsumoController
        public ActionResult Index()
        {
            return View();
        }

        // GET: CategoriaInsumoController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: CategoriaInsumoController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CategoriaInsumoController/Create
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

        // GET: CategoriaInsumoController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: CategoriaInsumoController/Edit/5
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

        // GET: CategoriaInsumoController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CategoriaInsumoController/Delete/5
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
