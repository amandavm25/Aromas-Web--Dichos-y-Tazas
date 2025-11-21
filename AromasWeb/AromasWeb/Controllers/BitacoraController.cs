using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AromasWeb.Controllers
{
    public class BitacoraController : Controller
    {
        // GET: BitacoraController
        public ActionResult Index()
        {
            return View();
        }

        // GET: BitacoraController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: BitacoraController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BitacoraController/Create
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

        // GET: BitacoraController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: BitacoraController/Edit/5
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

        // GET: BitacoraController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: BitacoraController/Delete/5
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
