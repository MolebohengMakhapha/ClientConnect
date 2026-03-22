using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Client_Connect.Helpers
{
    public class Helper
    {
        public static string ClientOptions(int clientId, string clientName, int stateId)
        {
            var sb = new StringBuilder();
            sb.Append("<div class=\"dropdown\">");
            sb.Append("<button class=\"btn btn-sm btn-outline-secondary dropdown-toggle\" type=\"button\" data-toggle=\"dropdown\">Options</button>");
            sb.Append("<div class=\"dropdown-menu dropdown-menu-right\">");

            // Edit — always visible
            sb.AppendFormat(
                "<a class=\"dropdown-item\" href=\"/Clients/Edit/{0}\">" +
                "<i class=\"fas fa-edit mr-2 text-secondary\"></i>Edit</a>",
                clientId);

            sb.Append("<div class=\"dropdown-divider\"></div>");

            if (stateId == 1)
            {
                sb.AppendFormat(
                    "<a class=\"dropdown-item text-danger btn-delete-client\" href=\"#\" " +
                    "data-id=\"{0}\" data-name=\"{1}\">" +
                    "<i class=\"fas fa-trash mr-2\"></i>Delete</a>",
                    clientId, clientName);
            }
            else
            {
                sb.AppendFormat(
                    "<a class=\"dropdown-item text-success btn-toggle-client\" href=\"#\" " +
                    "data-id=\"{0}\" data-name=\"{1}\" data-state=\"{2}\">" +
                    "<i class=\"fas fa-check-circle mr-2\"></i>Set Active</a>",
                    clientId, clientName, stateId);
            }

            sb.Append("</div></div>");
            return sb.ToString();
        }

        public static string ContactOptions(int contactId, string contactName, int stateId)
        {
            var sb = new StringBuilder();
            sb.Append("<div class=\"dropdown\">");
            sb.Append("<button class=\"btn btn-sm btn-outline-secondary dropdown-toggle\" type=\"button\" data-toggle=\"dropdown\">Options</button>");
            sb.Append("<div class=\"dropdown-menu dropdown-menu-right\">");

            // Edit — always visible
            sb.AppendFormat(
                "<a class=\"dropdown-item\" href=\"/Contacts/Edit/{0}\">" +
                "<i class=\"fas fa-edit mr-2 text-secondary\"></i>Edit</a>",
                contactId);

            sb.Append("<div class=\"dropdown-divider\"></div>");

            if (stateId == 1)
            {
                sb.AppendFormat(
                    "<a class=\"dropdown-item text-danger btn-delete-contact\" href=\"#\" " +
                    "data-id=\"{0}\" data-name=\"{1}\">" +
                    "<i class=\"fas fa-trash mr-2\"></i>Delete</a>",
                    contactId, contactName);
            }
            else
            {
                sb.AppendFormat(
                    "<a class=\"dropdown-item text-success btn-toggle-contact\" href=\"#\" " +
                    "data-id=\"{0}\" data-name=\"{1}\" data-state=\"{2}\">" +
                    "<i class=\"fas fa-check-circle mr-2\"></i>Set Active</a>",
                    contactId, contactName, stateId);                
            }

            sb.Append("</div></div>");
            return sb.ToString();
        }
    }
}