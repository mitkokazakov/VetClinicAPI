using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VetClinic.Data;
using VetClinic.Data.Models;
using VetClinic.DTO.Pets;
using VetClinic.DTO.Visitations;

namespace VetClinic.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PetsController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly VetClinicDbContext db;
        private readonly IWebHostEnvironment hostEnvironment;
        public PetsController(UserManager<ApplicationUser> userManager, VetClinicDbContext db, IWebHostEnvironment hostEnvironment)
        {
            this.userManager = userManager;
            this.db = db;
            this.hostEnvironment = hostEnvironment;
        }

        [HttpPost]
        [Route("AddPet/{userId}")]
        public async Task<ActionResult> AddPet(string userId, [FromForm]AddPetFormModel model)
        {
            var currentUser = await this.userManager.FindByIdAsync(userId);

            var currentPetToAdd = new Pet
            {
                Name = model.Name,
                Kind = model.Kind,
                Breed = model.Breed,
                BirthDate = model.BirthDate,
                User = currentUser
            };

            if (model.Image != null)
            {
                string extension = Path.GetExtension(model.Image.FileName);

                var image = new Image
                {
                    Extension = extension,
                    Pet = currentPetToAdd
                };

                string uploadsFolder = Path.Combine(this.hostEnvironment.WebRootPath, "images");

                string pictureFileName = image.Id + extension;

                string filePath = Path.Combine(uploadsFolder, pictureFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.Image.CopyTo(fileStream);
                }

                
            }
            

            await this.db.Pet.AddAsync(currentPetToAdd);
            await this.db.SaveChangesAsync();

            return Ok("Pet was successfully added");

        }

        [HttpDelete]
        [Route("DeletePet/{petId}")]
        public async Task<ActionResult> DeletePet(string petId)
        {
            var petToDelete = this.db.Pet.FirstOrDefault(p => p.Id == petId);

            petToDelete.IsDeleted = true;

            await this.db.SaveChangesAsync();

            return Ok("Pet was deleted successfully");
        }

        [HttpPut]
        [Route("ChangePet/{petId}")]
        public async Task<ActionResult> ChangePet(string petId, [FromForm] ChangePetFormModel model)
        {
            var currentPet = this.db.Pet.FirstOrDefault(p => p.Id == petId);

            if (model.Image != null)
            {
                string extension = Path.GetExtension(model.Image.FileName);

                var image = new Image
                {
                    Extension = extension,
                    Pet = currentPet
                };

                string uploadsFolder = Path.Combine(this.hostEnvironment.WebRootPath, "images");

                string pictureFileName = image.Id + extension;

                string filePath = Path.Combine(uploadsFolder, pictureFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.Image.CopyTo(fileStream);
                }

                currentPet.Image = image;
            }

            currentPet.Name = model.Name;
            currentPet.Breed = model.Breed;
            currentPet.Kind = model.Kind;

            
            await this.db.SaveChangesAsync();

            return Ok("Pet was successfully updated");

        }

        [HttpGet]
        [Route("GetAllPets")]
        public async Task<ActionResult<IEnumerable<PetViewModel>>> GetAllPets()
        {
            var allPets = this.db.Pet.Where(p => p.IsDeleted == false).Select(p => new PetViewModel
            {
                PetId = p.Id,
                Name = p.Name,
                Kind = p.Kind,
                Breed = p.Breed,
                BirthDate = p.BirthDate,
                ImageId = p.ImageId
            }).ToList();

            return allPets;
        }

        [HttpGet]
        [Route("GetPetsByUser/{userId}")]
        public async Task<ActionResult<IEnumerable<PetViewModel>>> GetPetsByUser(string userId)
        {
            var usersPets = this.db.Pet.Where(p => p.UserId == userId && p.IsDeleted == false).Select(p => new PetViewModel
            {
                PetId = p.Id,
                Name = p.Name,
                Kind = p.Kind,
                Breed = p.Breed,
                BirthDate = p.BirthDate,
                ImageId = p.ImageId
            }).ToList();

            return usersPets;
        }

        [HttpGet]
        [Route("GetPetById/{petId}")]
        public async Task<ActionResult<PetViewModel>> GetPetById(string petId)
        {
            var pet = this.db.Pet.FirstOrDefault(p => p.Id == petId && p.IsDeleted == false);

            var petToDisplay = new PetViewModel
            {
                Name = pet.Name,
                Kind = pet.Kind,
                Breed = pet.Breed,
                ImageId = pet.ImageId,
                BirthDate = pet.BirthDate
            };

            return petToDisplay;
        }

        [HttpGet]
        [Route("FindPetsByName/{petName}")]
        public async Task<ActionResult<IEnumerable<PetViewModel>>> FindPetsByName(string petName) 
        {
            var allFoundPets = this.db.Pet.Where(p => p.Name.Contains(petName) && p.IsDeleted == false).Select(p => new PetViewModel
            {
                PetId = p.Id,
                Name = p.Name,
                Kind = p.Kind,
                Breed = p.Breed,
                ImageId = p.ImageId,
                BirthDate = p.BirthDate
            }).ToList();

            return allFoundPets;
        }

        [HttpGet]
        [Route("GetImage/{petId}")]
        public async Task<ActionResult<Image>> GetImage(string petId)
        {
            var pet = this.db.Pet.FirstOrDefault(p => p.Id == petId);

            var image = this.db.Image.FirstOrDefault(i => i.PetId == petId);

            string currentDirectory = Environment.CurrentDirectory;

            var imageToReturn = System.IO.File.OpenRead($"{currentDirectory}/wwwroot/images/{image.Id}" + image.Extension);

            return File(imageToReturn, "image/jpeg");
        }

        [HttpPost]
        [Route("AddVisitation/{petId}")]
        public async Task<ActionResult> AddVisitation(string petId, [FromBody]AddVisitationFormModel model) 
        {
            var visitation = new Visitation
            {
                Reason = model.Reason,
                Description = model.Description,
                PetId = petId,
                Date = DateTime.Now
            };

            await this.db.Visitation.AddAsync(visitation);
            await this.db.SaveChangesAsync();

            return Ok("Visitation has been added successfully");
        }

        [HttpGet]
        [Route("GetVisitations/{petId}")]
        public async Task<ActionResult<IEnumerable<VisitationViewModel>>> GetVisitations(string petId)
        {
            var petsVisitation = this.db.Visitation
                                .Where(v => v.PetId == petId)
                                .Select(v => new VisitationViewModel
                                {
                                    Id = v.Id,
                                    Date = v.Date.ToString("d"),
                                    Reason = v.Reason,
                                    Description = v.Description
                                }).ToList();

            return petsVisitation;
        }

    }
}
