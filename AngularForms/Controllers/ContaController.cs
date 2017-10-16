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
using BrasaoHamburgueria.Model;
using BrasaoHamburgueria.Web.Context;
using Newtonsoft.Json.Serialization;
using BrasaoHamburgueria.Web.Filters;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.AspNet.Identity.Owin;
using BrasaoHamburgueria.Web.Extentions;
using System.Text.RegularExpressions;
using BrasaoHamburgueria.Helper;

namespace BrasaoHamburgueria.Web.Controllers
{
    [AllowCrossSiteJsonAttribute]
    [Authorize]
    public class ContaController : Controller
    {
        private ApplicationUserManager _userManager;
        private ApplicationSignInManager _signInManager;

        public ContaController()
        {
        }

        public ContaController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
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

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult LoginExterno(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("LoginExternoCallback", "Conta", new { ReturnUrl = returnUrl }));
        }

        [AllowAnonymous]
        public async Task<ActionResult> LoginExternoCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    ConfirmacaoLoginExternoViewModel conf = new ConfirmacaoLoginExternoViewModel();
                    conf.Email = loginInfo.Email;
                    conf.Provider = loginInfo.Login.LoginProvider;
                    conf.Nome = conf.Email;

                    if (conf.Provider == "Facebook")
                    {
                        //conf.Nome = loginInfo.ExternalIdentity.Claims.First(c => c.Type == "urn:facebook:name").Value;
                        conf.Nome = loginInfo.ExternalIdentity.Claims.Where(c => c.Type == System.Security.Claims.ClaimTypes.Name).Select(c => c.Value).SingleOrDefault();
                        conf.Telefone = loginInfo.ExternalIdentity.Claims.Where(c => c.Type == System.Security.Claims.ClaimTypes.MobilePhone).Select(c => c.Value).SingleOrDefault();

                    }
                    else if (conf.Provider == "Google")
                    {
                        conf.Nome = loginInfo.ExternalIdentity.Claims.Where(c => c.Type == System.Security.Claims.ClaimTypes.Name).Select(c => c.Value).SingleOrDefault();
                        conf.Telefone = loginInfo.ExternalIdentity.Claims.Where(c => c.Type == System.Security.Claims.ClaimTypes.MobilePhone).Select(c => c.Value).SingleOrDefault();
                    }
                    return View("ConfirmacaoLoginExterno", conf);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmacaoLoginExterno(ConfirmacaoLoginExternoViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                bool isEmail = Regex.IsMatch(model.Email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);

                if (!isEmail)
                {
                    ModelState.AddModelError("Email", "Email inválido.");
                }
                else
                {
                    var user = await UserManager.FindByEmailAsync(model.Email);
                    if (user != null)
                    {
                        ModelState.AddModelError("Email", "Este e-mail já está em uso. Por favor utilize outro.");
                    }
                }
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };

                Usuario usu = new Usuario();
                usu.Email = model.Email;
                usu.Nome = model.Nome;
                usu.Telefone = model.Telefone;
                usu.Estado = "MG";
                usu.Cidade = "Cataguases";

                user.DadosUsuario = usu;

                IdentityUserRole role = new IdentityUserRole();
                role.RoleId = "1";
                role.UserId = user.Id;
                user.Roles.Add(role);

                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
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

            if (user.DadosUsuario != null && !String.IsNullOrEmpty(user.DadosUsuario.DataNascimento))
            {
                user.DadosUsuario.DataNascimento = user.DadosUsuario.DataNascimento.Substring(0, 2) + "/" + user.DadosUsuario.DataNascimento.Substring(2, 2) + "/" + user.DadosUsuario.DataNascimento.Substring(4);
            }
            user.DadosUsuario.Email = user.Email;

            var usuario = new UsuarioViewModel();

            UsuarioCopy.DBToViewModel(user.DadosUsuario, usuario);
            
            return new JsonNetResult { Data = usuario };
        }

        public async Task<JsonNetResult> GetUsuarioByPhone(string telefone)
        {
            var result = new ServiceResultViewModel(true, new List<string>(), null);

            if (telefone.Length < 14)
            {
                result.Succeeded = false;
                result.Errors.Add("O telefone não está preenchido corretamente");
                return new JsonNetResult { Data = result };
            }

            ApplicationDbContext contexto = new ApplicationDbContext();
            var user = contexto.DadosUsuarios.Where(u => u.Telefone == telefone).FirstOrDefault();
            UsuarioViewModel usuario = new UsuarioViewModel();

            if (user != null)
            {
                if (!String.IsNullOrEmpty(user.DataNascimento))
                {
                    user.DataNascimento = user.DataNascimento.Substring(0, 2) + "/" + user.DataNascimento.Substring(2, 2) + "/" + user.DataNascimento.Substring(4);
                }

                UsuarioCopy.DBToViewModel(user, usuario);                
            }
            else
            {
                usuario.ClienteNovo = true;
                usuario.Telefone = telefone;
                usuario.Estado = "MG";
                usuario.Cidade = "Cataguases";
            }

            result.data = usuario;

            return new JsonNetResult { Data = result };
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

        private const string XsrfKey = "XsrfId";

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

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }

        #endregion

    }
}