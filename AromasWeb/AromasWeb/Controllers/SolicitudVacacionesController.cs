using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AromasWeb.Controllers
{
    public class SolicitudVacacionesController : Controller
    {
        // GET: SolicitudVacacionesController
        public ActionResult Index()
        {
            return View();
        }

        // GET: SolicitudVacacionesController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: SolicitudVacacionesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SolicitudVacacionesController/Create
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

        // GET: SolicitudVacacionesController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: SolicitudVacacionesController/Edit/5
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

        // GET: SolicitudVacacionesController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: SolicitudVacacionesController/Delete/5
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
