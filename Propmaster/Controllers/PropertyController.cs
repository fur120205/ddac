using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Propmaster.Areas.Identity.Data;
using Propmaster.Data;
using Propmaster.Models;

namespace Propmaster.Views
{
    public class PropertyController : Controller
    {
        private readonly UserManager<PropmasterUser> _userManager;

        public PropertyController(UserManager<PropmasterUser> userManager)
        {
            _userManager = userManager;
        }

        // GET: Property
        public IActionResult Index()
        {
            Repository repository = new Repository();
            IEnumerable<Property> entities = repository.GetAll();
            IEnumerable<Property> model = entities.Select(x => new Property
            {
                PropertyLocation = x.PartitionKey,
                PropertyId = x.RowKey,
                CreatedBy = x.CreatedBy,
                Title = x.Title,
                Description = x.Description,
                PropertySize = x.PropertySize,
                PropertyType = x.PropertyType,
                Price = x.Price,
                Furnished = x.Furnished,
                Bedroom = x.Bedroom,
                Bathroom = x.Bathroom,
                Carpark = x.Carpark,
                PicUrl = x.PicUrl,
                PropertyStatus = x.PropertyStatus,
                DateCreated = x.DateCreated
            });
            return View(model);
        }

        public IActionResult Create()
        {
            return View();
        }

        public IActionResult ConfirmEdit(string PropertyLocation, string PropertyId)
        {
            Repository repository = new Repository();
            Property item = repository.Get(PropertyLocation, PropertyId);
            return View("Edit", new Property
            {
                PropertyLocation = item.PartitionKey,
                PropertyId = item.RowKey,
                CreatedBy = item.CreatedBy,
                Title = item.Title,
                Description = item.Description,
                PropertySize = item.PropertySize,
                PropertyType = item.PropertyType,
                Price = item.Price,
                Furnished = item.Furnished,
                Bedroom = item.Bedroom,
                Bathroom = item.Bathroom,
                Carpark = item.Carpark,
                PicUrl = item.PicUrl,
                PropertyStatus = item.PropertyStatus,
                DateCreated = item.DateCreated
            });

        }
        public IActionResult ConfirmDelete(string PropertyLocation, string PropertyId)
        {
            Repository repository = new Repository();
            Property item = repository.Get(PropertyLocation, PropertyId);
            return View("Delete", new Property
            {
                PropertyLocation = item.PartitionKey,
                PropertyId = item.RowKey,
                CreatedBy = item.CreatedBy,
                Title = item.Title,
                Description = item.Description,
                PropertySize = item.PropertySize,
                PropertyType = item.PropertyType,
                Price = item.Price,
                Furnished = item.Furnished,
                Bedroom = item.Bedroom,
                Bathroom = item.Bathroom,
                Carpark = item.Carpark,
                PicUrl = item.PicUrl,
                PropertyStatus = item.PropertyStatus,
                DateCreated = item.DateCreated
            });

        }

        [HttpPost]
        public IActionResult Delete(string PropertyLocation, string PropertyId)
        {
            Repository repository = new Repository();
            Property item = repository.Get(PropertyLocation, PropertyId);
            repository.Delete(item);
            return RedirectToAction("Index");

        }
        [HttpPost]
        public async Task<IActionResult> Create(Property property)
        {
            PropmasterUser user = await _userManager.GetUserAsync(this.User);
            Repository repository = new Repository();
            repository.CreateOrUpdate(new Property
            {
                PartitionKey = property.PropertyLocation,
                RowKey = Guid.NewGuid().ToString(),
                CreatedBy = user.Email,
                Title = property.Title,
                Description = property.Description,
                PropertySize = property.PropertySize,
                PropertyType = property.PropertyType,
                Price = property.Price,
                Furnished = property.Furnished,
                Bedroom = property.Bedroom,
                Bathroom = property.Bathroom,
                Carpark = property.Carpark,
                PicUrl = property.PicUrl,
                PropertyStatus = "Available",
                DateCreated = DateTime.Now
            });
            return RedirectToAction("Index");
        }

        public IActionResult Edit(Property property)
        {
            Repository repository = new Repository();
            repository.CreateOrUpdate(new Property
            {
                Title = property.Title,
                Description = property.Description,
                PropertySize = property.PropertySize,
                PropertyType = property.PropertyType,
                Price = property.Price,
                Furnished = property.Furnished,
                Bedroom = property.Bedroom,
                Bathroom = property.Bathroom,
                Carpark = property.Carpark,
                PicUrl = property.PicUrl,
                PropertyStatus = property.PropertyStatus
            });
            return RedirectToAction("Index");
        }

        public IActionResult Details(string PropertyLocation, string PropertyId)
        {
            Repository repository = new Repository();
            Property item = repository.Get(PropertyLocation, PropertyId);
            return View( new Property
            {
                PropertyLocation = item.PartitionKey,
                PropertyId = item.RowKey,
                CreatedBy = item.CreatedBy,
                Title = item.Title,
                Description = item.Description,
                PropertySize = item.PropertySize,
                PropertyType = item.PropertyType,
                Price = item.Price,
                Furnished = item.Furnished,
                Bedroom = item.Bedroom,
                Bathroom = item.Bathroom,
                Carpark = item.Carpark,
                PicUrl = item.PicUrl,
                PropertyStatus = item.PropertyStatus,
                DateCreated = item.DateCreated
            });
        }

        //// GET: Property/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var @property = await _context.Property
        //        .FirstOrDefaultAsync(m => m.PropertyId == id);
        //    if (@property == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(@property);
        //}

        //// GET: Property/Create
        //public async Task<IActionResult> Create()
        //{
        //    PropmasterUser user = await _userManager.GetUserAsync(this.User);
        //    return View(new Property { PropertyStatus = "Available", DateCreated = DateTime.Now, CreatedBy = user.Id });
        //}

        //// POST: Property/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to, for 
        //// more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("PropertyId,CreatedBy,Title,Description,PropertySize,PropertyLocation,PropertyType,Price,Furnished,Bedroom,Bathroom,Carpark,PicUrl,PropertyStatus,DateCreated")] Property @property)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(@property);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(@property);
        //}

        //// GET: Property/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var @property = await _context.Property.FindAsync(id);
        //    if (@property == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(@property);
        //}

        //// POST: Property/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to, for 
        //// more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("PropertyId,CreatedBy,Title,Description,PropertySize,PropertyLocation,PropertyType,Price,Furnished,Bedroom,Bathroom,Carpark,PicUrl,PropertyStatus,DateCreated")] Property @property)
        //{
        //    if (id != @property.PropertyId)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(@property);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!PropertyExists(@property.PropertyId))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(@property);
        //}

        //// GET: Property/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var @property = await _context.Property
        //        .FirstOrDefaultAsync(m => m.PropertyId == id);
        //    if (@property == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(@property);
        //}

        //// POST: Property/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var @property = await _context.Property.FindAsync(id);
        //    _context.Property.Remove(@property);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool PropertyExists(int id)
        //{
        //    return _context.Property.Any(e => e.PropertyId == id);
        //}

    }
}
