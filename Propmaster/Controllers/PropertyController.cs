using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage.Blob;
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

        private CloudBlobContainer GetBlobStorageInformation()
        {
            //step 1: read appsettings.json
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            IConfigurationRoot configure = builder.Build();

            //to get key access
            //once linked, time to read the content to get the connectionstring
            Microsoft.WindowsAzure.Storage.CloudStorageAccount objectAccount = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(configure["ConnectionStrings:propertytablestorage"]);

            //step 3: how to create a new container / link to a container in the blob storage account.
            CloudBlobClient blobClient = objectAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("propertyblobstorage"); //give the container a name

            //step 4: return the conatiner to be created/referred
            return container;

        }

        public string UploadImages(List<IFormFile> images)
        {
            //step 1: refer to storage info
            CloudBlobContainer container = GetBlobStorageInformation();

            string urlList = "";

            foreach (IFormFile image in images)
            {
                if (image != null && image.Length > 0)
                {
                    try
                    {
                        using (var ms = new MemoryStream())
                        {
                            image.CopyTo(ms);
                            string extension = image.FileName.Split('.').ElementAt(1);
                            string blobName = Path.GetFileName(Guid.NewGuid().ToString().Replace("-", string.Empty)) + "." + extension;
                            CloudBlockBlob blob = container.GetBlockBlobReference(blobName);
                            ms.Position = 0;
                            blob.UploadFromStreamAsync(ms).Wait();
                            urlList = urlList + blobName + ",";
                        }
                    }
                    catch (Exception ex)
                    {
                        return ex.Message;
                    }
                }
                
            }

            if (string.IsNullOrWhiteSpace(urlList))
            {
                return urlList;
            }
            else
            {
                return urlList.Remove(urlList.Length - 1);
            }
        }

        //private string UploadedFile(EmployeeViewModel model)
        //{
        //    string uniqueFileName = null;

        //    if (model.ProfileImage != null)
        //    {
        //        string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
        //        uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ProfileImage.FileName;
        //        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
        //        using (var fileStream = new FileStream(filePath, FileMode.Create))
        //        {
        //            model.ProfileImage.CopyTo(fileStream);
        //        }
        //    }
        //    return uniqueFileName;
        //}
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult ConfirmEdit(string PropertyLocation, string PropertyId)
        {
            Repository repository = new Repository();
            Property item = repository.Get(PropertyLocation, PropertyId);
            List<string> urlList = new List<string>();
            string[] arr = item.PicUrl.Split(",");
            if (!string.IsNullOrWhiteSpace(arr[0]))
            {
                urlList = arr.ToList();
                if (urlList.Count != 0)
                {
                    urlList = urlList.Select(u => "https://propmasterstorage.blob.core.windows.net/propertyblobstorage/" + u).ToList();
                }
            }
            return View("Edit", new CreateListingModel
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
                PicUrlList = urlList,
                PropertyStatus = item.PropertyStatus,
                DateCreated = item.DateCreated
            });

        }


        public IActionResult ConfirmDelete(string PropertyLocation, string PropertyId)
        {
            //DeleteBlob("048092863729432fb08aed5e3fdfecad.png");
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

        public IActionResult DeleteBlob(string area, string PropertyLocation, string PropertyId)
        {
            Repository repository = new Repository();
            Property item = repository.Get(PropertyLocation, PropertyId);
            CloudBlobContainer container = GetBlobStorageInformation();
            area = area.Replace("https://propmasterstorage.blob.core.windows.net/propertyblobstorage/", string.Empty);
            string blobName = "";
            try
            {
                //specifying which blob to refer
                CloudBlockBlob deleteItem = container.GetBlockBlobReference(area);
                blobName = deleteItem.Name;
                //download file
                deleteItem.DeleteIfExistsAsync();
                var existingUrls = item.PicUrl.Split(',').ToList();
                existingUrls.Remove(area);
                string urls = String.Join(",", existingUrls.Select(u => u));
                item.PicUrl = urls;
                repository.CreateOrUpdate(item);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }

        public IActionResult ReplaceBlob(CreateListingModel propertyModel)
        {
            CloudBlobContainer container = GetBlobStorageInformation();
            string blobName = propertyModel.Urls.Replace("https://propmasterstorage.blob.core.windows.net/propertyblobstorage/", string.Empty);
            try
            {
                using (var ms = new MemoryStream())
                {
                    foreach (var image in propertyModel.PicUrl)
                    {
                        image.CopyTo(ms);
                        string extension = image.FileName.Split('.').ElementAt(1);
                        CloudBlockBlob blob = container.GetBlockBlobReference(blobName);
                        ms.Position = 0;
                        blob.UploadFromStreamAsync(ms).Wait();
                    }
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
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
        public async Task<IActionResult> Create(CreateListingModel newProperty)
        {
            List<IFormFile> images = newProperty.PicUrl.ToList();
            string urls = "";
            urls = UploadImages(images);

            PropmasterUser user = await _userManager.GetUserAsync(this.User);
            Repository repository = new Repository();
            repository.CreateOrUpdate(new Property
            {
                PartitionKey = newProperty.PropertyLocation,
                RowKey = Guid.NewGuid().ToString(),
                CreatedBy = user.Email,
                Title = newProperty.Title,
                Description = newProperty.Description,
                PropertySize = newProperty.PropertySize,
                PropertyType = newProperty.PropertyType,
                Price = newProperty.Price,
                Furnished = newProperty.Furnished,
                Bedroom = newProperty.Bedroom,
                Bathroom = newProperty.Bathroom,
                Carpark = newProperty.Carpark,
                PicUrl = urls,
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
