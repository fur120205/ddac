using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Propmaster.Areas.Identity.Data;
using Propmaster.Data;

namespace Propmaster.Controllers
{
    public class AccountController : Controller
    {
        private readonly PropmasterContext propmasterContext;
        private readonly UserManager<PropmasterUser> userManager;
        public AccountController(UserManager<PropmasterUser> userManager, PropmasterContext propmasterContext)
        {
            this.propmasterContext = propmasterContext;
            this.userManager = userManager;
        }
        // GET: AccountController
        public async Task<ActionResult> Index()
        {
            return View(await propmasterContext.Users.Where(u => u.Type == "Agent").ToListAsync());
        }

        // GET: AccountController/Details/5
        public async Task<ActionResult> Details(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await propmasterContext.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: AccountController/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        // POST: AccountController/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create(IFormCollection collection)
        //{
        //    try
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        // GET: AccountController/Edit/5
        //public async Task<ActionResult> Edit(string? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var user = await propmasterContext.Users.FindAsync(id);
        //    if (user == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(user);
        //}

        //// POST: AccountController/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(string? id, [Bind("ID,FlowerName,FlowerProducedDate,Type,SinglePrice")] PropmasterUser user)
        //{
        //    if (id != user.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            propmasterContext.Update(user);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!FlowerExists(flower.ID))
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
        //    return View(flower);
        //}

        // GET: AccountController/Delete/5
        public async Task<ActionResult> UpdatePassword(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await propmasterContext.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            return View(new ChangePasswordModel() { Id = id });
        }

        [BindProperty]
        public ChangePasswordModel Input { get; set; }

        public class ChangePasswordModel
        {
            [Required]
            public string Id { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }
        }

        [HttpPost]
        public async Task<ActionResult> UpdatePassword(string id, [Bind("Id, Password")] ChangePasswordModel newpassword) //Task<IActionResult>: is an api response, async function return (like BadRequestObjectResult(400), Ok(200), not found(404)); FormBody = JSON body; credentials = object of LoginCredentials
        {
            var user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                var token = await userManager.GeneratePasswordResetTokenAsync(user);

                var result = await userManager.ResetPasswordAsync(user, token, newpassword.Password);

                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: AccountController/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await propmasterContext.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Flowers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await propmasterContext.Users.FindAsync(id);
            propmasterContext.Users.Remove(user);
            await propmasterContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
