using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AromasWeb.Controllers
{
    public class HistorialTarifaController : Controller
    {
        // GET: HistorialTarifaController
        public ActionResult Index()
        {
            return View();
        }

        // GET: HistorialTarifaController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: HistorialTarifaController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: HistorialTarifaController/Create
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

        // GET: HistorialTarifaController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: HistorialTarifaController/Edit/5
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

        // GET: HistorialTarifaController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: HistorialTarifaController/Delete/5
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
