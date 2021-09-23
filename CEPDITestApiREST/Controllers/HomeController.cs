using CEPDITestApiREST.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace CEPDITestApiREST.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult TimbrarDocumento(string Usuario, string Password, string Layout, string UrlServer)
        {
            try
            {
                string xmlDesencriptado = string.Empty;
                dynamic clase = new ExpandoObject();
                clase.Usuario = Encriptar.AESEncrypt(Usuario);
                clase.Password = Encriptar.AESEncrypt(Password);
                clase.Lineas = Encriptar.AESEncrypt(Layout);

                var body = JsonConvert.SerializeObject(clase);

                var client = new RestClient(UrlServer);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", body, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var Json = JObject.Parse(response.Content);
                    if (Json.GetValue("exitoso").ToString() == "true")
                    {
                        xmlDesencriptado = Json.GetValue("xmlTimbrado").ToString();
                        xmlDesencriptado = Encriptar.AESDecrypt(xmlDesencriptado);
                    }

                }

                return Json(new { HttpStatusCode = response.StatusCode, Content = response.Content, xmlDesencriptado = xmlDesencriptado }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex) 
            {
                return Json(new { HttpStatusCode = 500, Content = ex.Message, xmlDesencriptado = "" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ObtenerPDF(string UUID, string Usuario, string Password, string UrlServer)
        {
            try
            {
                byte[] PDFDesencriptado = null;
                dynamic clase = new ExpandoObject();
                clase.Usuario = Encriptar.AESEncrypt(Usuario);
                clase.Password = Encriptar.AESEncrypt(Password);
                clase.UUID = Encriptar.AESEncrypt(UUID);

                var body = JsonConvert.SerializeObject(clase);

                var client = new RestClient(UrlServer);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", body, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var Json = JObject.Parse(response.Content);
                    if (Json.GetValue("exitoso").ToString() == "true")
                    {
                        PDFDesencriptado = Encriptar.AESDecryptByte(Json.GetValue("encriptadoPDF").ToString());
                    }

                }

                return Json(new { HttpStatusCode = response.StatusCode, Content = response.Content, PDFDesencriptado = PDFDesencriptado }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { HttpStatusCode = 500, Content = ex.Message, xmlDesencriptado = "" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}