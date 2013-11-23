using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WazniakWebsite.DAL;
using WazniakWebsite.Models;

namespace WazniakWebsite.Controllers
{
    public class SampleItemMobileController : Controller
    {
        private SampleContext db = new SampleContext();

        // GET: /SampleItem/
        public async Task<ActionResult> Index()
        {
            return View(await db.SampleItems.ToListAsync());
        }

        // GET: /SampleItem/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SampleItem sampleitem = await db.SampleItems.LookupAsync(id);
            if (sampleitem == null)
            {
                return HttpNotFound();
            }
            return View(sampleitem);
        }

        // GET: /SampleItem/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /SampleItem/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,Text,Number")] SampleItem sampleitem)
        {
            if (ModelState.IsValid)
            {
                await db.SampleItems.InsertAsync(sampleitem);
                return RedirectToAction("Index");
            }

            return View(sampleitem);
        }

        // GET: /SampleItem/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SampleItem sampleitem = await db.SampleItems.LookupAsync(id);
            if (sampleitem == null)
            {
                return HttpNotFound();
            }
            return View(sampleitem);
        }

        // POST: /SampleItem/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,Text,Number")] SampleItem sampleitem)
        {
            if (ModelState.IsValid)
            {
                await db.SampleItems.UpdateAsync(sampleitem);
                return RedirectToAction("Index");
            }
            return View(sampleitem);
        }

        // GET: /SampleItem/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SampleItem sampleitem = await db.SampleItems.LookupAsync(id);
            if (sampleitem == null)
            {
                return HttpNotFound();
            }
            return View(sampleitem);
        }

        // POST: /SampleItem/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            SampleItem sampleitem = await db.SampleItems.LookupAsync(id);
            await db.SampleItems.DeleteAsync(sampleitem);
            return RedirectToAction("Index");
        }

    }
}