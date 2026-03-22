using Client_Connect.Helpers;
using Client_Connect.Models;
using Client_Connect.Repositories;
using Client_Connect.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Client_Connect.Controllers
{
    public class ContactsController : Controller
    {
        #region Repositories
        private readonly IContactRepository _contactRepo;
        private readonly IClientRepository _clientRepo;

        public ContactsController()
        {
            _contactRepo = new ContactRepository();
            _clientRepo = new ClientRepository();
        }
        #endregion

        #region CRUD Actions
        // GET: /Contacts
        public ActionResult Index()
        {
            return View();
        }

        // GET: /Contacts/Create
        public ActionResult Create()
        {
            return View("Form", new ContactFormViewModel());
        }

        // GET: /Contacts/Edit/5
        public ActionResult Edit(int id)
        {
            var contact = _contactRepo.GetById(id);
            if (contact == null) return HttpNotFound();

            var vm = new ContactFormViewModel
            {
                ContactId = contact.ContactId,
                Name = contact.Name,
                Surname = contact.Surname,
                Email = contact.Email,
                LinkedClients = (System.Collections.Generic.List<LinkedClient>)
                                   _contactRepo.GetLinkedClients(id),
                AvailableClients = (System.Collections.Generic.List<Client>)
                                   _contactRepo.GetAvailableClients(id)
            };

            return View("Form", vm);
        }

        // POST (AJAX): /Contacts/Delete
        [HttpPost]
        public JsonResult Delete(int id)
        {
            try
            {
                _contactRepo.SoftDelete(id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region AJAX Actions
        // POST: /Contacts/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(ContactFormViewModel vm)
        {
            // Server-side uniqueness check for email
            if (_contactRepo.EmailExists(vm.Email, vm.ContactId))
                ModelState.AddModelError("Email", "This email address is already in use.");

            if (!ModelState.IsValid)
            {
                if (vm.ContactId != 0)
                {
                    vm.LinkedClients = (System.Collections.Generic.List<LinkedClient>)
                                          _contactRepo.GetLinkedClients(vm.ContactId);
                    vm.AvailableClients = (System.Collections.Generic.List<Client>)
                                          _contactRepo.GetAvailableClients(vm.ContactId);
                }
                return View("Form", vm);
            }

            if (vm.ContactId == 0)
            {
                int newId = _contactRepo.Create(vm.Name, vm.Surname, vm.Email);
                TempData["SuccessMessage"] = $"Contact created successfully.";
                return RedirectToAction("Edit", new { id = newId });
            }
            else
            {
                _contactRepo.Update(vm.ContactId, vm.Name, vm.Surname, vm.Email);
                TempData["SuccessMessage"] = $"Contact updated successfully.";
                return RedirectToAction("Edit", new { id = vm.ContactId });
            }
        }

        // POST (AJAX): /Contacts/LinkClient
        [HttpPost]
        public JsonResult LinkClient(int contactId, int clientId)
        {
            try
            {
                _contactRepo.LinkClient(contactId, clientId);
                var clients = _contactRepo.GetLinkedClients(contactId);
                return Json(new { success = true, clients });
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST (AJAX): /Contacts/UnlinkClient
        [HttpPost]
        public JsonResult UnlinkClient(int contactId, int clientId)
        {
            try
            {
                _contactRepo.UnlinkClient(contactId, clientId);
                var clients = _contactRepo.GetLinkedClients(contactId);
                return Json(new { success = true, clients });
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET (AJAX): /Contacts/AvailableClients
        public JsonResult AvailableClients(int contactId)
        {
            var clients = _contactRepo.GetAvailableClients(contactId);
            return Json(clients, JsonRequestBehavior.AllowGet);
        }

        // GET (AJAX): /Contacts/GetAll - DataTable source
        public JsonResult GetAll()
        {
            var contacts = _contactRepo.GetAll();
            var data = contacts.Select(c => new {
                c.ContactId,
                c.Name,
                c.Surname,
                c.Email,
                c.ClientCount,
                c.StateId,
                Options = Helper.ContactOptions(c.ContactId, c.FullName, c.StateId)
            });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LinkedClients(int contactId)
        {
            var clients = _contactRepo.GetLinkedClients(contactId);
            var data = clients.Select(c => new {
                c.ClientId,
                c.Name,
                c.ClientCode
            });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // POST (AJAX): /Contacts/ToggleState
        [HttpPost]
        public JsonResult ToggleState(int id)
        {
            try
            {
                var contact = _contactRepo.GetById(id);
                if (contact == null)
                    return Json(new { success = false, message = "Contact not found." });

                if (contact.StateId == 1)
                    _contactRepo.SoftDelete(id);
                else
                    _contactRepo.Restore(id);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion
    }
}