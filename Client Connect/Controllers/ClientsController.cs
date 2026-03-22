using Client_Connect.Helpers;
using Client_Connect.Models;
using Client_Connect.Repositories;
using Client_Connect.Services;
using Client_Connect.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Client_Connect.Controllers
{
    public class ClientsController : Controller
    {
        #region Repositories and Services
        private readonly IClientRepository _clientRepo;
        private readonly IContactRepository _contactRepo;
        private readonly ClientCodeService _codeService;

        public ClientsController()
        {
            _clientRepo = new ClientRepository();
            _contactRepo = new ContactRepository();
            _codeService = new ClientCodeService(_clientRepo);
        }
        #endregion

        #region CRUD Actions
        // GET: /Clients
        public ActionResult Index()
        {
            return View();
        }

        // GET: /Clients/Create
        public ActionResult Create()
        {
            return View("Form", new ClientFormViewModel());
        }

        // GET: /Clients/Edit/5
        public ActionResult Edit(int id)
        {
            var client = _clientRepo.GetById(id);
            if (client == null) return HttpNotFound();

            var vm = new ClientFormViewModel
            {
                ClientId = client.ClientId,
                Name = client.Name,
                ClientCode = client.ClientCode,
                LinkedContacts = (System.Collections.Generic.List<LinkedContact>)
                                   _clientRepo.GetLinkedContacts(id),
                AvailableContacts = (System.Collections.Generic.List<Contact>)
                                   _clientRepo.GetAvailableContacts(id)
            };

            return View("Form", vm);
        }

        // POST (AJAX): /Clients/Delete
        [HttpPost]
        public JsonResult Delete(int id)
        {
            try
            {
                _clientRepo.SoftDelete(id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region Form Submission and AJAX Actions
        // POST: /Clients/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(ClientFormViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                if (vm.ClientId != 0)
                {
                    vm.LinkedContacts = (System.Collections.Generic.List<LinkedContact>)
                                           _clientRepo.GetLinkedContacts(vm.ClientId);
                    vm.AvailableContacts = (System.Collections.Generic.List<Contact>)
                                           _clientRepo.GetAvailableContacts(vm.ClientId);
                }
                return View("Form", vm);
            }

            if (vm.ClientId == 0)
            {
                // CREATE
                string code = _codeService.Generate(vm.Name);
                int newId = _clientRepo.Create(vm.Name, code);

                TempData["SuccessMessage"] = $"Client created successfully with code {code}.";
                return RedirectToAction("Edit", new { id = newId });
            }
            else
            {
                // UPDATE
                _clientRepo.Update(vm.ClientId, vm.Name);
                TempData["SuccessMessage"] = $"Client updated successfully.";
                return RedirectToAction("Edit", new { id = vm.ClientId });
            }
        }    

        // POST: /Clients/LinkContact
        [HttpPost]
        public JsonResult LinkContact(int clientId, int contactId)
        {
            try
            {
                _clientRepo.LinkContact(clientId, contactId);
                var contacts = _clientRepo.GetLinkedContacts(clientId);
                return Json(new { success = true, contacts });
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: /Clients/UnlinkContact
        [HttpPost]
        public JsonResult UnlinkContact(int clientId, int contactId)
        {
            try
            {
                _clientRepo.UnlinkContact(clientId, contactId);
                var contacts = _clientRepo.GetLinkedContacts(clientId);
                return Json(new { success = true, contacts });
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: /Clients/AvailableContacts
        public JsonResult AvailableContacts(int clientId)
        {
            var contacts = _clientRepo.GetAvailableContacts(clientId);
            return Json(contacts, JsonRequestBehavior.AllowGet);
        }

        // GET: /Clients/GetAll - DataTable source
        public JsonResult GetAll()
        {
            var clients = _clientRepo.GetAll();
            var data = clients.Select(c => new {
                c.ClientId,
                c.Name,
                c.ClientCode,
                c.ContactCount,
                c.StateId,
                Options = Helper.ClientOptions(c.ClientId, c.Name, c.StateId)
            });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // GET: /Clients/LinkedContacts - DataTable source
        public JsonResult LinkedContacts(int clientId)
        {
            var contacts = _clientRepo.GetLinkedContacts(clientId);
            var data = contacts.Select(c => new {
                c.ContactId,
                c.Name,
                c.Surname,
                c.Email
            });
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        // POST (AJAX): /Clients/ToggleState
        [HttpPost]
        public JsonResult ToggleState(int id)
        {
            try
            {
                var client = _clientRepo.GetById(id);
                if (client == null)
                    return Json(new { success = false, message = "Client not found." });

                if (client.StateId == 1)
                    _clientRepo.SoftDelete(id);
                else
                    _clientRepo.Restore(id);

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