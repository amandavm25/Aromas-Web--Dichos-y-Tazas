using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AromasWeb.Controllers
{
    public class PermisosController : Controller
    {
        // GET: PermisosController
        public ActionResult Index()
        {
            return View();
        }

        // GET: PermisosController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: PermisosController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PermisosController/Create
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

        // GET: PermisosController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PermisosController/Edit/5
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

        // GET: PermisosController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PermisosController/Delete/5
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
