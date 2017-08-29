using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using AngularForms.Model;
using Newtonsoft.Json.Serialization;
using AngularForms.Filters;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.AspNet.Identity.Owin;
using AngularForms.Extentions;

namespace AngularForms.Controllers
{
    [Authorize]
    public class ContaController : Controller
    {
        private ApplicationUserManager _userManager;

        public ContaController()
        {
        }

        public ContaController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [MyValidateAntiForgeryToken]
        public async Task<JsonResult> Login(LoginViewModel model, string returnUrl)
        {
            var result = new { Succeeded = false, errors = new List<String>(), data = "" };

            if (ModelState.IsValid)
            {
                var user = await UserManager.FindAsync(model.Usuario, model.Senha);
                if (user != null)
                {
                    await SignInAsync(user, model.LembrarMe);
                    
                    result = new { Succeeded = true, errors = new List<String>(), data = model.ReturnUrl };
                }
                else
                {
                    result = new { Succeeded = false, errors = new List<String> { "Login ou senha inválida." }, data = model.ReturnUrl };
                }
            }

            // If we got this far, something failed, redisplay form
            return Json(result, "application/json", JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /Conta/
        [AllowAnonymous]
        public ActionResult Cadastrar()
        {
            return View();
        }

        public async Task<ActionResult> Alterar()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return View("Erro");
            }

            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            ViewBag.Email = user.Email;

            return View("Cadastrar");
        }

        public async Task<JsonNetResult> GetUsuario()
        {
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            user.DadosUsuario.DataNascimento = user.DadosUsuario.DataNascimento.Substring(0, 2) + "/" + user.DadosUsuario.DataNascimento.Substring(2, 2) + "/" + user.DadosUsuario.DataNascimento.Substring(4);

            return new JsonNetResult { Data = user.DadosUsuario };
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonNetResult EmailExiste(String email)
        {
            var usu = UserManager.Users.Where(u => u.UserName == email).FirstOrDefault();
            //var usu = UserManager.Find((String)email);
            return new JsonNetResult { Data = (usu != null) };
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<JsonResult> Cadastrar(Usuario usuario, bool novo, String senhaAtual, String novaSenha)
        {
            IdentityResult result = null;

            if (ModelState.IsValid)
            {
                try
                {
                    if (novo)
                    {
                        var user = new ApplicationUser() { UserName = usuario.Email, Email = usuario.Email };
                        user.DadosUsuario = usuario;
                        IdentityUserRole role = new IdentityUserRole();
                        role.RoleId = "1";
                        role.UserId = user.Id;
                        user.Roles.Add(role);
                        result = await UserManager.CreateAsync(user, novaSenha);
                        if (result.Succeeded)
                        {
                            await SignInAsync(user, isPersistent: false);
                            //return result.Errors;
                        }
                        else
                        {
                            AddErrors(result);
                        }
                    }
                    else
                    {
                        var user = UserManager.FindByEmail(usuario.Email);
                        bool atualiza = true;
                        if (senhaAtual != novaSenha)
                        {
                            result = UserManager.ChangePassword(user.Id, senhaAtual, novaSenha);
                            atualiza = result.Succeeded;
                        }
                        if (atualiza)
                        {
                            PropertyCopy.Copy(usuario, user.DadosUsuario);
                            result = UserManager.Update(user);
                        }
                    }
                }
                catch(Exception ex)
                {
                    result = new IdentityResult(new []{ex.Message});
                }
                
            }
            // If we got this far, something failed, redisplay form
            return Json(result, "application/json", JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult EsqueciMinhaSenha()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [MyValidateAntiForgeryToken]
        public async Task<JsonResult> EsqueciMinhaSenha(EsqueciMinhaSenhaViewModel model)
        {
            var result = new { Succeeded = false, errors = new List<String>(), data = "" };

            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null)
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    result = new { Succeeded = true, errors = new List<String>(), data = "" };
                }
                else
                {
                    string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                    var callbackUrl = Url.Action("RedefinirSenha", "Conta", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    String strHTML = System.IO.File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath("~/Emails/InstrucoesEsqueciMinhaSenha.html"));
                    strHTML = String.Format(strHTML, user.DadosUsuario.Nome, callbackUrl);
                    await UserManager.SendEmailAsync(user.Id, "Brasão Hamburgueria - Redefinir senha", strHTML);

                    result = new { Succeeded = true, errors = new List<String>(), data = callbackUrl };
                }
            }

            return Json(result, "application/json", JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult EsqueciMinhaSenhaConfirmacao()
        {
            ViewBag.Link = TempData["ViewBagLink"];
            return View();
        }

        [AllowAnonymous]
        public ActionResult RedefinirSenha(string code)
        {
            if (code == null)
            {
                return View("Erro");
            }

            ViewBag.Code = code;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [MyValidateAntiForgeryToken]
        public async Task<JsonResult> RedefinirSenha(RedefinirSenhaViewModel model)
        {
            var result = new { Succeeded = false, errors = new List<String>(), data = "" };

            if (!ModelState.IsValid)
            {
                result = new { Succeeded = false, errors = new List<String>{"Dados do formulário preenchidos incorretamente"}, data = "" };
            }
            else
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null)
                {
                    // Don't reveal that the user does not exist
                    result = new { Succeeded = true, errors = new List<String>(), data = "" };
                }
                var resultReset = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Senha);
                if (resultReset.Succeeded)
                {
                    result = new { Succeeded = true, errors = new List<String>(), data = "" };
                }
                else
                {
                    result = new { Succeeded = false, errors = new List<String> { resultReset.Errors.FirstOrDefault().ToString() }, data = "" };
                    AddErrors(resultReset);
                }
            }

            return Json(result, "application/json", JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult RedefinirSenhaConfirmacao()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Sair()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        #region Helpers

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        #endregion

    }
}